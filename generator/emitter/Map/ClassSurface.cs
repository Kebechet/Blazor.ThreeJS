using Blazor.ThreeJS.Emitter.Ir;

namespace Blazor.ThreeJS.Emitter.Map;

/// <summary>
/// Works out the real member set of a class, which is not the same thing as the members its own
/// declaration lists.
/// <para>
/// three.js gives its classes their property surface through declaration merging: the class
/// declaration carries little more than a constructor, and an <c>export interface X extends
/// XProperties {}</c> alongside it supplies everything else. Reading only class-declared members
/// therefore produces a material with no <c>color</c>, no <c>roughness</c> and no <c>side</c> — the
/// largest body of mirrored state in the library, invisible.
/// </para>
/// <para>
/// Three things are resolved here. Members reachable through the same-named interface and its
/// <c>extends</c> chain are pulled in. Ancestors with no C# mirror of their own (the abstract
/// <c>Light</c>, for instance) are flattened into the class, so their members are not lost with them.
/// And members the nearest mirrored ancestor already carries are subtracted, because C# inheritance
/// provides those — re-declaring them would hide the base member on every subclass.
/// </para>
/// </summary>
internal sealed class ClassSurfaceResolver
{
	private readonly Dictionary<string, IrClass> _classesByName;
	private readonly Dictionary<string, IrInterface> _interfacesByName;
	private readonly Dictionary<string, List<IrAugmentedDeclaration>> _augmentationsByName;
	private readonly Func<string, bool> _isMirrored;
	private readonly Dictionary<string, ClassSurface> _surfacesByName = new(StringComparer.Ordinal);
	private readonly Dictionary<string, IReadOnlySet<string>> _fullMemberNamesByClassName = new(StringComparer.Ordinal);

	/// <summary>
	/// Whether a class three.js declares is abstract. An abstract class is emitted - it is a real base
	/// and a real parameter type - but it cannot be instantiated, so a read that answers with one has
	/// nothing to build.
	/// </summary>
	/// <param name="name">Class name.</param>
	/// <returns><see langword="true"/> when three.js declares it abstract.</returns>
	public bool IsAbstractClass(string name)
	{
		return _classesByName.TryGetValue(name, out var irClass) && irClass.IsAbstract;
	}

	/// <summary>Builds a resolver over one IR snapshot.</summary>
	/// <param name="ir">The parsed IR.</param>
	/// <param name="isMirrored">Whether a class name has a C# type of its own — generated or hand-written.</param>
	public ClassSurfaceResolver(IrRoot ir, Func<string, bool> isMirrored)
	{
		_isMirrored = isMirrored;
		_classesByName = [];
		foreach (var irClass in ir.Classes)
		{
			_classesByName.TryAdd(irClass.Name, irClass);
		}

		_interfacesByName = [];
		foreach (var irInterface in ir.Interfaces)
		{
			_interfacesByName.TryAdd(irInterface.Name, irInterface);
		}

		_augmentationsByName = [];
		foreach (var augmentation in ir.ModuleAugmentations)
		{
			foreach (var augmented in augmentation.Augments)
			{
				if (!_augmentationsByName.TryGetValue(augmented.Name, out var list))
				{
					list = [];
					_augmentationsByName[augmented.Name] = list;
				}

				list.Add(augmented);
			}
		}
	}

	/// <summary>Resolves the members one class is responsible for declaring in C#.</summary>
	/// <param name="irClass">Class to resolve.</param>
	/// <returns>The resolved surface, cached per class name.</returns>
	public ClassSurface Resolve(IrClass irClass)
	{
		if (_surfacesByName.TryGetValue(irClass.Name, out var cached))
		{
			return cached;
		}

		// Seeded before the walk so a cycle in the IR's extends chain terminates instead of recursing.
		var placeholder = new ClassSurface { Members = [] };
		_surfacesByName[irClass.Name] = placeholder;

		var members = new List<SurfaceMember>();
		var takenNames = new HashSet<string>(StringComparer.Ordinal);
		var flattenedAncestors = new List<string>();
		string? mirroredBaseName = null;

		var current = irClass;
		var origin = MemberOrigin.Declared;
		IReadOnlyList<IrTypeParameter> typeParameters = current.TypeParameters;
		while (true)
		{
			CollectDeclaration(current.Name, typeParameters, current.Properties, current.Methods, origin, members, takenNames);

			var baseName = current.Extends?.Name;
			if (baseName is null)
			{
				break;
			}

			if (_isMirrored(baseName))
			{
				mirroredBaseName = baseName;
				break;
			}

			if (!_classesByName.TryGetValue(baseName, out var baseClass))
			{
				break;
			}

			typeParameters = BindTypeArguments(baseClass.TypeParameters, current.Extends, typeParameters);
			flattenedAncestors.Add(baseName);
			current = baseClass;
			origin = MemberOrigin.FlattenedAncestor;
		}

		if (mirroredBaseName is not null)
		{
			var inherited = FullMemberNames(mirroredBaseName);
			members.RemoveAll(x => inherited.Contains(x.Name));
		}

		var surface = new ClassSurface
		{
			Members = members,
			MirroredBaseName = mirroredBaseName,
			FlattenedAncestors = flattenedAncestors
		};

		_surfacesByName[irClass.Name] = surface;
		return surface;
	}

