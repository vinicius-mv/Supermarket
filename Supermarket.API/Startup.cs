using System;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Supermarket.API.Context;
using Supermarket.API.Extensions;
using Supermarket.API.Filters;
using Supermarket.API.Logging;
using Supermarket.API.Repository;
using Supermarket.API.V1.Mappings;
using Supermarket.API.Infrastructure;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace Supermarket.API
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
            string mySqlConnection = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<AppDbContext>(options =>
            {
                //options.UseSqlite(Configuration.GetConnectionString("DevConnection")); // Sqlite // old Pomelo versions
                options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection));   // get Connection with new Pomelo
            });

            // Identity -> define AppDbContext implements IdentityModel
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // JWT
            // Authentication Manager - Schema: Bearer
            // Validate: Issuer, Audience, Key
            // Use symmetric secret key to validate signature
            services.AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidAudience = Configuration["TokenConfiguration:Audience"],
                            ValidIssuer = Configuration["TokenConfiguration:Issuer"],
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                        });

            services.AddControllers();
            // using System.Text.Json -> doesn't have a reference looping treatment for now, using JsonIgnoreAttribute to handle this
            //services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // filter with custom logging service
            services.AddScoped<ApiLoggingFilter>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // AutoMapper - register and condig
            var mappingConfig = new MapperConfiguration(mc =>
                mc.AddProfile(new MappingProfile()));

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // CORS
            services.AddCors(opt =>
                opt.AddPolicy("MyCorsPolicy", policy =>
                    policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:4200", "http://www.apirequest.io")));

            // ApiVersioning
            services.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new ApiVersion(2, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            // Swagger Gen Setup
            services.AddSwaggerGen(c =>
            {
                // Swagger Enable Bearer Token Auth -> https://stackoverflow.com/questions/43447688/setting-up-swagger-asp-net-core-using-the-authorization-headers-bearer
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = @"JWT Authorization header using the Bearer scheme. <br/><br/>
                                  Enter 'Bearer' [space] and then your token in the text input below.
                                  <br/><br/>Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                // Swagger Custom Docs Setup
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.ConfigureOptions<ConfigureSwaggerOptions>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //  Swagger
            app.UseSwagger();

            // SwaggerUI
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "Supermarket.API v2"));

            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions.Reverse())
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors("MyCorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // add middleware to handle exceptions
            app.ConfigureExceptionHandler();

            // defining multiple loggers level and color
            loggerFactory.AddColoredConsoleLogger();
            loggerFactory.AddColoredConsoleLogger(c =>
            {
                c.LogLevel = LogLevel.Debug;
                c.Color = ConsoleColor.Gray;
                c.IsWritingToFile = false;
            });
            loggerFactory.AddColoredConsoleLogger(c =>
            {
                c.LogLevel = LogLevel.Information;
                c.Color = ConsoleColor.Blue;
                c.IsWritingToFile = true;
            });
        }
    }
}
