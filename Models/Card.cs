namespace PocketKanbanAPI.Models;

public class Card
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderNo { get; set; }
    public int ColumnId { get; set; }
    public Column? Column { get; set; }
}