using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avalonia.Platform;
using PropertyModels.ComponentModel;
using PropertyModels.Utils;
using Avalonia.PropertyGrid.Utils;
using PropertyModels.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Avalonia.PropertyGrid.Tests.Model
{
    [TestClass]
    public class UnitTest_Utils
    {
        [TestMethod]
        public void Test_PropertyDescriptorBuilder()
        {
            PropertyDescriptorBuilder builder1 = new PropertyDescriptorBuilder(new TestObject());

            var properties = builder1.GetProperties();

            Assert.IsNotNull(properties);
            Assert.AreEqual(2, properties.Count);

            PropertyDescriptorBuilder builder2 = new PropertyDescriptorBuilder(new TestObject[] { new TestObject() { ivalue = 100, str = "str0" }, new TestObject() { ivalue = 20, str = "str1" } });
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

        [TestMethod]
        public void Test_EnumUtils_ExcludeAttribute()
        {
            var enums = EnumUtils.GetEnumValues<EnumPlatformWithExclude>();

            Assert.IsNotNull(enums);
            Assert.AreEqual(3, enums.Length); // Only one should be included
            Assert.AreEqual("Linux", enums[2].DisplayName);
        }

        [TestMethod]
        public void Test_GetUniqueFlagsExcluding()
        {
            var flags = EnumFlags.OptionA | EnumFlags.OptionB | EnumFlags.OptionC;
            var uniqueFlags = flags.GetUniqueFlagsExcluding().ToArray();

            Assert.IsNotNull(uniqueFlags);
            Assert.AreEqual(2, uniqueFlags.Length); // OptionB should be excluded
            Assert.IsTrue(uniqueFlags.Contains(EnumFlags.OptionA));
            Assert.IsTrue(uniqueFlags.Contains(EnumFlags.OptionC));
        }

        [TestMethod]
        public void Test_EnumPermitValuesAttribute()
        {
            var testObject = new TestObjectWithAttributes();
            var permitField = typeof(TestObjectWithAttributes).GetField("PermittedField", BindingFlags.Instance | BindingFlags.NonPublic);
            var permitAttribute = permitField.GetCustomAttribute<EnumPermitValuesAttribute<EnumPlatform>>();

            Assert.IsNotNull(permitAttribute);
            Assert.IsTrue(permitAttribute.AllowValue(EnumPlatform.Windows));
            Assert.IsFalse(permitAttribute.AllowValue(EnumPlatform.MacOS));
        }

        [TestMethod]
        public void Test_EnumProhibitValuesAttribute()
        {
            var testObject = new TestObjectWithAttributes();
            var prohibitField = typeof(TestObjectWithAttributes).GetField("ProhibitedField", BindingFlags.Instance | BindingFlags.NonPublic);
            var prohibitAttribute = prohibitField.GetCustomAttribute<EnumProhibitValuesAttribute<EnumPlatform>>();

            Assert.IsNotNull(prohibitAttribute);
            Assert.IsFalse(prohibitAttribute.AllowValue(EnumPlatform.Windows));
            Assert.IsTrue(prohibitAttribute.AllowValue(EnumPlatform.MacOS));
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

    public enum EnumPlatformWithExclude
    {
        [EnumDisplayName("Microsoft.Windows")]
        Windows,

        [EnumDisplayName("Apple.MacOS")]
        MacOS,

        [EnumDisplayName("Linux")]
        Linux,

        [EnumExclude]
        [EnumDisplayName("Excluded.Platform")]
        ExcludedPlatform
    }

    [Flags]
    public enum EnumFlags
    {
        OptionA = 1,
        [EnumExclude]
        OptionB = 2,
        OptionC = 4
    }

    public class TestObjectWithAttributes
    {
        [EnumPermitValues<EnumPlatform>(EnumPlatform.Windows)]
        public EnumPlatform PermittedField;

        [EnumProhibitValues<EnumPlatform>(EnumPlatform.Windows)]
        public EnumPlatform ProhibitedField;
    }
}