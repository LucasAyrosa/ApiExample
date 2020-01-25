using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ToDoAPI.Data.Repository;

namespace API.Config
{
    public static class IdentityConfig
    {
        public static IdentityBuilder AddIdentityConfig(this IServiceCollection services)
        {
            return services.AddIdentityCore<IdentityUser>(opt => { })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<TodoContext>()
                .AddSignInManager<SignInManager<IdentityUser>>()
                .AddDefaultTokenProviders();
        }
    }
}