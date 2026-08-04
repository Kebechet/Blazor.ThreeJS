import ts from "typescript";
import { getDoc, memberDocText, numericKindFrom } from "./jsdoc.mjs";

/**
 * Turns TypeScript type *syntax* into a structured IR.
 *
 * Syntax rather than checker types on purpose: `ColorRepresentation` must stay
 * `ColorRepresentation` for the emitter to map, not get expanded to `Color | string | number`.
 * The checker is used only to classify what a named reference resolves to.
 */

const KEYWORD_TYPES = new Map([
	[ts.SyntaxKind.AnyKeyword, "any"],
	[ts.SyntaxKind.UnknownKeyword, "unknown"],
	[ts.SyntaxKind.NumberKeyword, "number"],
	[ts.SyntaxKind.BigIntKeyword, "bigint"],
	[ts.SyntaxKind.ObjectKeyword, "object"],
	[ts.SyntaxKind.BooleanKeyword, "boolean"],
	[ts.SyntaxKind.StringKeyword, "string"],
	[ts.SyntaxKind.SymbolKeyword, "symbol"],
	[ts.SyntaxKind.VoidKeyword, "void"],
	[ts.SyntaxKind.UndefinedKeyword, "undefined"],
	[ts.SyntaxKind.NeverKeyword, "never"],
	[ts.SyntaxKind.IntrinsicKeyword, "intrinsic"],
]);

function normalizeText(node) {
	return node.getText().replace(/\s+/g, " ").trim();
}

export function entityNameToString(name) {
	if (ts.isIdentifier(name)) {
		return name.text;
	}
	if (ts.isQualifiedName(name)) {
		return `${entityNameToString(name.left)}.${name.right.text}`;
	}
	return name.getText();
}

function rightMostName(name) {
	return ts.isQualifiedName(name) ? name.right : name;
}

