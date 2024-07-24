using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegAuth.Data;
using RegAuth.Models;
using System.Security.Claims;
using RegAuth.Services;

namespace RegAuth.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserService _userService;
        private bool isUserLoggedIn;

        public UserController(ApplicationDbContext dbContext, IHttpContextAccessor httpContext, IUserService userService)
        {
            _dbContext = dbContext;
            _httpContext = httpContext;
            _userService = userService;
            isUserLoggedIn = _httpContext.HttpContext.User.Identity.IsAuthenticated;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (isUserLoggedIn)
            {
                return RedirectToAction("List");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserViewModel data)
        {
            if (data.Email == null || data.Password == null)
            {
                return View(data);
            }

            var response = await _userService.ValidateUser(data.Email, data.Password);

            if (response.Success == false)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(data);
            }
            else
            {
                var claims = new List<Claim> { new Claim("email", data.Email), new Claim("role", "Admin") };
                var claimsIdentity = new ClaimsIdentity(claims, "pwd", "email", "role");
                var cp = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(cp);
                return RedirectToAction("List");
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserViewModel data)
        {


            if (data.Name == null || data.Email == null || data.Password == null)
            {
                return View(data);
            }
            var response = await _userService.RegisterUser(data.Email, data.Password, data.Name);
            if (response.Success == false)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Resistration successfull");
                return RedirectToAction("Login");

            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var userList = await _dbContext.Users.ToListAsync();
            return View(userList);
        }
        [HttpPost]
        public async Task<IActionResult> ProcessUserActions(string action, List<Guid> selectedUsers)
        {
            if (selectedUsers == null || !selectedUsers.Any())
            {
                ModelState.AddModelError(string.Empty, "No users selected.");
                return RedirectToAction("List");
            }
            var users = await _dbContext.Users.Where(u => selectedUsers.Contains(u.Id)).ToListAsync();

            var response =await _userService.TakeAction(action,users);
            if (response.Success == false) {
                ModelState.AddModelError(string.Empty, response.Message);
            }

            if ((action == "block" || action == "delete") && users.Exists(user => user.Email == _httpContext?.HttpContext?.User?.Identity?.Name?.ToString()))
            {
                await HttpContext.SignOutAsync();
            }

            return RedirectToAction("List");
        }
    }
}
