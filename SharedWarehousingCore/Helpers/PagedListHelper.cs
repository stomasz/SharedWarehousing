using Microsoft.EntityFrameworkCore;

namespace SharedWarehousingCore.Helpers;

public static class PagedListHelper<T> 
{
    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, 
        int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}