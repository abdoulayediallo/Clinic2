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
        //public Consultation GetConsultationPatient(int idConsultation)
        //{
        //    Consultation consultationPatient = new Consultation();
        //    using (var cp = new Clinic2Entities())
        //    {
        //        var obj = cp.Consultations
        //                                    .Where(consultationP => consultationP.ID_Patient == idConsultation)
        //                                    .Select(st => new {
        //                                        Motif = st.motif,
        //                                        Note = st.note,
        //                                        Poids = st.poids,
        //                                        Taille = st.taille,
        //                                        Temperature = st.temperature,
        //                                        Systol = st.systol,
        //                                        Diastol = st.diastol,
        //                                        Diagnostique = st.diagnostique,
        //                                        Maladie = st.maladie,
        //                                        Antecedent = st.antecedent,
        //                                        Statut = st.statut,
        //                                        DateCreation = st.creatieDate,
        //                                        DateChange = st.changeDate,
        //                                        CreerPar = st.createBy,
        //                                        ChangerPar = st.changeBy,
        //                                        IdConsultation = st.ID_Consultation,
        //                                        IdStaff = st.ID_Staff
        //                                    });

        //        consultationPatient.motif = obj.Select(x => x.Motif).DefaultIfEmpty("").First();
        //        consultationPatient.note = obj.Select(x => x.Note).DefaultIfEmpty("").First();
        //        consultationPatient.poids = obj.Select(x => x.Poids).DefaultIfEmpty().First();
        //        consultationPatient.taille = obj.Select(x => x.Taille).DefaultIfEmpty(0).First();
        //        consultationPatient.temperature = obj.Select(x => x.Temperature).DefaultIfEmpty(0).First();
        //        consultationPatient.systol = obj.Select(x => x.Systol).DefaultIfEmpty(0).First();
        //        consultationPatient.diastol = obj.Select(x => x.Diastol).DefaultIfEmpty(0).First();
        //        consultationPatient.diagnostique = obj.Select(x => x.Diagnostique).DefaultIfEmpty("").First();
        //        consultationPatient.maladie = obj.Select(x => x.Maladie).DefaultIfEmpty("").First();
        //        consultationPatient.antecedent = obj.Select(x => x.Antecedent).DefaultIfEmpty("").First();
        //        consultationPatient.statut = obj.Select(x => x.Statut).DefaultIfEmpty().First();
        //        consultationPatient.creatieDate = obj.Select(x => x.DateCreation).DefaultIfEmpty().First();
        //        consultationPatient.changeDate = obj.Select(x => x.DateChange).DefaultIfEmpty().First();
        //        consultationPatient.changeBy = obj.Select(x => x.CreerPar).DefaultIfEmpty().First();
        //        consultationPatient.ID_Consultation = obj.Select(x => x.IdConsultation).DefaultIfEmpty().First();
        //        consultationPatient.ID_Staff = obj.Select(x => x.IdStaff).DefaultIfEmpty().First();
        //    }
        //    return consultationPatient;
        //}

        public Adresse GetAdress(int idPatient)
        {
            Adresse adr = new Adresse();
            DateTime maxDate = new DateTime(9999, 12, 31);
            using (var cp = new Clinic2Entities())
            {
                var obj = cp.Adresses
                                            .Where(adresse => adresse.ID_Patient == idPatient && adresse.dateFin == maxDate)
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

        public ActionResult Index(string sortOrder)
        {
            //return View(db.Patients.ToList());
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NomSortParm = sortOrder == "Nom" ? "nom_desc" : "Nom";
            ViewBag.PrenomSortParm = sortOrder == "Prenom" ? "prenom_desc" : "Prenom";
            ViewBag.TelSortParm = sortOrder == "Telephone" ? "telephone_desc" : "Telephone";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.DateNaissanceSortParm = sortOrder == "DateNaissance" ? "dateNaissance_desc" : "DateNaissance";
            var patient = from s in db.Patients
                           select s;
            switch (sortOrder)
            {
                case "id_desc":
                    patient = patient.OrderByDescending(s => s.ID_Patient);
                    break;
                case "nom_desc":
                    patient = patient.OrderByDescending(s => s.nom);
                    break;
                case "Nom":
                    patient = patient.OrderBy(s => s.nom);
                    break;
                case "prenom_desc":
                    patient = patient.OrderByDescending(s => s.prenom);
                    break;
                case "Prenom":
                    patient = patient.OrderBy(s => s.prenom);
                    break;
                case "telephone_desc":
                    patient = patient.OrderByDescending(s => s.telephone);
                    break;
                case "Telephone":
                    patient = patient.OrderBy(s => s.telephone);
                    break;
                case "email_desc":
                    patient = patient.OrderByDescending(s => s.email);
                    break;
                case "Email":
                    patient = patient.OrderBy(s => s.email);
                    break;
                case "dateNaissance_desc":
                    patient = patient.OrderByDescending(s => s.date_naissance);
                    break;
                case "DateNaissance":
                    patient = patient.OrderBy(s => s.date_naissance);
                    break;
            }
            return View(patient.ToList());
        }

        public ActionResult HistoriqueAdresse(int? id)
        {
            return RedirectToAction("HistoriqueAdressePatient", "Adresses", new { id });
        }

        public ActionResult HistoriqueConsultationPatient(int? id)
        {
            return RedirectToAction("HistoriqueConsultationPatient", "Consultations", new { id });
        }

        public ActionResult DPI(int? id)
        {
            var model = new DPI();
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            model.patient = patient;
            model.consultation = db.Consultations.Where(x => x.ID_Patient == id).ToList();
            return View(model);
        }

        public ActionResult CarnetVaccin(int? id)
        {
            var consultation = new List<Consultation>();
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            consultation = db.Consultations.Where(x => x.ID_Patient == id).ToList();
            //vaccin = db.Vaccins.Where(x => x.ID_Consultation == consultation.Where(y => y.ID_Consultation));
            var vaccin = db.Vaccins.Where(v => consultation.Any(c => c.ID_Consultation == v.ID_Consultation));
            return View(vaccin);
        }

        public ActionResult PrintDPI(int id)
        {
            var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
            var report = new Rotativa.ActionAsPdf("DPI", new { id }) { FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName, Cookies = cookies };
            return report;
        }

        public ActionResult CreateConsultationPatient(int? id)
        {
            return RedirectToAction("Create", "Consultations", new { id });
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
            //patient.consultation = GetConsultationPatient(patient.ID_Patient);
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
                string pays = Request["country"].ToString() == "-1" ? null : Request["country"].ToString();
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
                    adress.dateDebut = DateTime.Now;
                    adress.dateFin = new DateTime(9999, 12, 31);
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
            //patient.consultation = GetConsultationPatient(patient.ID_Patient);
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
                string currentPays = db.Adresses.Where(x => x.ID_Patient == patient.ID_Patient).Select(x => x.pays).DefaultIfEmpty("").First();
                string currentVille = db.Adresses.Where(x => x.ID_Patient == patient.ID_Patient).Select(x => x.ville).DefaultIfEmpty("").First();
                string currentPrefecture = db.Adresses.Where(x => x.ID_Patient == patient.ID_Patient).Select(x => x.prefecture).DefaultIfEmpty("").First();
                string currentVillage = db.Adresses.Where(x => x.ID_Patient == patient.ID_Patient).Select(x => x.village).DefaultIfEmpty("").First();

                string pays = Request["country"] == "-1" ? currentPays : Request["country"].ToString();
                string ville = Request["state"] == "" ? currentVille : Request["state"].ToString();
                string prefecture = Request["adresse.prefecture"].ToString();
                string village = Request["adresse.village"].ToString();
                string dateDebut = Request["adresse.dateDebut"];
                string dateFin = Request["adresse.dateFin"];
                DateTime maxDate = new DateTime(9999, 12, 31);

                int idAdrc = db.Adresses.Where(id => id.ID_Patient == patient.ID_Patient && id.dateFin == maxDate).Select(x => x.ID_Adresse).DefaultIfEmpty(0).First();
                //int idConsultation = db.Consultations.Where(id => id.ID_Patient == patient.ID_Patient).Select(x => x.ID_Consultation).DefaultIfEmpty(0).First();

                if (idAdrc > 0)
                {
                    if (!string.IsNullOrEmpty(pays) || !string.IsNullOrEmpty(ville) || !string.IsNullOrEmpty(prefecture) || !string.IsNullOrEmpty(village))
                    {
                        if (pays != currentPays || ville != currentVille || prefecture != currentPrefecture || village != currentVillage)
                        {
                            // update old adress and set dateFin to today's date
                            db.Database.ExecuteSqlCommand("Update Adresse set dateFin='" + DateTime.Now.ToString("yyyy-MM-dd") + "'where ID_Adresse = " + idAdrc);
                            //db.Database.ExecuteSqlCommand("Update Adresse set pays='" + pays +
                            //                                            "',ville='" + ville +
                            //                                            "',prefecture='" + prefecture +
                            //                                            "',village='" + village +
                            //                                            "',dateFin='" + DateTime.Now +
                            //                                            "' where ID_Adresse =" + idAdrc);
                            //create new adress with
                            adrc.ID_Patient = patient.ID_Patient;
                            adrc.pays = pays;
                            adrc.ville = ville;
                            adrc.prefecture = prefecture;
                            adrc.village = village;
                            adrc.dateDebut = DateTime.Now;
                            adrc.dateFin = new DateTime(9999, 12, 31);
                            db.Adresses.Add(adrc);
                        }
                                            
                    }
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
                        adrc.dateDebut = DateTime.Now;
                        adrc.dateFin = new DateTime(9999, 12, 31);
                        db.Adresses.Add(adrc);
                    }

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
                patient.changeBy = name + " - ID: " + uid + " - Role:" + role;
                patient.changeDate = DateTime.Now;
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
