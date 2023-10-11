﻿using webapi.Models;

namespace webapi.Services
{
    public interface IBidService
    {
        public Task<Bid> Create(long itemId, CancellationToken cancel = default);

        public Task<IEnumerable<Bid>> GetItemBids(long itemId, CancellationToken cancel = default);
    }
}
