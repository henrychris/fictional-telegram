using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace api
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
            // gets value from appsettings.json
            string uriAttr = Configuration.GetValue<string>("Configuration:uri");

            services.AddControllers();
            services.AddCors();

            services.AddHttpClient("EpumpTestApi", client =>
            {
                client.BaseAddress = new Uri("https://www.epump.cf/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("EpumpApi", client =>
            {
                client.BaseAddress = new Uri(uriAttr);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Epump Report", Version = "v1" });
            });

            services.AddSwaggerGen(options =>
            {
                // This adds the authorize button
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    Description = "Auth Header using Bearer Scheme, e.g: 'Bearer [Token]' \n\n\nEnter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: 'Bearer 12345abcdef'",
                    In = ParameterLocation.Header
                });

                // This makes it work and send the header ??? I don't understand???
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }, new List<string>()
                }
            });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(policy => 
            policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
            );

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
