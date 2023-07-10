using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectRoomChat.Data;
using System.ComponentModel.DataAnnotations;

namespace ProjectRoomChat.Areas.Admin.Pages.Role
{
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context) : base(roleManager, context)
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
        public IdentityRole role { get; set; }
        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null)
                return NotFound("Role is not found");

            role = await _roleManager.FindByIdAsync(roleid);

            if (role != null)
            {
                Input = new InputModel()
                {
                    Name = role.Name,
                };
                return Page();
            }

            return NotFound("Role is not found");
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null)
                return NotFound("Role is not found");

            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
                return NotFound("Role is not found");

            if (!ModelState.IsValid)
                return Page();

            role.Name = Input.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                StatusMessage = $"Update role successfully {Input.Name}";
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
