using System.Text.Json;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// A plain data value three.js describes with an interface rather than a class: a geometry group, a
/// batched-mesh range. It has no identity on the JavaScript side, so a handle would be the wrong shape
/// for it — two groups with the same numbers are the same group, and nothing on either side keeps a
/// reference to one.
/// <para>
/// Implemented by generated records, which is why both halves are here rather than left to reflection:
/// the emitter knows every member and its type, so the wire form is written out rather than discovered,
/// and a member the mirror could not encode would have stopped the whole type being generated.
/// </para>
/// <para>
/// ⚠️ Both directions use three.js's own member names, not the C# ones. The object is assigned straight
/// onto a three.js instance or read straight off one, so <c>materialIndex</c> has to stay
/// <c>materialIndex</c>.
/// </para>
/// </summary>
public interface IThreeStructure
{
    /// <summary>
    /// This value's members, keyed by three.js's name for each, ready for <c>ThreeValue.Encode</c> to
    /// encode individually. A member the caller left unset is omitted rather than sent as null, so
    /// three.js applies its own default the way it would for an object literal that never mentioned it.
    /// </summary>
    /// <returns>The members to send.</returns>
    IReadOnlyDictionary<string, object?> ToWireMembers();

    /// <summary>
    /// Builds this value from the members the applier sent back.
    /// <para>
    /// Answers with a new instance rather than filling this one in, because the generated records are
    /// immutable: every member is <c>init</c>-only, which is what makes a value with no identity behave
    /// like one. The receiver is a blank instance the decoder created only to reach this method.
    /// </para>
    /// </summary>
    /// <param name="members">The decoded members, keyed by three.js's name for each.</param>
    /// <returns>The value those members describe.</returns>
    IThreeStructure FromWireMembers(IReadOnlyDictionary<string, JsonElement> members);
}
