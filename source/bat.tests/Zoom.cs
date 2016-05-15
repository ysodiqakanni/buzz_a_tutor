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
            var lessons = logic.Rules.ZoomApi.ListMeetings();
        }
    }
}
