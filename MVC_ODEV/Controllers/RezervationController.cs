using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_ODEV.Models;

namespace MVC_ODEV.Controllers
{
    public class RezervationController : Controller
    {
        DB_AracKiralamaEntities db = new DB_AracKiralamaEntities();

        // GET: Rezervation
        public ActionResult Index()
        {
            if(Session["session_giris"] != null)
            {
                string tc = Session["session_tc"].ToString();

                return View(db.Tbl_Rezervasyonlar.Where(x => x.Tc_Kimlik == tc).ToList());
            }
            else
            {
                return RedirectToAction("Giris", "Customer");
            }
            
        }

        public ActionResult Details(int? id)
        {
            Tbl_Rezervasyonlar rezervasyonlar = db.Tbl_Rezervasyonlar.Find(id);

            int arac_id = Convert.ToInt16(rezervasyonlar.Arac_Id);

            ViewData["a_tarihi"] = Convert.ToString(string.Format("{0:yyyy/MM/dd}", rezervasyonlar.Alma_Tarihi));
            ViewData["t_tarihi"] = Convert.ToString(string.Format("{0:yyyy/MM/dd}", rezervasyonlar.Teslim_Tarihi));
            ViewData["i_tarihi"] = Convert.ToString(string.Format("{0:yyyy/MM/dd}", rezervasyonlar.Iptal_Tarihi));
            ViewData["ucret"] = rezervasyonlar.Ucret;
            ViewData["iptal_mi"] = rezervasyonlar.Iptal_Mi;
            ViewData["sonuc"] = TempData["sonuc"];

            Tbl_Araclar araclar = db.Tbl_Araclar.Find(arac_id);


            return View(araclar);
        }

        public ActionResult Iptal(int? id)
        {
            Tbl_Rezervasyonlar rezervasyonlar = db.Tbl_Rezervasyonlar.Find(id);
            int rezId = rezervasyonlar.Rezervasyon_Id;
            DateTime alma = Convert.ToDateTime(rezervasyonlar.Alma_Tarihi);
            TimeSpan fark = alma - DateTime.Today;
            double days = fark.TotalDays;

            if(days> 2)
            {
                rezervasyonlar.Iptal_Mi = "Evet";
                db.Entry(rezervasyonlar).State = EntityState.Modified;
                db.SaveChanges();
                TempData["sonuc"] = "Rezervasyon başarıyla iptal edildi.";
            }
            else
            {
                TempData["sonuc"] = "İşlem başarısız. Rezervasyon iki günden az.";
            }

            return RedirectToAction("Details", new { id = rezId });

        }
    }
}