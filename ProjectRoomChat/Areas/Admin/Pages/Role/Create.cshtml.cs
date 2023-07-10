using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectRoomChat.Data;
using System.ComponentModel.DataAnnotations;

namespace ProjectRoomChat.Areas.Admin.Pages.Role
{
    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context) : base(roleManager, context)
        {
        }

        public class InputModel
        {
            [Display(Name = "Role's Name")]
            [Required(ErrorMessage = "You must input at here")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} must have length {2} to {1} character")]
            public string Name { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var newRole = new IdentityRole(Input.Name);
            var result = await _roleManager.CreateAsync(newRole);

            if (result.Succeeded)
            {
                StatusMessage = $"Add new role successfully {Input.Name}";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();
        }
    }
}
