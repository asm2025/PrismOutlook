using System.Windows.Data;
using FontAwesome.Sharp;

namespace PrismOutlook.Shared.Converters;

[ValueConversion(typeof(bool), typeof(IconChar))]
public abstract class BooleanToChevronIconConverter : BooleanConverterBase<IconChar>
{
	protected BooleanToChevronIconConverter()
	{
	}
}