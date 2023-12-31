﻿using FilmForumModels.Dtos.RequestDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RequestLogController : ControllerBase
{
    private readonly IRequestLogService _requestLogService;
    private readonly IUserService _userService;

    public RequestLogController(IRequestLogService requestLogService,
                                IUserService userService)
    {
        _requestLogService = requestLogService;
        _userService = userService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _requestLogService.GetAllAsync());

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
        => await _requestLogService.GetAsync(id) is GetRequestLogDto getRequestLogDto ? Ok(getRequestLogDto) : NotFound("Request log not found");

    [Authorize(Roles = "Admin")]
    [HttpGet("user-request-logs/{userId}")]
    public async Task<IActionResult> GetUserAllRequestsLogs(int userId)
        => !await _userService.UserWithIdExistsAsync(userId)
           ? NotFound("User not found")
           : Ok(await _requestLogService.GetUserAllRequestsLogsAsync(userId));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        await _requestLogService.RemoveAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("user-request-logs/{userId}")]
    public async Task<IActionResult> RemoveUserRequestsLogs(int userId)
    {
        if (!await _userService.UserWithIdExistsAsync(userId))
        {
            return NotFound("User not found");
        }
        await _requestLogService.RemoveUserRequestsLogsAsync(userId);
        return NoContent();
    }
}