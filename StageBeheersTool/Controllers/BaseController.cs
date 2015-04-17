using System.Web;
using System.Web.Mvc;

namespace StageBeheersTool.Controllers
{
    public abstract class BaseController : Controller
    {
        protected void SetViewError(string error)
        {
            TempData["error"] = error;
        }

        protected void SetViewMessage(string message)
        {
            TempData["message"] = message;
        }

        protected ActionResult RedirectToLocal(string url)
        {
            if (Url.IsLocalUrl(url))
            {
                return Redirect(url);
            }
            return RedirectToAction("Index", "Home");
        }

        protected string CurrentOverzicht
        {
            get { return Request.Url.PathAndQuery; }
        }

        protected string Overzicht
        {
            get
            {
                if (Request.HttpMethod == "POST")
                {
                    var referer = System.Web.HttpContext.Current.Request.UrlReferrer;
                    if (referer != null)
                    {
                        return HttpUtility.ParseQueryString(referer.Query)["Overzicht"];
                    }
                }
                var overzicht = System.Web.HttpContext.Current.Request.QueryString["Overzicht"];
                return string.IsNullOrWhiteSpace(overzicht)
                    ? "/" + Request.RequestContext.RouteData.Values["controller"] + "/List"
                    : overzicht;
            }
        }
    }
}