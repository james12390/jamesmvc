using jamesmvc.Data;
using jamesmvc.Models;
using Microsoft.AspNetCore.Mvc;
using static jamesmvc.Models.LogisticsOrder;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
namespace jamesmvc.Controllers
{
    [Authorize]
    public class LEGOController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LEGOController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //IActionResult 動作結果
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var today = DateTime.Today;
            var todayStr = today.ToString("yyyyMMdd");

            var countToday = await _context.LogisticsOrder.CountAsync(o => o.OrderNumber.StartsWith($"LOGI{todayStr}"));

            var sequence = countToday + 1;
            var orderNumber = $"LOGI{todayStr}-{sequence:D3}";

            var viewModel = new LogisticsOrder
            {
                OrderNumber = orderNumber,
                DeliveryDate = today
            };

            return View(viewModel);
        }
        //Async 讓非同步函式的內部以同步的方式執行非同步 可以使用await
        [HttpPost]
        public async Task<IActionResult> CreateAsync(LogisticsOrder model)
        {

            if (!ModelState.IsValid)
                return View(model);

            var order = new LogisticsOrder
            {
                OrderNumber = model.OrderNumber,
                SenderName = model.SenderName,
                SenderPhone = model.SenderPhone,
                SenderAddress = model.SenderAddress,
                ReceiverName = model.ReceiverName,
                ReceiverAddress = model.ReceiverAddress,
                ReceiverPhone = model.ReceiverPhone,
                ItemDescription = model.ItemDescription,
                DeliveryDate = model.DeliveryDate,
                SenderCity = model.SenderCity,
                SenderDistrict = model.SenderDistrict,
                ReceiverCity = model.ReceiverCity,
                ReceiverDistrict = model.ReceiverDistrict,
                Priority = model.Priority,
                PackageSize = model.PackageSize,
                DeliveryTimeSlot = model.DeliveryTimeSlot,
                Description = model.Description,

            };
            var driver = await _context.DriverProfiles.FirstOrDefaultAsync(d => d.ServiceCity == order.ReceiverCity && d.ServiceDistrict == order.ReceiverDistrict);
            if (driver != null)
            {
                order.AssignedDriverId = driver.UserId;
            }
            else
            { 
                 order.AssignedDriverId = null;
            }
            _context.LogisticsOrder.Add(order);
            await _context.SaveChangesAsync();


            TempData["Success"] = $"物流單已建立，單號為：{order.OrderNumber}";
            return RedirectToAction("OrderConfirmation","LEGO", new { order.OrderNumber });
        }


        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(string orderNumber)
        {

            var order = await _context.LogisticsOrder.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
                return NotFound("查無此單號");


            var model = new LogisticsOrderssViewModel
            {
                Order = order
            }; 
;

            return View("OrderConfirmation", model); // 顯示進度的頁面
        }

        public IActionResult Index(string id)
        {
            string sql = "select count() from LogisticsOrder";
            var count = _context.LogisticsOrder.FromSqlRaw("select count() from LogisticsOrder");
            return View();
        }



        public IActionResult Search()
        {
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Search(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                ViewBag.Error = "請輸入物流單號。";
                return View();
            }

            var order = await _context.LogisticsOrder.Include(o => o.AssignedDriver).FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                ViewBag.Error = "查無此單號。";
                return View();
            }

            return RedirectToAction("Details", "LEGO", new { orderNumber });
        }




        [AllowAnonymous]//無條件開放匿名存取
        [HttpGet("/LEGO/Details/{orderNumber}")]
        public async Task<IActionResult> Details(string orderNumber)
        {

            var order = await _context.LogisticsOrder.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
                return NotFound("查無此單號");

            var tracking = await _context.TrackingRecords.Where(t => t.LogisticsOrderId == order.Id).OrderByDescending(t => t.Timestamp).ToListAsync();

            var model = new LogisticsTrackingViewModel
            {
                Order = order,
                TrackingRecords = tracking
            };

            return View("TrackingDetails", model); // 顯示進度的頁面
        }




        public async Task<IActionResult> Edit()
        {
            var userId = _userManager.GetUserId(User);
            var profile = await _context.CustomerProfiles.FirstOrDefaultAsync(c => c.UserId == userId);

            if (profile == null) return NotFound();

            return View(profile);
        }


  
        [HttpPost]
        public async Task<IActionResult> Edit(CustomerProfile model)
        {
            Console.WriteLine($"ID: {model.Id}, FullName: {model.FullName}");
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"欄位 {state.Key} 錯誤：{error.ErrorMessage}");
                    }
                }
                return View(model);
            }
            Console.WriteLine($"ID: {model.Id}, FullName: {model.FullName}");

            var profile = await _context.CustomerProfiles.FindAsync(model.Id);
            if (profile == null || profile.UserId != _userManager.GetUserId(User))
                return Unauthorized();

            Console.WriteLine($"ID: {model.Id}, FullName: {model.FullName}");
            Console.WriteLine(profile.FullName);

            profile.FullName = model.FullName;
            profile.Address = model.Address;
            profile.PhoneNumber = model.PhoneNumber;
            
            _context.Update(profile);
            await _context.SaveChangesAsync();
            Console.WriteLine($"ID: {model.Id}, FullName: {model.FullName}");
            Console.WriteLine(profile.FullName);
            ViewBag.Message = "修改成功";
            return View(profile);
        }    
    }
}   

