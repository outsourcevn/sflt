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
        public string ticket(string phone, string name, string note, string uiid, int id,string regid) {
            try
            {
                ticket tk = new ticket();
                tk.idnhaxe = id;
                tk.name = name;
                tk.name = note;
                tk.phone = phone;
                tk.uiid = uiid;
                tk.regid = regid;
                db.tickets.Add(tk);
                db.SaveChanges();
                return sendGoogle(phone, name, note,uiid, id, regid);
            }catch(Exception ex)
            { return ex.ToString(); }
        }
        public string sendGoogle(string phone, string name, string note, string uiid, int id, string regid)
        {
            try { 
                string result = "";
                string[] arrRegid = db.tickets.Where(c => c.uiid == uiid).Select(c => c.regid).ToArray();
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
                string value = phone + "," + name + "," + note + "," + uiid + "," + id;
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
            var p = (from q in db.lists select new { id=q.id,F7 = q.F7, F8 = q.F8, F13 = q.F13 }).OrderBy(o => o.F13).ThenBy(o => o.F7).ThenBy(o => o.F8);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //public string getFromTo(int type)
        //{
        //    if ()
        //}
        public string register(int id,string password,string phone,string bienso,string	from,string	to){
            try{
                driver dv = new driver();
                dv.id_course = id;
                dv.password = password;
                dv.phone = phone;
                dv.bienso = bienso;
                dv.from = from;
                dv.to = to;
                db.drivers.Add(dv);
                db.SaveChanges();
                return "1";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
        public string login(string bienso,string password)
        {
             try{
                 bool f = db.drivers.Any(o=>o.bienso==bienso && o.password==password);
                 if (f) return "1"; else return "0";
             }
             catch (Exception ex)
             {
                 return "0";
             }
        }
        public string locale(string from, string to, string type, float lon, float lat, string token, string phone,string name)
        {
            try
            {
                list_online lo = new list_online();
                lo.F2 = from;
                lo.F3 = to;
                lo.F4=type;
                lo.F13 = name;
                lo.phone_driver = phone;
                lo.lon = lon;
                lo.lat = lat;
                lo.geo = Config.CreatePoint(lat, lon);
                return "1";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
       
    }
}
