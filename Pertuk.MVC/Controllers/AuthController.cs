using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pertuk.MVC.Models;
using Pertuk.MVC.PertukApiServices;
using System.Threading.Tasks;

namespace Pertuk.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly PertukApiService _pertukApiService;
        public AuthController(PertukApiService pertukApiService)
        {
            _pertukApiService = pertukApiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(ApiLoginRequestModel loginRequestModel)
        {
            var result = await _pertukApiService.LoginAsync(loginRequestModel);

            HttpContext.Session.SetString("userToken", result.Token);

            return Json("OK");
        }

        [HttpPost]
        public async Task<IActionResult> SendConfirmation(string userId = "28993c3c-c85d-409e-b522-324e413a26fb")
        {
            var result = await _pertukApiService.SendEmailConfirmation(userId);

            return Json("OK");
        }
    }
}
