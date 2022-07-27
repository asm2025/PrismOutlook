using System;
using System.Windows;
using System.Windows.Navigation;
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
using PrismOutlook.Views;
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
	private ILogger logger;

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
		Configuration = CreateConfiguration(e.Args);
		AppTitle = Configuration.GetValue("title", AppTitle);

		// Logging
		LoggerConfiguration loggerConfiguration = new LoggerConfiguration();

		if (Configuration.GetValue("LoggingEnabled", true))
		{
			loggerConfiguration.ReadFrom.Configuration(Configuration)
								.Enrich.WithProperty("ApplicationName", AppTitle);
		}

		Log.Logger = loggerConfiguration.CreateLogger();
		logger = Log.Logger;
		ApplicationConfiguration.Initialize();
		logger.Information($"{AppTitle} is starting...");
		base.OnStartup(e);
	}

	/// <inheritdoc />
	protected override void ConfigureViewModelLocator()
	{
		logger.Debug("Configuring view model locator.");
		base.ConfigureViewModelLocator();
	}

	/// <inheritdoc />
	protected override void Initialize()
	{
		logger.Debug("Initializing...");
		base.Initialize();
	}

	/// <inheritdoc />
	[NotNull]
	protected override IContainerExtension CreateContainerExtension()
	{
		logger.Debug("Creating container extension.");
		ServiceCollection services = new ServiceCollection();
		ConfigureServices(services, Configuration);
		UnityContainer container = new UnityContainer();
		container.BuildServiceProvider(services);
		return new UnityContainerExtension(container);
	}

	/// <inheritdoc />
	protected override IModuleCatalog CreateModuleCatalog()
	{
		logger.Debug("Creating module catalog.");
		return base.CreateModuleCatalog();
	}

	/// <inheritdoc />
	protected override void RegisterTypes(IContainerRegistry containerRegistry)
	{
		logger.Debug("Registering types.");
	}

	/// <inheritdoc />
	protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
	{
		logger.Debug("Configuring module catalog.");
		base.ConfigureModuleCatalog(moduleCatalog);
	}

	/// <inheritdoc />
	protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
	{
		logger.Debug("Configuring region adapter mappings.");
		base.ConfigureRegionAdapterMappings(regionAdapterMappings);
	}

	/// <inheritdoc />
	protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
	{
		logger.Debug("Configuring default region behavior.");
		base.ConfigureDefaultRegionBehaviors(regionBehaviors);
	}

	protected override Window CreateShell()
	{
		logger.Debug("Creating shell.");
		return Container.Resolve<MainWindow>();
	}

	/// <inheritdoc />
	protected override void InitializeShell(Window shell)
	{
		logger.Debug("Initializing shell.");
		base.InitializeShell(shell);
	}

	/// <inheritdoc />
	protected override void InitializeModules()
	{
		logger.Debug("Initializing modules.");
		base.InitializeModules();
	}

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		logger.Information($"{AppTitle} initialized.");
		base.OnInitialized();
	}

	/// <inheritdoc />
	protected override void OnLoadCompleted(NavigationEventArgs e)
	{
		logger.Debug("load completed.");
		base.OnLoadCompleted(e);
	}

	/// <inheritdoc />
	protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
	{
		logger.Debug($"Session ending, {e.ReasonSessionEnding}.");
		base.OnSessionEnding(e);
	}

	/// <inheritdoc />
	protected override void OnExit(ExitEventArgs e)
	{
		logger.Information($"{AppTitle} exiting with exit code ${e.ApplicationExitCode}.");
		Log.CloseAndFlush();
		base.OnExit(e);
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