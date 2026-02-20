using AutoMapper;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using Sqids;

namespace MyRecipeBook.Application.Services.AutoMapper
{
    public class AutoMapping : Profile
    {

        private readonly SqidsEncoder<long> _idEncoder;

        public AutoMapping(SqidsEncoder<long> idEnconder)
        {
            _idEncoder = idEnconder;
            RequestToDomain();
            DomainToResponse();
        }

        private void RequestToDomain()
        {
            CreateMap<RequestRegisterUserJson, Domain.Entities.User>()
               .ForMember(destination => destination.Password, option => option.Ignore()); //Seleciona a propriedade Password para ser ignorada no mapeamento

            CreateMap<RequestRecipeJson, Domain.Entities.Recipe>()
                .ForMember(destination => destination.Instructions, option => option.Ignore())
                .ForMember(destination => destination.Ingredients, option => option.MapFrom(source => source.Ingredients.Distinct())) //Para impedir que o usuário consiga registars ingredientes duplicados
                .ForMember(destination => destination.DishTypes, option => option.MapFrom(source => source.DishTypes.Distinct()));

            CreateMap<string, Domain.Entities.Ingredient>()
                .ForMember(destination => destination.Item, option => option.MapFrom(source => source));

            CreateMap<DishType, Domain.Entities.DishType>()
                .ForMember(destination => destination.Type, option => option.MapFrom(source => source));

            CreateMap<RequestInstructionJson, Domain.Entities.Instruction>();
        }

        private void DomainToResponse()
        {
            CreateMap<Domain.Entities.User, ResponseUserProfileJson>();
            CreateMap<Domain.Entities.Recipe, ResponseRegisteredRecipeJson>()
                .ForMember(dest => dest.Id, config => config.MapFrom(source => _idEncoder.Encode(source.Id)));
        }
    }
}
