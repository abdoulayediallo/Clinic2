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
    public class ConsultationsController : Controller
    {
        private Clinic2Entities db = new Clinic2Entities();

        // GET: Consultations
        public ActionResult Index()
        {
            var consultations = db.Consultations.Include(c => c.Patient).Include(c => c.Staff);
            return View(consultations.ToList());
        }

        // GET: Consultations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consultation consultation = db.Consultations.Find(id);
            if (consultation == null)
            {
                return HttpNotFound();
            }
            return View(consultation);
        }

        // GET: Consultations/Create
        public ActionResult Create()
        {
            ViewBag.ID_Patient = new SelectList(db.Patients, "ID_Patient", "createBy");
            ViewBag.ID_Staff = new SelectList(db.Staffs, "ID_Staff", "createBy");
            return View();
        }

        // POST: Consultations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Consultation,creatieDate,createBy,changeDate,changeBy,motif,note,poids,taille,temperature,systol,diastol,diagnostique,maladie,antecedent,statut,ID_Patient,ID_Staff")] Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                db.Consultations.Add(consultation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Patient = new SelectList(db.Patients, "ID_Patient", "createBy", consultation.ID_Patient);
            ViewBag.ID_Staff = new SelectList(db.Staffs, "ID_Staff", "createBy", consultation.ID_Staff);
            return View(consultation);
        }

        // GET: Consultations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consultation consultation = db.Consultations.Find(id);
            if (consultation == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Patient = new SelectList(db.Patients, "ID_Patient", "createBy", consultation.ID_Patient);
            ViewBag.ID_Staff = new SelectList(db.Staffs, "ID_Staff", "createBy", consultation.ID_Staff);
            return View(consultation);
        }

        // POST: Consultations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Consultation,creatieDate,createBy,changeDate,changeBy,motif,note,poids,taille,temperature,systol,diastol,diagnostique,maladie,antecedent,statut,ID_Patient,ID_Staff")] Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consultation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Patient = new SelectList(db.Patients, "ID_Patient", "createBy", consultation.ID_Patient);
            ViewBag.ID_Staff = new SelectList(db.Staffs, "ID_Staff", "createBy", consultation.ID_Staff);
            return View(consultation);
        }

        // GET: Consultations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consultation consultation = db.Consultations.Find(id);
            if (consultation == null)
            {
                return HttpNotFound();
            }
            return View(consultation);
        }

        // POST: Consultations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Consultation consultation = db.Consultations.Find(id);
            db.Consultations.Remove(consultation);
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
