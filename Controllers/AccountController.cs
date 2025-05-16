using api.Data;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("/api/account")]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDBContext _dbContext;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, ApplicationDBContext dBContext, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _dbContext = dBContext;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var appUser = new AppUser
                        {
                            UserName = registerDTO.Username,
                            Email = registerDTO.Email
                        };

                        var createdUser = await _userManager.CreateAsync(appUser, registerDTO.Password);

                        if (createdUser.Succeeded)
                        {
                            var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                            if (roleResult.Succeeded)
                            {
                                await transaction.CommitAsync();
                                return Ok(
                                    new NewUserDTO
                                    {
                                        UserName = appUser.UserName,
                                        Email = appUser.Email,
                                        Token = _tokenService.CreateToken(appUser)
                                    }
                                );
                            }
                            else
                            {
                                return StatusCode(500, new { message = "Failed to add role", errors = roleResult.Errors.Select(e => e.Description) });
                            }
                        }
                        else
                        {
                            return StatusCode(500, new { message = "Failed to add user", errors = createdUser.Errors.Select(e => e.Description) });
                        }
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "A problem occured duraing the register of the user", stackTrace = e.StackTrace, innerException = e.InnerException?.Message });
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username!");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Username not found and/or password incorrect");

            return Ok(
                new NewUserDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
        }


    }
}