namespace BuildingBlocks.Pagination;

public class PaginationResult<TEntity>(
    int pageIndex,
    int pageSize,
    long count,
    IEnumerable<TEntity> data)
{
    public int Page { get; set; } = pageIndex;
    public int PageSize { get; set; } = pageSize;
    public long Count { get; set; } = count;
    public IEnumerable<TEntity> Data { get; set; } = data;
}
