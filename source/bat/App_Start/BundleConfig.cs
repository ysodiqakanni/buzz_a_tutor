
using System.Web;
using System.Web.Optimization;

namespace bat
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/assets/css/main").Include(
                "~/assets/css/batmain.css",
                "~/assets/css/batstickyfoot.css"
            ));

            bundles.Add(new StyleBundle("~/assets/css/mainyucky").Include(
                "~/assets/css/bootstrap.css",
                "~/assets/css/style.css",
                "~/assets/css/dark.css",
                "~/assets/css/font-icons.css",
                "~/assets/css/animate.css",
                "~/assets/css/magnific-popup.css",
                "~/assets/css/responsive.css"
            ));

            bundles.Add(new StyleBundle("~/assets/css/landinglogin").Include(
                "~/assets/css/login.css",
                "~/assets/css/batmain.css"
            ));

            bundles.Add(new StyleBundle("~/assets/css/literallycanvas").Include(
                "~/assets/css/literallycanvas.css"
            ));

            bundles.Add(new StyleBundle("~/assets/css/blackboard").Include(
                "~/assets/css/blackboard.css"
            ));

            bundles.Add(new StyleBundle("~/assets/css/BootSideMenu").Include(
                "~/assets/css/BootSideMenu.css"
            ));

            bundles.Add(new ScriptBundle("~/assets/js/lesson").Include(
                "~/assets/js/lessonJS.js",
                "~/assets/js/lessonResource.js"
            ));

            bundles.Add(new ScriptBundle("~/assets/js/BootSideMenu").Include(
                "~/assets/js/BootSideMenu.js"
            ));

            //bundles.Add(new ScriptBundle("~/assets/vendor/slimscroll").Include(
            //    "~/assets/vendor/jQuery-slimScroll-1.3.8/jquery.slimscroll.min.js"
            //));

            bundles.Add(new ScriptBundle("~/assets/js/literallycanvas").Include(
               "~/assets/js/react-0.14.4.js",
               "~/assets/js/react-dom-0.14.3.js",
               "~/assets/js/literallycanvas.js",
               "~/assets/js/literallyCanvasCore.js"
           ));

            bundles.Add(new ScriptBundle("~/signalr").Include(
                "~/Scripts/jquery.signalR-2.2.0.js"
            ));

            bundles.Add(new ScriptBundle("~/assets/vendor/pdf").Include(
                "~/assets/vendor/pdf.js/build/pdf.js",
                "~/assets/vendor/pdf.js/build/pdf.worker.js"
           ));
        }
    }
}
