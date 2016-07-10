using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SearchFilter.Models;
using System.ComponentModel.DataAnnotations;
using PagedList;
using Newtonsoft.Json;
using System.Data;

namespace SearchFilter.Controllers
{
    public class ListController : Controller
    {
        private xe63Entities db = new xe63Entities();
        public partial class typegroup
        {

            public string name { get; set; }
        }
        //
        // GET: /List/

        public ActionResult Index(string from, string to, string type, string f13, string f7, string f8, string f9, string f10, string f11, string f17, string order, int? pg)
        {
            if (from == null) from = ""; if (to == null) to = ""; if (type == null) type = ""; if (order == null) order = "rank";
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

            var lf7 = db.Database.SqlQuery<typegroup>(Config.query(from, to, type, 7));
            ViewBag.lf7 = lf7.ToList();
            ViewBag.f7 = f7.Replace("%20", " ");
            var lf8 = db.Database.SqlQuery<typegroup>(Config.query(from, to, type, 8));
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
            ViewBag.lf13 = lf13.ToList();
            ViewBag.f13 = f13.Replace("%20", " ");
            ViewBag.order = order;
            return View(p.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /List/Details/5

        public ActionResult Details(int id = 0)
        {
            list list = db.lists.Find(id);
            if (list == null)
            {
                return HttpNotFound();
            }
            return View(list);
        }

        //
        // GET: /List/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /List/Create

        [HttpPost]
        public ActionResult Create(list list)
        {
            if (ModelState.IsValid)
            {
                db.lists.Add(list);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(list);
        }

        //
        // GET: /List/Edit/5

        public ActionResult Edit(int id = 0)
        {
            list list = db.lists.Find(id);
            if (list == null)
            {
                return HttpNotFound();
            }
            return View(list);
        }

        //
        // POST: /List/Edit/5

        [HttpPost]
        public ActionResult Edit(list list)
        {
            if (ModelState.IsValid)
            {
                db.Entry(list).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(list);
        }

        //
        // GET: /List/Delete/5

        public ActionResult Delete(int id = 0)
        {
            list list = db.lists.Find(id);
            if (list == null)
            {
                return HttpNotFound();
            }
            return View(list);
        }

        //
        // POST: /List/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            list list = db.lists.Find(id);
            db.lists.Remove(list);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}