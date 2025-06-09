using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using jamesmvc.Data;
using jamesmvc.Models;
namespace jamesmvc.Controllers
{
    [Authorize(Roles = "Driver")]
    public class DriverController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DriverController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        //  顯示指派給此司機的物流單
        public async Task<IActionResult> AssignedOrders()
        {
            var userId = _userManager.GetUserId(User);

            var orders = await _context.LogisticsOrder
                .Where(o => o.AssignedDriverId == userId)
                .ToListAsync();

            return View(orders);
        }
        [HttpPost]
        public async Task<IActionResult> AddTracking(int orderId, string status, string location)
        {
            if (string.IsNullOrWhiteSpace(status) || string.IsNullOrWhiteSpace(location))
            {
                ModelState.AddModelError("", "請填寫所有欄位");
                return View("UpdateTracking", new TrackingRecord { LogisticsOrderId = orderId });
            }

            var record = new TrackingRecord
            {
                LogisticsOrderId = orderId,
                Status = status,
                Location = location,
                Timestamp = DateTime.Now
            };

            _context.TrackingRecords.Add(record);
            await _context.SaveChangesAsync();

            return RedirectToAction("AssignedOrders");
        }
        //  顯示追蹤表單
        [HttpGet]
        public async Task<IActionResult> UpdateTracking(int id)
        {
            var order = await _context.LogisticsOrder.FindAsync(id);
            if (order == null || order.AssignedDriverId != _userManager.GetUserId(User))
                return NotFound();

            ViewBag.OrderNumber = order.OrderNumber;
            return View(new TrackingRecord { LogisticsOrderId = id });
        }

    }
}
