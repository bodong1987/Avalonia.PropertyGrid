using PropertyModels.Utils;
using System.Globalization;

namespace Avalonia.PropertyGrid.UnitTests.Model
{
    [TestClass]
    public class UnitTest_DecimalConvertUtils
    {
        [TestMethod]
        public void Test_ConvertTo_Byte()
        {
            byte value = 100;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(100m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_SByte()
        {
            sbyte value = -50;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(-50m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_UInt16()
        {
            ushort value = 1000;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(1000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_UInt32()
        {
            uint value = 100000;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(100000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_UInt64()
        {
            ulong value = 10000000000;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(10000000000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Int16()
        {
            short value = -1000;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(-1000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Int32()
        {
            var value = -100000;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(-100000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Int64()
        {
            var value = -10000000000;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(-10000000000m, result);

            var value2 = 583792581039233983;
            var result2 = DecimalConvertUtils.ConvertTo(value2);
            Assert.AreEqual(583792581039233983m, result2);
        }

        [TestMethod]
        public void Test_ConvertTo_Decimal()
        {
            var value = 12345.6789m;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(12345.6789m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Double()
        {
            var value = 12345.6789;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(12345.6789m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Single()
        {
            var value = 12345.67f;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(12345.67m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_NonNumeric()
        {
            var value = "Not a number";
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(0m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Double_FrenchCulture()
        {
            var value = 12345.6789;
            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
                var result = DecimalConvertUtils.ConvertTo(value);
                Assert.AreEqual(12345.6789m, result);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

        [TestMethod]
        public void Test_ConvertTo_Double_GermanCulture()
        {
            var value = 12345.6789;
            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                var result = DecimalConvertUtils.ConvertTo(value);
                Assert.AreEqual(12345.6789m, result);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

        [TestMethod]
        public void Test_ConvertTo_String_ValidNumber()
        {
            var value = "12345.6789";
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(12345.6789m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_String_InvalidNumber()
        {
            var value = "Not a number";
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(0m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_String_FrenchCulture()
        {
            var value = "12345,6789"; // Using comma as decimal separator
            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
                var result = DecimalConvertUtils.ConvertTo(value);
                Assert.AreEqual(12345.6789m, result);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

        [TestMethod]
        public void Test_ConvertTo_String_GermanCulture()
        {
            var value = "12345,6789"; // Using comma as decimal separator
            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                var result = DecimalConvertUtils.ConvertTo(value);
                Assert.AreEqual(12345.6789m, result);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }
    }
}