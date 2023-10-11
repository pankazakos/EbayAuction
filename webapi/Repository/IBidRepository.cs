﻿using webapi.Models;

namespace webapi.Repository
{
    public interface IBidRepository
    {
        public Task<Bid> Create(long itemId, CancellationToken cancel = default);

        public Task<IEnumerable<Bid>> GetItemBids(long itemId, CancellationToken cancel = default);
    }
}
