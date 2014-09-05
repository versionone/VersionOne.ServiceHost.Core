using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VersionOne.ServiceHost.Core.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestPass()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestFail()
        {
            Assert.IsTrue(false);
        }
    }
}
