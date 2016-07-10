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
    public class HomeController : Controller
    {
        private xe63Entities db = new xe63Entities();
        public partial class typegroup
        {
            
            public string name { get; set; }
        }
       
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            //var p = (from q in db.listprovince select q).OrderBy(o => o.name).ToList();
            try { 
                string query = "SELECT F2 as name FROM  list group by F2";
                var cat1 = db.Database.SqlQuery<typegroup>(query);
                ViewBag.cat1 = cat1;
                query = "SELECT F3 as name FROM  list group by F3";
                var cat2 = db.Database.SqlQuery<typegroup>(query);
                ViewBag.cat2 = cat2;
                query = "SELECT F4 as name FROM  list group by f4";
                var type = db.Database.SqlQuery<typegroup>(query);
                ViewBag.type = type.ToList();
                string spopular="";
                var popular=(from q in db.logs select q).OrderByDescending(o=>o.total).ThenBy(o=>o.id).Take(10).ToList();
                for(int i=0;i<popular.Count;i++){
                    spopular += "<a href=\"#\" onclick='setSearch(\"" + popular[i].F2 + "\",\"" + popular[i].F3 + "\");'>" + popular[i].F2 + " - " + popular[i].F3 + "</a>";
                    if (i < popular.Count - 1) spopular += ", ";
                }
                ViewBag.popular = spopular;
                ViewBag.from = Config.getCookie("from");
                ViewBag.to = Config.getCookie("to");
                if (ViewBag.from == "") ViewBag.from = -1;
                if (ViewBag.to == "") ViewBag.to = -1;
            }catch(Exception ex){

            }
            return View();
        }
        public ActionResult list(string from, string to, string type,string f13, string f7,string f8,string f9,string f10,string f11,string f17,string order,int? pg) {
            if (order == null) order = "rank";
            ViewBag.isMobile = false;
            if (Request.Browser.IsMobileDevice) ViewBag.isMobile = true;
            var p = db.lists.Where(o => o.F2.Contains(from)).Where(o => o.F3.Contains(to)).Take(1000);
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            if (type != null && type != "")
            {
                p = p.Where(o => o.F4.Contains(type));
            }
            if (f7 == null) f7 = ""; if (f8 == null) f8 = ""; if (f9 == null) f9 = ""; if (f10 == null) f10 = ""; if (f11 == null) f11 = ""; if (f13 == null) f13 = "";
            if (f7 != null && f7 != "")
            {
                p = p.Where(o => o.F7.Contains(f7));
            }
            if (f8 != null && f8 != "")
            {
                p = p.Where(o => o.F8.Contains(f8));
            }
            if (f9 != null && f9 != "")
            {
                p = p.Where(o => o.F9.Contains(f9));
            }
            if (f10 != null && f10 != "")
            {
                p = p.Where(o => o.F10.Contains(f10));
            }
            if (f11 != null && f11 != "")
            {
                p = p.Where(o => o.F11.Contains(f11));
            }
            if (f13 != null && f13 != "")
            {
                p = p.Where(o => o.F13.Contains(f13));
            }
            if (order == "")
            {
                p = p.OrderBy(o => o.F17);
            }
            else if (order != null && order != "rank")
            {
                p = p.OrderByDescending(o => o.F17);
            } 
            else
                if (order == null || order == "rank")
                {
                    p = p.OrderBy(o => o.rank);
                }
            if (from == null) from = "";
            if (to == null) to = "";
            if (type == null) type = "";
            ViewBag.from = from.Replace("%20", " ");
            ViewBag.to = to.Replace("%20", " ");           
            ViewBag.type = type.Replace("%20", " ");
            string query = "SELECT F2 as name FROM  list group by F2";
            var cat1 = db.Database.SqlQuery<typegroup>(query);
            ViewBag.cat1 = cat1;
            query = "SELECT F3 as name FROM  list group by F3";
            var cat2 = db.Database.SqlQuery<typegroup>(query);
            ViewBag.cat2 = cat2;
            query = "SELECT F4 as name FROM  list group by f4";
            var typecat = db.Database.SqlQuery<typegroup>(query);
            ViewBag.typecat = typecat.ToList();

            var lf7 = db.Database.SqlQuery<typegroup>(Config.query(from,to,type,7));
            ViewBag.lf7 = lf7.ToList();
            ViewBag.f7 = f7.Replace("%20", " ");
            var lf8= db.Database.SqlQuery<typegroup>(Config.query(from, to, type, 8));
            ViewBag.lf8 = lf8.ToList();
            ViewBag.f8 = f8.Replace("%20", " ");
            var lf9 = db.Database.SqlQuery<typegroup>(Config.query(from, to, type, 9));
            ViewBag.lf9 = lf9.ToList();
            ViewBag.f9 = f9.Replace("%20", " ");
            var lf10 = db.Database.SqlQuery<typegroup>(Config.query(from, to, type, 10));
            ViewBag.lf10 = lf10.ToList();
            ViewBag.f10 = f10.Replace("%20", " ");
            var lf11 = db.Database.SqlQuery<typegroup>(Config.query(from, to, type, 11));
            ViewBag.lf11 = lf11.ToList();
            ViewBag.f11 = f11.Replace("%20", " ");
            var lf13 = db.Database.SqlQuery<typegroup>(Config.query(from, to, type, 13));
            ViewBag.lf13= lf13.ToList();
            ViewBag.f13 = f13.Replace("%20", " ");
            ViewBag.order = order;
            return View(p.ToPagedList(pageNumber, pageSize));
           
        }
        public string getDetail(int id) {
            var p = (from q in db.lists where q.id == id select q).ToList();
            return JsonConvert.SerializeObject(p);
        }
        public string Log(string F2, string F3) {
            try
            {
                bool f = db.logs.Any(o => o.F2.Contains(F2) && o.F3.Contains(F3));
                if (f) {
                    db.Database.ExecuteSqlCommand("update log set total=total+1 where F2=N'" + F2 + "' and F3=N'" + F3 + "'");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("insert into log(F2,F3,total) values(N'" + F2 + "',N'" + F3 + "',1)");
                }
                Config.setCookie("from", F2);
                Config.setCookie("to", F3);
                return "1";
            }
            catch (Exception ex) {
                return "0";
            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
        public ActionResult Add() {
            return View();
        }
        public string getList() {
            var p = (from q in db.lists select q).OrderByDescending(o => o.id).ToList().Take(10);
            return JsonConvert.SerializeObject(p);
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public string AddNew() {
            int i = 0;
            try
            {
                string query = "Insert into list(F1";
                for (i = 2; i <= 19; i++)
                {
                    if (Request.Form["F" + i] != null) query += ",F" + i;
                }
                query += ",rank) Values(''";
                for (i = 2; i <= 19; i++)
                {
                    if (i != 17)
                    {
                        if (Request.Form["F" + i] != null) query += ",N'" + Request.Form["F" + i].ToString().Replace("\r\n", "").Replace("\r", "").Replace("\n", "") + "'";
                    }
                    else
                    {
                        query += "," + int.Parse(Request.Form["F" + i].ToString().Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));
                    }
                }
                query += ",null)";
                db.Database.ExecuteSqlCommand(query);
                query = "update list set rank=id where rank is null";
                db.Database.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        public class tuyenxe {
            public string F2 { get; set; }
            public string F3 { get; set; }
        }
        public ActionResult Detail(int? id) {
            list list = db.lists.Find(id);
            if (list == null)
            {
                return HttpNotFound();
            }
            return View(list);

        }
        public string generateSiteMap()
        {

            try
            {

                XmlWriterSettings settings = null;
                string xmlDoc = null;
                var p = db.Database.SqlQuery<tuyenxe>("select F2,F3 from list group by F2,F3 order by F2,F3").ToList();
                settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                xmlDoc = HttpRuntime.AppDomainAppPath + "sitemap.xml";//HttpContext.Server.MapPath("../") + 
                float percent = 0.85f;

                string urllink = "";
                using (XmlTextWriter writer = new XmlTextWriter(xmlDoc, Encoding.UTF8))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("urlset");
                    writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", "http://xe63.vn");
                    writer.WriteElementString("changefreq", "always");
                    writer.WriteElementString("priority", "1");
                    writer.WriteEndElement();

                    for (int i = 0; i < p.Count; i++)
                    {
                        try
                        {
                            writer.WriteStartElement("url");
                            urllink = "http://xe63.vn/home/list/?from="+HttpUtility.UrlEncode(p[i].F2)+ "&to=" + HttpUtility.UrlEncode(p[i].F3)+ "&type=&order=rank";
                            writer.WriteElementString("loc", urllink);
                            //writer.WriteElementString("lastmod", DR["datetime"].ToString());
                            try
                            {
                                if (i < 500)
                                {
                                    writer.WriteElementString("changefreq", "hourly");
                                    percent = 0.85f;
                                }
                                else
                                {
                                    writer.WriteElementString("changefreq", "monthly");
                                    percent = 0.70f;
                                }
                            }
                            catch (Exception ex1)
                            {
                            }

                            writer.WriteElementString("priority", percent.ToString("0.00"));
                            writer.WriteEndElement();
                        }
                        catch (Exception ex2)
                        {
                        }
                    }
                    var p2 = (from q in db.lists select q).OrderByDescending(o=>o.id).ToList();
                    for (int i = 0; i < p2.Count; i++)
                    {
                        try
                        {
                            writer.WriteStartElement("url");
                            urllink = "http://xe63.vn/Home/Detail?id="+p2[i].id;
                            writer.WriteElementString("loc", urllink);
                            //writer.WriteElementString("lastmod", DR["datetime"].ToString());
                            try
                            {
                                if (i < 500)
                                {
                                    writer.WriteElementString("changefreq", "hourly");
                                    percent = 0.85f;
                                }
                                else
                                {
                                    writer.WriteElementString("changefreq", "monthly");
                                    percent = 0.70f;
                                }
                            }
                            catch (Exception ex2)
                            {
                            }

                            writer.WriteElementString("priority", percent.ToString("0.00"));
                            writer.WriteEndElement();
                        }
                        catch (Exception ex2)
                        {
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

            }
            catch (Exception extry)
            {
                //StreamWriter sw = new StreamWriter();
            }
            return "ok";
        }
    }
}
