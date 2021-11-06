using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MVC_ODEV.Models;
using System.Data.Entity;

namespace MVC_ODEV.Controllers
{
    public class CustomerController : Controller
    {
        DB_AracKiralamaEntities db = new DB_AracKiralamaEntities();
        // GET: Customer
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

            for(int i = 0; i<targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }

        public ActionResult Giris()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Giris(FormCollection bilgi)
        {
            string tc = bilgi["tc"].ToString();
            string sifre = GetMD5_2(bilgi["sifre"].ToString());

            var count = db.Tbl_Musteriler.Where(x => x.Tc_Kimlik == tc && x.Sifre == sifre).Count();

            if(count == 0)
            {
                ViewData["sonuc"] = "HATA! Kayıtlı değilsiniz ve ya bilgileriniz yanlış.";
            }
            else
            {
                Session["session_giris"] = true;
                Session["session_tc"] = tc;
                return RedirectToAction("Profil", "Customer");
            }

            return View();
        }

        public ActionResult Kayit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Kayit([Bind(Include = "Tc_Kimlik, Ad_Soyad, Dogum_Tarihi, Cinsiyet, Telefon, Sifre")] Tbl_Musteriler musteriler)
        {
            db.Tbl_Musteriler.Add(musteriler);
            int result = db.SaveChanges();
            if(result > 0)
            {
                ViewData["sonuc"] = "Tebrikler kaydınız gerçekleşti...";
            }
            else
            {
                ViewData["sonuc"] = "HATA! Kaydınız gerçekleştirilemedi.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult Profil()
        {
            if(Session["session_giris"] != null)
            {
                string tc = Session["session_tc"].ToString();
                Tbl_Musteriler musteriler = db.Tbl_Musteriler.Find(tc);

                if(musteriler == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    return View(musteriler);
                }
            }
            else
            {
                return RedirectToAction("Giris");
            }
        }

        [HttpPost]
        public ActionResult Profil(FormCollection bilgi)
        {
            if (ModelState.IsValid)
            {
                string tc = Session["session_tc"].ToString();
                Tbl_Musteriler musteriler = db.Tbl_Musteriler.Find(tc);

                musteriler.Ad_Soyad = bilgi["Ad_Soyad"].ToString();
                musteriler.Dogum_Tarihi = Convert.ToDateTime(bilgi["Dogum_Tarihi"]);
                musteriler.Cinsiyet = bilgi["Cinsiyet"].ToString();
                musteriler.Telefon = bilgi["Telefon"].ToString();

                db.Entry(musteriler).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Profil");
            }
            return View();
        }

        public ActionResult Cikis()
        {
            Session.Remove("session_tc");
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult SifreDegistir()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SifreDegistir(FormCollection bilgi)
        {
            string tc = Session["session_tc"].ToString();
            string sif = GetMD5_2(bilgi["mevcutsifre"].ToString());

            var count = db.Tbl_Musteriler.Where(x => x.Tc_Kimlik == tc && x.Sifre == sif).Count();

            if(count > 0)
            {
                if(bilgi["yenisifre"] == bilgi["yenisifre2"])
                {
                    string yenisifre = GetMD5_2(bilgi["yenisifre"].ToString());
                    Tbl_Musteriler musteriler = db.Tbl_Musteriler.Find(tc);
                    musteriler.Sifre = yenisifre;
                    db.SaveChanges();
                    ViewData["sonuc"] = "Tebrikler şifre başarıyla değiştirildi";
                }
                else
                {
                    ViewData["sonuc"] = "Girdiğiniz şifreler aynı değildir.";
                }
            }
            else
            {
                ViewData["sonuc"] = "HATA! Mevcut şifreyi yanlış yazdınız.";
            }

            return View();
        }
    }
}