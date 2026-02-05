using Microsoft.AspNetCore.Mvc;
using PocketKanbanAPI.Data;
using PocketKanbanAPI.Models;

namespace PocketKanbanAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CardsController : ControllerBase
{
    private readonly AppDbContext _context;
    public CardsController(AppDbContext context) { _context = context; }

    [HttpPost] // Kart Ekle
    public async Task<ActionResult<Card>> CreateCard(Card card)
    {
        _context.Cards.Add(card);
        await _context.SaveChangesAsync();
        return Ok(card);
    }

    [HttpPut("move/{id}")] // KART TAŞIMA (Sihirli Kısım)
    public async Task<IActionResult> MoveCard(int id, [FromBody] MoveRequest req)
    {
        var card = await _context.Cards.FindAsync(id);
        if (card == null) return NotFound();
        card.ColumnId = req.TargetColumnId;
        await _context.SaveChangesAsync();
        return Ok();
    }
}

public class MoveRequest { public int TargetColumnId { get; set; } }