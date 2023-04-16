﻿namespace API.Helpers;

public class PaginationParams
{
    private const int _maxPageSize = 50;
    private int _pageSize { get; set; } = 20;
    public int PageNumber { get; init; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > _maxPageSize ? _maxPageSize : value;
    }

}
