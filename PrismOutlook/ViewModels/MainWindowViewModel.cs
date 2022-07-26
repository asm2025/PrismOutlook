using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Prism.Mvvm;

namespace PrismOutlook.ViewModels;

public class MainWindowViewModel : BindableBase
{
	private string _title;

	public MainWindowViewModel([NotNull] IConfiguration configuration)
	{
		_title = configuration.GetValue("title", AppDomain.CurrentDomain.FriendlyName);
	}

	public string Title
	{
		get => _title;
		set => SetProperty(ref _title, value);
	}
}