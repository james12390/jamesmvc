using jamesmvc.Data;
using jamesmvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jamesmvc.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ChatController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Customer()
        {
            var user = await _userManager.GetUserAsync(User);
            var customerId = user.Id;

            // 假設 Admin 固定 Id 或用角色抓第一個 Admin
            var admin = await _userManager.GetUsersInRoleAsync("Admin");
            var adminId = admin.FirstOrDefault()?.Id;

            ViewBag.AdminId = adminId;

            var messages = await _context.ChatMessages
                .Where(m =>
                    (m.SenderId == adminId && m.ReceiverId == customerId) ||
                    (m.SenderId == customerId && m.ReceiverId == adminId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            ViewBag.History = messages;
            return View(model: customerId);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin(string customerId)
        {
            var profile = await _context.CustomerProfiles.FirstOrDefaultAsync(p => p.UserId == customerId);
            ViewBag.CustomerName = profile?.FullName ?? "未知客戶";
            ViewBag.CustomerId = customerId;

            var currentAdmin = await _userManager.GetUserAsync(User);
            var adminId = currentAdmin.Id;

            var messages = await _context.ChatMessages
                .Where(m =>
                    (m.SenderId == adminId && m.ReceiverId == customerId) ||
                    (m.SenderId == customerId && m.ReceiverId == adminId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            ViewBag.History = messages;
            return View(model: adminId);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CustomerList()
        {
            var customers = await _context.Users
        .Where(u => _context.CustomerProfiles.Any(p => p.UserId == u.Id))
        .Select(u => new
        {
            u.Id,
            u.Email,
            FullName = _context.CustomerProfiles.FirstOrDefault(p => p.UserId == u.Id).FullName
        })
        .ToListAsync();

            // 建立簡單 ViewModel
            var model = customers.Select(c => new CustomerChatUserViewModel
            {
                UserId = c.Id,
                Email = c.Email,
                FullName = c.FullName
            }).ToList();

            return View(model);
        }
    }
}
