using Microsoft.EntityFrameworkCore;

namespace Server.Core.Paginations;

public class PaginatedResult<T>(List<T> items, int count, int pageNumber, int pageSize)
{
    public int CurrentPage { get; set; } = pageNumber;
    public int TotalPages { get; set; } = (int)Math.Ceiling(count / (double)pageSize);
    public int PageSize { get; set; } = pageSize;
                                                                                                                             public int TotalCount { get; set; } = count;
    public List<T> Items { get; set; } = items;

    public static async Task<PaginatedResult<T>> CreateAsync(IQueryable<T> query, int pageNumber, int pageSize)
    {
        var count = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedResult<T>(items, count, pageNumber, pageSize);
    }
}