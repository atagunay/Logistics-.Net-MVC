using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Entity.Concrete;
using FluentValidation.Results;
using BusinessLayer.ValidationRules_Fluent_Validation_;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using System.Windows.Forms;

namespace WebProje.Controllers
{
    public class AdminInformationController : Controller
    {
        /// <summary>
        /// Business katmanından manager alınır
        /// IInformationDal olarak EFInformationDal oluşturulur
        /// Index view nesnesine bağlıdır
        /// </summary>
        InformationManager cm = new InformationManager(new EFInformationDal());
        int flag = 0;

        /// <summary>
        /// Authentication yapılmadan bu sayfaya erişilemez
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {

            var informationvalues = cm.GetList();
            return View(informationvalues);

        }


        /// <summary>
        /// Authentication yapılmadan bu sayfaya erişilemez
        /// Get isteklerinde bu method çalışacak
        /// AddInformation viewına bağlıdır
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult AddInformation()
        {
            return View();
        }

        /// <summary>
        /// Post isteklerinde bu method çalışacaktır.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddInformation(Information p)
        {
            // Validasyon işlemi için nesne oluşturulur
            InformationValidation validationRules = new InformationValidation();

            // Değerler kontrol edilip bir sonuç döndürülür
            ValidationResult result = validationRules.Validate(p);

            // Eğer değerler uygunsa
            if (result.IsValid)
            {
                cm.InformationAdd(p); // Ekleme işlemi yapılır
                return RedirectToAction("Index"); // Index sayfası yeniden çağırılır
            }
            else
            {
                // Eğer değerler uygun değilse hata mesajı gösterilir
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
            }

            return View();
        }

        /// <summary>
        /// Delete operasyonu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteInformation(int id)
        {
            var information = cm.GetByID(id);
            cm.InformationDelete(information);
            return RedirectToAction("Index");

        }

        public ActionResult RefreshInformations()
        {
            // var options = new ChromeOptions { BinaryLocation = @"C:\Users\90507\AppData\Local\Programs\Opera GX\launcher.exe" };
            var driver = new ChromeDriver();
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("disable-infobars");

            string link = @"https://cargonline.samer.com/ferry/StMezzi6.idc";
            driver.Navigate().GoToUrl(link);

            while (flag == 0)
            {
                try
                {
                    driver.FindElement(By.Name("Data1"));
                    flag = 1;
                }
                catch (NoSuchElementException e)
                {
                    flag = 0;
                }
            }


            driver.FindElement(By.Name("Data1")).SendKeys("2021-01-01");
            driver.FindElement(By.Name("Data2")).SendKeys("2022-01-01");

            var transport = driver.FindElement(By.Name("transport"));
            var selectedElemet = new SelectElement(transport);
            selectedElemet.SelectByValue("OZ LIDERTRANS ULU.TAS.PET.GIDA");

            driver.FindElement(By.XPath("//input[@value = 'OK']")).Click();

            int j = -1;
            IList<IWebElement> allElement = driver.FindElements(By.XPath("//tr/td"));
         


            for (int i = 13; i < allElement.Count - 2; i++)
            {

                if (j < 1)
                {
                    Information deneme = new Information();
                    deneme.InformationDate = Convert.ToDateTime(driver.FindElement(By.XPath("(//tr/td) [" + i + "]")).Text);


                    deneme.InformationVessel = driver.FindElement(By.XPath("(//tr/td) [" + (i + 1) + "]")).Text;


                    deneme.InformationTravelCode = driver.FindElement(By.XPath("(//tr/td) [" + (i + 2) + "]")).Text;


                    deneme.InformationDeparture = driver.FindElement(By.XPath("(//tr/td) [" + (i + 3) + "]")).Text;


                    deneme.InformationArrivals = driver.FindElement(By.XPath("(//tr/td) [" + (i + 4) + "]")).Text;


                    deneme.InformationArrivalDate = Convert.ToDateTime(driver.FindElement(By.XPath("(//tr/td) [" + (i + 5) + "]")).Text);


                    deneme.InformationTruck = driver.FindElement(By.XPath("(//tr/td) [" + (i + 6) + "]")).Text;


                    deneme.InformationTrailer = driver.FindElement(By.XPath("(//tr/td) [" + (i + 7) + "]")).Text;


                    deneme.InformationDriver = driver.FindElement(By.XPath("(//tr/td) [" + (i + 8) + "]")).Text;


                   // deneme.InformationBL = (int)(driver.FindElement(By.XPath("(//tr/td) [" + (i + 9) + "]")).Text);


                    cm.InformationAdd(deneme);
                    j = 10;
                }

                j = j - 1;

            }

            MessageBox.Show("Informations Updated");
            driver.Quit();
            return RedirectToAction("Index");
        }
    }
}