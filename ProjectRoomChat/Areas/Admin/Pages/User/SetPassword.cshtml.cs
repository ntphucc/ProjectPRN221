using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectRoomChat.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectRoomChat.Areas.Admin.Pages.User
{
    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SetPasswordModel(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "Must input {0}")]
            [StringLength(100, ErrorMessage = "{0} must have length {2} to {1} characters.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string NewPassword { get; set; }
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("NewPassword", ErrorMessage = "It is not match password")]
            public string ConfirmPassword { get; set; }
        }
        public ApplicationUser user { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound("Do not have user");

            user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound($"Do not have user with id = {id}");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound("Do not have user");

            user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound($"Do not have user with id = {id}");

            if (!ModelState.IsValid)
                return Page();

            await _userManager.RemovePasswordAsync(user);

            var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }
            StatusMessage = $"Already update password for user: {user.UserName}";

            return RedirectToPage("./Index");
        }
    }
}
