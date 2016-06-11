using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bat.tests
{
    [TestClass]
    public class Zoom
    {
        [TestMethod]
        public void Users()
        {
            var user = logic.Rules.ZoomApi.CreateUser("Steve", "Shearn", "steve@syntronian.com", logic.Constants.Zoom.UserTypes.Basic);
            var getuser = logic.Rules.ZoomApi.GetUser("xEiSEKg6SWeQGNGEl4T8HA");
        }

        [TestMethod]
        public void ListSessions()
        {
            var meeting = logic.Rules.ZoomApi.CreateMeeting("EsgmJhBuRWWdv3nYU3KmWw", "test123");
            var meetings = logic.Rules.ZoomApi.ListMeetings("EsgmJhBuRWWdv3nYU3KmWw");
        }
    }
}
