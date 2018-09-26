using System;
using System.Collections.Generic;
using System.Net;
using Bazinga.AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using StackExchange.Profiling.SqlFormatters;
using StackExchange.Profiling.Storage;
using Swashbuckle.AspNetCore.Swagger;
using TestApp.Core.Configuration;
using TestApp.Data;
using TestApp.Web.Filters;
using TestApp.Web.Security;

namespace TestApp.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<TestAppDbContext>(options =>
                    options
                        .UseSqlServer(connectionString)
                        // EF core by default evaluates query on server side if it can't on db side, instead of throwing. We want to throw instead of n+1 calls.
                        .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)))
                .AddMvc(options => options.Filters.Add(new ExceptionFilterFactory(Environment)))
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.None;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters.Add(new IsoDateTimeConverter());
                    options.SerializerSettings.Converters.Add(new StringEnumConverter {CamelCaseText = true});
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<BasicAuthenticationConfiguration>(Configuration.GetSection("BasicAuthenticationConfiguration"));
            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme).AddBasicAuthentication<BasicAuthenticationVerifier>();

            if (Environment.IsDevelopment())
            {
                services.AddMiniProfiler(options =>
                {
                    // All of this is optional. You can simply call .AddMiniProfiler() for all defaults

                    // (Optional) Path to use for profiler URLs, default is /mini-profiler-resources
                    options.RouteBasePath = "/profiler";

                    // (Optional) Control storage
                    // (default is 30 minutes in MemoryCacheStorage)
                    ((MemoryCacheStorage) options.Storage).CacheDuration = TimeSpan.FromMinutes(60);

                    // (Optional) Control which SQL formatter to use, InlineFormatter is the default
                    options.SqlFormatter = new InlineFormatter();

                    // (Optional) You can disable "Connection Open()", "Connection Close()" (and async variant) tracking.
                    // (defaults to true, and connection opening/closing is tracked)
                    options.TrackConnectionOpenClose = true;
                }).AddEntityFramework();

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info
                    {
                        Title = "Test app api explorer",
                        Version = "v1",
                        Description = "A simple ASP.NET Core Web API app",
                        TermsOfService = "None",
                        Contact = new Contact
                        {
                            Name = "Max Grebeniuk",
                            Email = "grebreniukmax@gmail.com",
                            Url = "https://github.com/melanore"
                        }
                    });

                    c.AddSecurityDefinition("basic", new BasicAuthScheme
                    {
                        Type = "basic",
                        Description = "Some minimal auth for testing purposes"
                    });
                    c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                    {
                        {"basic", new string[] { }}
                    });
                });
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var auth = app.ApplicationServices.GetRequiredService<IOptions<BasicAuthenticationConfiguration>>().Value;
            if (string.IsNullOrEmpty(auth.Username) || string.IsNullOrEmpty(auth.Password) || string.IsNullOrEmpty(auth.ScopeName))
                throw new NotSupportedException("Please provide BasicAuthenticationConfiguration in appsettings.");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                MigrateDatabase(app.ApplicationServices);
                //https://miniprofiler.com/dotnet/HowTo/ProfileCode
                app.UseMiniProfiler();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test app api explorer");
                c.RoutePrefix = string.Empty;
            });
            app.UseAuthentication();
            app.UseMvc();

            //http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html
            // Http client not used, but once it is, - people typically miss this line.
            var sp = ServicePointManager.FindServicePoint(new Uri("http://foo.bar/baz/123?a=ab"));
            sp.ConnectionLeaseTimeout = 60 * 1000; // 1 minute
        }

        private static void MigrateDatabase(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var logger = serviceScope.ServiceProvider.GetService<ILogger<Startup>>();
                try
                {
                    var dbContext = serviceScope.ServiceProvider.GetService<TestAppDbContext>();
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to migrate database");
                }
            }
        }
    }
}