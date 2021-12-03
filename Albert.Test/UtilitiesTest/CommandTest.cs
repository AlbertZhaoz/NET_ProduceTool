using Albert.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Albert.Test
{
    [TestClass]
    public class CommandTest
    {
        //≥ı ºªØ
        //[TestInitialize]
        //public void Setup()
        //{
        //    DataReceiveList = Command.DataReceiveLis;
        //}

        [TestMethod]
        public void TestExecuteCmd()
        {
            Command.ExecuteCmd("dotnet --version");
            var str = "6.0.100";
            Assert.AreEqual(str, Command.DataReceiveList[4]);
        }
    }
}
