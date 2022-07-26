using Prism.Mvvm;

namespace PrismOutlook.ViewModels;

public class MainWindowViewModel : BindableBase
{
	private string _title;

	public MainWindowViewModel()
	{

	}

	public string Title
	{
		get => _title;
		set => SetProperty(ref _title, value);
	}
}