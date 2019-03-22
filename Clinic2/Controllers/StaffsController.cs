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
    [Authorize(Roles = "medecin")]
    public class StaffsController : Controller
    {
        private Clinic2Entities db = new Clinic2Entities();

        public Adresse GetAdress(int idStaff)
        {
            Adresse adr = new Adresse();
            DateTime maxDate = new DateTime(9999, 12, 31);
            using (var cp = new Clinic2Entities())
            {
                var obj = cp.Adresses
                                            .Where(adresse => adresse.ID_Staff == idStaff && adresse.dateFin == maxDate)
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

        // GET: Staffs
        public ActionResult Index()
        {
            return View(db.Staffs.ToList());
        }

        public ActionResult HistoriqueAdresse(int? id)
        {
            return RedirectToAction("HistoriqueAdresseStaff", "Adresses", new { id });
        }

        // GET: Staffs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            staff.adress = GetAdress(staff.ID_Staff);
            return View(staff);
        }

        // GET: Staffs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Staff,createDate,createBy,changeDate,changeBy,nom,prenom,sexe,role,phone,email,login,password,statut,debutService,finService")] Staff staff)
        {
            Adresse adress = new Adresse();
            if (ModelState.IsValid)
            {
                staff.createDate = DateTime.Now;

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
                staff.createBy = name + " - ID: " + uid + " - Role:" + role;
                
                db.Staffs.Add(staff);
                string pays = Request["country"].ToString() == "-1" ?  null : Request["country"].ToString();
                string ville = Request["state"].ToString();
                string prefecture = Request["adress.prefecture"].ToString();
                string village = Request["adress.village"].ToString();
                if (!string.IsNullOrEmpty(pays) || !string.IsNullOrEmpty(ville) || !string.IsNullOrEmpty(prefecture) || !string.IsNullOrEmpty(village))
                {
                    adress.ID_Staff = staff.ID_Staff;
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
                db.Entry(staff).GetDatabaseValues();
                int id = staff.ID_Staff;
                db.Database.ExecuteSqlCommand("Update Staff set login='" +staff.prenom + staff.ID_Staff+ "'where ID_Staff = " + staff.ID_Staff);
                return RedirectToAction("Index");
            }

            return View(staff);
        }

        // GET: Staffs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            staff.adress = GetAdress(staff.ID_Staff);
            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Staff,createDate,createBy,changeDate,changeBy,nom,prenom,sexe,role,phone,email,login,password,statut,debutService,finService")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                db.Entry(staff).State = EntityState.Modified;
                Adresse adrc = new Adresse();
                string currentPays = db.Adresses.Where(x => x.ID_Staff == staff.ID_Staff).Select(x => x.pays).DefaultIfEmpty("").First();
                string currentVille = db.Adresses.Where(x => x.ID_Staff == staff.ID_Staff).Select(x => x.ville).DefaultIfEmpty("").First();
                string currentPrefecture = db.Adresses.Where(x => x.ID_Staff == staff.ID_Staff).Select(x => x.prefecture).DefaultIfEmpty("").First();
                string currentVillage = db.Adresses.Where(x => x.ID_Staff == staff.ID_Staff).Select(x => x.village).DefaultIfEmpty("").First();

                string pays = Request["country"] == "-1" ? currentPays : Request["country"].ToString();
                string ville = Request["state"] == "" ? currentVille : Request["state"].ToString();
                string prefecture = Request["adress.prefecture"].ToString();
                string village = Request["adress.village"].ToString();
                string dateDebut = Request["adress.dateDebut"];
                string dateFin = Request["adress.dateFin"];
                DateTime maxDate = new DateTime(9999, 12, 31);

                int idAdrc = db.Adresses.Where(id => id.ID_Staff == staff.ID_Staff && id.dateFin == maxDate).Select(x => x.ID_Adresse).DefaultIfEmpty(0).First();
                //int idConsultation = db.Consultations.Where(id => id.ID_Staff == staff.ID_Staff).Select(x => x.ID_Consultation).DefaultIfEmpty(0).First();

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
                            adrc.ID_Staff = staff.ID_Staff;
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
                        adrc.ID_Staff = staff.ID_Staff;
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
                staff.changeBy = name + " - ID: " + uid + " - Role:" + role;
                staff.changeDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(staff);
        }

        // GET: Staffs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Staff staff = db.Staffs.Find(id);
            db.Staffs.Remove(staff);
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
