using Kebechet.Blazor.ThreeJS.Core;
using Microsoft.JSInterop;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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

	[Fact]
	public async Task ThreeContext_FlushWithNoPendingOps_MakesNoInteropCall()
	{
		// Arrange
		var module = Substitute.For<IJSObjectReference>();
		var context = new ThreeContext(module, contextId: 1);

		// Act
		await context.FlushAsync();

		// Assert
		module.ReceivedCalls().ShouldBeEmpty();
	}

	[Fact]
	public async Task ThreeContext_FlushWithApplierErrors_RaisesOnError()
	{
		// Arrange
		var applierErrors = new List<ThreeError> { new() { Handle = 1, Member = "roughness", Message = "Invalid roughness value." } };
		var module = Substitute.For<IJSObjectReference>();
		module.InvokeAsync<List<ThreeError>>(Arg.Any<string>(), Arg.Any<object[]>()).Returns(applierErrors);
		var context = new ThreeContext(module, contextId: 1);
		context.Batch.Set(1, "visible", true);
		IReadOnlyList<ThreeError>? raisedErrors = null;
		context.OnError += errors => raisedErrors = errors;

		// Act
		await context.FlushAsync();

		// Assert
		raisedErrors.ShouldBe(applierErrors);
	}

	[Fact]
	public async Task ThreeContext_FlushWithNoApplierErrors_DoesNotRaiseOnError()
	{
		// Arrange
		var module = Substitute.For<IJSObjectReference>();
		module.InvokeAsync<List<ThreeError>>(Arg.Any<string>(), Arg.Any<object[]>()).Returns([]);
		var context = new ThreeContext(module, contextId: 1);
		context.Batch.Set(1, "visible", true);
		var wasOnErrorRaised = false;
		context.OnError += _ => wasOnErrorRaised = true;

		// Act
		await context.FlushAsync();

		// Assert
		wasOnErrorRaised.ShouldBeFalse();
	}

	[Fact]
	public async Task ThreeContext_FlushWhenCircuitDisconnected_DoesNotThrow()
	{
		// Arrange
		var module = Substitute.For<IJSObjectReference>();
		module.InvokeAsync<List<ThreeError>>(Arg.Any<string>(), Arg.Any<object[]>())
			.ThrowsAsync(new JSDisconnectedException("Circuit disconnected."));
		var context = new ThreeContext(module, contextId: 1);
		context.Batch.Set(1, "visible", true);

		// Act
		var exception = await Record.ExceptionAsync(() => context.FlushAsync());

		// Assert
		exception.ShouldBeNull();
	}
}
