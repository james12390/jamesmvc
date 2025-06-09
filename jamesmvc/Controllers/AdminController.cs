using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using jamesmvc.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using jamesmvc.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace jamesmvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        
        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 顯示所有使用者 + 目前角色
        public async Task<IActionResult> Dashboard()
        {
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new List<UserWithRoleViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                model.Add(new UserWithRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    CurrentRole = userRoles.FirstOrDefault() ?? "未指定",
                    AvailableRoles = roles
                });
            }

            return View(model);
        }

        // 更改角色
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, newRole);

            return RedirectToAction("Dashboard");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageDrivers()
        {
            var drivers = await _userManager.GetUsersInRoleAsync("Driver");
            var driverProfiles = await _context.DriverProfiles.ToListAsync();

            var model = drivers.Select(d => new DriverWithProfilesViewModel
            {
                UserId = d.Id,
                Email = d.Email,
                Profiles = driverProfiles.Where(p => p.UserId == d.Id).ToList()
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddServiceArea(string userId, string serviceCity, string serviceDistrict)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(serviceCity) || string.IsNullOrEmpty(serviceDistrict))
            {
                TempData["Error"] = "請完整填寫縣市與行政區";
                return RedirectToAction("ManageDrivers");
            }

            var newProfile = new DriverProfile
            {
                UserId = userId,
                ServiceCity = serviceCity,
                ServiceDistrict = serviceDistrict
            };

            _context.DriverProfiles.Add(newProfile);
            await _context.SaveChangesAsync();

            TempData["Success"] = "服務範圍已新增";
            return RedirectToAction("ManageDrivers");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnassignedOrders()
        {
            var unassignedOrders = await _context.LogisticsOrder
                .Where(o => o.AssignedDriverId == null)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            var drivers = await _userManager.GetUsersInRoleAsync("Driver");

            ViewBag.DriverList = new SelectList(drivers, "Id", "Email");

            return View(unassignedOrders);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignDriver(int orderId, string driverId)
        {
            var order = await _context.LogisticsOrder.FindAsync(orderId);
            if (order == null)
                return NotFound();

            order.AssignedDriverId = driverId;
            await _context.SaveChangesAsync();

            TempData["Success"] = "司機指派成功";
            return RedirectToAction("UnassignedOrders");
        }
    }



}
