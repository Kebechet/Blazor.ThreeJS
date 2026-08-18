// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The generated half of <c>Object3D</c>: the commands and queries <c>THREE.Object3D</c> declares,
/// beside the hand-written half that owns the scene-graph behaviour. See the hand-written part for
/// what this type is. <para>⚠️ <b>Every command here leaves the mirror stale, and so do the queries
/// that mutate.</b> A command records a call and reads nothing back, so the state three.js changes
/// on its side goes on being reported by C# as whatever it was before. One that writes the
/// transform (<c>RotateX</c>, <c>TranslateOnAxis</c>, <c>ApplyMatrix4</c> and their kind) leaves
/// <c>Position</c>, <c>Rotation</c>, <c>Scale</c> and <c>Quaternion</c> reporting their pre-call
/// values, and writing one of those values back then records nothing at all, because the mirror
/// sees the value it already holds. One that changes the scene graph (<c>Attach</c>, <c>Copy</c>)
/// leaves <c>Children</c> reporting the parentage the mirror last arranged itself — and so do
/// <c>RemoveFromParentAsync</c> and <c>ClearAsync</c>, which are queries only because three.js
/// hands the changed object back: what they answer with is a handle, not a refreshed mirror. Both
/// answer the receiver, which always exists, so the nullable <c>Task&lt;Object3D?&gt;</c> they
/// declare never actually resolves null. Where a property or a hand-written method expresses what
/// you want, use that; where you want the command, treat what it wrote as three.js's from then
/// on.</para>
/// </summary>
public abstract partial class Object3D
{
	/// <summary>
	/// Applies the matrix transform to the object and updates the object's position, rotation and
	/// scale.
	/// </summary>
	/// <param name="matrix">Value forwarded to the <c>matrix</c> argument.</param>
	public void ApplyMatrix4(Matrix4 matrix)
	{
		RecordCall("applyMatrix4", matrix);
	}

	/// <summary>Applies the rotation represented by the quaternion to the object.</summary>
	/// <param name="quaternion">Value forwarded to the <c>quaternion</c> argument.</param>
	public void ApplyQuaternion(Quaternion quaternion)
	{
		RecordCall("applyQuaternion", quaternion);
	}

	/// <summary>Calls <c>setFromAxisAngle</c>(<c>axis</c>, <c>angle</c>) on the <c>.quaternion</c>.</summary>
	/// <param name="axis">A normalized vector in object space.</param>
	/// <param name="angle">Angle in radians.</param>
	public void SetRotationFromAxisAngle(Vector3 axis, float angle)
	{
		RecordCall("setRotationFromAxisAngle", axis, angle);
	}

	/// <summary>Calls <c>setFromEuler</c>(<c>euler</c>) on the <c>.quaternion</c>.</summary>
	/// <param name="euler">Euler angle specifying rotation amount.</param>
	public void SetRotationFromEuler(Euler euler)
	{
		RecordCall("setRotationFromEuler", euler);
	}

	/// <summary>Calls <c>setFromRotationMatrix</c>(<c>m</c>) on the <c>.quaternion</c>.</summary>
	/// <param name="m">Rotate the quaternion by the rotation component of the matrix.</param>
	public void SetRotationFromMatrix(Matrix4 m)
	{
		RecordCall("setRotationFromMatrix", m);
	}

	/// <summary>Copy the given <c>Quaternion</c> into <c>.quaternion</c>.</summary>
	/// <param name="q">Normalized Quaternion.</param>
	public void SetRotationFromQuaternion(Quaternion q)
	{
		RecordCall("setRotationFromQuaternion", q);
	}

	/// <summary>Rotate an object along an axis in object space.</summary>
	/// <param name="axis">A normalized vector in object space.</param>
	/// <param name="angle">The angle in radians.</param>
	public void RotateOnAxis(Vector3 axis, float angle)
	{
		RecordCall("rotateOnAxis", axis, angle);
	}

	/// <summary>Rotate an object along an axis in world space.</summary>
	/// <param name="axis">A normalized vector in world space.</param>
	/// <param name="angle">The angle in radians.</param>
	public void RotateOnWorldAxis(Vector3 axis, float angle)
	{
		RecordCall("rotateOnWorldAxis", axis, angle);
	}

	/// <summary>Rotates the object around _x_ axis in local space.</summary>
	/// <param name="angle">The angle to rotate in radians.</param>
	public void RotateX(float angle)
	{
		RecordCall("rotateX", angle);
	}

	/// <summary>Rotates the object around _y_ axis in local space.</summary>
	/// <param name="angle">The angle to rotate in radians.</param>
	public void RotateY(float angle)
	{
		RecordCall("rotateY", angle);
	}

	/// <summary>Rotates the object around _z_ axis in local space.</summary>
	/// <param name="angle">The angle to rotate in radians.</param>
	public void RotateZ(float angle)
	{
		RecordCall("rotateZ", angle);
	}

