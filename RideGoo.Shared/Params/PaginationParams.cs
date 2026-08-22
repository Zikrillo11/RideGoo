namespace RideGoo.Shared.Params;

public class PaginationParams
{
    private const int MaxPageSize = 50;
    private const int DefaultPageSize = 10;

    private int _pageSize = DefaultPageSize;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? DefaultPageSize : value);
    }
}