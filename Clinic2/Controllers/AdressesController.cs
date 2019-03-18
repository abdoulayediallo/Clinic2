using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clinic2.Models;

namespace Clinic2.Controllers
{
    [Authorize]
    public class AdressesController : Controller
    {
        private Clinic2Entities db = new Clinic2Entities();

        // GET: Adresses
        public ActionResult Index()
        {
            var adresses = db.Adresses.Include(a => a.Patient).Include(a => a.Staff);
            return View(adresses.ToList());
        }

        public ActionResult HistoriqueAdressePatient(int? id)
        {
            return View(db.Adresses.Where(x => x.ID_Patient == id).ToList().OrderByDescending(x => x.dateFin));
        }

        public ActionResult HistoriqueAdresseStaff(int? id)
        {
            return View(db.Adresses.Where(x => x.ID_Staff == id).ToList().OrderByDescending(x => x.dateFin));
        }
        // GET: Adresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adresse adresse = db.Adresses.Find(id);
            if (adresse == null)
            {
                return HttpNotFound();
            }
            return View(adresse);
        }

        // GET: Adresses/Create
        public ActionResult Create()
        {
            ViewBag.ID_Patient = new SelectList(db.Patients, "ID_Patient", "createBy");
            ViewBag.ID_Staff = new SelectList(db.Staffs, "ID_Staff", "createBy");
            return View();
        }

        // POST: Adresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Adresse,pays,ville,prefecture,village,dateDebut,dateFin,ID_Patient,ID_Staff")] Adresse adresse)
        {
            if (ModelState.IsValid)
            {
                db.Adresses.Add(adresse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Patient = new SelectList(db.Patients, "ID_Patient", "createBy", adresse.ID_Patient);
            ViewBag.ID_Staff = new SelectList(db.Staffs, "ID_Staff", "createBy", adresse.ID_Staff);
            return View(adresse);
        }

        // GET: Adresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adresse adresse = db.Adresses.Find(id);
            if (adresse == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Patient = new SelectList(db.Patients, "ID_Patient", "createBy", adresse.ID_Patient);
            ViewBag.ID_Staff = new SelectList(db.Staffs, "ID_Staff", "createBy", adresse.ID_Staff);
            return View(adresse);
        }

        // POST: Adresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Adresse,pays,ville,prefecture,village,dateDebut,dateFin,ID_Patient,ID_Staff")] Adresse adresse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adresse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Patient = new SelectList(db.Patients, "ID_Patient", "createBy", adresse.ID_Patient);
            ViewBag.ID_Staff = new SelectList(db.Staffs, "ID_Staff", "createBy", adresse.ID_Staff);
            return View(adresse);
        }

        // GET: Adresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adresse adresse = db.Adresses.Find(id);
            if (adresse == null)
            {
                return HttpNotFound();
            }
            return View(adresse);
        }

        // POST: Adresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Adresse adresse = db.Adresses.Find(id);
            db.Adresses.Remove(adresse);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
