namespace MyRecipeBook.Exceptions.ExceptionsBase
{
    public class ErrorOnValidationException : MyRecipeBookExceptions
    {

        public IList<string> ErrorMessages { get; set; }

        public ErrorOnValidationException(IList<string> errorMesssages) 
        {
            ErrorMessages = errorMesssages;
        }

    }
}
