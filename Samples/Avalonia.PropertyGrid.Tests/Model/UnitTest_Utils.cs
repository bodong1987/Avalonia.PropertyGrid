using Avalonia.Platform;
using PropertyModels.ComponentModel;
using PropertyModels.Utils;
using Avalonia.PropertyGrid.Utils;
// ReSharper disable InconsistentNaming

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

        [TestMethod]
        public void Test_EnumUtils()
        {
            var enums = EnumUtils.GetEnumValues<EnumPlatform>();

            Assert.IsNotNull(enums);
            Assert.AreEqual(2, enums.Length);
        }
        
        [TestMethod]
        public void Test_EnumValueWrapper_DisplayName()
        {
            var wrapper1 = new EnumValueWrapper(EnumPlatform.Windows);
            Assert.AreEqual("Microsoft.Windows", wrapper1.DisplayName);

            var wrapper2 = new EnumValueWrapper(EnumPlatform.MacOS);
            Assert.AreEqual("Apple.MacOS", wrapper2.DisplayName);
        }

        [TestMethod]
        public void Test_EnumValueWrapper_UndefinedValue()
        {
            // Create an enum value that is not defined in EnumPlatform
            EnumPlatform undefinedValue = (EnumPlatform)999;
            var wrapper = new EnumValueWrapper(undefinedValue);

            // The display name should fall back to the value's ToString()
            Assert.AreEqual("999", wrapper.DisplayName);
        }
    }

    public enum EnumPlatform
    {
        [EnumDisplayName("Microsoft.Windows")]
        Windows,

        [EnumDisplayName("Apple.MacOS")]
        MacOS
    }


    public class TestObject
    {
        public string str { get; set; }
        // ReSharper disable once IdentifierTypo
        public int ivalue { get; set; }
    }



}