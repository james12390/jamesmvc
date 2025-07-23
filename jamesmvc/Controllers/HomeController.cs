using System.Diagnostics;
using jamesmvc.Data;
using jamesmvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace jamesmvc.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
        }
        //首頁
        public IActionResult Homepage() => View();
        // 登入
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model ,string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded == false)
            {
                ModelState.AddModelError(string.Empty, "登入失敗，請檢查帳號密碼");
                return View(model);
            }

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                // 檢查角色
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (await _userManager.IsInRoleAsync(user, "Driver"))
                {
                    return RedirectToAction("AssignedOrders", "Driver");
                }
                else if (await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    return RedirectToAction("Index", "LEGO");
                }

                // 如果沒有角色或預設 fallback
                return RedirectToAction("Login", "Home");
            }
           
                ModelState.AddModelError(string.Empty, "登入失敗，請檢查帳號密碼");
                return View(model);
      
        }

        // 登出
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Homepage", "Home");
        }

        public IActionResult Register() => View();
        //註冊
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "資料驗證失敗！");
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                
                var profile = new CustomerProfile
                {
                    UserId = user.Id,
                    FullName = model.FullName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber
                };
                _context.CustomerProfiles.Add(profile);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);

                //確認使用者成功建立
                Console.WriteLine($" 使用者 {user.Email} 註冊成功！");
                return RedirectToAction("Login", "Home");
            }

            // 錯誤輸出
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                Console.WriteLine($" 註冊失敗：{error.Code} - {error.Description}");
            }

            return View(model);
        }


        //忘記密碼
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        //忘記密碼
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // 不洩漏用戶是否存在
                return RedirectToAction("ForgotPasswordConfirmation", "Home");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Home", new { token, email = model.Email }, Request.Scheme);

            ViewBag.ResetLink = callbackUrl; // 測試用途顯示出來

            return View("ForgotPasswordConfirmation");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }




        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
                return BadRequest("重設密碼連結無效");

            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }


        
        public IActionResult Index() => View();

        //查詢單號
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                ViewBag.Error = "請輸入物流單號。";
                return View("Index");
            }

            var order = await _context.LogisticsOrder
                .Include(o => o.AssignedDriver)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                ViewBag.Error = "查無此單號。";
                return View("Index");
            }

            var tracking = await _context.TrackingRecords
                .Where(t => t.LogisticsOrderId == order.Id)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();

            var viewModel = new LogisticsTrackingViewModel
            {
                Order = order,
                TrackingRecords = tracking
            };

            return View("TrackingDetails", viewModel);
        }

    }
}
