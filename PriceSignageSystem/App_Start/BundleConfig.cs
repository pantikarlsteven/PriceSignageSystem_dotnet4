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
                        "~/Scripts/jquery-3.6.0.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                       "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                       "~/Scripts/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-dt").Include(
                       "~/Scripts/jquery.dataTables.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminlte").Include(
                       "~/Scripts/adminlte.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.min.js"
                     ));


            bundles.Add(new StyleBundle("~/bundles/bootstrap").Include(
                      "~/Content/bootstrap.min.css"
                      ));

            bundles.Add(new StyleBundle("~/bundles/jquery-dt").Include(
                      "~/Content/jquery.dataTables.min.css"
                      ));

            bundles.Add(new StyleBundle("~/bundles/jquery-ui").Include(
                     "~/Content/jquery-ui.css"
                     ));

            bundles.Add(new StyleBundle("~/bundles/bootstrap-icons").Include(
                    "~/Content/bootstrap-icons.css"
                    ));

            bundles.Add(new StyleBundle("~/bundles/fonts").Include(
                    "~/Content/fonts.css"
                    ));

            bundles.Add(new StyleBundle("~/bundles/adminlte").Include(
                    "~/Content/adminlte.min.css"
                    ));

            bundles.Add(new ScriptBundle("~/Scripts/dataTables").Include(
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/dataTables.buttons.min.js",
                        "~/Scripts/jszip.min.js",
                        "~/Scripts/pdfmake.min.js",
                        "~/Scripts/vfs_fonts.js",
                        "~/Scripts/buttons.html5.min.js",
                        "~/Scripts/buttons.print.min.js",
                        "~/Scripts/popper.min.js"
                ));
          

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/buttons.dataTables.min.css",
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/bootstrap.min.css"
                      
                      ));

            
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

        }
    }
}
