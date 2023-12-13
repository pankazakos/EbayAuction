using contracts.Responses.Category;

namespace contracts.Responses.Item
{
    public class ItemCategoriesResponse : IItemResponse
    {
        long ItemId { get; init; }

        IEnumerable<BasicCategoryResponse> Categories { get; init; } = new List<BasicCategoryResponse>();
    }
}
