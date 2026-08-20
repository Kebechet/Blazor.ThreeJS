// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>Intersection</c>. A plain value rather than a handle-backed object:
/// three.js declares it as a shape, and nothing on either side keeps a reference to one. It travels
/// as its own members, under three.js's names for them.
/// </summary>
public sealed record Intersection : IThreeStructure
{
	/// <summary>Distance between the origin of the ray and the intersection.</summary>
	public float Distance { get; init; }

	/// <summary>
	/// Some objects (f.e. <see cref="Points"/>) provide the distance of the intersection to the nearest
	/// point on the ray. For other objects it will be <c>undefined</c>.
	/// </summary>
	public float? DistanceToRay { get; init; }

	/// <summary>Point of intersection, in world coordinates.</summary>
	public Vector3 Point { get; init; }

	/// <summary>three.js's <c>index</c>.</summary>
	public int? Index { get; init; }

	/// <summary>Intersected face.</summary>
	public Face? Face { get; init; }

	/// <summary>Index of the intersected face.</summary>
	public int? FaceIndex { get; init; }

	/// <summary>three.js's <c>barycoord</c>.</summary>
	public Vector3? Barycoord { get; init; }

	/// <summary>The intersected object.</summary>
	public Object3D Object { get; init; }

	/// <summary>three.js's <c>uv</c>.</summary>
	public Vector2? Uv { get; init; }

	/// <summary>three.js's <c>uv1</c>.</summary>
	public Vector2? Uv1 { get; init; }

	/// <summary>three.js's <c>normal</c>.</summary>
	public Vector3? Normal { get; init; }

	/// <summary>The index number of the instance where the ray intersects the <c>InstancedMesh</c>.</summary>
	public float? InstanceId { get; init; }

	/// <summary>three.js's <c>pointOnLine</c>.</summary>
	public Vector3? PointOnLine { get; init; }

	/// <summary>three.js's <c>batchId</c>.</summary>
	public float? BatchId { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["distance"] = Distance;

		if (DistanceToRay is not null)
		{
			members["distanceToRay"] = DistanceToRay;
		}
		members["point"] = Point;

		if (Index is not null)
		{
			members["index"] = Index;
		}

		if (Face is not null)
		{
			members["face"] = Face;
		}

		if (FaceIndex is not null)
		{
			members["faceIndex"] = FaceIndex;
		}

		if (Barycoord is not null)
		{
			members["barycoord"] = Barycoord;
		}
		members["object"] = Object;

		if (Uv is not null)
		{
			members["uv"] = Uv;
		}

		if (Uv1 is not null)
		{
			members["uv1"] = Uv1;
		}

		if (Normal is not null)
		{
			members["normal"] = Normal;
		}

		if (InstanceId is not null)
		{
			members["instanceId"] = InstanceId;
		}

		if (PointOnLine is not null)
		{
			members["pointOnLine"] = PointOnLine;
		}

		if (BatchId is not null)
		{
			members["batchId"] = BatchId;
		}

		return members;
	}

	/// <summary>
	/// Builds a <c>Intersection</c> from the members the applier sent back. A member three.js did not
	/// carry keeps this instance's own value, which for the blank instance the decoder builds is the C#
	/// default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new Intersection
		{
			Distance = members.TryGetValue("distance", out var distanceElement) ? ThreeValue.Decode<float>(distanceElement, context) : Distance,
			DistanceToRay = members.TryGetValue("distanceToRay", out var distanceToRayElement) ? ThreeValue.Decode<float?>(distanceToRayElement, context) : DistanceToRay,
			Point = members.TryGetValue("point", out var pointElement) ? ThreeValue.Decode<Vector3>(pointElement, context) : Point,
			Index = members.TryGetValue("index", out var indexElement) ? ThreeValue.Decode<int?>(indexElement, context) : Index,
			Face = members.TryGetValue("face", out var faceElement) ? ThreeValue.Decode<Face?>(faceElement, context) : Face,
			FaceIndex = members.TryGetValue("faceIndex", out var faceIndexElement) ? ThreeValue.Decode<int?>(faceIndexElement, context) : FaceIndex,
			Barycoord = members.TryGetValue("barycoord", out var barycoordElement) ? ThreeValue.Decode<Vector3?>(barycoordElement, context) : Barycoord,
			Object = members.TryGetValue("object", out var objectElement) ? ThreeObject.AdoptStructureMember<Object3D>(ThreeStructure.RequireContext(context, "object"), "object", ThreeValue.Decode<ThreeObjectReference?>(objectElement), (adoptedBatch, adoptedHandle) => new PrimitiveObject3D(adoptedBatch, adoptedHandle, "Object3D")) : Object,
			Uv = members.TryGetValue("uv", out var uvElement) ? ThreeValue.Decode<Vector2?>(uvElement, context) : Uv,
			Uv1 = members.TryGetValue("uv1", out var uv1Element) ? ThreeValue.Decode<Vector2?>(uv1Element, context) : Uv1,
			Normal = members.TryGetValue("normal", out var normalElement) ? ThreeValue.Decode<Vector3?>(normalElement, context) : Normal,
			InstanceId = members.TryGetValue("instanceId", out var instanceIdElement) ? ThreeValue.Decode<float?>(instanceIdElement, context) : InstanceId,
			PointOnLine = members.TryGetValue("pointOnLine", out var pointOnLineElement) ? ThreeValue.Decode<Vector3?>(pointOnLineElement, context) : PointOnLine,
			BatchId = members.TryGetValue("batchId", out var batchIdElement) ? ThreeValue.Decode<float?>(batchIdElement, context) : BatchId
		};
	}
}
