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
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace ToDoAPI.API.Config
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerConfig(this IServiceCollection service, IConfiguration configuration)
        {
            var swaggerInfoSection = configuration.GetSection("SwaggerInfo");
            service.Configure<SwaggerInfo>(swaggerInfoSection);
            var swaggerInfo = swaggerInfoSection.Get<SwaggerInfo>();

            service.AddSwaggerGen(swaggerGen =>
             {
                 swaggerGen.SwaggerDoc(swaggerInfo.Version, new OpenApiInfo
                 {
                     Title = swaggerInfo.Title,
                     Version = swaggerInfo.Version,
                     Description = swaggerInfo.Description,
                     Contact = new OpenApiContact
                     {
                         Name = swaggerInfo.Contact.Name,
                         Email = swaggerInfo.Contact.Email,
                         Url = new Uri(swaggerInfo.Contact.Url)
                     },
                     License = new OpenApiLicense
                     {
                         Name = swaggerInfo.License.Name
                     }
                 });

                 swaggerGen.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                 {
                     Description = "Cabeçalho de autorização JWT usando esquema Bearer. \r\n\r\n Digite 'Bearer' [espaço] e o seu token na caixa de texto abaixo.\r\n\r\nExemplo: \"Bearer 12345abcdef\"",
                     Name = "Authorization",
                     In = ParameterLocation.Header,
                     Type = SecuritySchemeType.ApiKey,
                     Scheme = "Bearer"
                 });
                 swaggerGen.AddSecurityRequirement(new OpenApiSecurityRequirement()
                 {
                     {
                         new OpenApiSecurityScheme
                         {
                             Reference = new OpenApiReference
                             {
                                 Type = ReferenceType.SecurityScheme,
                                 Id = "Bearer"
                             },
                             Scheme = "Bearer",
                             Name = "Bearer",
                             In = ParameterLocation.Header
                         },
                         new List<string>()
                     }
                 });

                 var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                 var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                 swaggerGen.IncludeXmlComments(xmlPath);
             });
        }

        public static void UseSwaggerUIConfig(this IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UseSwaggerUI(swaggerUI =>
            {
                swaggerUI.DisplayRequestDuration();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    swaggerUI.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
                swaggerUI.RoutePrefix = string.Empty;
                if (env.IsDevelopment())
                {
                    swaggerUI.SupportedSubmitMethods(new SubmitMethod[] { SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete });
                }
                else
                {
                    swaggerUI.SupportedSubmitMethods(new SubmitMethod[] { SubmitMethod.Get });
                }
            });
        }
    }

    internal class SwaggerInfo
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public SwaggerContact Contact { get; set; }
        public SwaggerLicense License { get; set; }
    }

    public class SwaggerLicense
    {
        public string Name { get; set; }
    }

    public class SwaggerContact
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }
}