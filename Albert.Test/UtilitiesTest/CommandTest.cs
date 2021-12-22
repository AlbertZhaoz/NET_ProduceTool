using Albert.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Albert.Test
{
    [TestClass]
    public class CommandTest
    {
        //初始化
        //[TestInitialize]
        //public void Setup()
        //{
        //    DataReceiveList = Command.DataReceiveLis;
        //}
        //直接使用MSTest.Unit

        [TestMethod]
        public void TestExecuteCmd()
        {
            Command.ExecuteCmd("dotnet --version");
            var str = "6.0.100";
            Assert.AreEqual(str, Command.DataReceiveList[4]);
        }
    }
}
