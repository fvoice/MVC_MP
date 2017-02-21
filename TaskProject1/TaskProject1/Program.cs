using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CsQuery;

namespace TaskProject1
{
	class Program
	{
		private static string _path;
		//private static string _baseUri = "https://habrahabr.ru/post/321856/#habracut";
		private static string _baseUri = "https://habrahabr.ru/";
		//private static string _baseUri = "https://en.wikipedia.org/";
		private static bool _useAsync = true;
		private static int _maxLevel = 1;
		private static int _chunkSize = 50;

		private static string _htmlContentType = "text/html";
		private static string _xmlContentType = "text/xml";
		private static string _problemLink = "###";
		private static readonly HttpClient Client = new HttpClient();

		private static bool _onlyBaseDomain = true;
		private static string _baseDomain;

		private static Dictionary<string, string> _urlCache = new Dictionary<string, string>();

		private static readonly object ModifyLock = new object();

		static void Main(string[] args)
		{;
		_path = string.Format("C:\\Sites\\{0}", DateTime.Now.Ticks);
			Directory.CreateDirectory(_path);

			if (_onlyBaseDomain)
			{
				var uri = NormalizeUri(_baseUri);
				if (!string.IsNullOrEmpty(uri.LocalPath) && uri.LocalPath != "/")
					_baseDomain = uri.AbsoluteUri.Replace(uri.LocalPath, string.Empty);
				else
					_baseDomain = uri.AbsoluteUri;
				if (!string.IsNullOrEmpty(uri.Fragment)) _baseDomain = uri.AbsoluteUri.Replace(uri.Fragment, string.Empty);
			}

			Task t = new Task(() => Do(_baseUri));
			t.Start();
			Console.ReadLine();
			Client.Dispose();
		}

		public static async void Do(string url)
		{
			await Download(url);
		}

		public static async Task<string> Download(string uriString, string subDirName = "", string webRelPath = "", int level = 0)
		{
			if (level > _maxLevel) return _problemLink;
			
			Uri uri = NormalizeUri(uriString);

			if (_urlCache.ContainsKey(uri.AbsoluteUri))
			{
				return _urlCache[uri.AbsoluteUri];
			}
			
			string name = _problemLink;

			try
			{
				using (HttpResponseMessage response = await Client.GetAsync(uri.AbsoluteUri))
				{
					Console.WriteLine("Downloading {0}", uri.AbsoluteUri);
					if (response.StatusCode != HttpStatusCode.OK)
					{
						WriteError(string.Format("{0} {1} for {2}", (int)response.StatusCode, response.StatusCode, uri.AbsoluteUri));
						return _problemLink;
					}
					using (HttpContent content = response.Content)
					{
						name = GetFileName(uri, content);

						var winPath = Path.Combine(_path, subDirName);
						Directory.CreateDirectory(winPath);

						string fileName = string.Format("{0}\\{1}", winPath, name);
						Console.WriteLine("Saving to {0}", fileName);

						if (content.Headers.ContentType.MediaType.Contains(_htmlContentType))
						{
							var result = await content.ReadAsStringAsync();
							ProcessHtml(subDirName, level, result, fileName);
						}
						else
						{
							var result = await content.ReadAsStreamAsync();
							ProcessContent(result, fileName);
						}
					}
				}
			}
			catch (Exception e)
			{
				WriteError(e.Message);
			}
			var newUri = string.Format("{0}/{1}", webRelPath, name);
			_urlCache[uri.AbsoluteUri] = newUri;
			return newUri;
		}

		private static void ProcessContent(Stream result, string fileName)
		{
			using (var fileStream = File.Create(fileName))
			{
				result.Seek(0, SeekOrigin.Begin);
				result.CopyTo(fileStream);
			}
		}

