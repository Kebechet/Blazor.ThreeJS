// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A special type of camera that uses two perspective cameras with stereoscopic projection. Can be
/// used for rendering stereo effects like [3D Anaglyph](https://en.wikipedia.org/wiki/Anaglyph_3D)
/// or [Parallax Barrier](https://en.wikipedia.org/wiki/parallax_barrier). The JavaScript-side
/// <c>THREE.StereoCamera</c>.
/// </summary>
public sealed class StereoCamera : ThreeObject
{
	private float _aspect = 1f;
	private float _eyeSep = 0.064f;
	private PerspectiveCamera? _cameraL;
	private PerspectiveCamera? _cameraR;
	private bool _isAspectWritten;
	private bool _isEyeSepWritten;
	private bool _isCameraLWritten;
	private bool _isCameraRWritten;

	/// <summary>Initializes a new <see cref="StereoCamera"/>.</summary>
	public StereoCamera()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.StereoCamera</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "StereoCamera"; }
	}

	/// <summary>
	/// The aspect. Writing it records a <c>aspect</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public float Aspect
	{
		get { return _aspect; }
		set
		{
			if (_aspect == value)
			{
				return;
			}

			_aspect = value;
			_isAspectWritten = true;
			RecordSet("aspect", value);
		}
	}

	/// <summary>
	/// The eye separation which represents the distance between the left and right camera. Writing it
	/// records a <c>eyeSep</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float EyeSep
	{
		get { return _eyeSep; }
		set
		{
			if (_eyeSep == value)
			{
				return;
			}

			_eyeSep = value;
			_isEyeSepWritten = true;
			RecordSet("eyeSep", value);
		}
	}

	/// <summary>
	/// The camera representing the left eye. This is added to layer <c>1</c> so objects to be rendered
	/// by the left camera must also be added to this layer. Writing it records a <c>cameraL</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public PerspectiveCamera? CameraL
	{
		get { return _cameraL; }
		set
		{
			if (ReferenceEquals(_cameraL, value))
			{
				return;
			}

			_cameraL = value;
			_isCameraLWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("cameraL", value);
		}
	}

	/// <summary>
	/// The camera representing the right eye. This is added to layer <c>2</c> so objects to be rendered
	/// by the right camera must also be added to this layer. Writing it records a <c>cameraR</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public PerspectiveCamera? CameraR
	{
		get { return _cameraR; }
		set
		{
			if (ReferenceEquals(_cameraR, value))
			{
				return;
			}

			_cameraR = value;
			_isCameraRWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("cameraR", value);
		}
	}

	/// <summary>Updates the stereo camera based on the given perspective camera.</summary>
	/// <param name="camera">The perspective camera.</param>
	public void Update(PerspectiveCamera camera)
	{
		RecordCall("update", camera);
	}

	/// <summary>
	/// Emits the create op for <c>THREE.StereoCamera</c>, then replays every property written before
	/// this object was attached. A replayed value that is itself a mirrored object is attached first,
	/// so its create op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isAspectWritten)
		{
			batch.Set(Handle, "aspect", ThreeValue.Encode(_aspect));
		}

		if (_isEyeSepWritten)
		{
			batch.Set(Handle, "eyeSep", ThreeValue.Encode(_eyeSep));
		}

		if (_isCameraLWritten)
		{
			_cameraL?.AttachTo(batch);
			batch.Set(Handle, "cameraL", ThreeValue.Encode(_cameraL));
		}

		if (_isCameraRWritten)
		{
			_cameraR?.AttachTo(batch);
			batch.Set(Handle, "cameraR", ThreeValue.Encode(_cameraR));
		}
	}
}
