using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using essentialMix.Extensions;
using essentialMix.Helpers;
using essentialMix.Newtonsoft.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using PrismOutlook.Modules.Calendar;
using PrismOutlook.Modules.Contacts;
using PrismOutlook.Modules.Mail;
using PrismOutlook.Modules.Tasks;
using PrismOutlook.Windows;
using Serilog;
using Unity;
using Unity.Microsoft.DependencyInjection;
using ILogger = Serilog.ILogger;

namespace PrismOutlook;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
	private ILogger _logger;

	static App()
	{
		AppTitle = AppDomain.CurrentDomain.FriendlyName;
		AppPath = AssemblyHelper.GetEntryAssembly().GetDirectoryPath();
		JsonConvert.DefaultSettings = () => JsonHelper.SetDefaults(JsonHelper.CreateSettings(false));
	}

	public static string AppTitle { get; private set; }
	public static string AppPath { get; }
	public static IConfiguration Configuration { get; private set; }

	/// <inheritdoc />
	protected override void OnStartup([NotNull] StartupEventArgs e)
	{
#if DEBUG
		Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
#endif

		// Configuration
		Configuration = CreateConfiguration(e.Args);
		AppTitle = Configuration.GetValue("title", AppTitle);

		// Logging
		LoggerConfiguration loggerConfiguration = new LoggerConfiguration();

		if (Configuration.GetValue("LoggingEnabled", true))
		{
			loggerConfiguration.ReadFrom.Configuration(Configuration)
								.Enrich.WithProperty("ApplicationName", AppTitle);
			Log.Logger = loggerConfiguration.CreateLogger();
		}

		_logger = Log.Logger;
		_logger.Information($"{AppTitle} is starting...");

		// Error handling
		AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
		TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
		Dispatcher.UnhandledException += OnDispatcherUnhandledException;
		DispatcherUnhandledException += OnDispatcherUnhandledException;

		base.OnStartup(e);
	}

	/// <inheritdoc />
	protected override void ConfigureViewModelLocator()
	{
		_logger.Debug("Configuring view model locator.");
		base.ConfigureViewModelLocator();
	}

	/// <inheritdoc />
	protected override void Initialize()
	{
		_logger.Debug("Initializing...");
		base.Initialize();
	}

	/// <inheritdoc />
	[NotNull]
	protected override IContainerExtension CreateContainerExtension()
	{
		_logger.Debug("Creating container extension.");
		ServiceCollection services = new ServiceCollection();
		ConfigureServices(services, Configuration);
		UnityContainer container = new UnityContainer();
		container.BuildServiceProvider(services);
		return new UnityContainerExtension(container);
	}

	/// <inheritdoc />
	protected override IModuleCatalog CreateModuleCatalog()
	{
		_logger.Debug("Creating module catalog.");
		return base.CreateModuleCatalog();
	}

	/// <inheritdoc />
	protected override void RegisterTypes(IContainerRegistry containerRegistry)
	{
		_logger.Debug("Registering types.");
	}

	/// <inheritdoc />
	protected override void ConfigureModuleCatalog([NotNull] IModuleCatalog moduleCatalog)
	{
		_logger.Debug("Configuring module catalog.");
		moduleCatalog.AddModule<MailModule>();
		moduleCatalog.AddModule<CalendarModule>();
		moduleCatalog.AddModule<TasksModule>();
		moduleCatalog.AddModule<ContactsModule>();
		base.ConfigureModuleCatalog(moduleCatalog);
	}

	/// <inheritdoc />
	protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
	{
		_logger.Debug("Configuring region adapter mappings.");
		base.ConfigureRegionAdapterMappings(regionAdapterMappings);
	}

	/// <inheritdoc />
	protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
	{
		_logger.Debug("Configuring default region behavior.");
		base.ConfigureDefaultRegionBehaviors(regionBehaviors);
	}

	protected override Window CreateShell()
	{
		_logger.Debug("Creating shell.");
		return Container.Resolve<MainWindow>();
	}

	/// <inheritdoc />
	protected override void InitializeShell(Window shell)
	{
		_logger.Debug("Initializing shell.");
		base.InitializeShell(shell);
	}

	/// <inheritdoc />
	protected override void InitializeModules()
	{
		_logger.Debug("Initializing modules.");
		base.InitializeModules();
	}

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		_logger.Information($"{AppTitle} initialized.");
		base.OnInitialized();
	}

	/// <inheritdoc />
	protected override void OnLoadCompleted(NavigationEventArgs e)
	{
		_logger.Debug("load completed.");
		base.OnLoadCompleted(e);
	}

	/// <inheritdoc />
	protected override void OnSessionEnding([NotNull] SessionEndingCancelEventArgs e)
	{
		_logger.Debug($"Session ending, {e.ReasonSessionEnding}.");
		base.OnSessionEnding(e);
	}

	/// <inheritdoc />
	protected override void OnExit([NotNull] ExitEventArgs e)
	{
		_logger.Information($"{AppTitle} exiting with exit code ${e.ApplicationExitCode}.");
		Log.CloseAndFlush();
		base.OnExit(e);
	}

	private void OnUnhandledException(object sender, [NotNull] UnhandledExceptionEventArgs e)
	{
		Exception ex = e.ExceptionObject as Exception;
		string error = ex?.CollectMessages() ?? e.ExceptionObject.ToString() ?? "Unknown error occurred.";
		_logger.Error(ex, error);
		MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
	}

	private void OnDispatcherUnhandledException(object sender, [NotNull] DispatcherUnhandledExceptionEventArgs e)
	{
		string error = e.Exception.CollectMessages();
		_logger.Error(e.Exception, error);
		e.Handled = true;
		MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
	}

	private void OnUnobservedTaskException(object sender, [NotNull] UnobservedTaskExceptionEventArgs e)
	{
		string error = e.Exception.CollectMessages();
		_logger.Error(e.Exception, error);
		e.SetObserved();
		MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
	}

	private static IConfiguration CreateConfiguration(string[] args)
	{
		string environment = EnvironmentHelper.GetEnvironmentName();
		return IConfigurationBuilderHelper.CreateConfiguration(AppPath)
											.AddConfigurationFiles(environment)
											.AddEnvironmentVariables()
											.AddUserSecrets()
											.AddArguments(args)
											.Build();
	}

	private static void ConfigureServices([NotNull] IServiceCollection services, [NotNull] IConfiguration configuration)
	{
		// services
		services
			// config
			.AddSingleton(configuration)
			// logging
			.AddLogging(config =>
			{
				config
					.AddDebug()
					.AddConsole()
					.AddEventSourceLogger()
					.AddSerilog();
			})
			.AddSingleton(typeof(ILogger<>), typeof(Logger<>))
			// Mapper
			//.AddAutoMapper((_, builder) =>
			//				{
			//					builder.AddProfile(new InvitationMapperProfile());
			//				},
			//				new[]
			//				{
			//					typeof(Constants).Assembly
			//				},
			//				ServiceLifetime.Singleton)
			// Database
			//.AddDbContext<AdminIdentityDbContext>(builder =>
			//{
			//	// https://docs.microsoft.com/en-us/ef/core/querying/tracking
			//	// https://stackoverflow.com/questions/12726878/global-setting-for-asnotracking
			//	//builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
			//	bool enableLogging = configuration.GetValue<bool>("DataLoggingEnabled");

			//	if (enableLogging)
			//	{
			//		builder.UseLoggerFactory(LogFactoryHelper.ConsoleLoggerFactory)
			//				.EnableSensitiveDataLogging();
			//	}

			//	string connectionString = configuration.GetConnectionString("IdentityDbConnection");
			//	builder.UseSqlServer(connectionString, e => e.MigrationsAssembly(typeof(AdminIdentityDbContext).Assembly.GetName().Name));
			//	builder.EnableDetailedErrors(environment.IsDevelopment());
			//}/*, ServiceLifetime.Singleton*/)
			//.AddScoped<IApplicationRepository, ApplicationRepository>()
			//.AddScoped<IFormulaRepository, FormulaRepository>()
			// Forms & Pages
			;
	}
}