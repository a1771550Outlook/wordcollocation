using System.Web.Mvc;
using BLL;
using BLL.Abstract;
using UI.Controllers.Abstract;
using UI.Helpers;

namespace UI.Controllers
{
    public class ContactController : ControllerBase<WcBase>
    {
	    [HttpGet]
	    public ActionResult Index()
	    {
		    return View(new Contact());
	    }

        [HttpPost]
        public ActionResult Index(Contact contact)
        {
	        if (ModelState.IsValid)
	        {
				EmailHelper.SendMail_Contact(contact);
		        return View("Completed");
	        }
	        return View(contact);
        }
    }
}