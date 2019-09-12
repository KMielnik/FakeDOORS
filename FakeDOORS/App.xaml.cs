using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ReqTools;
using FakeDOORS.DatabaseControls.TestCasesControls;

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
            services.AddTransient<DatabaseView>();
            services.AddTransient<RequirementsView>();
            services.AddTransient<TestCasesView>();
        }
    }
}