		private static async void ProcessHtml(string subDirName, int level, string result, string fileName)
		{
			var root = CQ.CreateDocument(result);
			var imgElements = root.Select("img");
			var linkElements = root.Select("link");
			var aElements = root.Select("a");

			List<Tuple<IDomElement, string, int>> elementsList = imgElements.Elements.Select(x => new Tuple<IDomElement, string, int>(x, "src", level)).ToList();
			elementsList.AddRange(linkElements.Elements
				.Where(x => x.HasAttribute("rel") && x.Attributes["rel"] == "stylesheet")
				.Select(x => new Tuple<IDomElement, string, int>(x, "href", level)));
			if (level < _maxLevel)
			{
				elementsList.AddRange(_onlyBaseDomain
					? aElements.Elements.Where(x => x.Attributes["href"].StartsWith(_baseDomain))
						.Select(x => new Tuple<IDomElement, string, int>(x, "href", level + 1))
					: aElements.Elements.Select(x => new Tuple<IDomElement, string, int>(x, "href", level + 1)));
			}

			try
			{
				if (_useAsync)
				{
					Random random = new Random();
					bool continueMark = true;
					while (continueMark)
					{
						if (elementsList.Count == 0)
						{
							continueMark = false;
							continue;
						}
						var chunkCount = elementsList.Count > _chunkSize ? _chunkSize : elementsList.Count;
						var chunk = elementsList.Take(chunkCount).ToList();
						elementsList.RemoveRange(0, chunkCount);

						await Task.WhenAll(chunk.Select(x =>
						{
							return Task.Run(async () =>
							{
								try
								{
									//await Task.Delay(random.Next(100, 3000)); //looks like some sites can consider such active work as dangerous behavior, let's add some times
									var src = x.Item1.Attributes[x.Item2];
									if (!string.IsNullOrEmpty(src))
									{
										var relPath = Guid.NewGuid().ToString();
										var newVal = await Download(src, Path.Combine(subDirName, relPath), relPath, x.Item3);
										lock (ModifyLock)
										{
											x.Item1.SetAttribute(x.Item2, newVal);
										}
									}
								}
								catch (Exception e)
								{
									WriteError(e.Message);
								}
							});
						}));	
					}
				}
				else
				{
					foreach (var tuple in elementsList)
					{
						try
						{
							var src = tuple.Item1.Attributes[tuple.Item2];
							var relPath = Guid.NewGuid().ToString();
							var newVal = await Download(src, Path.Combine(subDirName, relPath), relPath, tuple.Item3);
							lock (ModifyLock)
							{
								tuple.Item1.SetAttribute(tuple.Item2, newVal);
							}
						}
						catch (Exception e)
						{ 
							WriteError(e.Message);
						}
					}
				}
			}
			catch (Exception e)
			{
				WriteError(e.Message);
			}

			root.Save(fileName);
		}

		private static Uri NormalizeUri(string uri)
		{
			uri = HttpUtility.UrlDecode(uri);

			if (string.IsNullOrEmpty(uri)) throw new ArgumentException(string.Format("Uri {0} is not recognized", uri));

			if (!uri.StartsWith("http://") && !uri.StartsWith("https://"))
			{
				uri = uri.StartsWith("//") ? string.Format("http:{0}", uri) : string.Format("{0}{1}", _baseUri, uri);
			}
			return new Uri(uri);
		}

		private static void WriteError(string s)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(s);
			Console.ForegroundColor = ConsoleColor.Green;
		}

		private static string GetFileName(Uri uri, HttpContent content)
		{
			string name = uri.Segments.LastOrDefault();
			name = name.Replace(":", "");
			if (string.IsNullOrEmpty(name))
			{
				name = Guid.NewGuid().ToString();
			}
			if (name.EndsWith("/") && content.Headers.ContentType.MediaType.Contains(_xmlContentType))
			{
				name = "index.xml";
			}
			if (name.EndsWith("/") && content.Headers.ContentType.MediaType.Contains(_htmlContentType))
			{
				name = "index.html";
			}
			return name;
		}
	}
}
