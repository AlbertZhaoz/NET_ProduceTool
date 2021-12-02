using Albert.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Albert.Test
{
    [TestClass]
    public class CommandTest
    {
        List<string> DataReceiveList;
        //≥ı ºªØ
        [TestInitialize]
        //public void Setup()
        //{
        //    DataReceiveList = Command.DataReceiveLis;
        //}

        [TestMethod]
        public void TestExecuteCmd()
        {
            Command.ExecuteCmd("netstat -aon|findstr \"8080\"");
            var str = " TCP    10.10.2.47:64419       14.215.158.102:8080    ESTABLISHED     37648";
            Assert.AreEqual(str, Command.DataReceiveList[0]);


        }
    }
}
