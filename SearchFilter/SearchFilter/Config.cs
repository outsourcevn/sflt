using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchFilter
{
    public class Config
    {
        public static string query(string from,string to,string type,int col) { 
            string query="SELECT F"+col+" as name FROM  list where 1=1 ";
            if (from!=null && from!=""){
                query+=" and F2 like N'"+from+"' ";
            }
            if (to!=null && to!=""){
                query+=" and F3 like N'"+to+"' ";
            }
            if (type!=null && type!=""){
                query+=" and F4 like N'"+type+"' ";
            }
            query+=" group by F"+col;
            return query;
        }
        public static string tags(string keyword)
        {
            string[] all = keyword.Split('-');
            string val = "";
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] != "")
                {
                    val += "<span class=\"label label-default\" style=\"cursor:pointer;\" onclick=\"removeFilter('" + all[i] + "');\">" + all[i] + " X</span>&nbsp;";
                }
            }
            return val;
        }
        public static string mobilecall(string val) { 
            string[] arr=val.Split(';');
            string rs="";
            for(int i=0;i<arr.Length;i++){
                rs += "<p><a href=\"tel:" + arr[i] + "\" class=\"btn btn-default\" style=\"background-color: #337ab7;color:yellow;padding-left:15px;padding-bottom:5px;padding-top:5px;padding-right:15px;font-size:10px;\">" + arr[i] + "</a></p>";
            }
            return rs;
        }
        public static string hourrun(string val) {
            string[] arr = val.Split(';');
            string rs = "";
            for (int i = 0; i < arr.Length; i++)
            {
                rs += arr[i]+" ";
            }
            return rs;
        }
        public static string call(string val)
        {
            string[] arr = val.Split(';');
            string rs = "";
            for (int i = 0; i < arr.Length; i++)
            {
                rs += "<p><span style=\"background-color:#337ab7;color:yellow;padding-left:15px;padding-bottom:5px;padding-top:5px;padding-right:15px;\" class=\"glyphicon glyphicon-phone\" >" + arr[i] + "</span></p>";
            }
            return rs;
        }
        public static void setCookie(string field, string value)
        {
            HttpCookie MyCookie = new HttpCookie(field);
            MyCookie.Value = HttpUtility.UrlEncode(value);
            MyCookie.Expires = DateTime.Now.AddDays(365);
            HttpContext.Current.Response.Cookies.Add(MyCookie);
            //Response.Cookies.Add(MyCookie);           
        }
        public static string getCookie(string v)
        {
            try
            {
                return HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[v].Value.ToString());
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}