using System;
using System.Web.Mvc;
using BLL;
using UI.Controllers.Abstract;
using UI.Models;

namespace UI.Controllers
{
	public class WcExampleController : ControllerBase<WcExample>
	{
		private readonly WcExampleRepository repo = new WcExampleRepository();

		// for demo purpose...
		private ActionResult Index(int page = 1)
		{
			var model = new WcExampleViewModel(page);
			return View("Index",model);
		}

		[Authorize(Roles = "Editor")]
		// GET: Word
		public ActionResult IndexForEditor(int page = 1)
		{
			return Index(page);
		}

		[Authorize(Roles = "Admin")]
		public ActionResult IndexForAdmin(int page = 1)
		{
			return Index(page);
		}

		public ActionResult CreateForCollocation(long id)
		{
			var model = new WcExampleViewModel(id, CreateMode.Collocation);
			ViewData["Title"] = "Create For Collocation";
			ViewBag.CollocationId = id;
			ViewBag.Controller = "Collocation";
			ViewData["CreateMode"] = CreateMode.Collocation;
			return View("Edit", model);
		}

		public ViewResult Edit(long id)
		{
			var model = new WcExampleViewModel(id, CreateMode.WcExample);
			ViewData["Title"] = id == 0 ? "Create" : "Edit";
			ViewBag.WcExampleId = id;
			ViewBag.Controller = "WcExample";
			ViewData["CreateMode"] = CreateMode.WcExample;
			return View("Edit", model);
		}

		[HttpPost]
		public ActionResult Edit(WcExample wcExample, string source, string collocationId, string createMode, string returnUrl = null)
		{
			try
			{
				if (ModelState.IsValid)
				{
					if (wcExample.Id == "0" || wcExample.Id == null) //newly created wcExample
					{
						try
						{
							WcExample p = new WcExample
							{
								Entry = wcExample.Entry,
								EntryZht = wcExample.EntryZht,
								EntryZhs = wcExample.EntryZhs,
								EntryJap = wcExample.EntryJap,
								RemarkZht = wcExample.RemarkZht,
								RemarkJap = wcExample.RemarkJap,
								RemarkZhs = wcExample.RemarkZhs,
								Source = source,
								CollocationId = long.Parse(collocationId)
							};

							bool[] isOk = repo.Add(p);

							// duplicated entry
							if (isOk[0])
							{
								ViewBag.IsDuplicatedEntry = true;
								return View("Edit", new WcExampleViewModel(wcExample));
							}

							if (!isOk[1])  // add failed!
								return View("_DbActionErrorPartial");

							// add ok!
							if (!isOk[0] && isOk[1])
							{
								if (createMode != null)
								{
									CreateMode mode = (CreateMode)Enum.Parse(typeof(CreateMode), createMode, true);
									switch (mode)
									{
										case CreateMode.WcExample:
											if (User.IsInRole("Admin"))
												return RedirectToRoute(new { action = "IndexForAdmin" });
											if (User.IsInRole("Editor"))
												return RedirectToRoute(new { action = "IndexForEditor" });
											break;
										case CreateMode.Collocation:
											if (User.IsInRole("Admin"))
												return RedirectToRoute(new { controller = "Collocation", action = "IndexForAdmin" });
											if (User.IsInRole("Editor"))
												return RedirectToRoute(new { controller = "Collocation", action = "IndexForEditor" });
											break;
									}
								}
							}
						}
						catch (Exception exception)
						{
							throw new Exception("Create Failed", exception.InnerException);
						}

					}
					else //edit wcExample
					{
						WcExample p = repo.GetObjectById(wcExample.Id);
						if (p != null)
						{
							p.Id = wcExample.Id;
							p.Entry = wcExample.Entry;
							p.EntryZht = wcExample.EntryZht;
							p.EntryZhs = wcExample.EntryZhs;
							p.EntryJap = wcExample.EntryJap;
							p.RemarkZht = wcExample.RemarkZht;
							p.RemarkJap = wcExample.RemarkJap;
							p.RemarkZhs = wcExample.RemarkZhs;
							p.Source = wcExample.Source;
							p.CollocationId = wcExample.CollocationId;
							repo.Update(p);

							if (User.IsInRole("Admin"))
								return RedirectToRoute(new { action = "IndexForAdmin" });
							if (User.IsInRole("Editor"))
								return RedirectToRoute(new { action = "IndexForEditor" });
						}
					}

				}
				else return View("Edit");

			}
			catch (Exception exception)
			{
				ViewBag.ErrorMessage = exception.Message;
				ViewBag.InnerMessage = exception.InnerException;
				return View("Edit");
			}
			return null;
		}

		// GET: WcExample/Delete/5
		public ActionResult Delete(long id, string returnUrl = null)
		{
			try
			{
				if (repo.Delete(id.ToString()))
				{
					if (User.IsInRole("Admin"))
						return RedirectToRoute(new { action = "IndexForAdmin" });
					if (User.IsInRole("Editor"))
						return RedirectToRoute(new { action = "IndexForEditor" });
				}

				return View("_DbActionErrorPartial");
			}
			catch (Exception exception)
			{
				throw new Exception("Delete Failed", exception.InnerException);
			}
		}
	}
}