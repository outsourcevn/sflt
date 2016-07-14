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
        public class glol
        {
            public string F2 { get; set; }
            public string F3 { get; set; }
            public string F4 { get; set; }
            public DateTime datetime { get; set; }
            public double D { get; set; }
        }
        public string getlistonline(string from, string to, string type, double lon, double lat)
        {
            string query = "select F2,F3,F4,GETDATE() as datetime,ACOS(SIN(PI()*" + lat + "/180.0)*SIN(PI()*lat/180.0)+COS(PI()*" + lat + "/180.0)*COS(PI()*lat/180.0)*COS(PI()*lon/180.0-PI()*" + lon + "/180.0))*6371 As D from list_online where (1=1) ";
            query += " and (F2=N'" + from + "') ";
            query += " and (F3=N'" + to + "') ";
            if (type != null && type != "")
            {
                query += " and (F4=N'" + type + "') ";
            }
            query += " order by d";
            var p = db.Database.SqlQuery<glol>(query);
            return JsonConvert.SerializeObject(p.ToList());
        }
    }
}
