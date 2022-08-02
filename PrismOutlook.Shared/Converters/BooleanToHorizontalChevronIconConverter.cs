using FontAwesome.Sharp;

namespace PrismOutlook.Shared.Converters;

public class BooleanToHorizontalChevronIconConverter : BooleanToChevronIconConverter
{
	public BooleanToHorizontalChevronIconConverter()
	{
	}

	/// <inheritdoc />
	protected sealed override IconChar TrueValue => IconChar.ThumbTack;

	/// <inheritdoc />
	protected sealed override IconChar FalseValue => IconChar.ChevronLeft;
}