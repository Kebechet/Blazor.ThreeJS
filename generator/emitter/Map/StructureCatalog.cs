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

	/// <summary>The properties of a qualifying interface, in declaration order.</summary>
	/// <param name="name">Interface name.</param>
	/// <returns>The properties.</returns>
	public IReadOnlyList<IrProperty> PropertiesOf(string name)
	{
		return _interfacesByName[name].Properties;
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
		if (irInterface.Methods.Count > 0
			|| irInterface.TypeParameters.Count > 0
			|| irInterface.Extends.Count > 0
			|| irInterface.Properties.Count == 0)
		{
			return false;
		}

		foreach (var property in irInterface.Properties)
		{
			// Read through the mapper rather than pattern-matched here, so a record's members are exactly
			// as expressible as any other member - and so this stays right as the mapper gains rules.
			var mapping = mapper.Map(property.Type, new TypeMappingContext
			{
				MemberName = property.Name,
				NumericKind = property.NumericKind
			});

			// ⚠️ Values only. A member that is itself a mirrored class would need a handle, and a handle
			// inside a structure means the applier has to mint one while encoding a read result - which
			// it cannot do today. `Raycaster.intersectObject` is the member that wants this, and it stays
			// refused rather than half-answered.
			if (!mapping.IsMapped || !IsValue(mapping))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>Whether a mapping travels as a value, which is all a structure's members may do.</summary>
	/// <param name="mapping">The resolved member type.</param>
	/// <returns><see langword="true"/> when the wire carries it without a handle.</returns>
	private static bool IsValue(TypeMapping mapping)
	{
		return mapping.Kind switch
		{
			TypeMappingKind.Primitive => true,
			TypeMappingKind.GeneratedEnum => true,
			TypeMappingKind.HandWrittenMathType => true,
			TypeMappingKind.HandWrittenTypedArray => true,
			TypeMappingKind.Sequence => mapping.ElementMapping is { } element && IsValue(element),
			_ => false
		};
	}
}
