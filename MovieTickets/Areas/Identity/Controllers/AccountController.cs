using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MovieTickets.Models;
using MovieTickets.ViewModels;

namespace MovieTickets.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }


        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (roleManager.Roles.IsNullOrEmpty())
            {
                await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("Customer"));
                await roleManager.CreateAsync(new IdentityRole("Company"));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegiserVM regiserVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Save img in wwwroot
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    // Save img name in db
                    regiserVM.ImageUrl = fileName;
                }
                var existingUser = await userManager.FindByEmailAsync(regiserVM.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Already Exist.");
                    return View(regiserVM);
                }
                ApplicationUser user = new()
                {
                    UserName = regiserVM.UserName,
                    Email = regiserVM.Email,
                    ImageUrl = regiserVM.ImageUrl,
                    Address = regiserVM.Address,
                };
                var result = await userManager.CreateAsync(user, regiserVM.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    await userManager.AddToRoleAsync(user, "Customer");
                    return RedirectToAction("index", "Home", new { area = "Customer" });
                }
                else
                {
                    ModelState.AddModelError("password", "don't match the constrains");
                }
            }
            return View(regiserVM);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(loginVM.Email);

                if (user != null)
                {
                    var result = await userManager.CheckPasswordAsync(user, loginVM.Password);
                    if (result)
                    {

                        await signInManager.SignInAsync(user, loginVM.RememberMe);
                        return RedirectToAction("index", "Home", new { area = "Customer" });
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "password is not corect");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Email is not Found");
                }
            }
            return View(loginVM);
        }
        [HttpGet]
        public async Task<IActionResult> Profile(string name)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(name);
                if (user != null)
                {
                    //map
                    UserProfileVM profileVM = mapper.Map<UserProfileVM>(user);
                    return View(profileVM);

                }
                else
                {
                    ModelState.AddModelError("username", "Can Not Find ThE User");
                }
            }
            return RedirectToAction("NotFoundPage", "Home", new { area = "Customer" });


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileVM model, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }
            if ((file != null && file.Length > 0) && user != null && (!string.IsNullOrEmpty(model.OldPassword) && !string.IsNullOrEmpty(model.NewPassword)))
            {
                var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                else
                {
                    // Save img in wwwroot
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Save img name in db
                    user.ImageUrl = fileName;
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        await signInManager.RefreshSignInAsync(user);
                        return RedirectToAction("login");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                    }

                }

            }
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
