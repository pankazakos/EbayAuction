namespace contracts.Responses.Category
{
    public class BasicCategoryResponse : ICategoryResponse
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;
    }
}
