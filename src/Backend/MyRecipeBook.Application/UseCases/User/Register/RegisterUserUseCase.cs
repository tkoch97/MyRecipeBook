using AutoMapper;
using MyRecipeBook.Application.Services.AutoMapper;
using MyRecipeBook.Application.Services.Cryptography;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly PasswordEncrypter _passwordEncrypter;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserUseCase(IUserWriteOnlyRepository userWriteOnlyRepository,
                                   IUserReadOnlyRepository userReadOnlyRepository, IUnitOfWork unitOfWork,
                                   PasswordEncrypter passwordEncrypter, IMapper mapper)
        {
            _userWriteOnlyRepository = userWriteOnlyRepository;
            _userReadOnlyRepository = userReadOnlyRepository;
            _mapper = mapper;
            _passwordEncrypter = passwordEncrypter;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
        {
            // Validar se os dados do request são válidos (e-mail, senha, etc.)
            await Validate(request);

            // Mapear o request para a entidade de usuário
            var user = _mapper.Map<Domain.Entities.User>(request);

            // Criptografar a senha
            user.Password = _passwordEncrypter.Encrypt(request.Password);

            // Salvar o usuário no banco de dados
            await _userWriteOnlyRepository.Add(user);

            await _unitOfWork.Commit();

            return new ResponseRegisteredUserJson
            {
                Name = user.Name
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
