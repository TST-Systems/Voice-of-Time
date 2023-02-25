using ElectronNET.API.Entities;
using ElectronNET.API;

namespace Test_GUI_VoT
{
	public class Startup
	{

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			//services.AddSingleton(sg => { var log = sg.GetService<ILogger<Startup>>()});
			services.AddRazorPages();
			//services.AddServerSideBlazor();
			//services.AddSingleton<MyService>(); // Other singletons
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			//app.UseHttpsRedirection(); // disabled in project settings webserver too.
			app.UseStaticFiles();
			app.UseRouting();

			// For blazor
			//app.UseEndpoints(endpoints => 
			//{
			//		endpoints.MapBlazorHub();
			//		endpoints.MapFallbackToPage("/_Host");
			//});

			// For regular
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
			});


			if (HybridSupport.IsElectronActive)
			{
				ElectronCreateWindow();
			}
		}

		public async void ElectronCreateWindow()
		{
			var browserWindowOptions = new BrowserWindowOptions
			{
				Width = 1024,
				Height = 768,
				Show = false, // wait to open it
				WebPreferences = new WebPreferences
				{
					WebSecurity = false
				}
			};

			//Overwite MenuItems with "null"
			//var menu = new MenuItem[] { };
			//Electron.Menu.SetApplicationMenu(menu);

			var browserWindow = await Electron.WindowManager.CreateWindowAsync(browserWindowOptions);
			await browserWindow.WebContents.Session.ClearCacheAsync();
			

			// Handler to show when it is ready
			browserWindow.OnReadyToShow += () =>
			{
				browserWindow.Show();
			};

			// Close Handler
			browserWindow.OnClose += () => Environment.Exit(0);
		}
	}
	
}
