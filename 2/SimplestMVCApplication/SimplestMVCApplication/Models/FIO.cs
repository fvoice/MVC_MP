using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimplestMVCApplication.Models
{
	public class FIO
	{
		private static FIO _fio;

		public static FIO GetCurrentFIO()
		{
			return _fio ?? (_fio = new FIO { Id = 35, Surname = "Surname", Name = "Name", LastName = "LastName", Details = new List<string>() });
		}

		public static FIO GetFIOById(long Id)
		{
			return _fio ?? (_fio = new FIO {Id = 35, Surname = "Surname", Name = "Name", LastName = "LastName", Details = new List<string>()});
		}

		public static FIO AddDetail(long id, string detail)
		{
			//some logic with search by id
			var fio = GetFIOById(id);
			fio.Details.Add(detail);
			return fio;
		}

		public static void SetFIO(FIO fio)
		{
			_fio.Name = fio.Name;
			_fio.LastName = fio.LastName;
			_fio.Surname = fio.Surname;
		}

		public long Id { get; set; }

		public string Surname { get; set; }
		public string Name { get; set; }
		public string LastName { get; set; }
		
		public List<string> Details { get; set; }
	}
}