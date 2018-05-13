using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Stardew.MPSaveEditor.UI
{
    public class Startup
    {
        private static BrowserWindow _browserWindow;

        private static PhysicalFileProvider _fileProvider;
        private static Timer _ticker;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                if (HybridSupport.IsElectronActive)
                {
                    //the below is a hacky way to get hot module loading working for the ElectronNet app.
                    _fileProvider = new PhysicalFileProvider(env.WebRootPath);
                    _ticker = new Timer(TimerMethod, null, 1000, 1000);
                }
                else
                {
                    app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                    {
                        HotModuleReplacement = true
                    });
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    "spa-fallback",
                    new {controller = "Home", action = "Index"});
            });

            if (HybridSupport.IsElectronActive)
                ElectronBootstrap();
        }


        private async void ElectronBootstrap()
        {
            _browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Show = false
            });

            _browserWindow.OnReadyToShow += () => _browserWindow.Show();
        }

        private static void TimerMethod(object state)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var token = _fileProvider.Watch("**/*");
            var source = new TaskCompletionSource<object>();
            token.RegisterChangeCallback(state =>
                ((TaskCompletionSource<object>) state).TrySetResult(null), source);
            await source.Task.ConfigureAwait(false);
            _browserWindow.Reload();
        }
    }
}
