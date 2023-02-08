using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfTraining.Data;
using SelfTraining.Models;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SelfTraining.Controllers
{
    [Authorize]
    public class MailController : Controller
    {
        public ApplicationDbContext _db;
        public IHttpContextAccessor _httpContextAccessor;
        public MailController(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET: /<controller>/
        [Authorize]
        public IActionResult Index()
        {
                var name = _httpContextAccessor.HttpContext.User.FindFirst("name").Value;
            var role = _httpContextAccessor.HttpContext.User.HasClaim(ClaimTypes.Role, "User");
            var admin = _httpContextAccessor.HttpContext.User.HasClaim(ClaimTypes.Role, "Admin");
            if((role && !admin) || (!role && !admin))
            {
                return RedirectToAction("Index", "Mail", new { from = name });
            }
            IEnumerable<Mail> mails = _db.Mails.ToList();
            return View(mails);
        }

        [Authorize]
        [Route("/mail/{from}")]
        public IActionResult Index(string? from)
        {
            IEnumerable<Mail> mails = _db.Mails.Where(x=> x.From.Equals(from)).ToList();
            return View(mails);
        }
        //GET
        [Authorize]
        [Route("/[controller]/[action]")]
        public IActionResult Create()
        {
            return View();
        }
        //Post
        [Authorize]
        [HttpPost]
        [Route("/[controller]/[action]")]
        public IActionResult Create(Mail mail)
        {
            mail.Status = "Created";
            if (ModelState.IsValid)
            {
                _db.Mails.Add(mail);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        //public async Task Logout()
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        //}
    }
}

