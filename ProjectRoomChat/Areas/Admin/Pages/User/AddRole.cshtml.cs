using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectRoomChat.Data;
using ProjectRoomChat.Models;
using System.ComponentModel;

namespace ProjectRoomChat.Areas.Admin.Pages.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AddRoleModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }
        public ApplicationUser user { get; set; }
        [BindProperty]
        [DisplayName("Role assign to user")]
        public string[] RoleNames { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public SelectList allRoles { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            
            user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            List<string> roleName = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
            allRoles = new SelectList(roleName);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var OldRoleNames = (await _userManager.GetRolesAsync(user)).ToArray();

            var deleteRole = OldRoleNames.Where(x => !RoleNames.Contains(x));
            var addRole = RoleNames.Where(x => !OldRoleNames.Contains(x));

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);

            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRole);
            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            var resultAdd = await _userManager.AddToRolesAsync(user, addRole);
            if (!resultAdd.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            StatusMessage = $"Already Update Role for user: {user.UserName}";
            return RedirectToPage("./Index");
        }
    }
}
