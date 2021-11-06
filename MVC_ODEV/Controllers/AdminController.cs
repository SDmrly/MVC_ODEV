using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_ODEV.Models;
using System.Security.Cryptography;
using System.Text;

namespace MVC_ODEV.Controllers
{
    public class AdminController : Controller
    {
        DB_AracKiralamaEntities db = new DB_AracKiralamaEntities();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public static string GetMD5_2(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }

        public ActionResult AdminKayit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminKayit([Bind(Include = "UserName, AdSoyad, Sifre")] Tbl_Admin admin)
        {
            db.Tbl_Admin.Add(admin);
            int result = db.SaveChanges();

            if (result > 0)
            {
                ViewData["sonuc"] = "Tebrikler kaydınız gerçekleşti...";
            }
            else
            {
                ViewData["sonuc"] = "HATA! Kaydınız gerçekleştirilemedi.";
            }
            return View();
        }

        public ActionResult AdminGiris()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminGiris(FormCollection giris)
        {
            string userName = giris["username"].ToString();
            string sifre = GetMD5_2(giris["sifre"].ToString());

            var count = db.Tbl_Admin.Where(x => x.UserName == userName && x.Sifre == sifre).Count();


            if (count == 0)
            {
                ViewData["sonuc"] = "HATA! Kayıtlı değilsiniz ve ya bilgileriniz yanlış.";
            }
            else
            {
                Session["session_admingiris"] = true;
                Session["session_username"] = userName;
                return RedirectToAction("AdminProfil", "Admin");
            }

            return View();
        }

        [HttpGet]
        public ActionResult AdminProfil()
        {
            if(Session["session_admingiris"] != null)
            {
                string userName = Session["session_username"].ToString();
                Tbl_Admin admin = db.Tbl_Admin.Find(userName);

                if(admin == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    return View(admin);
                }
            }
            else
            {
                return RedirectToAction("AdminGiris");
            }
        }

        public ActionResult AdminCikis()
        {
            Session.Remove("session_username");
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}