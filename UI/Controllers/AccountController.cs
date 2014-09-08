using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BLL.Abstract;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using UI.Controllers.Abstract;
using UI.Helpers;
using UI.Models;
using WcResources;

namespace UI.Controllers
{
	[Authorize]
	public class AccountController : ControllerBase<WcBase>
	{
		public AccountController()
		{
		}

		public AccountController(ApplicationUserManager userManager)
		{
			UserManager = userManager;
		}

		private ApplicationUserManager _userManager;
		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			if (TempData["IsAdmin"] == null) TempData["IsAdmin"] = false;
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		private SignInHelper _helper;

		private SignInHelper SignInHelper
		{
			get
			{
				if (_helper == null)
				{
					_helper = new SignInHelper(UserManager, AuthenticationManager);
				}
				return _helper;
			}
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				List<ModelError> Errors = new List<ModelError>();
				foreach (var m in ModelState.Values)
				{
					Errors.AddRange(m.Errors);
				}
				ViewBag.ErrorList = Errors;
				return View(model);
			}

			// This doen't count login failures towards lockout only two factor authentication
			// To enable password failures to trigger lockout, change to shouldLockout: true
			var result = await SignInHelper.PasswordSignIn(model.UserNameEmail, model.Password, model.RememberMe, shouldLockout: false);

			//Add this to check if the email was confirmed.
			//if (!await UserManager.IsEmailConfirmedAsync(model.Id))
			//{
			//	ModelState.AddModelError("", "You need to confirm your email.");
			//	return View(model);
			//}

			switch (result)
			{
				case SignInStatus.EmailNotConfirmed:
					TempData["returnUrl"] = returnUrl;
					var user = await UserManager.FindByEmailAsync(model.UserNameEmail) ??
					           await UserManager.FindByNameAsync(model.UserNameEmail);
					if (user == null)
					{
						ModelState.AddModelError("", Resources.UserNotFound);
						return View(model);
					}
					
					ViewBag.UserId = user.Id;
					ViewBag.UserEmail = user.Email;
					return View("EmailNotConfirmed");
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresTwoFactorAuthentication:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", Resources.InvalidLoginAttempt);
					return View(model);
			}
		}

		//
		// GET: /Account/VerifyCode
		[AllowAnonymous]
		public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
		{
			// Require that the user has already logged in via username/password or external login
			if (!await SignInHelper.HasBeenVerified())
			{
				return View("Error");
			}
			var user = await UserManager.FindByIdAsync(await SignInHelper.GetVerifiedUserIdAsync());
			if (user != null)
			{
				// To exercise the flow without actually sending codes, uncomment the following line
				//ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
			}
			return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
		}

