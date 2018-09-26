using System;
using System.Collections.Generic;

namespace TestApp.Core.Helpers
{
    public class PagedResource<T>
    {
        public PagedResource(List<T> items, int count, ResourceQuery query)
        {
            Items = items;
            PageSize = query.PageSize;
            PageIndex = query.PageIndex;
            TotalItemCount = count;
            TotalPageCount = (int) Math.Ceiling((double) count / (query.PageSize == 0 ? 1 : query.PageSize));
        }

        public int PageSize { get; }
        public int PageIndex { get; }
        public int TotalItemCount { get; }
        public int TotalPageCount { get; }

        public List<T> Items { get; }
    }
}