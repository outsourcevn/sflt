using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SearchFilter.Models;
using System.ComponentModel.DataAnnotations;
using PagedList;
using Newtonsoft.Json;
using System.Xml;
using System.Text;
namespace SearchFilter.Controllers
{
    public class AppiController : Controller
    {
        //
        // GET: /Api/
        private xe63Entities db = new xe63Entities();
        public ActionResult Index()
        {
            return View();
        }
        public string getlist(string from, string to,string type)
        {
            var p = db.lists.Where(o => o.F2.Contains(from)).Where(o => o.F3.Contains(to)).Take(1000);
            if (type != null && type != "")
            {
                p = p.Where(o => o.F4.Contains(type));
            }
            p = p.OrderBy(o => o.rank);
            return JsonConvert.SerializeObject(p.ToList());
        }
    }
}
