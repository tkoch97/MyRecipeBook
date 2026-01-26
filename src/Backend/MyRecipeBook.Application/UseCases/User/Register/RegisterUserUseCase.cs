using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordEncrypter _passwordEncrypter;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessTokenGenerator _accessTokenGenerator;

        public RegisterUserUseCase(IUserWriteOnlyRepository userWriteOnlyRepository,
                                   IUserReadOnlyRepository userReadOnlyRepository, IUnitOfWork unitOfWork,
                                   IPasswordEncrypter passwordEncrypter, IMapper mapper,
                                   IAccessTokenGenerator accessTokenGenerator
                                   )
        {
            _userWriteOnlyRepository = userWriteOnlyRepository;
            _userReadOnlyRepository = userReadOnlyRepository;
            _mapper = mapper;
            _passwordEncrypter = passwordEncrypter;
            _unitOfWork = unitOfWork;
            _accessTokenGenerator = accessTokenGenerator;
        }

        public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
        {
            // Validar se os dados do request são válidos (e-mail, senha, etc.)
            await Validate(request);

            // Mapear o request para a entidade de usuário
            var user = _mapper.Map<Domain.Entities.User>(request);

            // Criptografar a senha
            user.Password = _passwordEncrypter.Encrypt(request.Password);

            // Criar Guid para o usuário
            user.UserIdentifier = Guid.NewGuid();

            // Salvar o usuário no banco de dados
            await _userWriteOnlyRepository.Add(user);

            // Gerar token de acesso
            var accessToken = _accessTokenGenerator.Generate(user.UserIdentifier);

            await _unitOfWork.Commit();

            return new ResponseRegisteredUserJson
            {
                Name = user.Name,
                Tokens = new ResponseTokensJson
                {
                    AccessToken = accessToken
                }
            };
        }

        private async Task Validate(RequestRegisterUserJson request)
        {
            var validator = new RegisterUserValidator();
            var result = validator.Validate(request);

            var userEmailExist = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.Email);
            if (userEmailExist) 
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessageException.EMAIL_ALREADY_REGISTERED));
            }

            if (!result.IsValid)
            {
                var errorsMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

               throw new ErrorOnValidationException(errorsMessages);
            }
        }
    }

}
