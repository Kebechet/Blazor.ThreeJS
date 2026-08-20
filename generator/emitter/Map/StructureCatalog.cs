using Blazor.ThreeJS.Emitter.Ir;

namespace Blazor.ThreeJS.Emitter.Map;

/// <summary>
/// The structural interfaces the mirror represents as generated records rather than refusing.
/// <para>
/// three.js describes some of what it hands back with an interface rather than a class:
/// <c>BufferGeometry.groups</c> is a <c>GeometryGroup[]</c>, and a <c>GeometryGroup</c> is
/// <c>{ start, count, materialIndex? }</c>. Those are values with no identity - two groups holding the
/// same numbers are the same group, and nothing on either side keeps a reference to one - so a handle
/// is the wrong shape for them and a record is the right one.
/// </para>
/// <para>
/// ⚠️ Deliberately narrow. An interface qualifies only when every one of its properties maps on its own,
/// it declares no methods, no type parameters and extends nothing, and it carries no member the mirror
/// would have to invent a representation for. Anything else stays refused and is reported, because a
/// half-represented structure is worse than a named absence: the caller cannot tell which half arrived.
/// </para>
/// </summary>
internal sealed class StructureCatalog
{
	private readonly IReadOnlyDictionary<string, IrInterface> _interfacesByName;
	private readonly Dictionary<string, bool> _qualifies = new(StringComparer.Ordinal);
	private readonly HashSet<string> _used = new(StringComparer.Ordinal);
	private readonly Dictionary<string, AnonymousStructure> _anonymousByShape = new(StringComparer.Ordinal);

	/// <summary>Builds a catalogue over one IR snapshot.</summary>
	/// <param name="ir">The parsed IR.</param>
	public StructureCatalog(IrRoot ir)
	{
		var byName = new Dictionary<string, IrInterface>(StringComparer.Ordinal);
		foreach (var irInterface in ir.Interfaces)
		{
			byName.TryAdd(irInterface.Name, irInterface);
		}

		_interfacesByName = byName;
	}

	/// <summary>
	/// Every interface some emitted member actually named, in name order. Only these are emitted: a
	/// record nothing references would be a public type carrying no capability.
	/// </summary>
	public IReadOnlyList<IrInterface> Used
	{
		get
		{
			return _used
				.Select(x => _interfacesByName[x])
				.OrderBy(x => x.Name, StringComparer.Ordinal)
				.ToList();
		}
	}

	/// <summary>
	/// Whether this interface is one the mirror represents, recording the use so it gets emitted.
	/// </summary>
	/// <param name="name">Interface name.</param>
	/// <param name="mapper">Type mapper, used to test each property.</param>
	/// <returns><see langword="true"/> when a record stands for it.</returns>
	public bool TryUse(string name, TypeMapper mapper)
	{
		if (!Qualifies(name, mapper))
		{
			return false;
		}

		_used.Add(name);
		return true;
	}

	/// <summary>
	/// Whether an interface is plain enough to be a record, without recording a use. Memoised, because
	/// the emission-scope fixpoint asks the same question many times.
	/// </summary>
	/// <param name="name">Interface name.</param>
	/// <param name="mapper">Type mapper, used to test each property.</param>
	/// <returns><see langword="true"/> when every rule holds.</returns>
	public bool Qualifies(string name, TypeMapper mapper)
	{
		if (_qualifies.TryGetValue(name, out var known))
		{
			return known;
		}

		var answer = Test(name, mapper);
		_qualifies[name] = answer;
		return answer;
	}

	/// <summary>Every anonymous shape some emitted member named, in name order.</summary>
	public IReadOnlyList<AnonymousStructure> UsedAnonymous
	{
		get
		{
			return _anonymousByShape.Values
				.OrderBy(x => x.Name, StringComparer.Ordinal)
				.ToList();
		}
	}

