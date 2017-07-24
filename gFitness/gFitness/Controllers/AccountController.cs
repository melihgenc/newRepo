using System.Web.Mvc;
using System.Web.Security;
using gFitness.Models.ViewModel;
using gFitness.Models.EntityManager;

namespace gFitness.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(Register USV)
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
        public ActionResult LogIn(UserLoginView ULV, string returnUrl)
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