using System.Web;
using System.Web.Optimization;

namespace Administrator
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the developD:\Ezeelo\ezeelo\Administrator\App_Start\BundleConfig.csment version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui.js").Include(
                     "~/Content/js/jquery-migrate-1.0.0.min.js",
                     "~/Content/js/jquery-ui-1.10.0.custom.min.js",
                     "~/Content/js/jquery.ui.touch-punch.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap1.css",
                      "~/Content/css/site.css"));

            bundles.Add(new StyleBundle("~/Content/AdminLeaderLayoutcss").Include(
                      "~/Content/css/site.css",
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/bootstrap-responsive.min.css",
                      "~/Content/css/style.css",
                      "~/Content/css/style-responsive.css",
                      "~/Content/css/halflings.css",
                      //"~/Content/css/jquery-ui-1.8.21.custom.css",
                      "~/Content/css/jquery.cleditor.css"));       


        }
    }
}
