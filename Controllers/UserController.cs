using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Models;
using Movies.Services;
using Movies.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Movies.Controllers
{
    public class UserController : Controller
    {
        private readonly ContextApp _context;
        private readonly IPasswordEncrypter _passwordEncrypter;

        public UserController(ContextApp context, IPasswordEncrypter passwordEncrypter)
        {
            _context = context;
            _passwordEncrypter = passwordEncrypter;
        }

        public IActionResult Register()
        {
            if(User.Identity != null && User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (registerViewModel.Password != registerViewModel.ConfirmPassword)
            {
                ViewData["Message"] = "Las contraseñas no coincien";
                return View();
            }
            var salt = _passwordEncrypter.GenerateSalt();
            var user = new User
            {
                Name = registerViewModel.Name,
                Username = registerViewModel.Username,
                Email = registerViewModel.Email,
                Password = _passwordEncrypter.HashPassword(registerViewModel.Password, salt) + ":" + salt,
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == loginViewModel.Email);
            if (user == null)
            {
                ViewData["Message"] = "Usuario no encontrado";
                return View();
            }
            var passwordData = user.Password.Split(":");
            var password = passwordData[0];
            var salt = passwordData[1];
            if (password != _passwordEncrypter.HashPassword(loginViewModel.Password, salt))
            {
                ViewData["Message"] = "Contraseña incorrecta";
                return View();
            }
            var claim = new List<Claim>()
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties
            {
                AllowRefresh = true,  
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
            return RedirectToAction("Index", "Home");
        }
    }
}
