using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectRoomChat.Data;
using System.ComponentModel.DataAnnotations;

namespace ProjectRoomChat.Areas.Admin.Pages.Role
{
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context) : base(roleManager, context)
        {
        }

        public IdentityRole role { get; set; }
        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null)
                return NotFound("Role is not found");

            role = await _roleManager.FindByIdAsync(roleid);

            if (role == null)
            {
                return NotFound("Role is not found");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null)
                return NotFound("Role is not found");

            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
                return NotFound("Role is not found");

            
            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                StatusMessage = $"Delete role successfully {role.Name}";
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
