using FontAwesome.Sharp;

namespace PrismOutlook.Shared.Converters;

public class BooleanToVerticalChevronIconConverter : BooleanToChevronIconConverter
{
	public BooleanToVerticalChevronIconConverter()
	{
	}

	/// <inheritdoc />
	protected sealed override IconChar TrueValue => IconChar.ThumbTack;

	/// <inheritdoc />
	protected sealed override IconChar FalseValue => IconChar.ChevronUp;
}