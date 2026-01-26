namespace MyRecipeBook.Exceptions.ExceptionsBase
{
    public class MyRecipeBookException : System.Exception
    {
        public MyRecipeBookException(string exceptionMessage) : base(exceptionMessage) { }
    }
}
// O : base(message) chama o construtor do "System.Exception" passando a mensagem de exceção para ele.