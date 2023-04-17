using Avalonia.PropertyGrid.Model.Utils;
using System.Reflection.Emit;

namespace Avalonia.PropertyGrid.Tests.Model
{
    [TestClass]
    public class UnitTest_Utils
    {
        [TestMethod]
        public void Test_PropertyDescriptorBuilder()
        {
            PropertyDescriptorBuilder builder1 = new PropertyDescriptorBuilder(new TestObject());

            var properties =  builder1.GetProperties();

            Assert.IsNotNull(properties);
            Assert.AreEqual(2, properties.Count);

            PropertyDescriptorBuilder builder2 = new PropertyDescriptorBuilder(new TestObject[] {new TestObject() { ivalue=100, str="str0"}, new TestObject() { ivalue=20, str="str1"} });
            var properties2 = builder2.GetProperties();

            Assert.IsNotNull(properties2);
            Assert.AreEqual(2, properties2.Count);
        }
    }

    public class TestObject
    {
        public string str { get; set; }
        public int ivalue { get; set; }
    }



}