using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Supermarket.API.Context;
using Supermarket.API.Dtos.Mappings;
using Supermarket.API.Extensions;
using Supermarket.API.Filters;
using Supermarket.API.Logging;
using Supermarket.API.Models;
using Supermarket.API.Repository;

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

            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Supermarket.API", Version = "v1" }));

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Supermarket.API v1"));
            }
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
