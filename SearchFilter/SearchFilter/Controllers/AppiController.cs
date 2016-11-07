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
using System.Net;
using System.IO;
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
            string sfrom=from;
            var p1 = (from p in db.lists
                      where (p.F2.Contains(sfrom) && p.F3.Contains(to)) || (p.F3.Contains(sfrom) && p.F2.Contains(to))
                      join q in db.drivers on p.id equals q.id_course
                     select new
                     {
                         F2 = p.F2,
                         F3 = p.F3,
                         F4 = p.F4,
                         F5 = p.F5,
                         F6 = p.F6,
                         F7 = p.F7,
                         F8 = p.F8,
                         F9 = p.F9,
                         F10 = p.F10,
                         F11 = p.F11,
                         F12 = p.F12,
                         F13 = p.F13,
                         F14 = p.F14,
                         F15 = p.F15,
                         F16 = p.F16,
                         F17 = p.F17,
                         F18=p.F18,
                         F19=p.F19,
                         rank = p.rank,
                         idtaixe = q.id,
                         last_online=q.last_online,
                     });//.Where(o => (o.F2.Contains(from) && o.F3.Contains(to)) || (o.F3.Contains(from) && o.F2.Contains(to)))
                //db.lists.Where(o => o.F2.Contains(from)).Where(o => o.F3.Contains(to)).Take(1000);
            if (type != null && type != "")
            {
                p1 = p1.Where(o => o.F4.Contains(type));
            }
            p1 = p1.OrderBy(o => o.rank);
            return JsonConvert.SerializeObject(p1.ToList());
        }
        public class glol
        {
            public string F2 { get; set; }
            public string F3 { get; set; }
            public string F4 { get; set; }
            public string F13 { get; set; }
            public DateTime datetime { get; set; }
            public string phone_driver { get; set; }
            public string bienso { get; set; }
            public string start { get; set; }
            public string end2 { get; set; }
            public double D { get; set; }
        }
        public class item
        {
            public string name { get; set; }
        }
        public string getfromto(int type) { 
            string field="F2";
            if (type == 2) field = "F3";
            var p = db.Database.SqlQuery<item>("select " + field + " as name from list group by " + field + " order by name");
            return JsonConvert.SerializeObject(p.ToList());
        }

        public string getlistonline(string from, string to, string type, double lon, double lat)
        {
            string query = "select F2,F3,F4,F13,phone_driver,bienso,start,end2,GETDATE() as datetime,ACOS(SIN(PI()*" + lat + "/180.0)*SIN(PI()*lat/180.0)+COS(PI()*" + lat + "/180.0)*COS(PI()*lat/180.0)*COS(PI()*lon/180.0-PI()*" + lon + "/180.0))*6371 As D from list_online where ";
            query += " ((F2=N'" + from + "' and F3=N'" + to + "') ";
            query += " or  (F2=N'" + to + "' and F3=N'" + from + "')) ";
            if (type != null && type != "")
            {
                query += " and (F4=N'" + type + "') ";
            }
            query += " order by d";
            var p = db.Database.SqlQuery<glol>(query);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string ticket(string phone, string name, string note, string uiid, string regid,int idtaixe) {
            try
            {
                driver taixe = db.drivers.Find(idtaixe);
                ticket tk = new ticket();
                tk.idtaixe = idtaixe;
                tk.name = name;
                tk.name = note;
                tk.phone = phone;
                tk.uiid = uiid;
                tk.regidkhach = regid;
                tk.regidtaixe = taixe.regid;
                db.tickets.Add(tk);
                db.SaveChanges();
                return sendGoogle(phone, name, note, uiid, idtaixe, regid);
            }catch(Exception ex)
            { return ex.ToString(); }
        }
        public string sendGoogle(string phone, string name, string note, string uiid, int idtaixe, string regid)
        {
            try { 
                string result = "";
                string[] arrRegid = db.tickets.Where(c => c.uiid == uiid).Select(c => c.regidtaixe).ToArray();
                //đây chính là Sender ID: (copy paste từ Google developer nhé)
                string SENDER_ID = "465163493878";
                //lấy nội dung thông báo
                //string value = value;
                WebRequest tRequest;
                //thiết lập GCM send
                tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "POST";
                tRequest.UseDefaultCredentials = true;

                tRequest.PreAuthenticate = true;

                tRequest.Credentials = CredentialCache.DefaultNetworkCredentials;

                //định dạng JSON
                tRequest.ContentType = "application/json";
                //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", "AIzaSyCYoQ0Y9X5pquWIaY12IFtbJpqaDI_plzc"));
                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

                string RegArr = string.Empty;

                RegArr = string.Join("\",\"", arrRegid);
                //Post Data có định dạng JSON như sau:
                /*
                *  { "collapse_key": "score_update",     "time_to_live": 108,       "delay_while_idle": true,
                "data": {
                "score": "223/3",
                "time": "14:13.2252"
                },
                "registration_ids":["dh4dhdfh", "dfhjj8", "gjgj", "fdhfdjgfj", "đfjdfj25", "dhdfdj38"]
                }
                 *            
                */
                string value = phone + "," + name + "," + note + "," + uiid + "," + idtaixe;
                //string postData = "{ \"registration_ids\": [ \"" + uiid + "\" ],\"data\": {\"phone\":\"" + phone + "\",\"name\":\"" + name + "\",\"note\":\"" + note + "\",\"token\":\"" + token + "\",\"uiid\":\"" + uiid + "\",\"id\":\"" + id + "\"}}";
                //string postData = "{ \"id\": [ \"" + id + "\" ],\"data\": {\"phone\": \"" + phone + "\",\"name\":\"" + name + "\"},\"note\":\"" + note + "\"},\"token\":\"" + token + "\"},\"uiid\":\"" + uiid + "\"}}";//phone	name	note	token	uiid	id(id tuyen xe nao)
                string postData = "{ \"registration_ids\": [ \"" + RegArr + "\" ],\"data\": {\"message\": \"" + value + "\",\"collapse_key\":\"" + value + "\"}}";
                //Console.WriteLine(postData);
                Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                tRequest.ContentLength = byteArray.Length;

                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tResponse = tRequest.GetResponse();

                dataStream = tResponse.GetResponseStream();

                StreamReader tReader = new StreamReader(dataStream);

                String sResponseFromServer = tReader.ReadToEnd();

                result = sResponseFromServer; //Lấy thông báo kết quả từ GCM server.
                tReader.Close();
                dataStream.Close();
                tResponse.Close();
                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string accept(int id,string uiid, string type)
        {
            try
            {
                string result = "";
                string[] arrRegid = db.tickets.Where(c => c.uiid == uiid).Select(c => c.regidkhach).ToArray();
                //đây chính là Sender ID: (copy paste từ Google developer nhé)
                string SENDER_ID = "465163493878";
                //lấy nội dung thông báo
                //string value = value;
                WebRequest tRequest;
                //thiết lập GCM send
                tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "POST";
                tRequest.UseDefaultCredentials = true;

                tRequest.PreAuthenticate = true;

                tRequest.Credentials = CredentialCache.DefaultNetworkCredentials;

                //định dạng JSON
                tRequest.ContentType = "application/json";
                //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", "AIzaSyCYoQ0Y9X5pquWIaY12IFtbJpqaDI_plzc"));
                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

                string RegArr = string.Empty;

                RegArr = string.Join("\",\"", arrRegid);
                //Post Data có định dạng JSON như sau:
                /*
                *  { "collapse_key": "score_update",     "time_to_live": 108,       "delay_while_idle": true,
                "data": {
                "score": "223/3",
                "time": "14:13.2252"
                },
                "registration_ids":["dh4dhdfh", "dfhjj8", "gjgj", "fdhfdjgfj", "đfjdfj25", "dhdfdj38"]
                }
                 *            
                */
                string value = id + "," + type;
                //string postData = "{ \"registration_ids\": [ \"" + uiid + "\" ],\"data\": {\"phone\":\"" + phone + "\",\"name\":\"" + name + "\",\"note\":\"" + note + "\",\"token\":\"" + token + "\",\"uiid\":\"" + uiid + "\",\"id\":\"" + id + "\"}}";
                //string postData = "{ \"id\": [ \"" + id + "\" ],\"data\": {\"phone\": \"" + phone + "\",\"name\":\"" + name + "\"},\"note\":\"" + note + "\"},\"token\":\"" + token + "\"},\"uiid\":\"" + uiid + "\"}}";//phone	name	note	token	uiid	id(id tuyen xe nao)
                string postData = "{ \"registration_ids\": [ \"" + RegArr + "\" ],\"data\": {\"message\": \"" + value + "\",\"collapse_key\":\"" + value + "\"}}";
                //Console.WriteLine(postData);
                Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                tRequest.ContentLength = byteArray.Length;

                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tResponse = tRequest.GetResponse();

                dataStream = tResponse.GetResponseStream();

                StreamReader tReader = new StreamReader(dataStream);

                String sResponseFromServer = tReader.ReadToEnd();

                result = sResponseFromServer; //Lấy thông báo kết quả từ GCM server.
                tReader.Close();
                dataStream.Close();
                tResponse.Close();
                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string getlistnhaxe() {
            var p = (from q in db.lists select new { id=q.id,F2=q.F2,F3=q.F3,F7 = q.F7, F8 = q.F8, F13 = q.F13,F4=q.F4 }).OrderBy(o => o.F13).ThenBy(o => o.F7).ThenBy(o => o.F8);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //public string getFromTo(int type)
        //{
        //    if ()
        //}
        public string register(int id,string password,string phone,string bienso,string	from,string	to,string regid){
            try{
                driver dv = new driver();
                dv.id_course = id;
                dv.password = password;
                dv.phone = phone;
                dv.bienso = bienso;
                dv.from = from;
                dv.to = to;
                dv.regid = regid;
                db.drivers.Add(dv);
                db.SaveChanges();
                return dv.id.ToString();
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
        public string newnhaxe(string f2,string f3,string f4,string f7,string f8,string f9,string f10,string f11,string f13,string f14,int f17,string f18) {
            try {
                if (f2 == null || f2 == "" || f3 == null || f3 == "" || f4 == null || f4 == "" || f7 == null || f7 == "" || f8 == null || f8 == "" || f9 == null || f9 == "" || f10 == null || f10 == "") return "0";
                if (f11 == null || f11 == "" || f13 == null || f13 == "" || f14 == null || f14 == "" || f17 == null  || f18 == null || f18 == "") return "0";
                int maxid = db.lists.Max(o => o.id)+1;
                list l=new list();
                l.F2=f2;
                l.F3=f3;
                l.F4=f4;
                l.F7=f7;
                l.F8=f8;
                l.F9=f9;
                l.F10=f10;
                l.F11=f11;
                l.F13=f13;
                l.F14=f14;
                l.F17=f17;
                l.F18=f18;
                l.rank=maxid+1;
                db.lists.Add(l);
                db.SaveChanges();
                return "1";
            }catch(Exception ex){
                return "0";
            }
        }
        public partial class typegroup
        {

            public string name { get; set; }
        }
        public string getauto(string field)
        {
            try
            {
                string query = "select distinct " + field + " as name from list group by " + field + " order by " + field;
                var p = db.Database.SqlQuery<typegroup>(query);
                return JsonConvert.SerializeObject(p.ToList());
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string login(string bienso,string password,string regid)
        {
             try{
                 var f = db.drivers.Where(o=>o.bienso==bienso && o.password==password && o.regid==regid);
                 if (f!=null) return JsonConvert.SerializeObject(f.ToList()); else return "0";
             }
             catch (Exception ex)
             {
                 return "0";
             }
        }
        public string locate(string from, string to, string type, float lon, float lat, string phone,string name,string bienso,string start,string end,int idtaixe)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from list_online where F2=N'"+from+"' and F3=N'"+to+"' and F4=N'"+type+"' and phone_driver=N'"+phone+"'");
                db.Database.ExecuteSqlCommand("update driver set last_online=getdate() where id=" + idtaixe);
                list_online lo = new list_online();
                lo.F2 = from;
                lo.F3 = to;
                lo.F4=type;
                lo.F13 = name;
                lo.date_time = DateTime.Now;
                lo.phone_driver = phone;
                lo.lon = lon;
                lo.lat = lat;
                lo.geo = Config.CreatePoint(lat, lon);
                lo.bienso = bienso;
                lo.start = start;
                lo.end2 = end;
                db.list_online.Add(lo);
                db.SaveChanges();
                return "1";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
       
    }
}