	/// <summary>
	/// Rewrites a base class's type parameters so each carries the concrete type its subclass supplied,
	/// as the parameter's default.
	/// <para>
	/// Without this a flattened member erases a type parameter to its <b>constraint</b>, which for
	/// <c>Curve&lt;TVector extends Vector2 | Vector3&gt;</c> is a union no single C# type expresses — so
	/// every inherited curve member was refused, on every curve. The subclass has already answered the
	/// question: <c>SplineCurve extends Curve&lt;Vector2&gt;</c> means <c>TVector</c> is <c>Vector2</c>
	/// there and <c>Vector3</c> on <c>CatmullRomCurve3</c>. Binding it as the default is enough, because
	/// erasure already prefers a default over a constraint.
	/// </para>
	/// </summary>
	/// <param name="baseTypeParameters">Type parameters the base class declares.</param>
	/// <param name="extends">The subclass's <c>extends</c> clause, carrying the type arguments it passed.</param>
	/// <param name="outerTypeParameters">
	/// Bindings already in scope on the subclass, so a chain that forwards its own parameter
	/// (<c>A&lt;T&gt; extends B&lt;T&gt;</c>) resolves to whatever <c>T</c> was bound to further out
	/// rather than stopping at the name.
	/// </param>
	/// <returns>The base's type parameters, with any supplied argument bound as the default.</returns>
	private static IReadOnlyList<IrTypeParameter> BindTypeArguments(
		IReadOnlyList<IrTypeParameter> baseTypeParameters,
		IrType? extends,
		IReadOnlyList<IrTypeParameter> outerTypeParameters)
	{
		var typeArguments = extends?.TypeArguments;
		if (baseTypeParameters.Count == 0 || typeArguments is null || typeArguments.Count == 0)
		{
			return baseTypeParameters;
		}

		var bound = new List<IrTypeParameter>(baseTypeParameters.Count);
		foreach (var (index, baseTypeParameter) in baseTypeParameters.Index())
		{
			var argument = index < typeArguments.Count ? typeArguments[index] : null;

			// An argument that is itself a type parameter name carries no concrete type of its own; the
			// binding it already has further out is the answer.
			if (argument is { Kind: "reference" } && outerTypeParameters.FirstOrDefault(x => x.Name == argument.Name) is { } forwarded)
			{
				argument = forwarded.Default ?? forwarded.Constraint;
			}

			// A copy rather than a mutation: the IR's own instance is shared by every subclass of this
			// base, so writing the default onto it would bind Curve's TVector to whichever subclass was
			// resolved last.
			bound.Add(new IrTypeParameter
			{
				Name = baseTypeParameter.Name,
				Constraint = baseTypeParameter.Constraint,
				Default = argument ?? baseTypeParameter.Default
			});
		}

		return bound;
	}

	/// <summary>
	/// Every member name reachable on a class, including everything it inherits. Used to subtract what
	/// a mirrored base already provides, so the same three.js member is declared in exactly one C#
	/// type.
	/// </summary>
	/// <param name="className">Class to walk.</param>
	/// <returns>The member names, class-declared and interface-inherited alike.</returns>
	private IReadOnlySet<string> FullMemberNames(string className)
	{
		if (_fullMemberNamesByClassName.TryGetValue(className, out var cached))
		{
			return cached;
		}

		var names = new HashSet<string>(StringComparer.Ordinal);
		_fullMemberNamesByClassName[className] = names;

		var visitedClassNames = new HashSet<string>(StringComparer.Ordinal);
		var currentName = className;
		while (currentName is not null && visitedClassNames.Add(currentName))
		{
			var members = new List<SurfaceMember>();
			var taken = new HashSet<string>(StringComparer.Ordinal);
			if (_classesByName.TryGetValue(currentName, out var currentClass))
			{
				CollectDeclaration(currentName, currentClass.TypeParameters, currentClass.Properties, currentClass.Methods, MemberOrigin.Declared, members, taken);
				currentName = currentClass.Extends?.Name;
			}
			else
			{
				CollectDeclaration(currentName, [], [], [], MemberOrigin.Declared, members, taken);
				currentName = null;
			}

			foreach (var member in members)
			{
				names.Add(member.Name);
			}
		}

		return names;
	}