	/// <summary>
	/// The C# name for an inline object shape, registering it so it gets emitted, or
	/// <see langword="null"/> when the shape is not one a record can stand for.
	/// <para>
	/// three.js writes a good deal of its surface as shapes with no name -
	/// <c>Curve.computeFrenetFrames</c> returns <c>{ tangents, normals, binormals }</c>, every geometry
	/// echoes its constructor arguments back as <c>parameters</c>. Refusing them for want of a name left
	/// forty-one members unreachable, which is a worse answer than a name chosen by rule.
	/// </para>
	/// <para>
	/// ⚠️ Keyed by the <em>shape</em>, not by the member. Ten curve classes return the same frenet-frame
	/// shape, and they share one record rather than getting ten identical ones - which is also what makes
	/// the name stable: it is settled by the first declarer in ordinal order, and ordinal order does not
	/// depend on which member the mapper happened to reach first.
	/// </para>
	/// </summary>
	/// <param name="type">The inline object node.</param>
	/// <param name="context">Declaring member and class, which the name is built from.</param>
	/// <param name="mapper">Type mapper, used to test each member.</param>
	/// <returns>The record's name, or <see langword="null"/>.</returns>
	public string? TryUseAnonymous(IrType type, TypeMappingContext context, TypeMapper mapper)
	{
		var members = type.Members.Where(x => x.MemberKind == "property").ToList();
		if (members.Count == 0 || members.Count != type.Members.Count)
		{
			return null;
		}

		// Named before its members are mapped, so a shape nested inside this one is named after it:
		// `AnimationMixerStats.actions` is `{ total, inUse }`, which on its own would be `Actions` - a
		// word too generic to put in a namespace beside three.js's own types.
		var name = BuildName(context);

		var resolved = new List<AnonymousStructureMember>();
		foreach (var member in members)
		{
			if (member.Name is not { Length: > 0 } memberName)
			{
				return null;
			}

			var mapping = mapper.Map(member.Type, new TypeMappingContext
			{
				MemberName = memberName,
				NumericKind = member.NumericKind,
				DeclaringClassName = name
			});

			if (!mapping.IsMapped || !(TypeMapper.IsWireValue(mapping) || IsMirroredObject(mapping)))
			{
				return null;
			}

			resolved.Add(new AnonymousStructureMember(memberName, mapping, member.IsOptional, member.Doc));
		}

		var shapeKey = string.Join(
			";",
			resolved.Select(x => $"{x.Name}:{x.Mapping.CSharpTypeName}{(x.IsOptional ? "?" : string.Empty)}"));

		if (_anonymousByShape.TryGetValue(shapeKey, out var existing))
		{
			return existing.Name;
		}

		var structure = new AnonymousStructure
		{
			Name = name,
			Members = resolved
		};

		_anonymousByShape[shapeKey] = structure;
		return structure.Name;
	}

	/// <summary>
	/// The name for a shape, built from the member that declares it.
	/// <para>
	/// <c>BoxGeometry.parameters</c> becomes <c>BoxGeometryParameters</c>: the class as well as the
	/// member, because sixteen geometries each call their own distinct shape <c>parameters</c> and the
	/// member alone would collide sixteen ways. Where a member's own name already reads as the thing -
	/// <c>computeFrenetFrames</c> - the verb is dropped and the class is not needed, since the shape is
	/// shared by every class that returns it.
	/// </para>
	/// </summary>
	/// <param name="context">Declaring member and class.</param>
	/// <returns>The C# type name.</returns>
	private static string BuildName(TypeMappingContext context)
	{
		var member = context.MemberName ?? "Structure";
		foreach (var verb in EmitterConfig.StructureNameVerbPrefixes)
		{
			if (!member.StartsWith(verb, StringComparison.Ordinal) || member.Length <= verb.Length)
			{
				continue;
			}

			// ⚠️ Class-free only where the member's own name is already a compound. `computeFrenetFrames`
			// gives `FrenetFrames`, which says what the shape is and is right for all ten curves that
			// return it. `getActions` would give `Actions`, which says nothing - so that one keeps its
			// class and becomes `AnimationMixerStatsActions`. The test is an internal capital, which is
			// what a compound has and a bare noun does not.
			var stripped = member[verb.Length..];
			if (stripped.Skip(1).Any(char.IsUpper))
			{
				return Pascal(stripped);
			}

			member = stripped;
			break;
		}

		return Pascal(context.DeclaringClassName ?? string.Empty) + Pascal(member);
	}

	/// <summary>Upper-cases the first character, leaving an empty string alone.</summary>
	/// <param name="value">The name to case.</param>
	/// <returns>The PascalCased name.</returns>
	private static string Pascal(string value)
	{
		return value.Length == 0 ? value : char.ToUpperInvariant(value[0]) + value[1..];
	}

	/// <summary>Whether a member is a mirrored object, which travels inside a structure as a handle.</summary>
	/// <param name="mapping">The resolved member type.</param>
	/// <returns><see langword="true"/> when the wire carries it as a reference.</returns>
	internal static bool IsMirroredObject(TypeMapping mapping)
	{
		return mapping.Kind == TypeMappingKind.GeneratedWrapperClass;
	}

	/// <summary>The properties of a qualifying interface, in declaration order.</summary>
	/// <param name="name">Interface name.</param>
	/// <returns>The properties.</returns>
	public IReadOnlyList<IrProperty> PropertiesOf(string name)
	{
		return PropertiesOf(_interfacesByName[name]);
	}

