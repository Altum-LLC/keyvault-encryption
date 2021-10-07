using azure_manager.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace azure_manager
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
            services.AddAzureClients(builder =>
            {
                builder.AddSecretClient(Configuration.GetSection("KeyVault"));
            });
            services.AddSingleton<SecretManager>();

            //Required for User Identity Only
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(microsoftIdentityOptions =>
                {
                    microsoftIdentityOptions.Instance = Configuration.GetValue<string>("AzureAd:Instance");
                    microsoftIdentityOptions.Domain = Configuration.GetValue<string>("AzureAd:Domain");
                    microsoftIdentityOptions.CallbackPath = Configuration.GetValue<string>("AzureAd:CallbackPath");
                    microsoftIdentityOptions.ClientId = Configuration.GetValue<string>("AzureAd:ClientId");
                    microsoftIdentityOptions.TenantId = Configuration.GetValue<string>("AzureAd:TenantId");
                    microsoftIdentityOptions.ClientSecret = Configuration.GetValue<string>("AzureAd:ClientSecret");
                })
                .EnableTokenAcquisitionToCallDownstreamApi(new string[] { "user.read" })
                .AddDistributedTokenCaches();

            services.AddDistributedMemoryCache();

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddRazorPages(options => {
                options.Conventions.AllowAnonymousToPage("/Index");
                options.Conventions.AllowAnonymousToPage("/EncryptManagedId");
                options.Conventions.AllowAnonymousToPage("/DecryptManagedId");
            }).AddMicrosoftIdentityUI();

            services.AddScoped<ICrypto, Crypto>();
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
