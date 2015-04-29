using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace StageBeheersTool.Helpers
{
    public static class ViewHelpers
    {
        public static MvcHtmlString DisplayEmail(this HtmlHelper htmlHelper,
                                               string email, string name)
        {
            var mailToLink = new TagBuilder("a");
            mailToLink.Attributes.Add("href", string.Format("mailto:{0}", email));
            mailToLink.SetInnerText(name);
            return MvcHtmlString.Create(mailToLink.ToString());
        }

        public static MvcHtmlString DisplayEmail(this HtmlHelper htmlHelper, string email)
        {
            return htmlHelper.DisplayEmail(email, email);
        }

        public static string CurrentOverzicht()
        {
            return HttpContext.Current.Request.Url.PathAndQuery;
        }


        public static string GetOverzicht()
        {
            if (HttpContext.Current.Request.HttpMethod == "POST")
            {
                var referer = HttpContext.Current.Request.UrlReferrer;
                if (referer != null)
                {
                    return HttpUtility.ParseQueryString(referer.Query)["Overzicht"];
                }
            }
            var overzicht = HttpContext.Current.Request.QueryString["Overzicht"];
            return string.IsNullOrWhiteSpace(overzicht) ?
                "/" + HttpContext.Current.Request.RequestContext.RouteData.Values["controller"] + "/List" : overzicht;
        }

        public static MvcHtmlString HiddenOverzicht(this HtmlHelper helper)
        {
            var hidden = new TagBuilder("input");
            hidden.Attributes.Add("type", "hidden");
            hidden.Attributes.Add("name", "Overzicht");
            hidden.Attributes.Add("id", "Overzicht");
            hidden.Attributes.Add("value", GetOverzicht());
            return MvcHtmlString.Create(hidden.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString OverzichtActionLink(this HtmlHelper helper, string fallbackOverzicht = null)
        {
            var actionlink = new TagBuilder("a");
            actionlink.SetInnerText("Terug naar overzicht");
            var overzicht = GetOverzicht();

            if (fallbackOverzicht != null)
            {
                int van = overzicht.LastIndexOf('/');
                if (van == -1)
                {
                    overzicht = fallbackOverzicht;
                }
                else
                {
                    var action = overzicht.Substring(++van);
                    var tot = action.IndexOf('?');
                    if (tot != -1) action = action.Remove(tot);
                    if (string.IsNullOrEmpty(action)) action = "Index";
                    var controller = overzicht.Split('/')[1];
                    var controllerFullName = string.Format("StageBeheersTool.Controllers.{0}Controller", controller.FirstLetterToUpper());
                    var cont = Assembly.GetExecutingAssembly().GetType(controllerFullName);
                    var exists = cont != null && cont.GetMethod(action.FirstLetterToUpper()) != null;
                    if (exists == false)
                    {
                        overzicht = fallbackOverzicht;
                    }
                }
            }
            actionlink.Attributes.Add("href", overzicht);
            return MvcHtmlString.Create(actionlink.ToString(TagRenderMode.Normal));
        }

    }
}