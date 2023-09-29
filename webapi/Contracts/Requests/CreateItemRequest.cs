namespace webapi.Contracts.Requests
{
    public class CreateItemRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<long> CategoryIds { get; set; } = new();
        public float FirstBid { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Ends { get; set; }
        public int SellerId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
