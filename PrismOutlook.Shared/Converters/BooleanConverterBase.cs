using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;

namespace PrismOutlook.Shared.Converters;

public abstract class BooleanConverterBase<T> : IValueConverter
{
	private static readonly IEqualityComparer<T> __equalityComparer = EqualityComparer<T>.Default;

	protected BooleanConverterBase()
	{
	}

	[NotNull]
	protected abstract T TrueValue { get; }

	[NotNull]
	protected abstract T FalseValue { get; }

	/// <inheritdoc />
	[NotNull]
	public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value is true
					? TrueValue
					: FalseValue;
	}

	/// <inheritdoc />
	public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is T v)
		{
			if (__equalityComparer.Equals(TrueValue, v)) return true;
			if (__equalityComparer.Equals(FalseValue, v)) return false;
		}

		throw new FormatException($"Value {value} was not recognized as a valid Boolean.");
	}
}