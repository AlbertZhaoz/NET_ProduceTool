using Albert.Commons.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Albert.Test
{
    [TestClass]
    public class CommandTest
    {
        //��ʼ��
        //[TestInitialize]
        //public void Setup()
        //{
        //    DataReceiveList = Command.DataReceiveLis;
        //}
        //ֱ��ʹ��MSTest.Unit

        [TestMethod]
        public void TestExecuteCmd()
        {
            CommandHelper.ExecuteCmd("dotnet --version");
            var str = "6.0.100";
            Assert.AreEqual(str, CommandHelper.DataReceiveList[4]);
        }
    }
}
