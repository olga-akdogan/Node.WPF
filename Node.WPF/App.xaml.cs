using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Node.ModelLibrary.Data;
using Node.ModelLibrary.Identity;
using Node.ModelLibrary.Seed;
using Node.WPF.Services;
using Node.WPF.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Node.WPF
{
    public partial class App : Application
    {
        private IHost? _host;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((ctx, services) =>
                {
                    
                    var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "node.db");
                    var connString = $"Data Source={dbPath}";


                    services.AddDbContext<AppDbContext>(opt =>
                        opt.UseSqlite(connString));

               
                    services.AddDbContextFactory<AppDbContext>(opt =>
                        opt.UseSqlite(connString));

                    services
                        .AddIdentityCore<AppUser>(options =>
                        {
                            
                            options.Password.RequireDigit = false;
                            options.Password.RequireLowercase = false;
                            options.Password.RequireUppercase = false;
                            options.Password.RequireNonAlphanumeric = false;
                            options.Password.RequiredLength = 6;
                        })
                        .AddRoles<IdentityRole>()
                        .AddEntityFrameworkStores<AppDbContext>();

                  
                    services.AddHttpClient<EphemerisClient>(c =>
                    {
                        c.BaseAddress = new Uri("https://ephemeris.fyi/");
                        c.Timeout = TimeSpan.FromSeconds(15);
                    });

                    services.AddHttpClient<GoogleGeocodingService>(c =>
                    {
                        c.Timeout = TimeSpan.FromSeconds(15);
                    });

                    services.AddHttpClient<OpenAiService>(c =>
                    {
                        c.Timeout = TimeSpan.FromSeconds(60);
                    })
                    .AddTypedClient((http, sp) =>
                    {
                        var apiKey = ctx.Configuration["OpenAI:ApiKey"] ?? "";
                        var model = ctx.Configuration["OpenAI:Model"] ?? "gpt-4.1-mini";
                        return new OpenAiService(http, apiKey, model);
                    });

                    
                    services.AddSingleton<Session>();
                    services.AddSingleton<NavigationService>();

                    services.AddTransient<AuthService>();
                    services.AddTransient<ChartService>();
                    services.AddTransient<LoveProfileService>();

                  
                    services.AddSingleton<MainViewModel>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<RegisterViewModel>();
                    services.AddTransient<BirthDataViewModel>();
                    services.AddTransient<HomeViewModel>();
                    services.AddTransient<ProfileViewModel>();


                    services.AddSingleton<MainWindow>();
                })
                .Build();

            await _host.StartAsync();

          
            using (var scope = _host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.EnsureCreatedAsync();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await DbSeeder.SeedAsync(db, userManager, roleManager);
            }

     
            var mainVm = _host.Services.GetRequiredService<MainViewModel>();
            mainVm.Start(); 

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = mainVm;
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                if (_host != null)
                    await _host.StopAsync();
            }
            finally
            {
                _host?.Dispose();
                base.OnExit(e);
            }
        }
    }
}