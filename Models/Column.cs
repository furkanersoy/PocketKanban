namespace PocketKanbanAPI.Models;

public class Column
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int OrderNo { get; set; }
    public int BoardId { get; set; }
    public Board? Board { get; set; }
    public List<Card>? Cards { get; set; } 
    
}