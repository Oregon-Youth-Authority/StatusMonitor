using ApplicationStatusMonitor.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSwag;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ApplicationStatusMonitor.Abstractions;
using ApplicationStatusMonitor.BackgroundServices;
using ApplicationStatusMonitor.Implementations;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using StatusMonitor.ApiKey.Providers;

namespace ApplicationStatusMonitor
{
   public class Startup
   {
      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
         ValidKeys = (Configuration["ApiKeySettings:ApiKeys"] ?? throw new Exception("Config value for ApiKeySettings:ApiKeys is missing"))
            .Split(",").ToArray();
      }

      public string[] ValidKeys { get; set; }

      public IConfiguration Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddSwaggerDocument(config => config.PostProcess = document =>
         {
            document.Info.Version = "v1.0.0";
            document.Info.Title = "Status Monitor API";
            document.Info.Description = "An API to record status updates and manage configuration for status monitors";
            document.Info.Contact = new OpenApiContact()
            {
               Name = "Oregon Youth Authority"
            };
         });

         services.AddControllers();
         services.AddRazorPages();
         services.Configure<StatusMonitorDatabaseSettings>(Configuration.GetSection(nameof(StatusMonitorDatabaseSettings)));
         services.AddSingleton<IStatusMonitorDatabaseSettings>(sp => sp.GetRequiredService<IOptions<StatusMonitorDatabaseSettings>>().Value);

         services.AddTransient<IStatusRepository<StatusMonitorReply>, StatusMonitorMongoRepository<StatusMonitorReply>>();
         services.AddTransient<IMonitorConfigurationRepository<MonitorConfiguration>, MonitorConfigurationMongoRepository<MonitorConfiguration>>();

         services.AddHostedService<DataCleanupBackgroundService>();
         services.AddHostedService<OfflineMonitorsBackgroundService>();

         services.AddIdentityMongoDbProvider<AppUser, AppRole>(identityOptions =>
            {
               identityOptions.SignIn.RequireConfirmedAccount = true;
               identityOptions.Password.RequiredLength = 6;
               identityOptions.Password.RequireLowercase = false;
               identityOptions.Password.RequireUppercase = false;
               identityOptions.Password.RequireNonAlphanumeric = false;
               identityOptions.Password.RequireDigit = false;
            }, mongoIdentityOptions =>
            {
               mongoIdentityOptions.UseDefaultIdentity = true;
               mongoIdentityOptions.ConnectionString = Configuration["StatusMonitorDatabaseSettings:ConnectionString"];
            })
            .AddRoleManager<RoleManager<AppRole>>()
            .AddDefaultUI();
         
         services
            .AddAuthentication(options =>
                {
                   options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                   options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                   options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
            .AddGoogle(options =>
               {
                  IConfigurationSection googleConfig = Configuration.GetSection("Authentication:Google");
                  options.ClientId = googleConfig["ClientId"];
                  options.ClientSecret = googleConfig["ClientSecret"];
               })
            .AddLinkedIn(options =>
            {
               var linkedinConfig = Configuration.GetSection("Authentication:LinkedIn");
               options.ClientId = linkedinConfig["ClientId"];
               options.ClientSecret = linkedinConfig["ClientSecret"];
            })
            //.AddMicrosoftAccount(microsoftOptions =>
            //{
            //   microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ClientId"];
            //   microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
            //})
            .AddApiKey(options =>
                {
                   options.Header = "X-API-KEY";
                   options.HeaderKey = String.Empty;
                   options.Events = new ApiKeyEvents
                   {
                      OnAuthenticationFailed = context =>
                      {
                         var ex = context.Exception;

                         Trace.TraceError(ex.Message);

                         context.Fail(ex);

                         return Task.CompletedTask;
                      },
                      OnApiKeyValidated = context =>
                      {
                         if (!ValidKeys.Contains(context.ApiKey))
                            return Task.CompletedTask;

                         context.Success();

                         return Task.CompletedTask;
                      }
                   };
                });

      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }

         app.UseHttpsRedirection();

         app.UseOpenApi();
         app.UseSwaggerUi3();

         app.UseRouting();

         app.UseAuthentication();
         app.UseAuthorization();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapRazorPages();
            endpoints.MapDefaultControllerRoute();
         });

         app.UseStaticFiles();

         CreateUserRoles(serviceProvider).Wait();
      }

      private async Task CreateUserRoles(IServiceProvider serviceProvider)
      {
         var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
         var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

         //Adding Admin Role
         var roleCheck = await roleManager.RoleExistsAsync("Approved");
         if (!roleCheck)
         {
            //create the roles and seed them to the database
            var roleResult = await roleManager.CreateAsync(new AppRole("Approved"));
         }
      }
   }


   public class AppRole : MongoRole
   {
      public AppRole(string roleName) : base(roleName)
      {
      }
   }

   public class AppUser : MongoUser
   {
   }
}