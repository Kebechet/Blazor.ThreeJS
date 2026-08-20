using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Math;

/// <summary>
/// The 2x2 matrix, ported by hand like the other math value types. What is pinned here is the storage
/// order: three.js and WebGL are column-major while <c>Set</c> reads row-major, and confusing the two
/// produces silently wrong transforms rather than an error.
/// </summary>
public class Matrix2Tests
{
	[Fact]
	public void Matrix2_New_IsTheIdentity()
	{
		new Matrix2().Elements.ShouldBe([1f, 0f, 0f, 1f]);
	}

	[Fact]
	public void Matrix2_SetInRowMajorReadingOrder_StoresColumnMajor()
	{
		// Written as it reads on paper: | 1 2 |
		//                               | 3 4 |
		var matrix = new Matrix2().Set(
			1f, 2f,
			3f, 4f);

		// Stored column-first, which is what WebGL uploads.
		matrix.Elements.ShouldBe([1f, 3f, 2f, 4f]);
	}

	[Fact]
	public void Matrix2_Determinant_IsTheCrossProductOfItsColumns()
	{
		new Matrix2().Set(1f, 2f, 3f, 4f).Determinant().ShouldBe(-2f);
	}

	[Fact]
	public void Matrix2_Transpose_ExchangesRowsAndColumns()
	{
		var matrix = new Matrix2().Set(1f, 2f, 3f, 4f).Transpose();

		matrix.Elements.ShouldBe([1f, 2f, 3f, 4f]);
	}

	[Fact]
	public void Matrix2_MultipliedByTheIdentity_IsUnchanged()
	{
		var matrix = new Matrix2().Set(1f, 2f, 3f, 4f);

		matrix.Multiply(new Matrix2());

		matrix.Elements.ShouldBe([1f, 3f, 2f, 4f]);
	}

	[Fact]
	public void Matrix2_Multiply_ComposesInTheOrderThreeJsDoes()
	{
		// | 1 2 | * | 0 1 | = | 2 3 |
		// | 3 4 |   | 1 1 |   | 4 7 |
		var matrix = new Matrix2().Set(1f, 2f, 3f, 4f);

		matrix.Multiply(new Matrix2().Set(0f, 1f, 1f, 1f));

		matrix.Elements.ShouldBe([2f, 4f, 3f, 7f]);
	}

	[Fact]
	public void Matrix2_EncodedAndDecoded_SurvivesTheRoundTrip()
	{
		var encoded = ThreeValue.Encode(new Matrix2().Set(1f, 2f, 3f, 4f));

		var tagged = encoded.ShouldBeOfType<ThreeValue.TaggedValue>();
		tagged.Tag.ShouldBe(ThreeWireFormat.Matrix2Tag);
		tagged.Values.ShouldBe([1f, 3f, 2f, 4f]);
	}

	[Fact]
	public void Matrix2_Clone_IsIndependentOfTheOriginal()
	{
		var original = new Matrix2().Set(1f, 2f, 3f, 4f);
		var clone = original.Clone();

		original.Identity();

		clone.Elements.ShouldBe([1f, 3f, 2f, 4f]);
	}
}
