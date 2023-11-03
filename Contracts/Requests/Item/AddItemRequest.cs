namespace contracts.Requests.Item
{
    public class AddItemRequest
    {
        public string Name { get; init; } = string.Empty;

        public List<int> CategoryIds { get; init; } = new();

        public float FirstBid { get; init; }

        public string Description { get; init; } = string.Empty;

        public string? ImageFilename { get; init; }
    }
}
