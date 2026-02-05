using Document_Manager.Application.Interface;
using Document_Manager.Domain.Entities;
using Document_Manager.Web.Models.AuthViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Document_Manager.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly IEmailService emailService;

        public AuthController(
            SignInManager<Users> signInManager,
            UserManager<Users> userManager,
            IEmailService emailService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.emailService = emailService;
        }

        // ---------- LOGIN ----------
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByEmailAsync(model.Email!);

            if (user == null)
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
                return View(model);
            }

            var result = await signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password!,
                model.RememberMe,
                false
            );

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            if (result.IsNotAllowed)
                return View("RegisterConfirmation");

            ModelState.AddModelError("", "Credenciales incorrectas.");
            return View(model);
        }

        // ---------- REGISTER ----------
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new Users
            {
                FullName = model.Name,
                Email = model.Email,
                UserName = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password!);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Auth",
                new { userId = user.Id, token = encodedToken },
                Request.Scheme
            );

            var body = $@"
                <h2>Confirma tu cuenta</h2>
                <p>Gracias por registrarte.</p>
                <a href='{confirmationLink}'>Confirmar cuenta</a>
            ";

            await emailService.SendEmailAsync(
                user.Email!,
                "Confirmación de cuenta",
                body
            );

            return View("RegisterConfirmation");
        }

        // ---------- CONFIRM EMAIL ----------
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return BadRequest();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
                return View("EmailConfirmed");

            return View("Error");
        }

        // ---------- LOGOUT ----------
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}


//using Document_Manager.Domain.Entities;
//using Document_Manager.Web.Models.AuthViewModels;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;


//namespace Document_Manager.Web.Controllers
//{

//    public class AuthController : Controller 
//    {
//        private readonly SignInManager<Users> signInManager;
//        private readonly UserManager<Users> userManager;

//        public AuthController(SignInManager<Users> signInManager, UserManager<Users> userManager)
//        {
//            this.signInManager = signInManager;
//            this.userManager = userManager;
//        }

//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Login(LoginViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return View(model);

//            var user = await userManager.FindByEmailAsync(model.Email!);

//            if (user == null)
//            {
//                ModelState.AddModelError("", "Usuario no encontrado.");
//                return View(model);
//            }

//            var result = await signInManager.PasswordSignInAsync(
//                user.UserName!,
//                model.Password!,
//                model.RememberMe,
//                false
//            );

//            if (result.Succeeded)
//            {
//                return RedirectToAction("Index", "Home");
//            }

//            if (result.IsNotAllowed)
//            {
//                // 👇 AQUÍ ESTÁ LA CLAVE
//                return View("RegisterConfirmation");
//            }

//            ModelState.AddModelError("", "Correo o contraseña incorrectos.");
//            return View(model);
//        }




//        public IActionResult Register()
//        {
//            return View();
//        }

//        //I'm testing this -> process
//        [HttpPost]
//        public async Task<IActionResult> Register(RegisterViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                Users users = new Users
//                {
//                    FullName = model.Name,
//                    Email = model.Email,
//                    UserName = model.Email,
//                };

//                var result = await userManager.CreateAsync(users, model.Password!);

//                if (result.Succeeded)
//                {

//                    var token = await userManager.GenerateEmailConfirmationTokenAsync(users);

//                    // 2️⃣ Crear link
//                    var confirmationLink = Url.Action(
//                        "ConfirmEmail",
//                        "Auth",
//                        new { userId = users.Id, token = token },
//                        Request.Scheme
//                    );

//                    Console.WriteLine("CONFIRMATION LINK:");
//                    Console.WriteLine(confirmationLink);

//                    return RedirectToAction("Login", "Auth");
//                }
//                else
//                {
//                    foreach (var error in result.Errors)
//                    {
//                        ModelState.AddModelError("", error.Description);
//                    }

//                    return View(model);
//                }
//            }
//            return View(model);
//        }

//        //Confirm changes cause this can be a mess
//        [HttpGet]
//        public async Task<IActionResult> ConfirmEmail(string userId, string token)
//        {
//            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
//                return BadRequest();

//            var user = await userManager.FindByIdAsync(userId);
//            if (user == null)
//                return NotFound();

//            var result = await userManager.ConfirmEmailAsync(user, token);

//            if (result.Succeeded)
//                return View("EmailConfirmed");

//            return View("Error");
//        }



//        public IActionResult VerifyEmail()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = await userManager.FindByNameAsync(model.Email!);

//                if (user == null)
//                {
//                    ModelState.AddModelError("", "Something is wrong!");
//                    return View(model);
//                }
//                else
//                {
//                    return RedirectToAction("ChangePassword", "Auth", new { username = user.UserName });
//                }
//            }
//            return View(model);
//        }

//        public IActionResult ChangePassword(string username)
//        {
//            if (string.IsNullOrEmpty(username))
//            {
//                return RedirectToAction("VerifyEmail", "Auth");
//            }
//            return View(new ChangePasswordViewModel { Email = username });
//        }

//        [HttpPost]
//        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = await userManager.FindByNameAsync(model.Email!);
//                if (user != null)
//                {
//                    var result = await userManager.RemovePasswordAsync(user);
//                    if (result.Succeeded)
//                    {
//                        result = await userManager.AddPasswordAsync(user, model.NewPassword!);
//                        return RedirectToAction("Login", "Auth");
//                    }
//                    else
//                    {
//                        foreach (var error in result.Errors)
//                        {
//                            ModelState.AddModelError("", error.Description);
//                        }

//                        return View(model);
//                    }
//                }
//                else
//                {
//                    ModelState.AddModelError("", "Email not found!");
//                    return View(model);
//                }
//            }
//            else
//            {
//                ModelState.AddModelError("", "Something went wrong. try again.");
//                return View(model);
//            }
//        }

//        [HttpGet]
//        public IActionResult LoggedOut()
//        {
//            return View();
//        }
//        public async Task<IActionResult> Logout()
//        {
//            await signInManager.SignOutAsync();
//            return RedirectToAction("LoggedOut");
//        }
//    }
//}
