using FluentValidation;
using FluentValidation.Validators;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.SharedValidators
{
    // O T no nome da classe indica que é um validador genérico que pode ser usado para validar propriedades
    // de qualquer tipo de objeto, a responsabilidade de definir fica para quem usar.
    public class PasswordValidator<T> : PropertyValidator<T, string>
    {
        public override string Name => "PasswordValidator";

        public override bool IsValid(ValidationContext<T> context, string password)
        {
            if(string.IsNullOrWhiteSpace(password))
            {
                context.MessageFormatter.AppendArgument("ErrorMessage", ResourceMessageException.PASSWORD_EMPTY);
                return false;
            }

            if(password.Length < 6)
            {
                context.MessageFormatter.AppendArgument("ErrorMessage", ResourceMessageException.PASSWORD_LENGTH);
                return false;
            }

            return true;
        }

        //RESPONSÁVEL POR RETORNAR A MENSAGEM DE ERRO PADRÃO DA VALIDAÇÃO
        protected override string GetDefaultMessageTemplate(string errorCode) => "{ErrorMessage}";
}
}
