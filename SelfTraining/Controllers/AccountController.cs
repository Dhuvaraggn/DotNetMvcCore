using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime.Internal;
//using Amazon;
//using Amazon.CognitoIdentityProvider;
//using Amazon.CognitoIdentityProvider.Model;
//using Amazon.Extensions.CognitoAuthentication;
//using Amazon.Runtime.Internal.Util;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using SelfTraining.Data;
using SelfTraining.Models;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static SelfTraining.Controllers.AccountController;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SelfTraining.Controllers
{
    public class AccountController : Controller
    {

        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private const string _clientId = "6ueplbp0hrna2t9rs7bus2urcc";
        //private readonly RegionEndpoint _region = RegionEndpoint.USEast1;
        private readonly ApplicationDbContext _applicationDbContext;
        //private readonly IPersistService _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CognitoUserPool _pool;

        public AccountController(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor, SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
            
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
            _pool = pool;
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        // GET: /<controller>/
        public IActionResult Register()
        {
            return View();
        }

        //GET
        [Authorize]
        public IActionResult New()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            var cognitoUser = _pool.GetUser(user.Email);
            if(cognitoUser == null)
            {
                throw new InvalidOperationException("Expected a non-null Cognito user to be returned.");
            }
            if(!string.IsNullOrWhiteSpace(cognitoUser.Status))
            {
                ModelState.AddModelError(string.Empty, $"A user having email '{user.Email}' already exists.");
                return View(user);
            }

            cognitoUser.Attributes.Add(CognitoAttribute.Name.AttributeName, user.Name);
            cognitoUser.Attributes.Add(CognitoAttribute.Email.AttributeName, user.Email);
            cognitoUser.Attributes.Add(CognitoAttribute.PhoneNumber.AttributeName, user.Phone);

            var identityRes = await _userManager.CreateAsync(cognitoUser, user.Password);

            if(!identityRes.Succeeded)
            {

                ModelState.AddModelError(string.Empty, $"A user having email '{user.Email}' could not be created.");

                foreach (var error in identityRes.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(user);
            }

            return RedirectToAction(nameof(Confirm),new { email = user.Email });
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {

            //if (!ModelState.IsValid)
            //    return View(user);
            var usere = await _userManager.FindByEmailAsync(user.Email);
            if(usere==null)
            {
                ModelState.AddModelError(string.Empty, $"A user having email '{user.Email}' does not exist.");
                return View(user);
            }
            var isconfirmed = await _userManager.IsEmailConfirmedAsync(usere);

            var signInResult = await _signInManager.PasswordSignInAsync(
                usere,
                user.Password,
                isPersistent: false,
                lockoutOnFailure: false);
            if(!signInResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials provided");
                return View(user);
            }

            return RedirectToAction(nameof(MailController.Index), "Mail");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(Login));
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        public IActionResult Confirm(String Email)
        {
            SignupConfirmationViewModel signup = new();
            signup.Email = Email;
            return View(signup);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(SignupConfirmationViewModel signup)
        {
            var user = await _userManager.FindByEmailAsync(signup.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, $"A user having email '{signup.Email}' does not exist.");
                return View(signup);
            }
            var confirmres = await ((CognitoUserManager<CognitoUser>)_userManager).ConfirmSignUpAsync(user, signup.Code,true);
            if (!confirmres.Succeeded)
            {
                foreach (var error in confirmres.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(signup);
            }
            return RedirectToAction(nameof(Index), "Mail");
        }
    }
}

