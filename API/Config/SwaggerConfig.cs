using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ToDoAPI.API.Config
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerConfig(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddSwaggerGen(c =>
             {
                 var swaggerInfo = configuration.GetSection("SwaggerInfo");
                 var swaggerContact = swaggerInfo.GetSection("Contact");
                 var swaggerLicense = swaggerInfo.GetSection("License");
                 c.SwaggerDoc(swaggerInfo.GetValue<string>("Version"), new OpenApiInfo
                 {
                     Title = swaggerInfo.GetValue<string>("Title"),
                     Version = swaggerInfo.GetValue<string>("Version"),
                     Description = swaggerInfo.GetValue<string>("Description"),
                     Contact = new OpenApiContact
                     {
                         Name = swaggerContact.GetValue<string>("Name"),
                         Email = swaggerContact.GetValue<string>("Email"),
                         Url = new Uri(swaggerContact.GetValue<string>("Url"))
                     },
                     License = new OpenApiLicense
                     {
                         Name = swaggerLicense.GetValue<string>("Name")
                     }
                 });

                 var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                 var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                 c.IncludeXmlComments(xmlPath);
             });
        }

        public static void UseSwaggerUIConfig(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
                c.RoutePrefix = string.Empty;
                if (env.IsDevelopment())
                {
                    c.SupportedSubmitMethods(new SubmitMethod[] { SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete });
                }
                else
                {
                    c.SupportedSubmitMethods(new SubmitMethod[] { SubmitMethod.Get });
                }
            });
        }
    }
}