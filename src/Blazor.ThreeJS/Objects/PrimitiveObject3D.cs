using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// <see cref="Primitive"/> for a class that belongs in the scene graph. Everything <see cref="Object3D"/>
/// carries comes with it — the transform, the parent/child relationships, visibility, the shadow and
/// culling flags, <c>OnClick</c> — so it is added to a <c>Scene</c>, attached, batched and hit-tested
/// exactly like a generated type, while the class itself is one three.js has and this mirror does not.
/// <para>
/// ⚠️ Use <see cref="Primitive"/> for anything that is not an <c>Object3D</c>. This type replays a
/// transform on attach, which on, say, a material would write <c>position</c>, <c>rotation</c> and
/// <c>scale</c> onto an object three.js never gave them to. Nothing here can tell the difference —
/// three.js has no C# type system for this class to check against, which is the whole reason it exists.
/// </para>
/// </summary>
/// <example>
/// <code>
/// // PositionalAudio is blocked from generation because its base needs a constructor argument a
/// // generated subclass has nothing to supply. Untyped, it is four lines.
/// var positionalAudio = new PrimitiveObject3D("PositionalAudio", audioListener);
/// positionalAudio.Set("refDistance", 2f);
/// positionalAudio.Call("play");
/// scene.Add(positionalAudio);
/// </code>
/// </example>
public sealed class PrimitiveObject3D : Object3D
{
	private readonly string _threeTypeName;
	private readonly object?[] _constructorArgs;

	/// <summary>
	/// Names the three.js class to construct and the arguments to construct it with. Same rules, same
	/// refusals and the same <c>params</c> caveat as <see cref="Primitive(string, object?[])"/>.
	/// </summary>
	/// <param name="threeTypeName">Name of the export on the <c>THREE</c> namespace, e.g. <c>"PositionalAudio"</c>.</param>
	/// <param name="constructorArgs">
	/// Positional constructor arguments. ⚠️ A lone <b>reference-type</b> array binds as this parameter
	/// array itself, so its elements arrive as separate constructor arguments — cast it,
	/// <c>(object?) points</c>. Value-type arrays (<c>float[]</c>, <c>int[]</c>) are unaffected. See
	/// <see cref="ThreeObject.Call(string, object?[])"/> for why no overload fixes this.
	/// </param>
	/// <exception cref="ArgumentException">Thrown when <paramref name="threeTypeName"/> is blank.</exception>
	/// <exception cref="NotSupportedException">Thrown for an argument with no wire encoding.</exception>
	public PrimitiveObject3D(string threeTypeName, params object?[] constructorArgs)
	{
		_threeTypeName = PrimitiveArguments.Validate(threeTypeName, constructorArgs);
		_constructorArgs = constructorArgs ?? [];
	}

	/// <summary>
	/// Adopts a scene-graph object the browser already made, under the handle it was registered with.
	/// <para>
	/// This is what a read declared to return <see cref="Object3D"/> answers with. That base is
	/// abstract in C#, so it cannot be the thing constructed; this is the concrete scene-graph wrapper,
	/// and it satisfies the declared type while carrying three.js's own name for what actually came
	/// back.
	/// </para>
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	/// <param name="threeTypeName">three.js's own <c>type</c> for the object.</param>
	internal PrimitiveObject3D(ThreeBatch batch, int handle, string threeTypeName)
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