		//
		// POST: /Account/VerifyCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await SignInHelper.TwoFactorSignIn(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(model.ReturnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", Resources.InvalidLoginAttempt);
					return View(model);
			}
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			if (TempData["IsAdmin"] == null) TempData["IsAdmin"] = false;
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
				IdentityResult result1 = await UserManager.CreateAsync(user, model.Password);
				//assign the 'User' role to the new registrator; otherwise he/she will not be authenticated on next login...
				string[] roles = { "Users" };
				IdentityResult result2 = await UserManager.AddUserToRolesAsync(user.Id, roles);

				if (result1.Succeeded && result2.Succeeded)
				//if (result1.Succeeded)
				{
					return await SendConfirmEmailLink(user.Id, user.Email);
				}
				AddErrors_Register(result1, model);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		private async Task<ActionResult> SendConfirmEmailLink(string userId, string userEmail)
		{
			var code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
			var callbackUrl = Url.Action("ConfirmEmail", "Account", new {userId = userId, code = code},
				protocol: Request.Url.Scheme);
			string body = string.Format(@"{0}, <br/><br/><p>{1} {2}： <a href=""{3}"" title=""{4}"">{4}</a></p><br/>{5}",
				Resources.DearNewUser, Resources.ThanksForRegister,
				Resources.ConfirmAccountByLink, callbackUrl, Resources.ConfirmAccount,
				string.Format(@"<a href=""{0}"">{1}</a>", SiteConfiguration.SiteUrl, Resources.SiteTitle));
			await EmailHelper.SendMailAsnyc(Resources.ConfirmYourAccount, body, userEmail);
			return View("DisplayEmail");
		}


		[AllowAnonymous]
		public async Task<ActionResult> ReConfirmEmail(string userId, string userEmail)
		{
			return await SendConfirmEmailLink(userId, userEmail);
		} 

		/// <summary>
		/// Customize the returned Idnetity.Result Error Messages
		/// </summary>
		/// <param name="result"></param>
		/// <param name="model"></param>
		private void AddErrors_Register(IdentityResult result, RegisterViewModel model)
		{
			foreach (var error in result.Errors)
			{
				//[0]Name a1771550@gmail.com is already taken.
				//[1]Email 'a1771550@gmail.com' is already taken.
				if (error.ToLower().StartsWith("name"))
				{
					//currently the field 'name' has yet been implemented...
					//ModelState.AddModelError("",Resources.UserNameTaken);
				}
				if (error.ToLower().StartsWith("email"))
				{
					ModelState.AddModelError("", Resources.EmailTaken);
				}
			}
		}

		//
		// GET: /Account/ConfirmEmail
		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return View("Error");
			}
			var result = await UserManager.ConfirmEmailAsync(userId, code);
			if (result.Succeeded)
			{
				return View("ConfirmEmail");
			}
			AddErrors(result);
			return View();
		}

		//
		// GET: /Account/ForgotPassword
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		//
		// POST: /Account/ForgotPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByNameAsync(model.Email);
				if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return View("ForgotPasswordConfirmation");
				}

				var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
				await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
				ViewBag.Link = callbackUrl;
				return View("ForgotPasswordConfirmation");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ForgotPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/ResetPassword
		[AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			return code == null ? View("Error") : View();
		}

		//
		// POST: /Account/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await UserManager.FindByNameAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			AddErrors(result);
			return View();
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		//
		// POST: /Account/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
		}

		//
		// GET: /Account/SendCode
		[AllowAnonymous]
		public async Task<ActionResult> SendCode(string returnUrl)
		{
			var userId = await SignInHelper.GetVerifiedUserIdAsync();
			if (userId == null)
			{
				return View("Error");
			}
			var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
			var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
			return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
		}

		//
		// POST: /Account/SendCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendCode(SendCodeViewModel model)
		{
			// Generate the token and send it
			if (!ModelState.IsValid)
			{
				return View();
			}

			if (!await SignInHelper.SendTwoFactorCode(model.SelectedProvider))
			{
				return View("Error");
			}
			return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
		}

		//
		// GET: /Account/ExternalLoginCallback
		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return RedirectToAction("Login");
			}

			// Sign in the user with this external login provider if the user already has a login
			var result = await SignInHelper.ExternalSignIn(loginInfo, isPersistent: false);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresTwoFactorAuthentication:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
				case SignInStatus.Failure:
				default:
					// If the user does not have an account, then prompt the user to create an account
					ViewBag.ReturnUrl = returnUrl;
					ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
					return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
			}
		}

		//
		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "ManageUsers");
			}

			if (ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				var info = await AuthenticationManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return View("ExternalLoginFailure");
				}
				var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
				var result = await UserManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await UserManager.AddLoginAsync(user.Id, info.Login);
					if (result.Succeeded)
					{
						await SignInHelper.SignInAsync(user, isPersistent: false, rememberBrowser: false);
						return RedirectToLocal(returnUrl);
					}
				}
				AddErrors(result);
			}

			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}

		//
		// GET: /Account/ExternalLoginFailure
		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _userManager != null)
			{
				_userManager.Dispose();
				_userManager = null;
			}
			base.Dispose(disposing);
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
	}
}