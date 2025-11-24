namespace Application.Queries;

/// <summary>
/// Параметры для пагинации
/// </summary>
public class PagingParams
{
    public int Skip { get; set; }

    public int Take { get; set; }
}