export function createTypeMapper(context) {
	const { checker, classifyDeclarationFile } = context;

	/** Resolves a named type/value reference to what it actually is and where it lives. */
	function resolveReference(entityName) {
		const target = rightMostName(entityName);
		let symbol = checker.getSymbolAtLocation(target);
		if (symbol !== undefined && (symbol.flags & ts.SymbolFlags.Alias) !== 0) {
			try {
				symbol = checker.getAliasedSymbol(symbol);
			} catch {
				/* keep the alias symbol */
			}
		}
		const declarations = symbol?.declarations ?? [];
		if (declarations.length === 0) {
			return { refKind: "unresolved", origin: "unresolved" };
		}
		const declaration = declarations.find(
			(x) =>
				ts.isClassDeclaration(x) ||
				ts.isInterfaceDeclaration(x) ||
				ts.isEnumDeclaration(x) ||
				ts.isTypeAliasDeclaration(x) ||
				ts.isTypeParameterDeclaration(x),
		) ?? declarations[0];

		let refKind = "other";
		if (ts.isClassDeclaration(declaration)) {
			refKind = "class";
		} else if (ts.isInterfaceDeclaration(declaration)) {
			refKind = "interface";
		} else if (ts.isEnumDeclaration(declaration)) {
			refKind = "enum";
		} else if (ts.isTypeAliasDeclaration(declaration)) {
			refKind = "typeAlias";
		} else if (ts.isTypeParameterDeclaration(declaration)) {
			refKind = "typeParameter";
		} else if (ts.isVariableDeclaration(declaration)) {
			refKind = "constant";
		} else if (ts.isFunctionDeclaration(declaration)) {
			refKind = "function";
		} else if (ts.isModuleDeclaration(declaration)) {
			refKind = "namespace";
		}

		const placement = classifyDeclarationFile(declaration.getSourceFile().fileName);
		const reference = { refKind, origin: placement.origin };
		if (placement.file !== undefined) {
			reference.file = placement.file;
		}
		return reference;
	}

	function parameterToIr(parameter) {
		const ir = { name: parameter.name.getText() };
		if (parameter.dotDotDotToken !== undefined) {
			ir.isRest = true;
		}
		ir.isOptional = parameter.questionToken !== undefined || parameter.initializer !== undefined;
		const type = typeToIr(parameter.type);
		if (type !== undefined) {
			ir.type = type;
		}
		return ir;
	}

	function typeParametersToIr(typeParameters) {
		if (typeParameters === undefined || typeParameters.length === 0) {
			return undefined;
		}
		return typeParameters.map((typeParameter) => {
			const ir = { name: typeParameter.name.text };
			const constraint = typeToIr(typeParameter.constraint);
			if (constraint !== undefined) {
				ir.constraint = constraint;
			}
			const defaultType = typeToIr(typeParameter.default);
			if (defaultType !== undefined) {
				ir.default = defaultType;
			}
			return ir;
		});
	}

	function typeLiteralMemberToIr(member) {
		if (ts.isPropertySignature(member)) {
			const ir = { name: member.name.getText(), memberKind: "property" };
			if (member.questionToken !== undefined) {
				ir.isOptional = true;
			}
			if (member.modifiers?.some((x) => x.kind === ts.SyntaxKind.ReadonlyKeyword)) {
				ir.isReadonly = true;
			}
			const type = typeToIr(member.type);
			if (type !== undefined) {
				ir.type = type;
			}
			const doc = getDoc(member);
			const numericKind = numericKindFrom(memberDocText(doc));
			if (numericKind !== undefined) {
				ir.numericKind = numericKind;
			}
			if (doc?.defaultValue !== undefined) {
				ir.defaultValue = doc.defaultValue;
			}
			if (doc !== undefined) {
				ir.doc = doc;
			}
			return ir;
		}
		if (ts.isMethodSignature(member)) {
			return {
				name: member.name.getText(),
				memberKind: "method",
				parameters: member.parameters.map(parameterToIr),
				returnType: typeToIr(member.type),
			};
		}
		if (ts.isIndexSignatureDeclaration(member)) {
			return {
				memberKind: "index",
				parameters: member.parameters.map(parameterToIr),
				returnType: typeToIr(member.type),
			};
		}
		if (ts.isCallSignatureDeclaration(member)) {
			return {
				memberKind: "call",
				parameters: member.parameters.map(parameterToIr),
				returnType: typeToIr(member.type),
			};
		}
		if (ts.isConstructSignatureDeclaration(member)) {
			return {
				memberKind: "construct",
				parameters: member.parameters.map(parameterToIr),
				returnType: typeToIr(member.type),
			};
		}
		return { memberKind: "other", text: normalizeText(member) };
	}

	function typeToIr(node) {
		if (node === undefined) {
			return undefined;
		}
		if (ts.isParenthesizedTypeNode(node)) {
			return typeToIr(node.type);
		}

		const text = normalizeText(node);
		const keyword = KEYWORD_TYPES.get(node.kind);
		if (keyword !== undefined) {
			return { kind: "primitive", name: keyword, text };
		}
		if (node.kind === ts.SyntaxKind.ThisType) {
			return { kind: "primitive", name: "this", text };
		}

		if (ts.isLiteralTypeNode(node)) {
			const literal = node.literal;
			if (literal.kind === ts.SyntaxKind.NullKeyword) {
				return { kind: "primitive", name: "null", text };
			}
			if (literal.kind === ts.SyntaxKind.TrueKeyword || literal.kind === ts.SyntaxKind.FalseKeyword) {
				return { kind: "literal", literalKind: "boolean", value: literal.kind === ts.SyntaxKind.TrueKeyword, text };
			}
			if (ts.isStringLiteral(literal)) {
				return { kind: "literal", literalKind: "string", value: literal.text, text };
			}
			if (ts.isNumericLiteral(literal)) {
				return { kind: "literal", literalKind: "number", value: Number(literal.text), text };
			}
			if (ts.isPrefixUnaryExpression(literal) && ts.isNumericLiteral(literal.operand)) {
				const sign = literal.operator === ts.SyntaxKind.MinusToken ? -1 : 1;
				return { kind: "literal", literalKind: "number", value: sign * Number(literal.operand.text), text };
			}
			return { kind: "literal", literalKind: "other", text };
		}

		if (ts.isTypeReferenceNode(node)) {
			const ir = {
				kind: "reference",
				name: entityNameToString(node.typeName),
				text,
				target: resolveReference(node.typeName),
			};
			if (node.typeArguments !== undefined && node.typeArguments.length > 0) {
				ir.typeArguments = node.typeArguments.map(typeToIr);
			}
			return ir;
		}

		if (ts.isArrayTypeNode(node)) {
			return { kind: "array", element: typeToIr(node.elementType), text };
		}
		if (ts.isTupleTypeNode(node)) {
			return { kind: "tuple", elements: node.elements.map(typeToIr), text };
		}
		if (ts.isNamedTupleMember(node)) {
			return { kind: "namedTupleMember", name: node.name.text, element: typeToIr(node.type), text };
		}
		if (ts.isOptionalTypeNode(node)) {
			return { kind: "optional", element: typeToIr(node.type), text };
		}
		if (ts.isRestTypeNode(node)) {
			return { kind: "rest", element: typeToIr(node.type), text };
		}
		if (ts.isUnionTypeNode(node)) {
			return { kind: "union", types: node.types.map(typeToIr), text };
		}
		if (ts.isIntersectionTypeNode(node)) {
			return { kind: "intersection", types: node.types.map(typeToIr), text };
		}
		if (ts.isFunctionTypeNode(node) || ts.isConstructorTypeNode(node)) {
			const ir = {
				kind: ts.isConstructorTypeNode(node) ? "constructorType" : "function",
				parameters: node.parameters.map(parameterToIr),
				returnType: typeToIr(node.type),
				text,
			};
			const typeParameters = typeParametersToIr(node.typeParameters);
			if (typeParameters !== undefined) {
				ir.typeParameters = typeParameters;
			}
			return ir;
		}
		if (ts.isTypeLiteralNode(node)) {
			return { kind: "object", members: node.members.map(typeLiteralMemberToIr), text };
		}
		if (ts.isTypeOperatorNode(node)) {
			const operators = {
				[ts.SyntaxKind.KeyOfKeyword]: "keyof",
				[ts.SyntaxKind.ReadonlyKeyword]: "readonly",
				[ts.SyntaxKind.UniqueKeyword]: "unique",
			};
			return { kind: "typeOperator", operator: operators[node.operator] ?? "unknown", type: typeToIr(node.type), text };
		}
		if (ts.isIndexedAccessTypeNode(node)) {
			return {
				kind: "indexedAccess",
				objectType: typeToIr(node.objectType),
				indexType: typeToIr(node.indexType),
				text,
			};
		}
		if (ts.isTypeQueryNode(node)) {
			return {
				kind: "typeQuery",
				name: entityNameToString(node.exprName),
				target: resolveReference(node.exprName),
				text,
			};
		}
		if (ts.isTypePredicateNode(node)) {
			return { kind: "typePredicate", parameterName: node.parameterName.getText(), type: typeToIr(node.type), text };
		}
		if (ts.isConditionalTypeNode(node)) {
			return { kind: "conditional", text };
		}
		if (ts.isMappedTypeNode(node)) {
			return { kind: "mapped", text };
		}
		if (ts.isTemplateLiteralTypeNode(node)) {
			return { kind: "templateLiteral", text };
		}
		if (ts.isImportTypeNode(node)) {
			return { kind: "importType", text };
		}
		if (ts.isInferTypeNode(node)) {
			return { kind: "infer", text };
		}
		if (ts.isExpressionWithTypeArguments(node)) {
			const ir = { kind: "reference", name: normalizeText(node.expression), text };
			if (ts.isIdentifier(node.expression) || ts.isPropertyAccessExpression(node.expression)) {
				ir.target = resolveReference(
					ts.isIdentifier(node.expression) ? node.expression : node.expression.name,
				);
			}
			if (node.typeArguments !== undefined && node.typeArguments.length > 0) {
				ir.typeArguments = node.typeArguments.map(typeToIr);
			}
			return ir;
		}

		return { kind: "unsupported", syntaxKind: ts.SyntaxKind[node.kind], text };
	}

	return { typeToIr, parameterToIr, typeParametersToIr, resolveReference };
}
