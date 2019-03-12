namespace Clinic2.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    [Authorize]
    public class HomeController : Controller
    {
        #region Index method.  
        /// <summary>  
        /// Index method.   
        /// </summary>  
        /// <returns>Returns - Index view</returns>  
        public ActionResult Index()
        {
            return this.View();
        }
        #endregion
       

        public ActionResult about()
        {
            ViewBag.message = "your application description page.";

            return View();
        }

        public ActionResult contact()
        {
            ViewBag.message = "your contact page.";

            return View();
        }
    }
}