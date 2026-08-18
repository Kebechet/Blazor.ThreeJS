using System.Reflection;
using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

// `Path` is a three.js curve here, not `System.IO.Path`, which `ImplicitUsings` also brings in.
using Path = Kebechet.Blazor.ThreeJS.Objects.Path;

namespace Blazor.ThreeJS.Tests.Objects;

/// <summary>
/// Pins what the generator emits for a required parameter whose three.js type unions several types:
/// one overload per arm the mirror can express, each recording the op its own arm encodes to.
/// <para>
/// Behaviour rather than shape is what these assert. Two overloads that compile prove nothing on
/// their own — the question is whether the argument reaches the wire as the thing it is, which is
/// where a shared backing slot or a wrongly picked arm would show up.
/// </para>
/// </summary>
public class UnionArmOverloadTests
{
	private static readonly JsonSerializerOptions _webOptions = new(JsonSerializerDefaults.Web);

	[Fact]
	public void SetIndex_CalledWithABufferAttribute_RecordsTheArgumentAsAHandleReference()
	{
		// Arrange
		var batch = new ThreeBatch();
		var geometry = new BufferGeometry();
		var index = new BufferAttribute(new Float32Array(0f, 1f, 2f), 1f);
		geometry.AttachTo(batch);
		batch.Drain();

		// Act
		geometry.SetIndex(index);
		var ops = batch.Drain();

		// Assert
		var call = ops.Single(x => x.Kind == ThreeOpKind.Call);
		call.Member.ShouldBe("setIndex");
		JsonSerializer.Serialize(call.Args, _webOptions).ShouldBe($$"""[{"$ref":{{index.Handle}}}]""");
	}

	[Fact]
	public void SetIndex_CalledWithAnIntegerArray_RecordsTheArgumentAsAPlainArray()
	{
		// Arrange
		var batch = new ThreeBatch();
		var geometry = new BufferGeometry();
		geometry.AttachTo(batch);
		batch.Drain();

		// Act
		geometry.SetIndex([0, 2, 1]);
		var ops = batch.Drain();

		// Assert
		var call = ops.Single(x => x.Kind == ThreeOpKind.Call);
		call.Member.ShouldBe("setIndex");
		JsonSerializer.Serialize(call.Args, _webOptions).ShouldBe("[[0,2,1]]");
	}

	[Fact]
	public void SetIndex_CalledWithABufferAttribute_AttachesItBeforeTheCallThatNamesItByHandle()
	{
		// Arrange
		var batch = new ThreeBatch();
		var geometry = new BufferGeometry();
		var index = new BufferAttribute(new Float32Array(0f, 1f, 2f), 1f);
		geometry.AttachTo(batch);
		batch.Drain();

		// Act
		geometry.SetIndex(index);
		var ops = batch.Drain();

		// Assert
		ops.Select(x => x.Kind).ShouldBe([ThreeOpKind.Create, ThreeOpKind.Call]);
		ops[0].Handle.ShouldBe(index.Handle);
	}

	[Fact]
	public void SetFromPoints_CalledWithEachArm_TagsTheElementsOfTheArmItWasGiven()
	{
		// Arrange
		var batch = new ThreeBatch();
		var geometry = new BufferGeometry();
		geometry.AttachTo(batch);
		batch.Drain();

		// Act
		geometry.SetFromPoints([new Vector3(1f, 2f, 3f), new Vector3(4f, 5f, 6f)]);
		geometry.SetFromPoints([new Vector2(7f, 8f), new Vector2(9f, 10f)]);
		var ops = batch.Drain();

		// Assert. Two points, not one: the array has to arrive as a single argument, and a one-element
		// array would read the same whether it did or was spread across the argument list.
		var calls = ops.Where(x => x.Kind == ThreeOpKind.Call).ToList();
		JsonSerializer.Serialize(calls[0].Args, _webOptions)
			.ShouldBe("""[[{"$t":"Vector3","v":[1,2,3]},{"$t":"Vector3","v":[4,5,6]}]]""");
		JsonSerializer.Serialize(calls[1].Args, _webOptions)
			.ShouldBe("""[[{"$t":"Vector2","v":[7,8]},{"$t":"Vector2","v":[9,10]}]]""");
	}

	[Fact]
	public void SetColorAt_CalledWithEachArm_TagsTheArgumentAsThatArmsOwnMathType()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new BatchedMesh(1, 8);
		mesh.AttachTo(batch);
		batch.Drain();

		// Act
		mesh.SetColorAt(0f, Color.FromHex(0xff0000));
		mesh.SetColorAt(0f, new Vector4(0f, 1f, 0f, 1f));
		var ops = batch.Drain();

