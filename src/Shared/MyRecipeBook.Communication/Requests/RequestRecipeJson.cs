using MyRecipeBook.Communication.Enums;

namespace MyRecipeBook.Communication.Requests
{
    public class RequestRecipeJson
    {
        public string Title { get; set; } = string.Empty;
        public CookingTime? CookingTime { get; set; }
        public Difficulty? Difficulty { get; set; }
        public IList<string> Ingredients { get; set; } = [];
        public IList<RequestInstructionJson> Instructions { get; set; } = [];
        public List<DishType> DishTypes { get; set; } = [];

    }
}
