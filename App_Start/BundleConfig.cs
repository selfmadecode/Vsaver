using System.Web;
using System.Web.Optimization;

namespace VSaver.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new Bundle("~/bundles/templatejs").Include(
                      "~/Content/Template/js/scripts.js",
                      "~/Content/Template/assets/demo/chart-area-demo.js",
                      "~/Content/Template/assets/demo/chart-bar-demo.js",
                      "~/Content/Template/assets/demo/chart-pie-demo.js",
                      "~/Content/Template/assets/demo/date-range-picker-demo.js.js",
                      "~/Content/Template/js/sb-customizer.js"));

            bundles.Add(new ScriptBundle("~/bundles/dataTableJs").Include(
                "~/lib/datatables/js/jquery.dataTables.min.js"
                ));
        }
    }
}
