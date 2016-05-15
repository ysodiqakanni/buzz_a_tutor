using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bat.tests
{
    [TestClass]
    public class Zoom
    {
        [TestMethod]
        public void ListSessions()
        {
            //var meeting = logic.Rules.ZoomApi.CreateMeeting("test123");
            var meetings = logic.Rules.ZoomApi.ListMeetings();
        }
    }
}
