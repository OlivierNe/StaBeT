using System.Web.Mvc;

namespace StageBeheersTool.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString DisplayEmail(this HtmlHelper htmlHelper,
                                               string email, string name)
        {
            var mailToLink = new TagBuilder("a");
            mailToLink.Attributes.Add("href", string.Format("mailto: {0}", email));
            mailToLink.SetInnerText(name);
            return MvcHtmlString.Create(mailToLink.ToString());
        }

        public static MvcHtmlString DisplayEmail(this HtmlHelper htmlHelper, string email)
        {
            return htmlHelper.DisplayEmail(email, email);
        }
    }
}