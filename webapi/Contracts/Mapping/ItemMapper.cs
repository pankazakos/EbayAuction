using webapi.Contracts.Responses;
using webapi.Models;

namespace webapi.Contracts.Mapping
{
    public static class ItemMapper
    {
        public static CreateItemResponse MapToCreateItemResponse(Item item)
        {
            return new CreateItemResponse
            {
                ItemId = item.ItemId,
                Name = item.Name,
                Currently = item.Currently,
                BuyPrice = item.BuyPrice,
                FirstBid = item.FirstBid,
                NumBids = item.NumBids,
                Active = item.Active,
                Description = item.Description,
                SellerId = item.SellerId
            };
        }
    }
}
