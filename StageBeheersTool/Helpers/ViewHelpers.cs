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

        public static MvcHtmlString OverzichtActionLink(this HtmlHelper helper)
        {
            var actionlink = new TagBuilder("a");
            actionlink.SetInnerText("Terug naar overzicht");
            actionlink.Attributes.Add("href", HttpUtility.UrlDecode(GetOverzicht()));
            return MvcHtmlString.Create(actionlink.ToString(TagRenderMode.Normal));
        }

    }
}