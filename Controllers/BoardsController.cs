using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PocketKanbanAPI.Data;
using PocketKanbanAPI.Models;

namespace PocketKanbanAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BoardsController : ControllerBase
{
    private readonly AppDbContext _context;
    public BoardsController(AppDbContext context) { _context = context; }

    [HttpGet] // Tüm Panolar
    public async Task<ActionResult<IEnumerable<Board>>> GetBoards() => await _context.Boards.ToListAsync();

    [HttpGet("{id}")] // Detaylı Pano (Frontend için kritik)
    public async Task<ActionResult<Board>> GetBoard(int id)
    {
        var board = await _context.Boards
            .Include(b => b.Columns).ThenInclude(c => c.Cards)
            .FirstOrDefaultAsync(b => b.Id == id);
        return board == null ? NotFound() : board;
    }

    [HttpPost] 
    public async Task<ActionResult<Board>> CreateBoard(Board board)
    {
        _context.Boards.Add(board);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, board);
    }
}