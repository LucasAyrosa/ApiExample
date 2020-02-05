using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.API.Config;
using API.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Logging;
using ToDoAPI.Repository.Data;

namespace ToDoAPI
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
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            services.AddIdentityConfig();
            services.AddJwtConfig(Configuration);
            IdentityModelEventSource.ShowPII = true;
            services.AddScoped<TodoItemRepository>();
            services.AddAutoMapperConfig();
            services.AddSwaggerConfig(Configuration);
            services.AddCors();
            services.AddControllers(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUIConfig(env);
            app.UseStaticFiles();

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