	/// <summary>Translate an object by distance along an axis in object space.</summary>
	/// <param name="axis">A normalized vector in object space.</param>
	/// <param name="distance">The distance to translate.</param>
	public void TranslateOnAxis(Vector3 axis, float distance)
	{
		RecordCall("translateOnAxis", axis, distance);
	}

	/// <summary>Translates object along x axis in object space by <c>distance</c> units.</summary>
	/// <param name="distance"></param>
	public void TranslateX(float distance)
	{
		RecordCall("translateX", distance);
	}

	/// <summary>Translates object along _y_ axis in object space by <c>distance</c> units.</summary>
	/// <param name="distance"></param>
	public void TranslateY(float distance)
	{
		RecordCall("translateY", distance);
	}

	/// <summary>Translates object along _z_ axis in object space by <c>distance</c> units.</summary>
	/// <param name="distance"></param>
	public void TranslateZ(float distance)
	{
		RecordCall("translateZ", distance);
	}

	/// <summary>
	/// Adds a <see cref="Object3D"/> as a child of this, while maintaining the object's world
	/// transform.
	/// </summary>
	/// <param name="object">Value forwarded to the <c>object</c> argument.</param>
	public void Attach(Object3D @object)
	{
		RecordCall("attach", @object);
	}

	/// <summary>Updates local transform.</summary>
	public void UpdateMatrix()
	{
		RecordCall("updateMatrix");
	}

	/// <summary>
	/// Updates the global transform of the object. And will update the object descendants if
	/// <c>.matrixWorldNeedsUpdate</c> is set to true or if the <c>force</c> parameter is set to
	/// <c>true</c>.
	/// </summary>
	/// <param name="force">
	/// A boolean that can be used to bypass <c>.matrixWorldAutoUpdate</c>, to recalculate the world
	/// matrix of the object and descendants on the current frame. Useful if you cannot wait for the
	/// renderer to update it on the next frame, assuming <c>.matrixWorldAutoUpdate</c> set to
	/// <c>true</c>.
	/// </param>
	public void UpdateMatrixWorld(bool force)
	{
		RecordCall("updateMatrixWorld", force);
	}

	/// <summary>
	/// An alternative version of <c>Object3D#updateMatrixWorld</c> with more control over the update of
	/// ancestor and descendant nodes.
	/// </summary>
	/// <param name="updateParents">Whether ancestor nodes should be updated or not.</param>
	/// <param name="updateChildren">Whether descendant nodes should be updated or not.</param>
	/// <param name="force">
	/// When set to <c>true</c>, a recomputation of world matrices is forced even when
	/// <c>Object3D#matrixWorldNeedsUpdate</c> is <c>false</c>.
	/// </param>
	public void UpdateWorldMatrix(bool updateParents, bool updateChildren, bool force = false)
	{
		RecordCall("updateWorldMatrix", updateParents, updateChildren, force);
	}

	/// <summary>Copies the given object into this object.</summary>
	/// <param name="object">Value forwarded to the <c>object</c> argument.</param>
	/// <param name="recursive">
	/// If set to <c>true</c>, descendants of the object are copied next to the existing ones. If set to
	/// <c>false</c>, descendants are left unchanged. Default is <c>true</c>.
	/// </param>
	public void Copy(Object3D @object, bool recursive = true)
	{
		RecordCall("copy", @object, recursive);
	}

	/// <summary>
	/// Flag to check if a given object is of type <see cref="Object3D"/>. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isObject3D</c> held.
	/// </summary>
	/// <returns>The value <c>isObject3D</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsObject3DAsync()
	{
		return GetAsync<bool>("isObject3D");
	}

	/// <summary>
	/// Unique number for this <see cref="Object3D"/> instance. Read-only in three.js, so it is read on
	/// demand rather than mirrored: records a get op, sends it behind every write already pending, and
	/// completes with the value <c>id</c> held.
	/// </summary>
	/// <returns>The value <c>id</c> held, once the JavaScript side has answered.</returns>
	public Task<int> IdAsync()
	{
		return GetAsync<int>("id");
	}

	/// <summary>
	/// A Read-only _string_ to check <c>this</c> object type. Read-only in three.js, so it is read on
	/// demand rather than mirrored: records a get op, sends it behind every write already pending, and
	/// completes with the value <c>type</c> held.
	/// </summary>
	/// <returns>The value <c>type</c> held, once the JavaScript side has answered.</returns>
	public Task<string> TypeAsync()
	{
		return GetAsync<string>("type");
	}

	/// <summary>
	/// Converts the vector from this object's local space to world space. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>localToWorld</c> returned.
	/// </summary>
	/// <param name="vector">A vector representing a position in this object's local space.</param>
	/// <returns>The value <c>localToWorld</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector3> LocalToWorldAsync(Vector3 vector)
	{
		return RecordRead<Vector3>("localToWorld", vector);
	}

