using FakeDOORS.DatabaseControls.ChapterSelectionControls;
using FakeDOORS.DatabaseControls.DatabaseSettingsControls;
using FakeDOORS.DatabaseControls.TestCasesControls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReqTools;
using System;
using System.IO;
using System.Windows;

namespace FakeDOORS
{
    public partial class App : Application
    {
        static public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddScoped<IReqParser, ReqParser>();
            services.AddTransient<MainWindow>();
            services.AddTransient<IDatabaseView, DatabaseView>();
            services.AddTransient<IRequirementsView, RequirementsView>();
            services.AddTransient<ITestCasesView, TestCasesView>();
            services.AddTransient<IChapterSelectionView, ChapterSelectionView>();
            services.AddTransient<IDatabaseSettingsView, DatabaseSettingsView>();
            services.AddTransient<IUpdaterView, UpdaterView>();
        }
    }
}
