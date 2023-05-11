using System.Web;
using System.Web.Optimization;

namespace PriceSignageSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        //"~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-{version}.min.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/dataTables.buttons.min.js",
                        "~/Scripts/jszip.min.js",
                        "~/Scripts/pdfmake.min.js",
                        "~/Scripts/vfs_fonts.js",
                        "~/Scripts/buttons.html5.min.js",
                        "~/Scripts/buttons.print.min.js",
                        "~/Scripts/bootstrap-datepicker.min.js",
                        "~/Scripts/popper.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/bootstrap.min.css",
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/buttons.dataTables.min.css",
                       "~/Content/bootstrap-datepicker.min.css"
                      ));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Enable bundle optimizations in debug mode
            BundleTable.EnableOptimizations = true;


        }
    }
}