		// Assert
		var calls = ops.Where(x => x.Kind == ThreeOpKind.Call).ToList();
		JsonSerializer.Serialize(calls[0].Args, _webOptions).ShouldBe("""[0,{"$t":"Color","v":[1,0,0]}]""");
		JsonSerializer.Serialize(calls[1].Args, _webOptions).ShouldBe("""[0,{"$t":"Vector4","v":[0,1,0,1]}]""");
	}

	[Fact]
	public void UncacheAction_CalledWithEachArm_SendsTheClipAsAHandleOrAsAString()
	{
		// Arrange
		var batch = new ThreeBatch();
		var root = new Group();
		var mixer = new AnimationMixer(root);
		var clip = new AnimationClip("walk");
		mixer.AttachTo(batch);
		batch.Drain();

		// Act
		mixer.UncacheAction(clip, root);
		mixer.UncacheAction("walk", root);
		var ops = batch.Drain();

		// Assert
		var calls = ops.Where(x => x.Kind == ThreeOpKind.Call).ToList();
		JsonSerializer.Serialize(calls[0].Args, _webOptions).ShouldBe($$"""[{"$ref":{{clip.Handle}}},{"$ref":{{root.Handle}}}]""");
		JsonSerializer.Serialize(calls[1].Args, _webOptions).ShouldBe($$"""["walk",{"$ref":{{root.Handle}}}]""");
	}

	[Fact]
	public void CubeCamera_AttachedToBatch_EmitsItsRenderTargetBeforeItsOwnCreateOp()
	{
		// Arrange. The class is emitted at all only because one arm of `WebGLCubeRenderTarget |
		// CubeRenderTarget` maps: the other names a class three.js does not export.
		var batch = new ThreeBatch();
		var renderTarget = new CubeRenderTarget(256f);
		var camera = new CubeCamera(0.1f, 1000f, renderTarget);

		// Act
		camera.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var creates = ops.Where(x => x.Kind == ThreeOpKind.Create).ToList();
		creates.Select(x => x.Handle).ShouldBe([renderTarget.Handle, camera.Handle]);

		var create = creates.Single(x => x.Handle == camera.Handle);
		create.Type.ShouldBe("CubeCamera");
		JsonSerializer.Serialize(create.Args, _webOptions).ShouldBe($$"""[0.1,1000,{"$ref":{{renderTarget.Handle}}}]""");
	}

	[Fact]
	public void PathSetFromPoints_CalledWithSeveralPoints_SendsOneArrayArgumentRatherThanOnePerPoint()
	{
		// Arrange. `Path` is the member whose wire shape this change actually corrected: a lone
		// reference-type array used to bind as the `params object?[]` itself and arrive spread out.
		var batch = new ThreeBatch();
		var path = new Path();
		path.AttachTo(batch);
		batch.Drain();

		// Act
		path.SetFromPoints([new Vector2(0f, 0f), new Vector2(1f, 1f)]);
		var ops = batch.Drain();

		// Assert
		var call = ops.Single(x => x.Kind == ThreeOpKind.Call);
		call.Member.ShouldBe("setFromPoints");
		call.Args!.Length.ShouldBe(1);
		JsonSerializer.Serialize(call.Args, _webOptions)
			.ShouldBe("""[[{"$t":"Vector2","v":[0,0]},{"$t":"Vector2","v":[1,1]}]]""");
	}

	[Fact]
	public void PathSplineThru_CalledWithSeveralPoints_SendsOneArrayArgumentRatherThanOnePerPoint()
	{
		// Arrange
		var batch = new ThreeBatch();
		var path = new Path();
		path.AttachTo(batch);
		batch.Drain();

		// Act
		path.SplineThru([new Vector2(2f, 3f), new Vector2(4f, 5f)]);
		var ops = batch.Drain();

		// Assert
		var call = ops.Single(x => x.Kind == ThreeOpKind.Call);
		call.Member.ShouldBe("splineThru");
		call.Args!.Length.ShouldBe(1);
		JsonSerializer.Serialize(call.Args, _webOptions)
			.ShouldBe("""[[{"$t":"Vector2","v":[2,3]},{"$t":"Vector2","v":[4,5]}]]""");
	}

	[Fact]
	public void SetIndex_OnTheGeneratedType_DeclaresExactlyOneOverloadPerArmThatMaps()
	{
		// Arrange & Act
		var signatures = ParameterTypeNamesOf(typeof(BufferGeometry), nameof(BufferGeometry.SetIndex));

		// Assert. Three declared arms — `BufferAttribute | number[] | null` — and two signatures: the
		// null arm is an annotation on both rather than an arm of its own.
		signatures.ShouldBe(["BufferAttribute", "Int32[]"], ignoreOrder: true);
	}

	[Fact]
	public void Set_OnBufferAttribute_DeclaresOnlyTheArmTheMirrorCanExpress()
	{
		// Arrange & Act
		var signatures = ParameterTypeNamesOf(typeof(BufferAttribute), nameof(BufferAttribute.Set));

		// Assert. `ArrayLike<number> | ArrayBufferView` has one arm the wire can carry, so the member
		// exists with one signature rather than being refused for the arm that has no C# type.
		signatures.ShouldBe(["Single[], Int32"]);
	}

	/// <summary>
	/// The parameter types of every overload of one method, as one string each, so a test can state
	/// the whole emitted overload set rather than assert one signature at a time.
	/// </summary>
	/// <param name="declaringType">Generated type to read.</param>
	/// <param name="methodName">C# method name.</param>
	/// <returns>One entry per overload.</returns>
	private static List<string> ParameterTypeNamesOf(Type declaringType, string methodName)
	{
		return declaringType
			.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
			.Where(x => string.Equals(x.Name, methodName, StringComparison.Ordinal))
			.Select(x => string.Join(", ", x.GetParameters().Select(parameter => parameter.ParameterType.Name)))
			.ToList();
	}
}
