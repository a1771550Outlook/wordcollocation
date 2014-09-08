using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BLL;
using UI.Controllers.Abstract;
using UI.Models;
using WcResources;

namespace UI.Controllers
{
	public class CollocationController : ControllerBase<Collocation>
	{
		private readonly CollocationRepository repo = new CollocationRepository();

		// for demo purpose...
		private ActionResult Index(int page = 1)
		{
			var model = new CollocationViewModel(page);
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
		public ActionResult Create()
		{
			GetPatternDropDownList();
			var model = new CollocationViewModel();
			ViewBag.SourceList = model.SourceList;
			ViewBag.CollocationId = 0;
			ViewData["CreateMode"] = CreateMode.Collocation;
			return View("Edit", model);
		}

		private void GetPatternDropDownList(int patternKey = -1)
		{
			List<SelectListItem> items = new List<SelectListItem>();

			items.Add(new SelectListItem { Selected = patternKey == -1, Text = Resources.PatternDropDownListSelect, Value = "-1" });

			var patterns = repo.GetColPatternDictionary();
			foreach (var key in patterns.Keys)
			{
				SelectListItem item = new SelectListItem();
				item.Value = patterns[key].ToString();
				item.Text = key;
				if (patternKey == patterns[key])
				{
					item.Selected = true;
				}
				items.Add(item);
			}
			ViewBag.PatternList = items;
		}

		public ViewResult Edit(long id)
		{
			var collocation = repo.GetObjectById(id.ToString());
			GetPatternDropDownList((int)collocation.CollocationPattern);
			var model = new CollocationViewModel(collocation);
			ViewBag.SourceList = model.SourceList;
			ViewBag.CollocationId = id;
			return View(model);
		}
		// POST: Collocation/Edit/5
		[HttpPost]
		public ActionResult Edit(long collocationId, string posId, string colPosId, string wordId, string colWordId, string pattern, string returnUrl = null)
		{
			try
			{
				if (ModelState.IsValid)
				{
					Collocation p;
					if (collocationId == 0) //newly created collocation
					{
						try
						{
							p = new Collocation
							{
								posId = short.Parse(posId),
								colPosId = short.Parse(colPosId),
								wordId = long.Parse(wordId),
								colWordId = long.Parse(colWordId),
								CollocationPattern = (CollocationPattern)Enum.Parse(typeof(CollocationPattern), pattern, true)
							};

							bool[] isOk = repo.Add(p, out collocationId);

							// duplicated entry
							if (isOk[0])
							{
								var collocation = repo.GetObjectByIds(posId, colPosId, wordId, colWordId, pattern);
								ViewBag.IsDuplicatedEntry = true;
								return View("Edit", new CollocationViewModel(collocation));
							} 
								

							if (!isOk[1]) // add failed
								return View("_DbActionErrorPartial"); 

							if (collocationId > 0 && isOk[1] && isOk[2]) // add and updateTables ok!
							{
								//ViewData["CreateMode"] = CreateMode.Collocation;
								return RedirectToAction("CreateForCollocation", "WcExample", new {id = collocationId});
								//return RedirectToRoute(new { controller = "WcExample", action = "Edit", id = collocationId });
							}
							return View("_DbActionErrorPartial");
						}
						catch (Exception exception)
						{
							throw new Exception(exception.Message, exception.InnerException);
						}

					}

					//edit collocation
					p = repo.GetObjectById(collocationId.ToString());
					if (p != null)
					{
						p.Id = collocationId;
						p.posId = short.Parse(posId);
						p.colPosId = short.Parse(colPosId);
						p.wordId = short.Parse(wordId);
						p.colWordId = short.Parse(colWordId);
						p.CollocationPattern = (CollocationPattern)Enum.Parse(typeof(CollocationPattern), pattern, true);
						repo.Update(p);

						if (User.IsInRole("Admin"))
							return RedirectToRoute(new { action = "IndexForAdmin" });
						if (User.IsInRole("Editor"))
							return RedirectToRoute(new { action = "IndexForEditor" });
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

		// GET: Collocation/Delete/5
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
