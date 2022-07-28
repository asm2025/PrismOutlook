using essentialMix.Patterns.NotifyChange;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace PrismOutlook.Core.ViewModels;

public abstract class ViewModelBase : DisposableNotifyPropertyChangedBase
{
	protected ViewModelBase([NotNull] ILogger logger)
	{
		Logger = logger;
	}

	[NotNull]
	public ILogger Logger { get; }

}