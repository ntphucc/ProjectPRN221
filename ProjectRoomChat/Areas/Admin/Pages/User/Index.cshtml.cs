using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectRoomChat.Data;
using ProjectRoomChat.Models;

namespace ProjectRoomChat.Areas.Admin.Pages.User
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public class UserAndRole : ApplicationUser
        {
            public string RoleNames { get; set; }
        }
        public List<UserAndRole> Users { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public const int ITEMS_PER_PAGE = 10;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPage { get; set; }
        public int CountPages { get; set; }

        public int totalUser { get; set; }

        public async Task OnGet()
        {
            //Users = await _userManager.Users.OrderBy(x => x.UserName).ToListAsync();
            var qr = _userManager.Users.OrderBy(x => x.UserName);

            totalUser = await qr.CountAsync();
            CountPages = (int)Math.Ceiling((double)totalUser / ITEMS_PER_PAGE);

            if (CurrentPage < 1)
                CurrentPage = 1;
            if (CurrentPage > CountPages)
                CurrentPage = CountPages;

            var qr1 = qr.Skip((CurrentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).Select(x => new UserAndRole
            {
                Id = x.Id,
                UserName = x.UserName,
            });

            Users = await qr1.ToListAsync();

            foreach (var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(",", roles);
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
