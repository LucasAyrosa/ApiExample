using API.GraphQL;
using API.GraphQL.Types;
using API.GraphQL.Types.InputTypes;
using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace API.Config
{
    public static class GraphQLConfig
    {
        public static void AddGraphQLConfig(this IServiceCollection services)
        {
            services.AddScoped<IDependencyResolver>(_ => new FuncDependencyResolver(_.GetRequiredService));
            services.AddScoped<Query>();
            services.AddScoped<Mutation>();
            services.AddScoped<ISchema, MySchema>();
            services.AddSingleton<TodoItemType>();
            services.AddSingleton<InputTodoItemType>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
        }
    }
}