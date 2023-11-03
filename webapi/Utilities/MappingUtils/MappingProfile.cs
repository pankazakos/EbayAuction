using AutoMapper;
using contracts.Responses.bid;
using contracts.Responses.Category;
using contracts.Responses.Item;
using contracts.Responses.User;
using webapi.Models;

namespace webapi.Utilities.MappingUtils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            var entityToInterfaceMappings = new Dictionary<Type, Type>
            {
                { typeof(User), typeof(IUserResponse) },
                { typeof(Item), typeof(IItemResponse) },
                { typeof(Bid), typeof(IBidResponse)} ,
                { typeof(Category), typeof(ICategoryResponse) }
            };

            foreach (var mapping in entityToInterfaceMappings)
            {
                var responseTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => t.IsClass &&
                                !t.IsAbstract &&
                                t.GetInterfaces().Contains(mapping.Value))
                    .ToList();

                foreach (var responseType in responseTypes)
                {
                    var mapMethod = typeof(Profile)
                        .GetMethods()
                        .First(m => m.Name == nameof(CreateMap) && m.IsGenericMethod)
                        .MakeGenericMethod(mapping.Key, responseType);

                    mapMethod.Invoke(this, null);
                }
            }

        }
    }

}
