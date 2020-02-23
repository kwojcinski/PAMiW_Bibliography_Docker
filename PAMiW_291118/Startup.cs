using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using PAMiW_291118.Models;
using PAMiW_291118.Services;

namespace PAMiW_291118
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
            string securityKey = "supper_long_security_key_for_token_validation_pamiw_291118";
            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Add authentication services
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect("Auth0", options => {
        // Set the authority to your Auth0 domain
        options.Authority = $"https://dev-xhy1efmt.eu.auth0.com";

        // Configure the Auth0 Client ID and Client Secret
        options.ClientId = "btCygcyGlEECHPU7Sb5WWZkD2ujSOwh5";
                options.ClientSecret = "CNsYL83v4qKY_jcuqj9t-hx4UhRZTmHI4sENXXXm6Hdn9s_PQT7EQfO2SjbDJbFh";

        // Set response type to code
                options.ResponseType = OpenIdConnectResponseType.Code;

        // Configure the scope
        options.Scope.Add("openid");

        // Set the callback path, so Auth0 will call back to http://localhost:3000/callback
        // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
        options.CallbackPath = new PathString("/Home/Callback");

        // Configure the Claims Issuer to be Auth0
        options.ClaimsIssuer = "Auth0";

                options.Events = new OpenIdConnectEvents
                {
            // handle the logout redirection
            OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                        var logoutUri = "https://dev-xhy1efmt.eu.auth0.com/v2/logout?client_id=btCygcyGlEECHPU7Sb5WWZkD2ujSOwh5";

                        var postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                        //    if (postLogoutUri.StartsWith("/"))
                        //    {
                        //// transform to absolute
                        //var request = context.Request;
                        //        postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + "/Home/Index";
                        //    }
                            var request = context.Request;
                            postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + "/Home/Index";
                            logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                        }

                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    }
                };
            });
            services.AddServerSentEvents();

            // Registers custom ServerSentEventsService which will be used by second middleware, otherwise they would end up sharing connected users.
            services.AddServerSentEvents<INotificationsServerSentEventsService, NotificationsServerSentEventsService>();

            services.AddSingleton<IHostedService, HeartbeatService>();
            services.AddTransient<INotificationsService, LocalNotificationsService>();

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "text/event-stream" });
            });
            // Add framework services.
            services.AddControllersWithViews();

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options =>
            //    {
            //        options.LoginPath = "/Home/Login";
            //        options.AccessDeniedPath = "/Home/Denied";
            //    });

            services.AddCors();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = "https://localhost:8080",
                       ValidAudience = "https://localhost:8082",
                       IssuerSigningKey = symetricSecurityKey
                   };
               });
            // services.AddControllersWithViews();
            // services.AddDbContext<PAMiWContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapServerSentEvents("/see-heartbeat");

                // Set up second (separated) Server-Sent Events endpoint.
                endpoints.MapServerSentEvents<NotificationsServerSentEventsService>("/sse-notifications");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
