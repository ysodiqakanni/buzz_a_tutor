
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
                "~/assets/css/bootstrap.css",
                "~/assets/css/style.css",
                "~/assets/css/dark.css",
                "~/assets/css/font-icons.css",
                "~/assets/css/animate.css",
                "~/assets/css/magnific-popup.css",
                "~/assets/css/responsive.css"

            ));

            bundles.Add(new StyleBundle("~/assets/css/landinglogin").Include(
                "~/assets/css/login.css"
            ));
        }
    }
}
