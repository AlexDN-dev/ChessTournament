namespace Domain.Filters;

public class TournamentFilter
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool? WomenOnly { get; set; }
    public string? Location { get; set; }
    public string? Name { get; set; }
}