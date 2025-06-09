using jamesmvc.Data;
using jamesmvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jamesmvc.Controllers
{
    public class TrackingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TrackingController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> AddTracking(int orderId, string status, string location)
        {
            var record = new TrackingRecord
            {
                LogisticsOrderId = orderId,
                Status = status,
                Location = location
            };

            _context.TrackingRecords.Add(record);
            await _context.SaveChangesAsync();

            return RedirectToAction("AssignedOrders");
        }
        public async Task<IActionResult> UpdateTracking(int id)
        {
            var order = await _context.LogisticsOrder.FindAsync(id);
            if (order == null || order.AssignedDriverId != _userManager.GetUserId(User))
                return NotFound();

            ViewBag.OrderNumber = order.OrderNumber;
            return View(new TrackingRecord { LogisticsOrderId = id });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTracking(TrackingRecord model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Timestamp = DateTime.Now;
            try
            {
                _context.TrackingRecords.Add(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // 可暫時用 Console 或 ViewBag 顯示錯誤
                ViewBag.Error = ex.Message;
                return View(model);
            }

            return RedirectToAction("AssignedOrders");
        }
    }
}
