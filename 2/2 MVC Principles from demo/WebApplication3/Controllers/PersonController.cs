using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
	public class PersonController : Controller
	{
		// GET: Person
		public ActionResult Index()
		{
			//return "Hello from controller!";

			var model = PersonRepo.CurrentPerson();
			return View("Index", model);
		}

		[HttpPost]
		public ActionResult AddDetail(string detail)
		{
			var model = PersonRepo.CurrentPerson();

			model.Details.Add(detail);

			return View("Index", model);
		}
	}

}