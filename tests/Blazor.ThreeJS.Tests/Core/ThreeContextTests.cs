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
	public async Task ThreeContext_DisposeWhenModuleAlreadyDisposed_DoesNotThrow()
	{
		// Arrange
		var context = new ThreeContext(new AlreadyDisposedJsObjectReference(), contextId: 1);

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

	[Fact]
	public async Task ThreeContext_SetActiveSceneWhenCircuitDisconnected_DoesNotThrow()
	{
		// Arrange
		var context = new ThreeContext(new ThrowingJsObjectReference(), contextId: 1);

		// Act
		var exception = await Record.ExceptionAsync(() => context.SetActiveSceneAsync(1, 2));

		// Assert
		exception.ShouldBeNull();
	}
}

/// <summary>
/// Fake <see cref="IJSObjectReference"/> that fails every call with <see cref="JSDisconnectedException"/>.
/// NSubstitute cannot stand in for this: the interop path under test goes through
/// <c>InvokeVoidAsync</c>, which closes over <c>Microsoft.JSInterop.Infrastructure.IJSVoidResult</c> —
/// a type internal to the JSInterop assembly, so a substitute cannot be configured against that
/// generic instantiation from this test project. Implementing the interface directly sidesteps the
/// problem, since the generic type argument never has to be named by the caller.
/// </summary>
internal sealed class ThrowingJsObjectReference : IJSObjectReference
{
	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
	{
		return ValueTask.FromException<TValue>(new JSDisconnectedException("Circuit disconnected."));
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
	{
		return ValueTask.FromException<TValue>(new JSDisconnectedException("Circuit disconnected."));
	}

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
}

/// <summary>
/// Fake <see cref="IJSObjectReference"/> that fails every call with <see cref="ObjectDisposedException"/>,
/// standing in for a module reference that was already disposed by a racing <c>ThreeCanvas</c> disposal
/// before <see cref="ThreeContext.DisposeAsync"/> gets to use it. Real
/// <c>Microsoft.JSInterop.Implementation.JSObjectReference</c> throws exactly this from
/// <c>InvokeAsync</c>/<c>InvokeVoidAsync</c> once disposed, per <c>ThrowIfDisposed()</c>.
/// </summary>
internal sealed class AlreadyDisposedJsObjectReference : IJSObjectReference
{
	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
	{
		return ValueTask.FromException<TValue>(new ObjectDisposedException(nameof(AlreadyDisposedJsObjectReference)));
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
	{
		return ValueTask.FromException<TValue>(new ObjectDisposedException(nameof(AlreadyDisposedJsObjectReference)));
	}

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
}
