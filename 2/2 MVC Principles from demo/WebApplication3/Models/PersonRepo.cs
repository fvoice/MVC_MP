using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication3.Models
{
	public class PersonRepo
	{
		private static Person _person;

		public static Person CurrentPerson()
		{
			if (_person == null) _person = new Person() { Name = "Mike", Details = new List<string>() { "detail1", "detail2"}};
			return _person;
		}
	}
}