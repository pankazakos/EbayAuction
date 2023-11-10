namespace contracts.Requests.Category
{
    public class AddCategoryRequest : IRequest
    {
        public string Name { get; init; } = string.Empty;

        public void Validate()
        {
            RequestUtils.EnsureStringContent(nameof(Name), Name);
        }
    }
}
