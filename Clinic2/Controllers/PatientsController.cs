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
    [Authorize]
    public class PatientsController : Controller
    {
        private Clinic2Entities db = new Clinic2Entities();

        // GET: Patients
        public Consultation GetConsultationPatient(int idConsultation)
        {
            Consultation consultationPatient = new Consultation();
            using (var cp = new Clinic2Entities())
            {
                var obj = cp.Consultations
                                            .Where(consultationP => consultationP.ID_Patient == idConsultation)
                                            .Select(st => new {
                                                Motif = st.motif,
                                                Note = st.note,
                                                Poids = st.poids,
                                                Taille = st.taille,
                                                Temperature = st.temperature,
                                                Systol = st.systol,
                                                Diastol = st.diastol,
                                                Diagnostique = st.diagnostique,
                                                Maladie = st.maladie,
                                                Antecedent = st.antecedent,
                                                Statut = st.statut,
                                                DateCreation = st.creatieDate,
                                                DateChange = st.changeDate,
                                                CreerPar = st.createBy,
                                                ChangerPar = st.changeBy,
                                                IdConsultation = st.ID_Consultation,
                                                IdStaff = st.ID_Staff
                                            });

                consultationPatient.motif = obj.Select(x => x.Motif).DefaultIfEmpty("").First();
                consultationPatient.note = obj.Select(x => x.Note).DefaultIfEmpty("").First();
                consultationPatient.poids = obj.Select(x => x.Poids).DefaultIfEmpty().First();
                consultationPatient.taille = obj.Select(x => x.Taille).DefaultIfEmpty(0).First();
                consultationPatient.temperature = obj.Select(x => x.Temperature).DefaultIfEmpty(0).First();
                consultationPatient.systol = obj.Select(x => x.Systol).DefaultIfEmpty(0).First();
                consultationPatient.diastol = obj.Select(x => x.Diastol).DefaultIfEmpty(0).First();
                consultationPatient.diagnostique = obj.Select(x => x.Diagnostique).DefaultIfEmpty("").First();
                consultationPatient.maladie = obj.Select(x => x.Maladie).DefaultIfEmpty("").First();
                consultationPatient.antecedent = obj.Select(x => x.Antecedent).DefaultIfEmpty("").First();
                consultationPatient.statut = obj.Select(x => x.Statut).DefaultIfEmpty().First();
                consultationPatient.creatieDate = obj.Select(x => x.DateCreation).DefaultIfEmpty().First();
                consultationPatient.changeDate = obj.Select(x => x.DateChange).DefaultIfEmpty().First();
                consultationPatient.changeBy = obj.Select(x => x.CreerPar).DefaultIfEmpty().First();
                consultationPatient.ID_Consultation = obj.Select(x => x.IdConsultation).DefaultIfEmpty().First();
                consultationPatient.ID_Staff = obj.Select(x => x.IdStaff).DefaultIfEmpty().First();
            }
            return consultationPatient;
        }

        public Adresse GetAdress(int idPatient)
        {
            Adresse adr = new Adresse();
            using (var cp = new Clinic2Entities())
            {
                var obj = cp.Adresses
                                            .Where(adresse => adresse.ID_Patient == idPatient)
                                            .Select(st => new {
                                                Pays = st.pays,
                                                Ville = st.ville,
                                                Prefecture = st.prefecture,
                                                Village = st.village
                                            });

                adr.pays = obj.Select(x => x.Pays).DefaultIfEmpty("").First();
                adr.ville = obj.Select(x => x.Ville).DefaultIfEmpty("").First();
                adr.prefecture = obj.Select(x => x.Prefecture).DefaultIfEmpty("").First();
                adr.village = obj.Select(x => x.Village).DefaultIfEmpty("").First();
            }
            return adr;
        }

        public Adresse GetEarliestAdressPatient(int id)
        {
            Adresse adr = new Adresse();
            using (var cp = new Clinic2Entities())
            {
                var obj = cp.Adresses.Where(adresse => adresse.ID_Patient == id).Select(st => new { });
            }                         
            return new Adresse();
        }
        public ActionResult Index()
        {
            return View(db.Patients.ToList());
        }

        // GET: Patients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            patient.adresse = GetAdress(patient.ID_Patient);
            patient.consultation = GetConsultationPatient(patient.ID_Patient);
            return View(patient);
        }

        // GET: Patients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Patient,createDate,createBy,changeDate,changeBy,nom,prenom,date_naissance,sexe,profession,situation_familial,groupe_sanguin,email,telephone,extra_info,statut")] Patient patient)
        {
            Adresse adress = new Adresse();
            Consultation cp = new Consultation();
            if (ModelState.IsValid)
            {
                patient.createDate = DateTime.Now;
                patient.groupe_sanguin = Request["groupe_sanguin"];
                //patient.createBy = User.Identity.Name;


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
                patient.createBy = name + " - ID: " + uid + " - Role:" + role;
                db.Patients.Add(patient);
                string pays = Request["country"].ToString();
                string ville = Request["state"].ToString();
                string prefecture = Request["prefecture"].ToString();
                string village = Request["village"].ToString();
                if (!string.IsNullOrEmpty(pays) || !string.IsNullOrEmpty(ville) || !string.IsNullOrEmpty(prefecture) || !string.IsNullOrEmpty(village))
                {
                    adress.ID_Patient = patient.ID_Patient;
                    adress.pays = !string.IsNullOrEmpty(pays) ? pays : "";
                    adress.ville = !string.IsNullOrEmpty(ville) ? ville : "";
                    adress.prefecture = !string.IsNullOrEmpty(prefecture) ? prefecture : "";
                    adress.village = !string.IsNullOrEmpty(village) ? village : "";
                    //adress.dateDebut = DateTime.Now;
                    //adress.dateFin
                    db.Adresses.Add(adress);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(patient);
        }

        // GET: Patients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            patient.adresse = GetAdress(patient.ID_Patient);
            patient.consultation = GetConsultationPatient(patient.ID_Patient);
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Patient,createDate,createBy,changeDate,changeBy,nom,prenom,date_naissance,sexe,profession,situation_familial,groupe_sanguin,email,telephone,extra_info,statut")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                Adresse adrc = new Adresse();
                Consultation cp = new Consultation();

                string currentPays = db.Adresses.Where(x => x.ID_Patient == patient.ID_Patient).Select(x => x.pays).DefaultIfEmpty("").First();
                string currentVille = db.Adresses.Where(x => x.ID_Patient == patient.ID_Patient).Select(x => x.ville).DefaultIfEmpty("").First();

                string pays = Request["country"] == "-1" ? currentPays : Request["country"].ToString();
                string ville = Request["state"] == "" ? currentVille : Request["state"].ToString();
                string prefecture = Request["adresse.prefecture"].ToString();
                string village = Request["adresse.village"].ToString();
                string dateDebut = Request["adresse.dateDebut"];
                string dateFin = Request["adresse.dateFin"];


                //DateTime changeDateConsultation = DateTime.Now;
                //// string changeByConsultation = ;
                ////string changeDate = Request["consultation.changeDate"].ToString();
                //string changeBy = Request["consultation.changeBy"].ToString();
                //string motif = Request["consultation.motif"].ToString();
                //string note = Request["consultation.note"].ToString();
                //Decimal poids = Decimal.Parse(Request["consultation.poids"]);
                //Decimal taille = Decimal.Parse(Request["consultation.taille"]);
                //Decimal temperature = Decimal.Parse(Request["consultation.temperature"]);
                //Decimal systol = Decimal.Parse(Request["consultation.systol"]);
                //Decimal diastol = Decimal.Parse(Request["consultation.diastol"]);
                //string diagnostique = Request["consultation.diagnostique"].ToString();
                //string maladie = Request["consultation.maladie"].ToString();
                //string antecedent = Request["consultation.antecedent"].ToString();
                //bool statut = Request["consultation.statut"] == "True" ? true : false;

                
                int idAdrc = db.Adresses.Where(id => id.ID_Patient == patient.ID_Patient).Select(x => x.ID_Adresse).DefaultIfEmpty(0).First();
                int idConsultation = db.Consultations.Where(id => id.ID_Patient == patient.ID_Patient).Select(x => x.ID_Consultation).DefaultIfEmpty(0).First();

                if (idAdrc > 0)
                {
                    if (!string.IsNullOrEmpty(pays) || !string.IsNullOrEmpty(ville) || !string.IsNullOrEmpty(prefecture) || !string.IsNullOrEmpty(village))
                    {
                        db.Database.ExecuteSqlCommand("Update Adresse set pays='" + pays +
                                                                        "',ville='" + ville +
                                                                        "',prefecture='" + prefecture +
                                                                        "',village='" + village +
                                                                        "' where ID_Adresse =" + idAdrc);
                        //db.Database.ExecuteSqlCommand("Update Adresse set pays='" + pays + "' where ID_Adresse =" + idAdrc);
                        //db.Database.ExecuteSqlCommand("Update Adresse set ville='" + ville + "' where ID_Adresse =" + idAdrc);
                        //db.Database.ExecuteSqlCommand("Update Adresse set prefecture='" + prefecture + "' where ID_Adresse =" + idAdrc);
                        //db.Database.ExecuteSqlCommand("Update Adresse set village='" + village + "' where ID_Adresse =" + idAdrc);
                    }
                    //if (!string.IsNullOrEmpty(ville))
                    //{
                    //    db.Database.ExecuteSqlCommand("Update Adresse set ville='" + ville + "' where ID_Adresse =" + idAdrc);
                    //}
                    //if (!string.IsNullOrEmpty(prefecture))
                    //{
                    //    db.Database.ExecuteSqlCommand("Update Adresse set prefecture='" + prefecture + "' where ID_Adresse =" + idAdrc);
                    //}
                    //if (!string.IsNullOrEmpty(village))
                    //{
                    //    db.Database.ExecuteSqlCommand("Update Adresse set village='" + village + "' where ID_Adresse =" + idAdrc);
                    //}
                }
                if (idAdrc == 0)
                {
                    if (!string.IsNullOrEmpty(pays) || !string.IsNullOrEmpty(ville) || !string.IsNullOrEmpty(prefecture) || !string.IsNullOrEmpty(village))
                    {
                        adrc.ID_Patient = patient.ID_Patient;
                        adrc.pays = pays;
                        adrc.ville = ville;
                        adrc.prefecture = prefecture;
                        adrc.village = village;
                        db.Adresses.Add(adrc);
                    }

                }

                //if (idConsultation > 0)
                //{
                //    if (!string.IsNullOrEmpty(motif) || !string.IsNullOrEmpty(note) || !string.IsNullOrEmpty(poids) || !string.IsNullOrEmpty(taille) || temperature. || systol) 
                //        || !string.IsNullOrEmpty(diastol) || !string.IsNullOrEmpty(diagnostique) || !string.IsNullOrEmpty(maladie) || !string.IsNullOrEmpty(antecedent) || statut == true)
                //    {
                //        db.Database.ExecuteSqlCommand("Update Consultation set motif='" + motif +
                //                                                        "',note='" + note +
                //                                                        "',poids='" + poids +
                //                                                        "',taille='" + taille +
                //                                                        "',temperature='" + temperature +
                //                                                        "',systol='" + systol +
                //                                                         "',diastol='" + diastol +
                //                                                        "',diagnostique='" + diagnostique +
                //                                                        "',maladie='" + maladie +
                //                                                        "',antecedent='" + antecedent +
                //                                                        "',statut='" + statut +
                //                                                        "' where ID_Consultation =" + idConsultation);
                //    }
                    //if (!string.IsNullOrEmpty(note))
                    //{
                    //    db.Database.ExecuteSqlCommand("Update Consultation set note='" + note + "' where ID_Consultation =" + idConsultation);
                    //}
                    //if (!string.IsNullOrEmpty(poids))
                    //{
                    //    db.Database.ExecuteSqlCommand("Update Consultation set poids='" + poids + "' where ID_Consultation =" + idConsultation);
                    //}
                    //if (!string.IsNullOrEmpty(taille))
                    //{
                    //    db.Database.ExecuteSqlCommand("Update Consultation set taille='" + taille + "' where ID_Consultation =" + idConsultation);
                    //}
                    //if (!string.IsNullOrEmpty(temperature))
                    //{
                    //    db.Database.ExecuteSqlCommand("Update Consultation set temperature='" + temperature + "' where ID_Consultation =" + idConsultation);
                    //}
                    //if (!string.IsNullOrEmpty(systol))
                    //{
                    //    db.Database.ExecuteSqlCommand("Update Consultation set systol='" + systol.ToString() + "' where ID_Consultation =" + idConsultation);
                    //}
                //}
                //if (idConsultation == 0)
                //{
                //    if (!string.IsNullOrEmpty(motif) || !string.IsNullOrEmpty(note) || !string.IsNullOrEmpty(poids) || !string.IsNullOrEmpty(taille) || !string.IsNullOrEmpty(temperature) || !string.IsNullOrEmpty(systol)
                //        || !string.IsNullOrEmpty(diastol) || !string.IsNullOrEmpty(diagnostique) || !string.IsNullOrEmpty(maladie) || !string.IsNullOrEmpty(antecedent) || !string.IsNullOrEmpty(statut))
                //    {
                //        cp.ID_Patient = patient.ID_Patient;
                //        cp.changeDate = DateTime.Now;
                //        cp.changeBy = changeBy;
                //        cp.motif = motif;
                //        cp.note = note;
                //        cp.poids = poids;
                //        cp.taille = taille;
                //        cp.temperature = temperature;
                //        cp.systol = systol;
                //        cp.diastol = diastol;
                //        cp.diagnostique = diagnostique;
                //        cp.maladie = maladie;
                //        cp.antecedent = antecedent;
                //        cp.statut = statut;

                //        db.Consultations.Add(cp);
                //    }

                //}
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            db.Patients.Remove(patient);
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
