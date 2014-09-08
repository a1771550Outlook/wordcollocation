using System.Web.Mvc;
using BLL.Abstract;
using UI.Controllers.Abstract;
using UI.Models;

namespace UI.Controllers
{
    public class AdminController : ControllerBase<WcBase>
    {
		[Authorize(Roles="Admin")]
        // GET: Admin
		public ActionResult Index(string letter = null, int page = 1)
		{
			if (string.IsNullOrEmpty(letter)) letter = "a";
			WcSearchViewModel model = new WcSearchViewModel(ViewMode.Admin, letter, page);
            return View(model);
        }

		//public ViewResult CollocationList(string letter = null, int page = 1)
		//{
		//	if (string.IsNullOrEmpty(letter)) letter = "a";

		//	WcAdminViewModel model = new WcAdminViewModel(letter, page);

		//	return View(model);
		//}

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

	    public ActionResult WordList()
	    {
		    return null;
	    }
    }
}
