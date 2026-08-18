using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Any three.js class the mirror does not wrap, named by its three.js export and constructed by the
/// applier the same way every generated class is — <c>new THREE[name](…)</c>. The escape hatch for
/// construction: a class with no generated wrapper is reachable through this, and its members through
/// <see cref="ThreeObject.Set"/>, <see cref="ThreeObject.Call"/>, <see cref="ThreeObject.CallAsync{TValue}"/>
/// and <see cref="ThreeObject.GetAsync{TValue}"/>.
/// <para>
/// This one is for anything that does <b>not</b> belong in the scene graph — a geometry, a material, a
/// texture, a curve, a math value another object needs by reference. Use <see cref="PrimitiveObject3D"/>
/// for a class that does, which is what gives it a transform and lets it be added to a <c>Scene</c>.
/// </para>
/// <para>
/// ⚠️ Untyped by definition. Nothing here checks that the name is a class three.js ships, that the
/// constructor arguments are the ones it takes, or that a member you write exists — three.js is the
/// only thing in a position to know, and it says so when the batch runs. A name the bundle does not
/// export fails the create op with <c>Unknown three.js type</c>, which reaches C# through
/// <see cref="ThreeContext.OnError"/> rather than at the call site.
/// </para>
/// <para>
/// A class that <i>is</i> generated is still better reached through its generated type: that one carries
/// three.js's own documentation, its parameter order, and a compiler that checks both.
/// </para>
/// </summary>
/// <example>
/// <code>
/// // InstancedBufferAttribute has no generated type: the generator refuses it for the same
/// // structural reason as PrimitiveObject3D's PositionalAudio example - its base needs a
/// // constructor argument a generated subclass has nothing to supply. Passing a Primitive as a
/// // value sends it as a handle reference, and attaches it first.
/// var offsets = new Primitive("InstancedBufferAttribute", new Float32Array(0f, 0f, 0f, 1f, 0f, 0f), 3f);
/// geometry.Call("setAttribute", "offset", offsets);
/// </code>
/// </example>
public sealed class Primitive : ThreeObject
{
	private readonly string _threeTypeName;
	private readonly object?[] _constructorArgs;

	/// <summary>
	/// Names the three.js class to construct and the arguments to construct it with.
	/// <para>
	/// ⚠️ <c>new Primitive("X", null)</c> passes <see langword="null"/> as the whole argument array
	/// rather than as one null argument — C#'s <c>params</c> rule, not this class's. Write
	/// <c>new Primitive("X", [null])</c> when one null argument is what you mean.
	/// </para>
	/// <para>
	/// An argument you leave out is left out, which is what makes three.js apply its own default. There
	/// is no way to spell "not supplied" in a <i>middle</i> position here — the sentinel that does it for
	/// the generated constructors is internal to the wire format — so pass the documented default
	/// explicitly when an argument after it has to be supplied.
	/// </para>
	/// </summary>
	/// <param name="threeTypeName">Name of the export on the <c>THREE</c> namespace, e.g. <c>"Vector2"</c>.</param>
	/// <param name="constructorArgs">
	/// Positional constructor arguments, under the same encoding rules as
	/// <see cref="ThreeObject.Set"/>: any primitive, <see cref="string"/>, <see cref="Enum"/>,
	/// <see langword="null"/>, another mirrored object, a <see cref="TypedArray"/> (the example above
	/// passes one), or one of the hand-written math types in
	/// <c>Kebechet.Blazor.ThreeJS.Math</c>. ⚠️ A lone <b>reference-type</b> array binds as this parameter
	/// array itself, so its elements arrive as separate constructor arguments — cast it,
	/// <c>(object?) points</c>. Value-type arrays (<c>float[]</c>, <c>int[]</c>) are unaffected. See
	/// <see cref="ThreeObject.Call(string, object?[])"/> for why no overload fixes this.
	/// </param>
	/// <exception cref="ArgumentException">Thrown when <paramref name="threeTypeName"/> is blank.</exception>
	/// <exception cref="NotSupportedException">
	/// Thrown for an argument with no wire encoding. Raised here rather than at attach time, so the
	/// failure names the constructor call that carried the value instead of a flush somewhere else.
	/// </exception>
	public Primitive(string threeTypeName, params object?[] constructorArgs)
	{
		_threeTypeName = PrimitiveArguments.Validate(threeTypeName, constructorArgs);
		_constructorArgs = constructorArgs ?? [];
	}

	/// <summary>
	/// Adopts an object the browser already made, under the handle it was registered with. Used by
	/// <see cref="ThreeObject.GetObjectAsync"/> and <see cref="ThreeObject.CallObjectAsync"/> to hand
	/// back something reachable for a member whose result is a three.js object.
	/// <para>
	/// The type name is what three.js itself reported, so it is accurate rather than assumed - but it
	/// is a label here, not an instruction: no create op is ever emitted for an adopted object.
	/// </para>
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	/// <param name="threeTypeName">three.js's own <c>constructor.name</c> for the object.</param>
	internal Primitive(ThreeBatch batch, int handle, string threeTypeName)
		: base(handle)
	{
		_threeTypeName = threeTypeName;
		_constructorArgs = [];
		Batch = batch;
	}

	/// <summary>
	/// three.js's name for this object: the export the caller named when constructing one, or what
	/// three.js itself reported when this was adopted from a read.
	/// </summary>
	public string ThreeType
	{
		get { return _threeTypeName; }
	}

	/// <summary>Name of the corresponding export on the three.js namespace, as the caller gave it.</summary>
	protected override string ThreeTypeName
	{
		get { return _threeTypeName; }
	}

	/// <summary>Positional arguments forwarded to the three.js constructor, as the caller gave them.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return _constructorArgs; }
	}

	/// <summary>
	/// Attaches every constructor argument that is itself a mirrored object, so its create op reaches
	/// the batch before the one that references it by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		AttachMirroredArguments(batch, _constructorArgs);

		base.EmitCreate(batch);
	}
}

/// <summary>
/// The checks both primitive types run on what they were handed. Shared rather than duplicated because
/// the two differ only in which C# base they carry, and a check that held for one of them but not the
/// other would be the more surprising outcome.
/// </summary>
internal static class PrimitiveArguments
{
	/// <summary>
	/// Rejects a blank type name, and forces every argument through the encoder so an unencodable one
	/// fails at the constructor rather than at the attach that would otherwise carry it.
	/// </summary>
	/// <param name="threeTypeName">Name of the export on the <c>THREE</c> namespace.</param>
	/// <param name="constructorArgs">Positional constructor arguments, possibly <see langword="null"/>.</param>
	/// <returns><paramref name="threeTypeName"/>, when it is usable.</returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="threeTypeName"/> is blank.</exception>
	/// <exception cref="NotSupportedException">Thrown for an argument with no wire encoding.</exception>
	public static string Validate(string threeTypeName, object?[]? constructorArgs)
	{
		if (string.IsNullOrWhiteSpace(threeTypeName))
		{
			throw new ArgumentException(
				"A primitive needs the name of a three.js export to construct, e.g. \"Vector2\". The applier resolves it as `THREE[name]`.",
				nameof(threeTypeName));
		}

		foreach (var constructorArg in constructorArgs ?? [])
		{
			// Encoded now and thrown away: the encoder is the only thing that knows which reference types
			// have a wire form, and asking it here is what makes an unencodable argument fail at this
			// constructor rather than at whichever attach would otherwise have carried it. The real
			// encoding happens again at attach time, which costs one allocation for a math value and
			// nothing at all for everything else.
			ThreeValue.Encode(constructorArg);
		}

		return threeTypeName;
	}
}
