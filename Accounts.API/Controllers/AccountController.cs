﻿using Microsoft.AspNetCore.Mvc;
using Accounts.API.DTO;
using Accounts.API.Services;

namespace Accounts.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly SessionStore _sessionStore;

        public AccountController(AccountService accountService, SessionStore  sessionStore)
        {
            _accountService = accountService;
            _sessionStore = sessionStore;
        }
        
        public class CreateUserResponse
        {
            public Guid UserId { get; set; }
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserRequest registerUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = await _accountService.CreateUserAsync(registerUser);

            return Ok(new CreateUserResponse { UserId = userId });
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _accountService.ValidateUserAsync(loginRequest.Username, loginRequest.Password);

            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddHours(1);

            _sessionStore.AddSession(token, user.Id, expiry);

            return Ok(new LoginResponse { Token = token, Expiry = expiry });
        }



        [HttpGet("logout")]
        public IActionResult Logout([FromHeader(Name = "Authorization")] string token)
        {
            _sessionStore.RemoveSession(token); // Use the correct method from SessionStore
            return Ok("Logged out successfully.");
        }


        [HttpGet("me")]
        public IActionResult Me()
        {
            if (HttpContext.Items.TryGetValue("UserId", out var userIdObj) &&
                HttpContext.Items.TryGetValue("Token", out var tokenObj))
            {
                var userId = (Guid)userIdObj;
                var token = tokenObj.ToString();
                
                return Ok(new MeResponse { UserId = userId, Token = token });

            }

            return Unauthorized(new { message = "User is not logged in or session is invalid." });
        }

        }

    
}