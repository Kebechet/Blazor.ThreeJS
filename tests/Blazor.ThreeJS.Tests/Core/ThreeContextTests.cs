using Kebechet.Blazor.ThreeJS.Core;
using Microsoft.JSInterop;
using NSubstitute;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

public class ThreeContextTests
{
	[Fact]
	public async Task ThreeContext_DisposeWhenCircuitDisconnected_DoesNotThrow()
	{
		// Arrange
		var module = Substitute.For<IJSObjectReference>();
		module.DisposeAsync().Returns(ValueTask.FromException(new JSDisconnectedException("Circuit disconnected.")));
		var context = new ThreeContext(module, contextId: 1);

		// Act
		var exception = await Record.ExceptionAsync(() => context.DisposeAsync().AsTask());

		// Assert
		exception.ShouldBeNull();
	}
}
