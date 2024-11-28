using Microsoft.EntityFrameworkCore;

namespace ProjectAPI.Application.Common.Models;

public class PaginatedList<T>
{
    public List<T> Data { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (decimal)pageSize);
        TotalCount = count;
        Data = items;
    }

    public bool HasPreviousPage => PageNumber > 0;

    public bool HasNextPage => (PageNumber + 1) < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}