# `three-api.json` — IR schema

`generator/three-api.json` is a machine-readable snapshot of the three.js public API, extracted from
`@types/three` by `generator/extractor/extract.mjs`. It is the contract between the two halves of the
code generator: the Node extractor produces it, the C# emitter consumes it. **The emitter never parses
`.d.ts`** — every TypeScript-specific concern is resolved here.

The file is committed on purpose: its diff is how a three.js version bump gets reviewed.

## Regenerating

```
npm install
npm run extract
```

Output is byte-identical for the same inputs (see [Guarantees](#guarantees)), which is what lets
`npm run extract:check` re-extract in memory and fail on any difference from the committed snapshot.
CI runs it, so a change to the extractor cannot land without the regenerated artifact beside it.
Nothing under `generator/` is part of `src/Blazor.ThreeJS.slnx` — it is never built or shipped with
the package.

Inputs are pinned exactly (`@types/three@0.185.3`, `typescript@5.9.3`) so the artifact only moves when
somebody means it to.

## Scope

| | |
|---|---|
| Source | `@types/three@0.185.3`, `src/` only (`examples/` addons are out of scope) |
| Excluded | `src/nodes/**` — the TSL / WebGPU node stack |
| Files scanned | 315 |
| Classes | 309 |

Constants that are only re-export aliases into the excluded stack (the 637
`export const X: typeof TSL.X` lines in `src/Three.TSL.d.ts`) are dropped, and counted in
`meta.counts.constantsSkippedOutOfScope`. Types from the excluded stack are *not* hidden when an
in-scope member references one — the reference is kept and marked `origin: "excluded"` so the emitter
can decide what to do (see [Reference targets](#reference-targets)).

## Top level

```jsonc
{
  "meta":               { /* provenance + counts, see below */ },
  "classes":            [ /* ClassEntry     */ ],
  "interfaces":         [ /* InterfaceEntry */ ],
  "enums":              [ /* EnumEntry      */ ],
  "constants":          [ /* ConstantEntry  */ ],
  "typeAliases":        [ /* TypeAliasEntry */ ],
  "functions":          [ /* FunctionEntry  */ ],
  "namespaces":         [ /* NamespaceEntry */ ],
  "moduleAugmentations":[ /* AugmentationEntry */ ],
  "reExports":          [ /* ReExportEntry  */ ]
}
```

`classes` is the primary deliverable. `interfaces`, `typeAliases`, `enums` and `constants` are
supporting surface the emitter needs to resolve member types (a `MeshStandardMaterial` constructor
takes a `MeshStandardMaterialParameters` interface; a `side` property is typed by the `Side` alias).

### `meta`

```jsonc
{
  "irVersion": 1,                 // bump when this schema changes incompatibly
  "generator": "generator/extractor/extract.mjs",
  "schema": "generator/IR-SCHEMA.md",
  "typesPackage": "@types/three",
  "typesVersion": "0.185.3",
  "typescriptVersion": "5.9.3",
  "sourceRoot": "src",
  "excludedDirectories": [ { "path": "src/nodes", "files": 215, "classes": 118 } ],
  "addons": { "path": "examples/jsm", "files": 383, "classes": 383 },
  "publicSurface": { /* the barrel graph and the shipped bundle, see below */ },
  "counts": { /* filesScanned, classes, interfaces, reExports, numericKindMarkers, … */ },
  "duplicateClassNames": [ { "name": "PMREMGenerator", "files": ["…", "…"] } ]
}
```

`duplicateClassNames` lists names declared in more than one in-scope file (4 of them). Class entries
are **not** unique by name — key them by `name` + `file`.

`excludedDirectories` and `addons` describe what is deliberately **not** in this snapshot, counted
rather than asserted so the package's coverage table can state the size of each exclusion. Their
files are parsed with a standalone `createSourceFile` purely to count class declarations; nothing
from them enters the program, the checker, or any list below. A declared exclusion path that does not
exist **throws**: counting a moved directory as zero would publish a false completeness claim in the
README while the files it was holding back flooded the snapshot in the same run.

`counts.numericKindMarkers` is the number of `Expects a `Float`` / `Expects an `Integer`` markers read
out of the JSDoc — the sole source of the float/integer distinction, and prose rather than syntax. It
is reproducible by grepping the snapshot for `"numericKind"` plus `"returnNumericKind"`, and it is in
`meta` so that a DefinitelyTyped reflow silently zeroing it is a visible `243 → 0` line in the diff
rather than a run that looks entirely normal.

#### `meta.publicSurface`

```jsonc
{
  "barrel": "src/Three.d.ts",
  "runtimeBundle": "src/Blazor.ThreeJS/wwwroot/three.webgpu.min.js",
  "barrelFiles": 212,      // files reachable through the barrel graph
  "valueExports": 443,     // names the barrel publishes as values
  "typeOnlyExports": 235,  // names it publishes `type`-only
  "runtimeExports": 441,   // names on the shipped bundle's namespace
  "valueExportsAbsentFromRuntime": ["SRGBToLinear", "SourceJSON"],
  "runtimeExportsAbsentFromBarrel": []
}
```

Every `.d.ts` under `src/` uses the `export` keyword, so "does this file export it" answers yes for
everything and says only that a sibling module could import it. What decides whether a consumer can
reach a name is the barrel graph out of `src/Three.d.ts`, which is what `index.d.ts` re-exports and
therefore the public surface of the WebGL build this package ships — `src/Three.WebGPU.d.ts` is the
barrel for the build it does not. `isExported` on every entry below is resolved against that graph.

The two lists are the drift between the types and the runtime, and both are floors. `@types/three`
declares `SourceJSON` an `export class` where every other JSON shape is an `interface`, so the barrel
publishes a value three.js has no constructor for; `isRuntimeExport` is what keeps it out of the
generated surface. `runtimeExportsAbsentFromBarrel` being empty is the evidence that the barrel walk
is not missing anything the bundle has.

## `ClassEntry`

```jsonc
{
  "name": "BoxGeometry",
  "file": "src/geometries/BoxGeometry.d.ts",   // always POSIX, relative to the @types/three package
  "isExported": true,       // the public barrel re-exports it as a VALUE, not `type`-only
  "isRuntimeExport": true,  // the shipped three.js bundle puts the name on THREE
  "exportName": "…",        // only when the export name differs from the declared name
  "isDefaultExport": true,  // only when true
  "isAbstract": true,       // only when true
  "typeParameters": [ TypeParameter ],
  "extends": Type,          // absent for a root class (98 of them)
  "implements": [ Type ],
  "doc": Doc,
  "constructors": [ Signature ],
  "properties": [ Property ],
  "methods": [ Method ],
  "indexSignatures": [ Signature ]
}
```

`InterfaceEntry` has the same shape minus `constructors`/`isAbstract`, with `extends` as an **array**
(interfaces may extend several), plus optional `callSignatures` / `constructSignatures`.

### `Method`

Overloads are grouped under one entry; static and instance members with the same name stay separate.

```jsonc
{
  "name": "getInterpolation",
  "isStatic": true,      // only when true; likewise isOverride, isAbstract, isOptional
  "visibility": "protected",  // absent means public
  "overloads": [ Signature ]  // declaration order
}
```

### `Signature`

```jsonc
{
  "parameters": [ Parameter ],
  "typeParameters": [ TypeParameter ],
  "returnType": Type,             // absent on constructors
  "returnNumericKind": "float",   // see Numeric kinds
  "doc": Doc
}
```

### `Parameter`

```jsonc
{
  "name": "widthSegments",
  "type": Type,
  "isOptional": true,       // `?` or an initializer
  "isRest": true,           // only when true
  "numericKind": "integer", // see below — absent means unspecified
  "defaultValue": "1",      // documented default, verbatim from JSDoc, as a string
  "doc": "Number of segmented rectangular faces along the width of the sides.",
  "docSource": "position"   // only when the doc was matched positionally, see below
}
```

`doc` on a parameter is **plain text**, not a `Doc` object.

### `Property`

```jsonc
{
  "name": "id",
  "type": Type,
  "isStatic": true, "isReadonly": true, "isOptional": true,
  "isOverride": true, "isAbstract": true,   // each present only when true
  "accessor": "get" | "set" | "get-set",    // absent for a plain field
  "visibility": "protected",
  "numericKind": "integer",
  "defaultValue": "Object3D",
  "doc": Doc
}
```

A getter-only accessor also carries `isReadonly: true`. A get/set pair is merged into one entry with
`accessor: "get-set"`.

## Numeric kinds

⚠️ **This is the field the whole numeric type mapping depends on.** TypeScript erases both integers
and floats to `number`; the JSDoc does not. Wherever the upstream docs say ``Expects a `Float` `` or
``Expects a `Integer` ``, the member carries:

```jsonc
"numericKind": "float" | "integer"
```

**Absent means unspecified, not float.** Only the older DefinitelyTyped-authored docs (40 files, 245
markers) carry the marker; classes documented from three.js's own newer JSDoc say only `{number}`.
The emitter needs a stated default for the unspecified case — that is a plan-level decision, not
something this IR invents.

`BoxGeometry`, the canonical case:

| parameter | TS type | `numericKind` | `defaultValue` |
|---|---|---|---|
| `width`, `height`, `depth` | `number` | `float` | `1` |
| `widthSegments`, `heightSegments`, `depthSegments` | `number` | `integer` | `1` |

Where the marker is read from:

- **parameters** — the matching `@param` tag's text only, so a method with an integer parameter never
  makes the method itself look integer-typed.
- **properties and inline object members** — the summary, `@remarks` and `@defaultValue` text (never
  `@param`/`@returns`).
- **return values** — the `@returns` text, surfaced as `returnNumericKind`.

243 of the 245 markers present in the in-scope sources land in the IR. The 2 that do not are in
`src/extras/curves/ArcCurve.d.ts`, whose JSDoc documents a 7-parameter `EllipseCurve` signature for a
6-parameter constructor — the markers describe parameters that do not exist.

### `docSource: "position"`

Some upstream `@param` names have drifted from the signature (`Path.absarc(aX, aY, …)` is documented
as `@param x`, `@param y`, …). Tags are matched by name first; positional matching is used as a
fallback **only** when the tag count equals the parameter count *and* every name that does match sits
at its own index, so name and position can never disagree. Parameters resolved this way are flagged
`docSource: "position"` (52 of them) so the inference stays auditable. When the counts disagree the
fallback declines and the doc is dropped rather than guessed.

## `Doc`

Present only when the declaration actually carries JSDoc; every field is omitted when empty.

```jsonc
{
  "summary": "…",            // `{@link Target label}` markers are preserved verbatim
  "remarks": "…",
  "examples": ["```typescript\n…\n```"],
  "returns": "…",
  "defaultValue": "1",       // from @defaultValue or @default, backticks stripped
  "see": [ { "url": "https://threejs.org/docs/…", "label": "Official Documentation" } ],
  "isDeprecated": true, "deprecated": "…",
  "isInternal": true,        // @internal — kept, not filtered; the emitter decides
  "tags": [ { "name": "augments", "text": "NodeLibrary" } ]   // anything not handled above
}
```

`summary` keeps `{@link …}` markers so the emitter can rewrite them as `<see cref="…"/>`; it must
escape XML-significant characters itself. This text is what makes
`GenerateDocumentationFile=true` viable across 309 classes × 5 TFMs without a CS1591 storm.

## `Type`

Types are modelled from **syntax**, not from the checker's resolved types, so `ColorRepresentation`
stays `ColorRepresentation` instead of being expanded to `Color | string | number`. Every node has a
`kind` and a `text` (the source text, whitespace-collapsed) — `text` is a faithful fallback for cases
the emitter chooses not to model structurally.

| `kind` | extra fields |
|---|---|
| `primitive` | `name`: `number`, `string`, `boolean`, `void`, `any`, `unknown`, `never`, `undefined`, `null`, `object`, `symbol`, `bigint`, `this` |
| `literal` | `literalKind`: `string`/`number`/`boolean`/`other`, `value` |
| `reference` | `name`, `typeArguments`, `target` (see below) |
| `array` | `element` |
| `tuple` | `elements` |
| `namedTupleMember` | `name`, `element` |
| `optional`, `rest` | `element` |
| `union`, `intersection` | `types` |
| `function`, `constructorType` | `parameters`, `returnType`, `typeParameters` |
| `object` | `members` — anonymous type literal, see below |
| `typeOperator` | `operator`: `keyof`/`readonly`/`unique`, `type` |
| `indexedAccess` | `objectType`, `indexType` |
| `typeQuery` | `name`, `target` — `typeof X` |
| `typePredicate` | `parameterName`, `type` |
| `conditional`, `mapped`, `templateLiteral`, `importType`, `infer` | `text` only |
| `unsupported` | `syntaxKind`, `text` — nothing in the current snapshot produces this |

Parenthesised types are unwrapped.

`object` members carry `memberKind` (`property`, `method`, `index`, `call`, `construct`, `other`).
Property members also carry `isOptional`, `isReadonly`, `numericKind`, `defaultValue` and `doc` — the
inline object types (`LOD.levels`, `PerspectiveCamera.view`, `BoxGeometry.parameters`) document their
own fields and those docs are not lost.

### Reference targets

`reference` and `typeQuery` nodes carry a `target` saying what the name resolves to and where it
lives. This is how the emitter knows a type is a DOM handle it cannot mirror.

```jsonc
"target": { "refKind": "class", "origin": "in-scope", "file": "src/core/BufferGeometry.d.ts" }
```

- `refKind`: `class`, `interface`, `enum`, `typeAlias`, `typeParameter`, `constant`, `function`,
  `namespace`, `other`, `unresolved`
- `origin`:
  - `in-scope` — declared in a scanned file, has an entry in this IR (`file` present)
  - `excluded` — declared under `src/nodes/**` (`file` present); ~88 references from in-scope members
  - `package` — elsewhere in `@types/three` but not scanned
  - `lib` — a TypeScript lib type (`ArrayLike`, `HTMLCanvasElement`, `WebGL2RenderingContext`, …); ~535 references
  - `external` — another package
  - `unresolved` — the checker could not resolve it

## Constants, enums and aliases

`src/constants.d.ts` mixes two things and they are kept distinguishable.

**Real TS enums** (`MOUSE`, `TOUCH`, plus 25 WebGPU ones) →

```jsonc
{ "name": "MOUSE", "file": "src/constants.d.ts", "isExported": true, "isConst": false,
  "members": [ { "name": "LEFT", "value": 0, "initializerText": "0", "doc": Doc } ] }
```

`value` is the checker-computed constant value; `initializerText` is the source text. Note `MOUSE` has
duplicate values (`LEFT`/`ROTATE` are both `0`) — it is not a valid C# enum without dropping or
aliasing members.

**Loose `export const`s** (209 in `constants.d.ts`, 17 elsewhere) →

```jsonc
{ "name": "FrontSide", "file": "src/constants.d.ts", "isExported": true, "isConst": true,
  "type": { "kind": "literal", "literalKind": "number", "value": 0, "text": "0" } }
```

**Type aliases that group them** — the grouping signal for turning loose constants into C# enums. When
every member of an alias's union is a `typeof` of an in-scope constant, the alias carries a
`constantGroup` naming them (36 aliases do):

```jsonc
{ "name": "Side", "file": "src/constants.d.ts",
  "constantGroup": ["FrontSide", "BackSide", "DoubleSide"],
  "type": { "kind": "union", … }, "doc": Doc }
```

## Functions, namespaces, module augmentations

`FunctionEntry` is `{ name, file, isExported, overloads: [Signature], doc }` — 65 top-level functions
(`MathUtils`-style helpers, `AnimationUtils`, colour-space converters).

`NamespaceEntry` records `declare namespace` blocks (only `PropertyBinding`) as `{ name, file,
isExported, members: [{ name, declarationKind }] }`.

`AugmentationEntry` records `declare module "…" { interface X { … } }` declaration merging — 15 of
them, all adding node-stack properties to existing types:

```jsonc
{ "file": "src/renderers/common/Renderer.d.ts",
  "targetModule": "../../scenes/Scene.js",
  "targetOrigin": "in-scope", "targetFile": "src/scenes/Scene.d.ts",
  "augments": [ { "name": "Scene", "extends": [Type], "properties": [Property], "methods": [Method] } ] }
```

⚠️ A class's real member set is its own entry **plus** any augmentation targeting it. `Scene` and
`Object3D` both get extra members this way. All 15 pull in TSL types, so the emitter most likely skips
them — but it has to know they exist rather than silently disagree with the runtime.

## Re-exports

`ReExportEntry` records every `export … from` / `export { … }` statement — 359 of them. The five barrel
files are nothing else, so without this entry they contribute no row to the snapshot at all while
still counting towards `filesScanned`:

```jsonc
{ "file": "src/Three.Core.d.ts", "module": "./lights/DirectionalLightShadow.js",
  "targetOrigin": "in-scope", "targetFile": "src/lights/DirectionalLightShadow.d.ts",
  "isTypeOnly": true,          // only when true
  "kind": "named",             // "star" | "named" | "namespace"
  "names": [ { "name": "DirectionalLightShadow", "localName": "…", "isValue": false } ] }
```

`isValue` is false when the name is re-exported `type`-only, or when it resolves to something that is
only a type. Following these edges out of `src/Three.d.ts` is what produces
[`meta.publicSurface`](#metapublicsurface).

## Guarantees

- **Deterministic.** Two runs produce byte-identical output. Top-level arrays are sorted by name then
  file with a plain codepoint comparator (no locale); members keep declaration order, which is stable.
- **No machine-specific data.** Paths are POSIX and relative to the `@types/three` package root; there
  are no timestamps and no absolute paths.
- **Empty means absent.** Optional fields are omitted rather than emitted as `null`/`[]`/`{}`. Boolean
  flags appear only when `true`. Read every field with a default.
- Trailing newline, 2-space indent, UTF-8.

## Things that will complicate the emitter

1. **Unspecified numeric kinds.** Only `meta.counts.numericKindMarkers` (243) declarations are marked;
   everything else is a bare `number` and needs a project-wide default decision.
2. **`origin: "lib"` types.** ~535 references to DOM/ES lib types (`HTMLCanvasElement`, `TypedArray`,
   `ArrayLike<number>`, `WebGL2RenderingContext`). They have no C# mirror.
3. **Generics.** 44 classes are generic, most via the `EventDispatcher<TEventMap>` chain that
   `Object3D` sits on, so genericity propagates through the whole object hierarchy.
4. **Pseudo-overloads via rest tuples.** `Color.set(...args: [color: ColorRepresentation] | [r, g, b])`
   is one TS signature that means two C# overloads. Detect `isRest` + `union` of `tuple`.
5. **Real overloads.** 5 classes have constructor overloads; `Triangle.getInterpolation` has 3 both as
   an instance and a static method (same name, different `isStatic`).
6. **Duplicate class names** across files (4), and 93 classes the public barrel does not publish as a
   value — the WebGPU / node stack plus the `LightShadow` subclasses `@types/three` exports `type`-only.
7. **Literal-union `type` properties.** 45 classes declare `override readonly type: string | "Mesh"` —
   a widened string literal, not an enum.
8. **`MOUSE` has duplicate enum values**, which C# permits only as explicit aliases.
9. **Scope inside scope.** Roughly a third of the 309 classes are renderer internals
   (`renderers/webgl` 25, `renderers/common` 22, `materials/nodes` 18, `renderers/webgl-fallback`,
   `renderers/webxr`, `lights/webgpu`) rather than the user-facing three.js API. They are in the IR
   because they are in `src/` and not under `src/nodes/`; deciding which ones to emit is a separate
   call.
