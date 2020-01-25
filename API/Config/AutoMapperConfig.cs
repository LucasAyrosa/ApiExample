using API.Dto;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using ToDoAPI.Domain.Models;

namespace API.Config
{
    public static class AutoMapperConfig
    {
        public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.AddProfile<AutoMapperProfile>();
            });

            IMapper mapper = mappingConfig.CreateMapper();
            return services.AddSingleton(mapper);

        }
    }

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<TodoItem, TodoItemDto>().ReverseMap();
        }
    }
}