// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>This class can be used to analyse audio data. The JavaScript-side <c>THREE.AudioAnalyser</c>.</summary>
public sealed class AudioAnalyser : ThreeObject
{
	private readonly Audio _audio;
	private readonly float _fftSize;
	private Uint8Array _data = new Uint8Array();
	private bool _isDataWritten;

	/// <summary>Constructs a new audio analyzer.</summary>
	/// <param name="audio">The audio to analyze.</param>
	/// <param name="fftSize">
	/// The window size in samples that is used when performing a Fast Fourier Transform (FFT) to get
	/// frequency domain data.
	/// </param>
	public AudioAnalyser(Audio audio, float fftSize = 2048f)
	{
		_audio = audio;
		_fftSize = fftSize;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>AudioAnalyser</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal AudioAnalyser(ThreeBatch batch, int handle)
		: base(handle)
	{
		_audio = default!;
		_fftSize = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AudioAnalyser</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "AudioAnalyser"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.AudioAnalyser</c>: audio, fftSize.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_audio, _fftSize]; }
	}

	/// <summary>
	/// Holds the analyzed data. Writing it records a <c>data</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public Uint8Array Data
	{
		get { return _data; }
		set
		{
			if (_data == value)
			{
				return;
			}

			_data = value;
			_isDataWritten = true;
			RecordSet("data", value);
		}
	}

	/// <summary>
	/// Returns an array with frequency data of the audio. Each item in the array represents the decibel
	/// value for a specific frequency. The frequencies are spread linearly from 0 to 1/2 of the sample
	/// rate. For example, for 48000 sample rate, the last item of the array will represent the decibel
	/// value for 24000 Hz. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getFrequencyData</c> returned.
	/// </summary>
	/// <returns>The value <c>getFrequencyData</c> returned, once the JavaScript side has answered.</returns>
	public Task<Uint8Array> GetFrequencyDataAsync()
	{
		return RecordRead<Uint8Array>("getFrequencyData");
	}

	/// <summary>
	/// Returns the average of the frequencies returned by <c>AudioAnalyser#getFrequencyData</c>.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>getAverageFrequency</c> returned.
	/// </summary>
	/// <returns>The value <c>getAverageFrequency</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetAverageFrequencyAsync()
	{
		return RecordRead<float>("getAverageFrequency");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.AudioAnalyser</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_audio.AttachTo(batch);

		base.EmitCreate(batch);

		if (_isDataWritten)
		{
			batch.Set(Handle, "data", ThreeValue.Encode(_data));
		}
	}
}
