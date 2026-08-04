#!/usr/bin/env node
import ts from "typescript";
import fs from "node:fs";
import path from "node:path";
import url from "node:url";
import { createTypeMapper } from "./types.mjs";
import { getDoc, getParameterDocs, memberDocText, numericKindFrom } from "./jsdoc.mjs";

/**
 * Extracts the three.js public API from `@types/three` into `generator/three-api.json`.
 * See `generator/IR-SCHEMA.md` for the contract this produces.
 *
 * Everything TypeScript-specific lives here so the C# emitter never has to parse `.d.ts`.
 */

const IR_VERSION = 1;
const HERE = path.dirname(url.fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(HERE, "..", "..");
const TYPES_PACKAGE = path.join(REPO_ROOT, "node_modules", "@types", "three");
const SOURCE_ROOT = path.join(TYPES_PACKAGE, "src");
/** Directories under `src/` that are out of scope: the TSL / WebGPU node stack. */
const EXCLUDED_DIRECTORIES = ["nodes"];
const DEFAULT_OUTPUT = path.join(REPO_ROOT, "generator", "three-api.json");

function byText(a, b) {
	if (a === b) {
		return 0;
	}
	return a < b ? -1 : 1;
}

function toPosix(filePath) {
	return filePath.split(path.sep).join("/");
}

function relativeToPackage(fileName) {
	return toPosix(path.relative(TYPES_PACKAGE, path.resolve(fileName)));
}

function discoverSourceFiles(directory, acc = []) {
	const entries = fs.readdirSync(directory, { withFileTypes: true }).sort((a, b) => byText(a.name, b.name));
	for (const entry of entries) {
		const name = entry.name;
		const full = path.join(directory, name);
		if (entry.isDirectory()) {
			discoverSourceFiles(full, acc);
			continue;
		}
		if (name.endsWith(".d.ts")) {
			acc.push(full);
		}
	}
	return acc;
}

function isExcluded(fileName) {
	const relative = toPosix(path.relative(SOURCE_ROOT, path.resolve(fileName)));
	if (relative.startsWith("..")) {
		return false;
	}
	return EXCLUDED_DIRECTORIES.some((dir) => relative === dir || relative.startsWith(`${dir}/`));
}

function hasModifier(node, kind) {
	return node.modifiers?.some((modifier) => modifier.kind === kind) ?? false;
}

function memberName(node) {
	if (node.name === undefined) {
		return "<anonymous>";
	}
	if (ts.isIdentifier(node.name) || ts.isPrivateIdentifier(node.name)) {
		return node.name.text;
	}
	if (ts.isStringLiteral(node.name) || ts.isNumericLiteral(node.name)) {
		return node.name.text;
	}
	return node.name.getText();
}

function visibilityOf(node) {
	if (hasModifier(node, ts.SyntaxKind.PrivateKeyword)) {
		return "private";
	}
	if (hasModifier(node, ts.SyntaxKind.ProtectedKeyword)) {
		return "protected";
	}
	return undefined;
}

function assignIfDefined(target, key, value) {
	if (value !== undefined) {
		target[key] = value;
	}
}

function main() {
	const outputIndex = process.argv.indexOf("--out");
	const outputPath = outputIndex >= 0 ? path.resolve(process.argv[outputIndex + 1]) : DEFAULT_OUTPUT;

	if (!fs.existsSync(SOURCE_ROOT)) {
		console.error(`@types/three not found at ${SOURCE_ROOT}. Run 'npm install' first.`);
		process.exit(1);
	}

	const allFiles = discoverSourceFiles(SOURCE_ROOT).sort(byText);
	const inScopeFiles = allFiles.filter((x) => !isExcluded(x));

	const program = ts.createProgram(inScopeFiles, {
		target: ts.ScriptTarget.ESNext,
		module: ts.ModuleKind.ESNext,
		moduleResolution: ts.ModuleResolutionKind.Bundler,
		skipLibCheck: true,
		strict: false,
		// Pinned so the run does not depend on whatever other @types packages happen to be installed.
		types: [],
	});
	const checker = program.getTypeChecker();

	const inScopePaths = new Set(inScopeFiles.map((x) => path.resolve(x)));

	function classifyDeclarationFile(fileName) {
		const resolved = path.resolve(fileName);
		if (inScopePaths.has(resolved)) {
			return { origin: "in-scope", file: relativeToPackage(resolved) };
		}
		if (isExcluded(resolved)) {
			return { origin: "excluded", file: relativeToPackage(resolved) };
		}
		if (toPosix(resolved).includes("/node_modules/typescript/lib/")) {
			return { origin: "lib" };
		}
		if (toPosix(resolved).startsWith(toPosix(TYPES_PACKAGE))) {
			return { origin: "package", file: relativeToPackage(resolved) };
		}
		return { origin: "external" };
	}

	const mapper = createTypeMapper({ checker, classifyDeclarationFile });
	const { typeToIr, parameterToIr, typeParametersToIr } = mapper;

	const classes = [];
	const interfaces = [];
	const enums = [];
	const constants = [];
	const typeAliases = [];
	const functions = [];
	const namespaces = [];
	const moduleAugmentations = [];
	let constantsSkippedOutOfScope = 0;

	/**
	 * `@param` tags are matched by name. Some upstream docs drift from the signature
	 * (`Path.absarc` documents `x`/`y` for parameters named `aX`/`aY`), which would silently drop the
	 * numeric kind. Fall back to position only when the tag count matches the parameter count and
	 * every name that *does* match sits at its own index, so name and position never disagree.
	 * Parameters resolved this way are marked `docSource: "position"` so the inference stays auditable.
	 */
	function resolveParameterDocs(node, parameterNames) {
		const documented = getParameterDocs(node);
		const byName = new Map();
		for (const [index, name] of parameterNames.entries()) {
			const entry = documented.get(name);
			if (entry !== undefined) {
				byName.set(index, entry);
			}
		}
		if (byName.size === parameterNames.length || documented.size !== parameterNames.length) {
			return byName;
		}
		const documentedNames = [...documented.keys()];
		const namesAgreeWithPositions = [...byName.keys()].every(
			(index) => documentedNames[index] === parameterNames[index],
		);
		if (!namesAgreeWithPositions) {
			return byName;
		}
		for (const [index, name] of documentedNames.entries()) {
			if (byName.has(index)) {
				continue;
			}
			byName.set(index, { ...documented.get(name), docSource: "position" });
		}
		return byName;
	}

	function signatureIr(node, options = {}) {
		const doc = getDoc(node);
		const parameterIrs = node.parameters.map(parameterToIr);
		const parameterDocs = resolveParameterDocs(node, parameterIrs.map((x) => x.name));
		const parameters = parameterIrs.map((parameterIr, index) => {
			const documented = parameterDocs.get(index);
			if (documented !== undefined) {
				assignIfDefined(parameterIr, "numericKind", documented.numericKind);
				assignIfDefined(parameterIr, "defaultValue", documented.defaultValue);
				assignIfDefined(parameterIr, "doc", documented.text);
				assignIfDefined(parameterIr, "docSource", documented.docSource);
			}
			return parameterIr;
		});

		const ir = { parameters };
		const typeParameters = typeParametersToIr(node.typeParameters);
		assignIfDefined(ir, "typeParameters", typeParameters);
		if (options.hasReturnType) {
			assignIfDefined(ir, "returnType", typeToIr(node.type));
			assignIfDefined(ir, "returnNumericKind", numericKindFrom(doc?.returns));
		}
		assignIfDefined(ir, "doc", doc);
		return ir;
	}

	function propertyIr(node, kind) {
		const doc = getDoc(node);
		const ir = { name: memberName(node) };
		if (kind !== "property") {
			ir.accessor = kind;
		}
		if (hasModifier(node, ts.SyntaxKind.StaticKeyword)) {
			ir.isStatic = true;
		}
		if (hasModifier(node, ts.SyntaxKind.ReadonlyKeyword)) {
			ir.isReadonly = true;
		}
		if (hasModifier(node, ts.SyntaxKind.OverrideKeyword)) {
			ir.isOverride = true;
		}
		if (hasModifier(node, ts.SyntaxKind.AbstractKeyword)) {
			ir.isAbstract = true;
		}
		if (node.questionToken !== undefined) {
			ir.isOptional = true;
		}
		assignIfDefined(ir, "visibility", visibilityOf(node));
		const typeNode = ts.isSetAccessorDeclaration(node) ? node.parameters[0]?.type : node.type;
		assignIfDefined(ir, "type", typeToIr(typeNode));
		assignIfDefined(ir, "numericKind", numericKindFrom(memberDocText(doc)));
		assignIfDefined(ir, "defaultValue", doc?.defaultValue);
		assignIfDefined(ir, "doc", doc);
		return ir;
	}

	/** Class and interface members share a shape; overloads are grouped under one entry. */
	function collectMembers(members) {
		const properties = [];
		const methodsByKey = new Map();
		const methodOrder = [];
		const indexSignatures = [];
		const callSignatures = [];
		const constructSignatures = [];
		const constructors = [];
		const accessorsByName = new Map();

		for (const member of members) {
			if (ts.isConstructorDeclaration(member)) {
				constructors.push(signatureIr(member, { hasReturnType: false }));
				continue;
			}
			if (ts.isPropertyDeclaration(member) || ts.isPropertySignature(member)) {
				properties.push(propertyIr(member, "property"));
				continue;
			}
			if (ts.isGetAccessorDeclaration(member) || ts.isSetAccessorDeclaration(member)) {
				const name = memberName(member);
				const isGetter = ts.isGetAccessorDeclaration(member);
				const existing = accessorsByName.get(name);
				if (existing !== undefined) {
					existing.accessor = "get-set";
					delete existing.isReadonly;
					if (isGetter && existing.type === undefined) {
						assignIfDefined(existing, "type", typeToIr(member.type));
					}
					continue;
				}
				const ir = propertyIr(member, isGetter ? "get" : "set");
				if (isGetter) {
					ir.isReadonly = true;
				}
				accessorsByName.set(name, ir);
				properties.push(ir);
				continue;
			}
			if (ts.isMethodDeclaration(member) || ts.isMethodSignature(member)) {
				const name = memberName(member);
				const isStatic = hasModifier(member, ts.SyntaxKind.StaticKeyword);
				const key = `${isStatic ? "static " : ""}${name}`;
				let entry = methodsByKey.get(key);
				if (entry === undefined) {
					entry = { name, overloads: [] };
					if (isStatic) {
						entry.isStatic = true;
					}
					if (hasModifier(member, ts.SyntaxKind.OverrideKeyword)) {
						entry.isOverride = true;
					}
					if (hasModifier(member, ts.SyntaxKind.AbstractKeyword)) {
						entry.isAbstract = true;
					}
					if (member.questionToken !== undefined) {
						entry.isOptional = true;
					}
					assignIfDefined(entry, "visibility", visibilityOf(member));
					methodsByKey.set(key, entry);
					methodOrder.push(entry);
				}
				entry.overloads.push(signatureIr(member, { hasReturnType: true }));
				continue;
			}
			if (ts.isIndexSignatureDeclaration(member)) {
				indexSignatures.push(signatureIr(member, { hasReturnType: true }));
				continue;
			}
			if (ts.isCallSignatureDeclaration(member)) {
				callSignatures.push(signatureIr(member, { hasReturnType: true }));
				continue;
			}
			if (ts.isConstructSignatureDeclaration(member)) {
				constructSignatures.push(signatureIr(member, { hasReturnType: true }));
			}
		}

		return { constructors, properties, methods: methodOrder, indexSignatures, callSignatures, constructSignatures };
	}

	function buildExportMap(sourceFile) {
		const map = new Map();
		const moduleSymbol = checker.getSymbolAtLocation(sourceFile);
		if (moduleSymbol === undefined) {
			return map;
		}
		for (const exported of checker.getExportsOfModule(moduleSymbol)) {
			let target = exported;
			if ((exported.flags & ts.SymbolFlags.Alias) !== 0) {
				try {
					target = checker.getAliasedSymbol(exported);
				} catch {
					/* keep the alias symbol */
				}
			}
			for (const declaration of target.declarations ?? []) {
				if (!map.has(declaration)) {
					map.set(declaration, exported.name);
				}
			}
		}
		return map;
	}

	for (const fileName of inScopeFiles) {
		const sourceFile = program.getSourceFile(fileName);
		if (sourceFile === undefined) {
			throw new Error(`TypeScript did not load ${fileName}`);
		}
		const file = relativeToPackage(sourceFile.fileName);
		const exportMap = buildExportMap(sourceFile);

		for (const statement of sourceFile.statements) {
			const exportName = exportMap.get(statement);
			const isExported = exportName !== undefined || hasModifier(statement, ts.SyntaxKind.ExportKeyword);

			if (ts.isClassDeclaration(statement)) {
				const members = collectMembers(statement.members);
				const ir = {
					name: statement.name?.text ?? "<anonymous>",
					file,
					isExported,
				};
				if (exportName !== undefined && exportName !== ir.name) {
					ir.exportName = exportName;
				}
				if (hasModifier(statement, ts.SyntaxKind.DefaultKeyword)) {
					ir.isDefaultExport = true;
				}
				if (hasModifier(statement, ts.SyntaxKind.AbstractKeyword)) {
					ir.isAbstract = true;
				}
				assignIfDefined(ir, "typeParameters", typeParametersToIr(statement.typeParameters));

				for (const clause of statement.heritageClauses ?? []) {
					if (clause.token === ts.SyntaxKind.ExtendsKeyword && clause.types.length > 0) {
						ir.extends = typeToIr(clause.types[0]);
					} else if (clause.token === ts.SyntaxKind.ImplementsKeyword) {
						ir.implements = clause.types.map(typeToIr);
					}
				}

				assignIfDefined(ir, "doc", getDoc(statement));
				ir.constructors = members.constructors;
				ir.properties = members.properties;
				ir.methods = members.methods;
				if (members.indexSignatures.length > 0) {
					ir.indexSignatures = members.indexSignatures;
				}
				classes.push(ir);
				continue;
			}

			if (ts.isInterfaceDeclaration(statement)) {
				const members = collectMembers(statement.members);
				const ir = { name: statement.name.text, file, isExported };
				assignIfDefined(ir, "typeParameters", typeParametersToIr(statement.typeParameters));
				const extended = (statement.heritageClauses ?? [])
					.filter((clause) => clause.token === ts.SyntaxKind.ExtendsKeyword)
					.flatMap((clause) => clause.types.map(typeToIr));
				if (extended.length > 0) {
					ir.extends = extended;
				}
				assignIfDefined(ir, "doc", getDoc(statement));
				ir.properties = members.properties;
				ir.methods = members.methods;
				if (members.indexSignatures.length > 0) {
					ir.indexSignatures = members.indexSignatures;
				}
				if (members.callSignatures.length > 0) {
					ir.callSignatures = members.callSignatures;
				}
				if (members.constructSignatures.length > 0) {
					ir.constructSignatures = members.constructSignatures;
				}
				interfaces.push(ir);
				continue;
			}

			if (ts.isEnumDeclaration(statement)) {
				const ir = {
					name: statement.name.text,
					file,
					isExported,
					isConst: hasModifier(statement, ts.SyntaxKind.ConstKeyword),
				};
				assignIfDefined(ir, "doc", getDoc(statement));
				ir.members = statement.members.map((member) => {
					const memberIr = { name: memberName(member) };
					const value = checker.getConstantValue(member);
					if (value !== undefined) {
						memberIr.value = value;
					}
					if (member.initializer !== undefined) {
						memberIr.initializerText = member.initializer.getText();
					}
					assignIfDefined(memberIr, "doc", getDoc(member));
					return memberIr;
				});
				enums.push(ir);
				continue;
			}

			if (ts.isTypeAliasDeclaration(statement)) {
				const ir = { name: statement.name.text, file, isExported };
				assignIfDefined(ir, "typeParameters", typeParametersToIr(statement.typeParameters));
				ir.type = typeToIr(statement.type);
				const group = constantGroupOf(ir.type);
				if (group !== undefined) {
					ir.constantGroup = group;
				}
				assignIfDefined(ir, "doc", getDoc(statement));
				typeAliases.push(ir);
				continue;
			}

			if (ts.isFunctionDeclaration(statement)) {
				const name = statement.name?.text ?? "<anonymous>";
				let entry = functions.find((x) => x.name === name && x.file === file);
				if (entry === undefined) {
					entry = { name, file, isExported, overloads: [] };
					assignIfDefined(entry, "doc", getDoc(statement));
					functions.push(entry);
				}
				entry.overloads.push(signatureIr(statement, { hasReturnType: true }));
				continue;
			}

			if (ts.isVariableStatement(statement)) {
				for (const declaration of statement.declarationList.declarations) {
					const type = typeToIr(declaration.type);
					if (type?.kind === "typeQuery" && type.target?.origin === "excluded") {
						constantsSkippedOutOfScope++;
						continue;
					}
					const ir = {
						name: declaration.name.getText(),
						file,
						isExported,
						isConst: (statement.declarationList.flags & ts.NodeFlags.Const) !== 0,
					};
					assignIfDefined(ir, "type", type);
					assignIfDefined(ir, "doc", getDoc(declaration));
					constants.push(ir);
				}
				continue;
			}

			if (ts.isModuleDeclaration(statement) && statement.body !== undefined && ts.isModuleBlock(statement.body)) {
				if (ts.isStringLiteral(statement.name)) {
					moduleAugmentations.push(moduleAugmentationIr(statement, file));
					continue;
				}
				const ir = { name: statement.name.getText(), file, isExported };
				assignIfDefined(ir, "doc", getDoc(statement));
				ir.members = statement.body.statements
					.flatMap((member) => {
						if (ts.isTypeAliasDeclaration(member) || ts.isInterfaceDeclaration(member) || ts.isClassDeclaration(member)) {
							return [{ name: member.name?.getText() ?? "<anonymous>", declarationKind: ts.SyntaxKind[member.kind] }];
						}
						if (ts.isVariableStatement(member)) {
							return member.declarationList.declarations.map((x) => ({
								name: x.name.getText(),
								declarationKind: "VariableDeclaration",
							}));
						}
						if (ts.isFunctionDeclaration(member)) {
							return [{ name: member.name?.text ?? "<anonymous>", declarationKind: "FunctionDeclaration" }];
						}
						if (ts.isExportDeclaration(member) && member.exportClause !== undefined && ts.isNamedExports(member.exportClause)) {
							return member.exportClause.elements.map((element) => ({
								name: element.name.text,
								declarationKind: "ExportSpecifier",
							}));
						}
						return [{ name: "<unknown>", declarationKind: ts.SyntaxKind[member.kind] }];
					})
					.sort((a, b) => byText(a.name, b.name) || byText(a.declarationKind, b.declarationKind));
				namespaces.push(ir);
			}
		}
	}

	/**
	 * `declare module "../../scenes/Scene.js" { interface Scene { ... } }` - declaration merging that
	 * bolts extra members onto a type declared elsewhere. In three.js these come from the node stack,
	 * so the emitter must see them to know a class's real member set is not just its own declaration.
	 */
	function moduleAugmentationIr(statement, file) {
		const ir = { file, targetModule: statement.name.text };
		const moduleSymbol = checker.getSymbolAtLocation(statement.name);
		const targetSourceFile = (moduleSymbol?.declarations ?? []).find((x) => ts.isSourceFile(x));
		if (targetSourceFile !== undefined) {
			const placement = classifyDeclarationFile(targetSourceFile.fileName);
			ir.targetOrigin = placement.origin;
			assignIfDefined(ir, "targetFile", placement.file);
		}
		ir.augments = statement.body.statements
			.filter((member) => ts.isInterfaceDeclaration(member) || ts.isClassDeclaration(member))
			.map((member) => {
				const members = collectMembers(member.members);
				const augmented = { name: member.name?.getText() ?? "<anonymous>" };
				const extended = (member.heritageClauses ?? [])
					.filter((clause) => clause.token === ts.SyntaxKind.ExtendsKeyword)
					.flatMap((clause) => clause.types.map(typeToIr));
				if (extended.length > 0) {
					augmented.extends = extended;
				}
				if (members.properties.length > 0) {
					augmented.properties = members.properties;
				}
				if (members.methods.length > 0) {
					augmented.methods = members.methods;
				}
				return augmented;
			});
		return ir;
	}

	/** `type Side = typeof FrontSide | typeof BackSide` - the grouping signal for loose constants. */
	function constantGroupOf(type) {
		if (type === undefined) {
			return undefined;
		}
		const members = type.kind === "union" ? type.types : [type];
		if (type.kind !== "union" || members.length < 2) {
			return undefined;
		}
		const names = [];
		for (const member of members) {
			if (member.kind !== "typeQuery" || member.target?.origin !== "in-scope" || member.target.refKind !== "constant") {
				return undefined;
			}
			names.push(member.name);
		}
		return names;
	}

	const sortByNameThenFile = (a, b) => byText(a.name, b.name) || byText(a.file, b.file);
	classes.sort(sortByNameThenFile);
	interfaces.sort(sortByNameThenFile);
	enums.sort(sortByNameThenFile);
	constants.sort(sortByNameThenFile);
	typeAliases.sort(sortByNameThenFile);
	functions.sort(sortByNameThenFile);
	namespaces.sort(sortByNameThenFile);
	moduleAugmentations.sort((a, b) => byText(a.targetModule, b.targetModule) || byText(a.file, b.file));

	const duplicateClassNames = [];
	const filesByClassName = new Map();
	for (const entry of classes) {
		filesByClassName.set(entry.name, [...(filesByClassName.get(entry.name) ?? []), entry.file]);
	}
	for (const [name, files] of [...filesByClassName].sort((a, b) => byText(a[0], b[0]))) {
		if (files.length > 1) {
			duplicateClassNames.push({ name, files });
		}
	}

	const typesPackageJson = JSON.parse(fs.readFileSync(path.join(TYPES_PACKAGE, "package.json"), "utf8"));
	const ir = {
		meta: {
			irVersion: IR_VERSION,
			generator: "generator/extractor/extract.mjs",
			schema: "generator/IR-SCHEMA.md",
			typesPackage: typesPackageJson.name,
			typesVersion: typesPackageJson.version,
			typescriptVersion: ts.version,
			sourceRoot: "src",
			excludedDirectories: EXCLUDED_DIRECTORIES.map((x) => `src/${x}`),
			counts: {
				filesScanned: inScopeFiles.length,
				filesExcluded: allFiles.length - inScopeFiles.length,
				classes: classes.length,
				interfaces: interfaces.length,
				enums: enums.length,
				constants: constants.length,
				constantsSkippedOutOfScope,
				typeAliases: typeAliases.length,
				functions: functions.length,
				namespaces: namespaces.length,
				moduleAugmentations: moduleAugmentations.length,
			},
			duplicateClassNames,
		},
		classes,
		interfaces,
		enums,
		constants,
		typeAliases,
		functions,
		namespaces,
		moduleAugmentations,
	};

	fs.mkdirSync(path.dirname(outputPath), { recursive: true });
	fs.writeFileSync(outputPath, `${JSON.stringify(ir, null, 2)}\n`, "utf8");

	const syntacticErrors = program.getSyntacticDiagnostics().length;
	console.log(`wrote ${toPosix(path.relative(REPO_ROOT, outputPath))}`);
	console.log(JSON.stringify(ir.meta.counts, null, 2));
	if (syntacticErrors > 0) {
		console.warn(`WARNING: ${syntacticErrors} syntactic diagnostics while parsing @types/three`);
	}
}

main();
