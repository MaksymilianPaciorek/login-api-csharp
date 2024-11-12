using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class AccountPanelController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountPanelController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

// zmiana maila

        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("Błąd! Nie jesteś zalogowany.");

            user.Email = model.NewEmail;
            user.UserName = model.NewEmail;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok("E-mail został zmieniony.");

            return BadRequest("Wystąpił błąd podczas zmiany e-maila.");
        }

// zmiana hasla

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("Błąd! Nie jesteś zalogowany.");

            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!passwordCheck)
                return BadRequest(new { message = "Podane stare hasło jest nieprawidłowe." });

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return Ok("Hasło zostało zmienione. Zaloguj się ponownie.");
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { message = "Wystąpił błąd podczas zmiany hasła.", errors });
        }
    }

    public class ChangeEmailModel
    {
        public string NewEmail { get; set; }
    }

    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
