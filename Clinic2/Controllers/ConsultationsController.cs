using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Clinic2.Models;

namespace Clinic2.Controllers
{
    public class ConsultationsController : Controller
    {
        private Clinic2Entities db = new Clinic2Entities();

        public Vaccin GetVaccin(int idConsultation)
        {
            Vaccin vac = new Vaccin();
            using (var cp = new Clinic2Entities())
            {
                var obj = cp.Vaccins
                                            .Where(vaccin => vaccin.ID_Consultation == idConsultation)
                                            .Select(st => new {
                                                Description = st.description,
                                                Date = st.date
                                            });

                vac.description = obj.Select(x => x.Description).DefaultIfEmpty("").First();
                vac.date = obj.Select(x => x.Date).DefaultIfEmpty().First();
            }
            return vac;
        }

        public Ordonnance GetOrdonnance(int idConsultation)
        {
            Ordonnance ord = new Ordonnance();
            using (var cp = new Clinic2Entities())
            {
                var obj = cp.Ordonnances
                                            .Where(ordonnance => ordonnance.ID_Consultation == idConsultation)
                                            .Select(st => new {
                                                Prescription = st.prescription,
                                                Medicament = st.medicament
                                            });

                ord.prescription = obj.Select(x => x.Prescription).DefaultIfEmpty("").First();
                ord.medicament = obj.Select(x => x.Medicament).DefaultIfEmpty("").First();
            }
            return ord;
        }

        // GET: Consultations
        public ActionResult Index()
        {
            
            return View(db.Consultations.ToList());
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
        //public ActionResult Create()
        //{
        //    //ViewBag.ID_Patient = new SelectList(db.Patients, "ID_Patient", "createBy");
        //    //ViewBag.ID_Staff = new SelectList(db.Staffs, "ID_Staff", "createBy");
        //    return View();
        //}

        // GET: Consultations/Create ==> w/ reference to a patient in param
        public ActionResult Create(int? id)
        {
            //Consultation consultation = new Consultation();
            //ViewBag.ID_Patient = new SelectList(db.Patients, "ID_Patient", "createBy", idPatient) ;
            ViewBag.ID_Patient = id;
            ViewBag.Nom_Patient = db.Patients.Where(x => x.ID_Patient == id).Select(x => x.nom).DefaultIfEmpty("").First();
            ViewBag.Prenom_Patient = db.Patients.Where(x => x.ID_Patient == id).Select(x => x.prenom).DefaultIfEmpty("").First();

            //---------------------------------------------------------- https://stackoverflow.com/questions/22246538/access-claim-values-in-controller-in-mvc-5
            //Get the current claims principal
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            // Get the claims values
            var name = identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                               .Select(c => c.Value).SingleOrDefault();
            var uid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();
            //-----------------------------------------------------------------

            ViewBag.Staff_Id = uid;
            ViewBag.Staff_Nom = name;

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
                //---------------------------------------------------------- https://stackoverflow.com/questions/22246538/access-claim-values-in-controller-in-mvc-5
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                // Get the claims values
                var name = identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                                   .Select(c => c.Value).SingleOrDefault();
                var uid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                                   .Select(c => c.Value).SingleOrDefault();
                var role = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                                   .Select(c => c.Value).SingleOrDefault();
                //-----------------------------------------------------------------
                Ordonnance ordonnance = new Ordonnance();
                Vaccin vaccin = new Vaccin();
                consultation.createBy = name + " - ID: " + uid + " - Role:" + role;
                consultation.creatieDate = DateTime.Now;
                consultation.ID_Staff = Int32.Parse(uid);
                consultation.ID_Patient = Int32.Parse(Request["ID_Patient"].ToString());

                ordonnance.medicament = Request["medicament"].ToString();
                ordonnance.prescription = Request["prescription"].ToString();
                ordonnance.ID_Consultation = consultation.ID_Consultation;
                db.Ordonnances.Add(ordonnance);

                vaccin.date = DateTime.Now;
                vaccin.description = Request["vac"].ToString();
                vaccin.ID_Consultation = consultation.ID_Consultation;
                db.Vaccins.Add(vaccin);

                db.Consultations.Add(consultation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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
            consultation.ordonnance = GetOrdonnance(consultation.ID_Consultation);
            consultation.vaccin = GetVaccin(consultation.ID_Consultation);
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
                Ordonnance ordonnance = new Ordonnance();
                Vaccin vaccin = new Vaccin();

                string currentOrdPrescription = db.Ordonnances.Where(x => x.ID_Consultation == consultation.ID_Consultation).Select(x => x.prescription).DefaultIfEmpty("").First();
                string currentOrdMedicament = db.Ordonnances.Where(x => x.ID_Consultation == consultation.ID_Consultation).Select(x => x.medicament).DefaultIfEmpty("").First();

                string currentVacDescription = db.Vaccins.Where(x => x.ID_Consultation == consultation.ID_Consultation).Select(x => x.description).DefaultIfEmpty("").First();

                string ordPrescription = Request["ordonnance.prescription"];
                string ordMedicament = Request["ordonnance.medicament"];
                string vacDescription = Request["vaccin.description"];

                int idOrdonnance = db.Ordonnances.Where(x => x.ID_Consultation == consultation.ID_Consultation).Select(x => x.ID_Ordonnance).DefaultIfEmpty(0).First();
                int idVaccin = db.Vaccins.Where(x => x.ID_Consultation == consultation.ID_Consultation).Select(x => x.ID_Vaccin).DefaultIfEmpty(0).First();

                if(idOrdonnance > 0)
                {
                    if (!string.IsNullOrEmpty(ordPrescription) || !string.IsNullOrEmpty(ordMedicament))
                    {
                        db.Database.ExecuteSqlCommand("Update Ordonnance set prescription='" + ordPrescription +
                                                                    "',medicament='" + ordMedicament +
                                                                    "' where ID_Consultation =" + consultation.ID_Consultation);
                    }
                }
                else
                {
                    ordonnance.prescription = ordPrescription;
                    ordonnance.medicament = ordMedicament;
                    ordonnance.ID_Consultation = consultation.ID_Consultation;
                    db.Ordonnances.Add(ordonnance);
                }

                if (idVaccin > 0)
                {
                    if (!string.IsNullOrEmpty(vacDescription))
                    {
                        db.Database.ExecuteSqlCommand("Update Vaccin set description='" + vacDescription +
                                                                    "' where ID_Consultation =" + consultation.ID_Consultation);
                    }
                }
                else
                {
                    vaccin.description = vacDescription;
                    vaccin.ID_Consultation = consultation.ID_Consultation;
                    db.Vaccins.Add(vaccin);
                }

                //---------------------------------------------------------- https://stackoverflow.com/questions/22246538/access-claim-values-in-controller-in-mvc-5
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                // Get the claims values
                var name = identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                                   .Select(c => c.Value).SingleOrDefault();
                var uid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                                   .Select(c => c.Value).SingleOrDefault();
                var role = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                                   .Select(c => c.Value).SingleOrDefault();
                //-----------------------------------------------------------------

                consultation.changeBy = name + " - ID: " + uid + " - Role:" + role;
                consultation.changeDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
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
