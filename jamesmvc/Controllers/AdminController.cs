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
        private readonly ILogger<LEGOController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        
        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<LEGOController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // 顯示所有使用者 + 目前角色
        public async Task<IActionResult> Dashboard()
        {
            Console.WriteLine("11111111");
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



        [Authorize(Roles = "Admin")]//設定授權
        public async Task<IActionResult> ManageDrivers()
        {
            var drivers = await _userManager.GetUsersInRoleAsync("Driver");

            var driverProfiles = await _context.DriverProfiles.ToListAsync();

            var model = drivers.Select(d =>
            {
                var profiles = driverProfiles.Where(p => p.UserId == d.Id).ToList();
                return new DriverWithProfilesViewModel
                {
                    UserId = d.Id,
                    Email = d.Email,
                    FullName = profiles.FirstOrDefault()?.FullName ?? "(未填姓名)", 
                    Profiles = profiles
                };
            }).ToList();

            return View(model);
        }

        [Authorize(Roles = "Admin")] 
        [HttpGet]
        public IActionResult RegisterEmployee()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> RegisterEmployee(RegisterViewModel model, string role)
        {
    
            Console.WriteLine("11111111");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("2222222");
                Console.WriteLine(role);
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"欄位 {state.Key} 錯誤：{error.ErrorMessage}");
                        ModelState.AddModelError(state.Key, error.ErrorMessage);
                    }
                }

                return View(model);
            }
            Console.WriteLine("22222222");
            var user = new IdentityUser 
            { 
                UserName = model.Email, 
                Email = model.Email 
            };
            Console.WriteLine(role);
            var result = await _userManager.CreateAsync(user, model.Password);
            Console.WriteLine(role);

            if (result.Succeeded)
            {
                Console.WriteLine("3333333");
                Console.WriteLine(role);
                await _userManager.AddToRoleAsync(user, role);

                if (role == "Driver") 
                {
                    var profile = new DriverProfile
                    {
                        UserId = user.Id,
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address
                    };
                    _context.DriverProfiles.Add(profile);
                    await _context.SaveChangesAsync();
                }
                Console.WriteLine($" 使用者 {user.Email} 建立成功！");
                TempData["Success"] = $"已成功建立 {role} 帳號：{model.Email}";
                return RedirectToAction("Dashboard");
            }

            foreach (var error in result.Errors)
            {
                Console.WriteLine($"建立帳號失敗：{error.Code} - {error.Description}");
                ModelState.AddModelError(string.Empty, error.Description);
            }
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

           if (ModelState.IsValid)
            {
                var update = await _context.DriverProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

                if (update != null)
                {

                    update.ServiceCity = serviceCity;
                    update.ServiceDistrict = serviceDistrict;
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "服務範圍已新增";
                    return RedirectToAction("ManageDrivers");

                }

            }

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

        public async Task<IActionResult> UnassignedByCity()
        {
            var result = await _context.LogisticsOrder
                .Where(o => o.AssignedDriverId == null)
                .GroupBy(o => o.SenderCity)
                .Select(g => new UnassignedOrderStatViewModel
                {
                    City = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return View(result);
        }
        public async Task<IActionResult> UnassignedList(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return RedirectToAction("UnassignedByCity");

            var orders = await _context.LogisticsOrder
                .Where(o => o.AssignedDriverId == null && o.SenderCity == city)
                .OrderByDescending(o => o.DeliveryDate)
                .ToListAsync();

            var driversInCity = await _context.DriverProfiles
            .Where(d => d.ServiceCity == city)
            .Select(d => new SelectListItem
        {
            Value = d.UserId,
            Text = d.FullName
        })
        .ToListAsync();

            ViewBag.City = city;
            ViewBag.Drivers = driversInCity;

            return View(orders);
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

        [HttpPost]
        public async Task<IActionResult> AssignssDriver(int orderId, string driverId)
        {
            var order = await _context.LogisticsOrder.FindAsync(orderId);
            if (order == null)
                return NotFound();

            order.AssignedDriverId = driverId;
            await _context.SaveChangesAsync();

            TempData["Message"] = "指派成功！";
            return RedirectToAction("UnassignedList", new { city = order.ReceiverCity });
        }

    }



}
