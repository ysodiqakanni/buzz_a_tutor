
using System.Web;
using System.Web.Optimization;

namespace bat
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/assets/css/main").Include(
                "~/assets/css/batmain.css"
            ));

            bundles.Add(new StyleBundle("~/assets/css/landinglogin").Include(
                "~/assets/css/login.css"
            ));
        }
    }
}
