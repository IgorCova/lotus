namespace LotusWebApp.Data.Models;

public class PageDto : NewPageDto
{
    public required int Id { get; set; }
}

public class NewPageDto
{
    public required string? Title { get; set; }
    public required string? Body { get; set; }
    public required string? Author { get; set; }
}