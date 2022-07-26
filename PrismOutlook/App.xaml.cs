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
using Prism.Unity;
using PrismOutlook.Views;
using Serilog;
using Unity;
using Unity.Microsoft.DependencyInjection;

namespace PrismOutlook;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
	private ILogger<App> logger;

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
		ApplicationConfiguration.Initialize();
		base.OnStartup(e);
	}

	/// <inheritdoc />
	[NotNull]
	protected override IContainerExtension CreateContainerExtension()
	{
		ServiceCollection services = new ServiceCollection();
		ConfigureServices(services, Configuration);
		UnityContainer container = new UnityContainer();
		container.BuildServiceProvider(services);
		return new UnityContainerExtension(container);
	}

	protected override void RegisterTypes(IContainerRegistry containerRegistry)
	{

	}

	protected override Window CreateShell()
	{
		logger = Container.Resolve<ILogger<App>>();
		logger.LogInformation($"{AppTitle} is starting...");
		return Container.Resolve<MainWindow>();
	}

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		logger.LogInformation($"{AppTitle} initialized.");
		base.OnInitialized();
	}

	/// <inheritdoc />
	protected override void OnLoadCompleted(NavigationEventArgs e)
	{
		logger.LogInformation($"{AppTitle} load completed.");
		base.OnLoadCompleted(e);
	}

	/// <inheritdoc />
	protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
	{
		logger.LogInformation($"{AppTitle} session ending.");
		base.OnSessionEnding(e);
	}

	/// <inheritdoc />
	protected override void OnExit(ExitEventArgs e)
	{
		logger.LogInformation($"{AppTitle} finished.");
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