	/// <summary>
	/// An interface's own properties plus every one it inherits, base-first.
	/// <para>
	/// A record standing for `CurvePathJSON` has to carry `CurveJSON`'s members too - three.js sends one
	/// flat object, not a nested one, and a record holding only the half declared here would bind that
	/// half and silently drop the rest. Flattening is what makes the C# type the same shape as the JSON.
	/// </para>
	/// <para>
	/// A base declared but not in the snapshot ends the walk, and a name already seen ends it too, so a
	/// cycle in the declarations cannot spin here.
	/// </para>
	/// </summary>
	/// <param name="irInterface">The interface.</param>
	/// <returns>Its full property set, each name appearing once.</returns>
	private IReadOnlyList<IrProperty> PropertiesOf(IrInterface irInterface)
	{
		var properties = new List<IrProperty>();
		var seenNames = new HashSet<string>(StringComparer.Ordinal);
		var seenInterfaces = new HashSet<string>(StringComparer.Ordinal) { irInterface.Name };

		void Collect(IrInterface current)
		{
			foreach (var extended in current.Extends)
			{
				if (extended.Name is { } baseName
					&& seenInterfaces.Add(baseName)
					&& _interfacesByName.TryGetValue(baseName, out var baseInterface))
				{
					Collect(baseInterface);
				}
			}

			foreach (var property in current.Properties)
			{
				// A name redeclared on the derived interface wins, which is what TypeScript means by it.
				if (seenNames.Add(property.Name))
				{
					properties.Add(property);
				}
			}
		}

		Collect(irInterface);
		return properties;
	}

	/// <summary>Applies every rule. Split out so <see cref="Qualifies"/> is only the memoisation.</summary>
	/// <param name="name">Interface name.</param>
	/// <param name="mapper">Type mapper.</param>
	/// <returns>Whether it qualifies.</returns>
	private bool Test(string name, TypeMapper mapper)
	{
		if (!_interfacesByName.TryGetValue(name, out var irInterface))
		{
			return false;
		}

		// A method needs a receiver, which is exactly what a value with no identity does not have.
		// Extending another interface, or taking a type parameter, both mean the member set here is not
		// the whole story - and a record standing for part of a shape is the failure this guards against.
		// A type parameter is fine, and erases the way a class's does - `Intersection<TIntersected>`
		// defaults to `Object3D`, which is what every raycast against the mirror produces anyway. A
		// method needs a receiver, which a value with no identity does not have.
		if (irInterface.Methods.Count > 0 || PropertiesOf(irInterface) is { Count: 0 })
		{
			return false;
		}

		foreach (var property in PropertiesOf(irInterface))
		{
			// Read through the mapper rather than pattern-matched here, so a record's members are exactly
			// as expressible as any other member - and so this stays right as the mapper gains rules.
			var mapping = mapper.Map(property.Type, new TypeMappingContext
			{
				MemberName = property.Name,
				NumericKind = property.NumericKind,
				TypeParameters = irInterface.TypeParameters,

				// The interface names anything nested inside it. `AnimationMixerStats.actions` is
				// `{ total, inUse }`, which alone would be `Actions` - a word too generic to sit in a
				// namespace beside three.js's own types.
				DeclaringClassName = name
			});

			// Values, or a mirrored object carried by handle. The applier mints one while encoding a read
			// result - `Raycaster.intersectObject` answers `{ distance, point, object }`, and `object` is
			// the mesh that was hit, which has identity a copy of its fields would lose.
			if (!mapping.IsMapped || !(TypeMapper.IsWireValue(mapping) || IsMirroredObject(mapping)))
			{
				return false;
			}
		}

		return true;
	}

}

/// <summary>An inline object shape the mirror stands for with a generated record.</summary>
internal sealed class AnonymousStructure
{
	/// <summary>C# name for the record.</summary>
	public required string Name { get; init; }

	/// <summary>Its members, in declaration order.</summary>
	public required IReadOnlyList<AnonymousStructureMember> Members { get; init; }
}

/// <summary>One member of an inline object shape, with its type resolved.</summary>
/// <param name="Name">three.js's own name for it.</param>
/// <param name="Mapping">Its resolved C# type.</param>
/// <param name="IsOptional">Whether three.js declares it optional.</param>
/// <param name="Doc">JSDoc attached to it, which the inline shapes do carry.</param>
internal sealed record AnonymousStructureMember(string Name, TypeMapping Mapping, bool IsOptional, IrDoc? Doc);
