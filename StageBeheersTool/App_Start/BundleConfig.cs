using System.Web;
using System.Web.Optimization;

namespace StageBeheersTool
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.IgnoreList.Clear();
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/lib/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/lib/jquery.validate*", "~/Scripts/lib/jquery.validate.unobtrusive*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/lib/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/lib/bootstrap.js",
                      "~/Scripts/lib/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
           "~/Content/themes/base/jquery.ui.core.css",
           "~/Content/themes/base/jquery.ui.resizable.css",
           "~/Content/themes/base/jquery.ui.selectable.css",
           "~/Content/themes/base/jquery.ui.accordion.css",
           "~/Content/themes/base/jquery.ui.autocomplete.css",
           "~/Content/themes/base/jquery.ui.button.css",
           "~/Content/themes/base/jquery.ui.dialog.css",
           "~/Content/themes/base/jquery.ui.slider.css",
           "~/Content/themes/base/jquery.ui.tabs.css",
           "~/Content/themes/base/jquery.ui.datepicker.css",
           "~/Content/themes/base/jquery.ui.progressbar.css",
           "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css/datepicker").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include("~/Scripts/lib/jquery-ui-{version}.js"));
        }
    }
}
