using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Helpers
{
    public class WebRTC
    {
        public static bool WebRTCAvailable()
        {
            var browser = Shearnie.Net.Web.ServerInfo.GetBrowser.ToLower();
            if (browser.Contains("firefox"))
                return true;

            // internet explorer has a plugin available
            if (browser.Contains("internetexplorer"))
                return true;

            if (browser.Contains("chrome"))
            {
                // from: https://stackoverflow.com/questions/31870789/check-whether-browser-is-chrome-or-edge
                if (Shearnie.Net.Web.ServerInfo.GetDevice.IndexOf("Edge") > -1)
                {
                    return false;
                }

                return true;

            }

            return false;
        }
    }
}
