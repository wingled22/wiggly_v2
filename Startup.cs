using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyExtensions.BackgroundServiceExtensions;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Wiggly.BackgroundJob;
using Wiggly.Entities;
using Wiggly.Hubs;
using Wiggly.Identity;
using Wiggly.Services;

namespace Wiggly
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
               .UseSimpleAssemblyNameTypeSerializer().UseDefaultTypeSerializer().UseMemoryStorage()
            );
            services.AddHangfireServer();

            services.AddTransient<ITextJob, TextJob>();

            services.AddDbContext<WigglyContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.AddDbContext<WigglyIdentityContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<AppUser, AppRole>(option => {
                option.SignIn.RequireConfirmedAccount = false;
                option.SignIn.RequireConfirmedEmail = false;
                option.SignIn.RequireConfirmedPhoneNumber = false;
                option.User.RequireUniqueEmail = true;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireLowercase = false;
                option.Password.RequiredUniqueChars = 0;
                option.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<WigglyIdentityContext>();

            services.ConfigureApplicationCookie(option => {
                option.LoginPath = "/Account/Login";
                option.AccessDeniedPath = "/Account/Login";
            });

            services.AddSession();
            services.AddAutoMapper(typeof(Startup));
            services.AddControllersWithViews().AddJsonOptions(options => {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            })
            .AddNewtonsoftJson(options => {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            //services.AddScheduledService<BackgroundTextScheduler>(Configuration);
           

            services.AddCors();
            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddSignalR();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IBackgroundJobClient backgroundJobClient, 
            IRecurringJobManager recurringJobManager, 
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseMvc(route =>
            //{
            //    route.MapRoute(name: "mvcAreaRoute", template: "{area:exists}/{controller=Home}/{action=Index}");
            //    route.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapControllerRoute(
                       name: "default",
                       pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapHub<ChatHub>("/chatHub");
            });

            //TODO: do this when doing the textmessage
            //app.UseHangfireDashboard();
            //backgroundJobClient.Enqueue(() => Console.WriteLine("hahaha text plsss"));
            //recurringJobManager.AddOrUpdate("run every minute", () => serviceProvider.GetService<ITextJob>().TextSchedules(), "* * * * *");

        }
    }
}


//TODO:, notif
