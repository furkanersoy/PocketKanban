using Microsoft.AspNetCore.Mvc;
using PocketKanbanAPI.Data;
using PocketKanbanAPI.Models;

namespace PocketKanbanAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ColumnsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ColumnsController(AppDbContext context) { _context = context; }

    [HttpPost]
    public async Task<ActionResult<Column>> CreateColumn(Column column)
    {
        _context.Columns.Add(column);
        await _context.SaveChangesAsync();
        return Ok(column);
    }
}