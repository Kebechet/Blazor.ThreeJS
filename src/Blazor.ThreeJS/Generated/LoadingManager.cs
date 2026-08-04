// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Handles and keeps track of loaded and pending data. The JavaScript-side
/// <c>THREE.LoadingManager</c>.
/// </summary>
public sealed class LoadingManager : ThreeObject
{
	/// <summary>Initializes a new <see cref="LoadingManager"/>.</summary>
	public LoadingManager()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.LoadingManager</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "LoadingManager"; }
	}
}
