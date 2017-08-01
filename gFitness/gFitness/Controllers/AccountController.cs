using System.Web.Mvc;
using System.Web.Security;
using gFitness.Models.ViewModel;
using gFitness.Models;
using gFitness.Models.EntityManager;
using System.Net.Mail;
using System;

namespace gFitness.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        gFitnessEntities database = new gFitnessEntities();

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                MailMessage email = new MailMessage();
                email.From = new MailAddress("a123524@outlook.com");
                email.To.Add(model.Email);
                email.Subject = "KampusShop Şifre Gönderme";
                //  eposta.Body = "mesaj içeriği";

                string username = (from u in database.Users where u.UyeEmail == model.ePosta select u.UyeKadi).FirstOrDefault();
                if (String.IsNullOrEmpty(username))
                {
                    return View();
                }
                string ID = (from u in db.Users where u.UserName == isim select u.Id).FirstOrDefault();

                UserManager.RemovePassword(ID);
                bool sifreVarmi = UserManager.HasPassword(ID);
                if (sifreVarmi == false)
                {
                    Random r = new Random();
                    int parola = r.Next(100000, 999999);
                    UserManager.AddPassword(ID, parola.ToString());
                    bool varmi = UserManager.HasPassword(ID);
                    if (varmi == true)
                    {
                        email.Body = parola.ToString();
                    }
                }

                using (var smtp = new SmtpClient())
                {
                    smtp.Credentials = new System.Net.NetworkCredential("a123524@outlook.com", "123Zx#aS");
                    smtp.Host = "smtp-mail.outlook.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(email);
                }

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(RegisterView USV)
        {
            if (ModelState.IsValid)
            {
                UserManager UM = new UserManager();
                if (!UM.IsLoginNameExist(USV.Username))
                {
                    UM.Register(USV);
                    FormsAuthentication.SetAuthCookie(USV.Name, false);
                    return RedirectToAction("Welcome", "Home");

                }
                else
                    ModelState.AddModelError("", "Login Name already taken.");
            }
            return View();
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(LoginView ULV, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                UserManager UM = new UserManager();
                string password = UM.GetUserPassword(ULV.LoginName);

                if (string.IsNullOrEmpty(password))
                    ModelState.AddModelError("", "The user login or password provided is incorrect.");
                else
                {
                    if (ULV.Password.Equals(password))
                    {
                        FormsAuthentication.SetAuthCookie(ULV.LoginName, false);
                        return RedirectToAction("Welcome", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The password provided is incorrect.");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(ULV);
        }

        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}