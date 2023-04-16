namespace API.Extensions;
public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse response,
        int currentPage,
        int itemsPerPage,
        int totalPages,
        int totalItems)
    {
        var paginationMetaData = new
        {
            totalCount = totalItems,
            totalPages = totalPages,
            pageNumber = currentPage,
            pageSize = itemsPerPage
        };

        response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));
        response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");
    }
}
