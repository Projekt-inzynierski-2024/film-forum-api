using FilmForumWebAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FilmForumWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilmController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFilmDto createFilmDto)
    {
        return Ok();
    }
}
