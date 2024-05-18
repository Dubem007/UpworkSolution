namespace Common.Models.Paging;

public abstract class PagingModel
{
    public int Page { get; set; } = 1;
    
    public int ItemsPerPage
    {
        get;
        set;
    } = 10;

    public int Skip
    {
        get
        {
            return (Page - 1) * ItemsPerPage;
        }
    }

    public string? Search { get; set; }
}
