using System;
using System.Web;
using System.Web.Mvc;
using BLL;
using BLL.Abstract;
using BLL.Helpers;
using UI.Controllers.Abstract;
using UI.Models;

namespace UI.Controllers
{
	public class HomeController : ControllerBase<WcBase>
	{
		public const string CollocationListSessionName = "CollocationList";
		//
		// GET: /Home/
		public ActionResult Index()
		{
			WcSearchViewModel model = new WcSearchViewModel(ViewMode.Home);



			return View(model);
		}

		public void SetCulture(string culture, string returnUrl)
		{
			culture = CultureHelper.GetImplementedCulture(culture);
			HttpCookie cookie = Request.Cookies["_culture"];
			if (cookie != null) cookie.Value = culture;
			else
			{
				cookie = new HttpCookie("_culture");
				cookie.Value = culture;
				cookie.Expires = DateTime.Now.AddYears(1);
			}
			Response.Cookies.Add(cookie);
			//RouteData.Values["culture"] = culture;

			Response.Redirect(returnUrl);

			//return null;
		}

		[HttpPost]
		public ActionResult Search(WcSearchViewModel model)
		{
			if (ModelState.IsValid)
			{
				string word = model.Word;
				short colPosId = Convert.ToInt16(model.ColPosId);
				var repo = new CollocationRepository();
				var collocationList = repo.GetCollocationListByWordColPosId(word, colPosId);
				if (collocationList.Count > 0)
				{
					Session[CollocationListSessionName] = collocationList;
					//return View("SearchResult",model);
					return RedirectToAction("SearchResult");
				}
				return View("NoSearchResult", model);
			}
			return null;
		}

		/*
		 * public ActionResult Index(string letter = null, int page = 1)
		{
			if (string.IsNullOrEmpty(letter)) letter = "a";
			WcSearchViewModel model = new WcSearchViewModel(ViewMode.Admin, letter, page);
            return View(model);
        }
		 */
		[HttpGet]
		public ViewResult SearchResult(int page=1)
		{
			WcSearchViewModel model = new WcSearchViewModel(ViewMode.SearchResult, page);
			return View("SearchResult", model);
			//return null;
		}

		public ViewResult UnderConstruction()
		{
			return View("_SiteUnderConstruction");
		}
	}
}