﻿using FilmForumWebAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FilmForumWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
    {
        return Ok();
    }
}
