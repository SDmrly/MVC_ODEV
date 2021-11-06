using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_ODEV.Models;

namespace MVC_ODEV.Controllers
{
    public class VehicleController : Controller
    {
        DB_AracKiralamaEntities db = new DB_AracKiralamaEntities();
        // GET: Vehicle
        public ActionResult Index()
        {
            return View(db.Tbl_Araclar.ToList());
        }

        public ActionResult Details(int? id)
        {
            Tbl_Araclar arac_bilgileri = db.Tbl_Araclar.Find(id);
            return View(arac_bilgileri);
        }

        [HttpGet]
        public ActionResult Rezervations(int? id)
        {
            Tbl_Araclar arac_bilgileri = db.Tbl_Araclar.Find(id);
            ViewData["Marka"] = arac_bilgileri.Marka;
            ViewData["Model"] = arac_bilgileri.Model;
            ViewData["Fiyat"] = arac_bilgileri.Fiyat;
            return View();
        }

        [HttpPost]
        public ActionResult Rezervations([Bind(Include ="Rezervasyon_Id, Arac_Id, Tc_Kimlik, Ad_Soyad, Alma_Tarihi, Teslim_Tarihi, Ucret")] Tbl_Rezervasyonlar rezervasyonlar)
        {
            if (ModelState.IsValid)
            {
                db.Tbl_Rezervasyonlar.Add(rezervasyonlar);
                db.SaveChanges();
            }
            ViewBag.Message = "Tebrikler, rezervasyon işleminiz başarıyla gerçekleşti.";
            return View();
        }
    }
}