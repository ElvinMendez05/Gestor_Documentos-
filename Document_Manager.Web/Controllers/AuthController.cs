using Document_Manager.Application.Interface;
using Document_Manager.Domain.Entities;
using Document_Manager.Web.Models;
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

            // SI EL EMAIL NO ESTÁ CONFIRMADO
            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                // Generar nuevo token
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
                <p>Tu cuenta aún no ha sido confirmada.</p>
                <a href='{confirmationLink}'>Confirmar cuenta</a>
        ";

                await emailService.SendAsync(
                    user.Email!,
                    "Confirma tu cuenta",
                    body
                );

                return View("RegisterConfirmation");
            }

            var result = await signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password!,
                model.RememberMe,
                false
            );

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

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

            await emailService.SendAsync(
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
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var decodedToken = WebUtility.UrlDecode(token);

            var result = await userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
                return View("EmailConfirmed");

            return View("Error", new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier
            });
        }

        public IActionResult VerifyEmail()
        {
            return View();
        } 

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByEmailAsync(model.Email!);

            if (user == null)
            {
                ModelState.AddModelError("", "El correo no existe.");
                return View(model);
            }

            //GENERAR TOKEN DE RESET
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            //LINK CON TOKEN
            var link = Url.Action(
                "ChangePassword",
                "Auth",
                new { email = user.Email, token = encodedToken },
                Request.Scheme
            );

            //ENVIAR EMAIL
                var body = $@"
                    <h2>Recuperación de contraseña</h2>
                    <p>Haz click en el botón para cambiar tu contraseña:</p>
                    <a href='{link}'>Cambiar contraseña</a>
                ";

            await emailService.SendAsync(
                user.Email!,
                "Recuperar contraseña",
                body
            );

            return View("VerifyEmailConfirmation");
        }


        public IActionResult ChangePassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return RedirectToAction("VerifyEmail");

            return View(new ChangePasswordViewModel
            {
                Email = email,
                Token = token
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Datos inválidos.");
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
                return View(model);
            }

            // DECODIFICAR JUSTO ANTES DE USAR
            var decodedToken = WebUtility.UrlDecode(model.Token);

            var result = await userManager.ResetPasswordAsync(
                user,
                decodedToken,
                model.NewPassword!
            );

            if (result.Succeeded)
            {
                TempData["Success"] = "Contraseña actualizada correctamente.";
                return RedirectToAction("Login", "Auth");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ---------- LOGOUT ----------

        [HttpGet]
        public IActionResult LoggedOut()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("LoggedOut");
        }
    }
}

