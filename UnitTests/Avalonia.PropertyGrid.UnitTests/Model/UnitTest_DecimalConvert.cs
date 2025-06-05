using System.Globalization;
using PropertyModels.Utils;

namespace Avalonia.PropertyGrid.UnitTests.Model
{
    [TestClass]
    public class UnitTestDecimalConvertUtils
    {
        [TestMethod]
        public void Test_ConvertTo_Byte()
        {
            const byte value = 100;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(100m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_SByte()
        {
            const sbyte value = -50;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(-50m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_UInt16()
        {
            const ushort value = 1000;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(1000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_UInt32()
        {
            const uint value = 100000;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(100000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_UInt64()
        {
            const ulong value = 10000000000;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(10000000000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Int16()
        {
            const short value = -1000;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(-1000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Int32()
        {
            const int value = -100000;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(-100000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Int64()
        {
            const long value = -10000000000;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(-10000000000m, result);

            const long value2 = 583792581039233983;
            var result2 = DecimalConvertUtils.ConvertTo(value2);
            Assert.AreEqual(583792581039233983m, result2);
        }

        [TestMethod]
        public void Test_ConvertTo_Decimal()
        {
            const decimal value = 12345.6789m;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(12345.6789m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Double()
        {
            const double value = 12345.6789;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(12345.6789m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Single()
        {
            const float value = 12345.67f;
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(12345.67m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_NonNumeric()
        {
            const string value = "Not a number";
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(0m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Double_FrenchCulture()
        {
            const double value = 12345.6789;
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
            const double value = 12345.6789;
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
            const string value = "12345.6789";
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(12345.6789m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_String_InvalidNumber()
        {
            const string value = "Not a number";
            var result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(0m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_String_FrenchCulture()
        {
            const string value = "12345,6789"; // Using comma as decimal separator
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
            const string value = "12345,6789"; // Using comma as decimal separator
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