	/// <summary>
	/// Converts the vector from world space to this object's local space. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>worldToLocal</c> returned.
	/// </summary>
	/// <param name="vector">A vector representing a position in world space.</param>
	/// <returns>The value <c>worldToLocal</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector3> WorldToLocalAsync(Vector3 vector)
	{
		return RecordRead<Vector3>("worldToLocal", vector);
	}

	/// <summary>
	/// Removes this object from its current parent. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>removeFromParent</c> returned.
	/// </summary>
	/// <returns>The value <c>removeFromParent</c> returned, once the JavaScript side has answered.</returns>
	public Task<Object3D?> RemoveFromParentAsync()
	{
		return RecordReadObject<Object3D>("removeFromParent", (adoptedBatch, adoptedHandle) => new PrimitiveObject3D(adoptedBatch, adoptedHandle, "Object3D"));
	}

	/// <summary>
	/// Removes all child objects. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>clear</c> returned.
	/// </summary>
	/// <returns>The value <c>clear</c> returned, once the JavaScript side has answered.</returns>
	public Task<Object3D?> ClearAsync()
	{
		return RecordReadObject<Object3D>("clear", (adoptedBatch, adoptedHandle) => new PrimitiveObject3D(adoptedBatch, adoptedHandle, "Object3D"));
	}

	/// <summary>
	/// Searches through an object and its children, starting with the object itself, and returns the
	/// first with a matching id. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getObjectById</c> returned.
	/// </summary>
	/// <param name="id">Unique number of the object instance.</param>
	/// <returns>The value <c>getObjectById</c> returned, once the JavaScript side has answered.</returns>
	public Task<Object3D?> GetObjectByIdAsync(int id)
	{
		return RecordReadObject<Object3D>("getObjectById", (adoptedBatch, adoptedHandle) => new PrimitiveObject3D(adoptedBatch, adoptedHandle, "Object3D"), id);
	}

	/// <summary>
	/// Searches through an object and its children, starting with the object itself, and returns the
	/// first with a matching name. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getObjectByName</c> returned.
	/// </summary>
	/// <param name="name">String to match to the children's Object3D.name property.</param>
	/// <returns>The value <c>getObjectByName</c> returned, once the JavaScript side has answered.</returns>
	public Task<Object3D?> GetObjectByNameAsync(string name)
	{
		return RecordReadObject<Object3D>("getObjectByName", (adoptedBatch, adoptedHandle) => new PrimitiveObject3D(adoptedBatch, adoptedHandle, "Object3D"), name);
	}

	/// <summary>
	/// Returns a vector representing the position of the object in world space. Records a read op,
	/// sends it behind every write already pending, and completes with what <c>getWorldPosition</c>
	/// returned.
	/// </summary>
	/// <param name="target">The result will be copied into this Vector3.</param>
	/// <returns>The value <c>getWorldPosition</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector3> GetWorldPositionAsync(Vector3 target)
	{
		return RecordRead<Vector3>("getWorldPosition", target);
	}

	/// <summary>
	/// Returns a quaternion representing the rotation of the object in world space. Records a read op,
	/// sends it behind every write already pending, and completes with what <c>getWorldQuaternion</c>
	/// returned.
	/// </summary>
	/// <param name="target">The result will be copied into this Quaternion.</param>
	/// <returns>The value <c>getWorldQuaternion</c> returned, once the JavaScript side has answered.</returns>
	public Task<Quaternion> GetWorldQuaternionAsync(Quaternion target)
	{
		return RecordRead<Quaternion>("getWorldQuaternion", target);
	}

	/// <summary>
	/// Returns a vector of the scaling factors applied to the object for each axis in world space.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>getWorldScale</c> returned.
	/// </summary>
	/// <param name="target">The result will be copied into this Vector3.</param>
	/// <returns>The value <c>getWorldScale</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector3> GetWorldScaleAsync(Vector3 target)
	{
		return RecordRead<Vector3>("getWorldScale", target);
	}

	/// <summary>
	/// Returns a vector representing the direction of object's positive z-axis in world space. Records
	/// a read op, sends it behind every write already pending, and completes with what
	/// <c>getWorldDirection</c> returned.
	/// </summary>
	/// <param name="target">The result will be copied into this Vector3.</param>
	/// <returns>The value <c>getWorldDirection</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector3> GetWorldDirectionAsync(Vector3 target)
	{
		return RecordRead<Vector3>("getWorldDirection", target);
	}

	/// <summary>
	/// Returns a clone of <c>this</c> object and optionally all descendants. Records a read op, sends
	/// it behind every write already pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <param name="recursive">If true, descendants of the object are also cloned.</param>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<Object3D?> CloneAsync(bool recursive = true)
	{
		return RecordReadObject<Object3D>("clone", (adoptedBatch, adoptedHandle) => new PrimitiveObject3D(adoptedBatch, adoptedHandle, "Object3D"), recursive);
	}
}
