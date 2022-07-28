using System;
using System.Windows.Shell;
using essentialMix;
using essentialMix.Extensions;
using essentialMix.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PrismOutlook.Core.ViewModels;

namespace PrismOutlook.ViewModels;

public class MainViewModel : ViewModelBase
{
	private const string STATUS_DEF = "Ready...";

	private string _status;
	private string _operation;
	private int _progress;
	private TaskbarItemProgressState _progressState;

	public MainViewModel([NotNull] IConfiguration configuration, [NotNull] ILogger<MainViewModel> logger)
		: base(logger)
	{
		Info = new AppInfo(AssemblyHelper.GetEntryAssembly());
		Title = configuration.GetValue("title", Info.Title.ToNullIfEmpty() ?? AppDomain.CurrentDomain.FriendlyName);
	}

	[NotNull]
	public AppInfo Info { get; }

	public string Title { get; }

	public string Status
	{
		get => _status;
		set => SetProperty(ref _status, value ?? STATUS_DEF, nameof(Status));
	}

	public string Operation
	{
		get => _operation;
		set => SetProperty(ref _operation, value, nameof(Operation));
	}

	public int Progress
	{
		get => _progress;
		set => SetProperty(ref _progress, value, nameof(Progress));
	}

	public TaskbarItemProgressState ProgressState
	{
		get => _progressState;
		set => SetProperty(ref _progressState, value, nameof(ProgressState));
	}
}