	/// <summary>
	/// Adds one declaration's own members, then everything reachable through the same-named interface
	/// and any module augmentation targeting it.
	/// </summary>
	/// <param name="declarationName">Name shared by the class and its merged interface.</param>
	/// <param name="typeParameters">Type parameters in scope for these members.</param>
	/// <param name="properties">Class-declared properties.</param>
	/// <param name="methods">Class-declared methods.</param>
	/// <param name="origin">Where these members came from, for the coverage report.</param>
	/// <param name="members">Accumulator.</param>
	/// <param name="takenNames">Names already claimed, so the nearest declaration wins.</param>
	private void CollectDeclaration(
		string declarationName,
		IReadOnlyList<IrTypeParameter> typeParameters,
		IReadOnlyList<IrProperty> properties,
		IReadOnlyList<IrMethod> methods,
		MemberOrigin origin,
		List<SurfaceMember> members,
		HashSet<string> takenNames)
	{
		AddMembers(properties, methods, typeParameters, declarationName, origin, members, takenNames);
		CollectInterfaceChain(declarationName, origin, members, takenNames, new HashSet<string>(StringComparer.Ordinal));
	}

	/// <summary>
	/// Walks an interface and everything it extends, adding their members. The entry point is the
	/// interface that shares the class's name — three.js's declaration-merging idiom — so the walk
	/// starts on the class's own surface rather than on an unrelated type.
	/// </summary>
	/// <param name="interfaceName">Interface to walk.</param>
	/// <param name="classOrigin">Origin of the declaration this chain hangs off.</param>
	/// <param name="members">Accumulator.</param>
	/// <param name="takenNames">Names already claimed.</param>
	/// <param name="visitedInterfaceNames">Interfaces already walked, so a cyclic chain terminates.</param>
	private void CollectInterfaceChain(
		string interfaceName,
		MemberOrigin classOrigin,
		List<SurfaceMember> members,
		HashSet<string> takenNames,
		HashSet<string> visitedInterfaceNames)
	{
		if (!visitedInterfaceNames.Add(interfaceName))
		{
			return;
		}

		var extendedNames = new List<string>();
		if (_interfacesByName.TryGetValue(interfaceName, out var irInterface))
		{
			var origin = classOrigin == MemberOrigin.Declared
				? MemberOrigin.InterfaceInheritance
				: classOrigin;

			AddMembers(irInterface.Properties, irInterface.Methods, irInterface.TypeParameters, interfaceName, origin, members, takenNames);
			extendedNames.AddRange(irInterface.Extends.Select(x => x.Name).Where(x => x is not null).Select(x => x!));
		}

		if (_augmentationsByName.TryGetValue(interfaceName, out var augmentations))
		{
			foreach (var augmented in augmentations)
			{
				AddMembers(augmented.Properties, augmented.Methods, [], interfaceName, MemberOrigin.ModuleAugmentation, members, takenNames);
				extendedNames.AddRange(augmented.Extends.Select(x => x.Name).Where(x => x is not null).Select(x => x!));
			}
		}

		foreach (var extendedName in extendedNames)
		{
			CollectInterfaceChain(extendedName, classOrigin, members, takenNames, visitedInterfaceNames);
		}
	}

	private static void AddMembers(
		IReadOnlyList<IrProperty> properties,
		IReadOnlyList<IrMethod> methods,
		IReadOnlyList<IrTypeParameter> typeParameters,
		string declaringName,
		MemberOrigin origin,
		List<SurfaceMember> members,
		HashSet<string> takenNames)
	{
		foreach (var property in properties)
		{
			if (!takenNames.Add(property.Name))
			{
				continue;
			}

			members.Add(new SurfaceMember
			{
				Name = property.Name,
				Property = property,
				DeclaringName = declaringName,
				Origin = origin,
				TypeParameters = typeParameters
			});
		}

		foreach (var method in methods)
		{
			if (!takenNames.Add(method.Name))
			{
				continue;
			}

			members.Add(new SurfaceMember
			{
				Name = method.Name,
				Method = method,
				DeclaringName = declaringName,
				Origin = origin,
				TypeParameters = typeParameters
			});
		}
	}
}

/// <summary>The members one class declares in C#, once inheritance has been resolved.</summary>
internal sealed class ClassSurface
{
	/// <summary>Members this class is responsible for, in resolution order.</summary>
	public required IReadOnlyList<SurfaceMember> Members { get; init; }

	/// <summary>Nearest ancestor that has a C# type of its own, or <see langword="null"/> for a root.</summary>
	public string? MirroredBaseName { get; init; }

	/// <summary>Ancestors with no C# mirror, whose members were folded into this class instead of lost.</summary>
	public IReadOnlyList<string> FlattenedAncestors { get; init; } = [];
}

/// <summary>One member of a resolved surface: a property or a method, and where it came from.</summary>
internal sealed class SurfaceMember
{
	/// <summary>Member name as three.js spells it, and the wire token.</summary>
	public required string Name { get; init; }

	/// <summary>The property, when this member is one.</summary>
	public IrProperty? Property { get; init; }

	/// <summary>The method, when this member is one.</summary>
	public IrMethod? Method { get; init; }

	/// <summary>Class or interface the member was read from.</summary>
	public required string DeclaringName { get; init; }

	/// <summary>How the member reached this class.</summary>
	public required MemberOrigin Origin { get; init; }

	/// <summary>Type parameters in scope where the member was declared, for erasure.</summary>
	public required IReadOnlyList<IrTypeParameter> TypeParameters { get; init; }
}
