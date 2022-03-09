using DataAccessLayer.Concrete;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace WebProje.Controllers
{
    public class LoginController : Controller
    {
        
        /// <summary>
        /// Get isteklerinde çalışacak method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Post işlemlerine çalışacak method
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(Admin p)
        {
            
            Context c = new Context();
            var adminUserInfo = c.Admins.FirstOrDefault(x => x.AdminUsername == p.AdminUsername && x.AdminPassword == p.AdminPassword);
            if(adminUserInfo != null)
            {
                // Sisteme giriş yapıcak kişinin bilgilerinin hazırlanması
                FormsAuthentication.SetAuthCookie(adminUserInfo.AdminUsername, false);

                //Bir oturum yönetimi oluşturuluyor. Session username üzerinden olucak.
                Session["AdminUsername"] = adminUserInfo.AdminUsername; 

                return RedirectToAction("Index", "AdminInformation");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}