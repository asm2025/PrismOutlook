using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using FontAwesome.Sharp;

namespace PrismOutlook.Shared.Converters;

[ValueConversion(typeof(bool), typeof(Image))]
public class BooleanToVerticalChevronImageConverter : BooleanToVerticalChevronIconConverter, IHaveIconFont
{
	private readonly IconToImageConverter _converter;

	public BooleanToVerticalChevronImageConverter()
	{
		_converter = new IconToImageConverter();
	}

	public Brush Foreground
	{
		get => _converter.Foreground;
		set => _converter.Foreground = value;
	}

	public Style ImageStyle
	{
		get => _converter.ImageStyle;
		set => _converter.ImageStyle = value;
	}

	/// <inheritdoc />
	public IconFont IconFont
	{
		get => _converter.IconFont;
		set => _converter.IconFont = value;
	}

	/// <inheritdoc />
	public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		object iconChar = base.Convert(value, targetType, parameter, culture);
		return _converter.Convert(iconChar, targetType, parameter, culture);
	}

	/// <inheritdoc />
	public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
}