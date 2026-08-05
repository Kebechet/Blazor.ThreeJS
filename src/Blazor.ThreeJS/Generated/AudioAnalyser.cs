// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>This class can be used to analyse audio data. The JavaScript-side <c>THREE.AudioAnalyser</c>.</summary>
public sealed class AudioAnalyser : ThreeObject
{
	private readonly Audio _audio;
	private readonly float _fftSize;

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
	}
}
