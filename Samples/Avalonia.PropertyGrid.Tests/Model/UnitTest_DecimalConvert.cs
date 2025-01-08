using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avalonia.PropertyGrid.Utils;

namespace Avalonia.PropertyGrid.Tests.Model
{
    [TestClass]
    public class UnitTest_DecimalConvertUtils
    {
        [TestMethod]
        public void Test_ConvertTo_Byte()
        {
            byte value = 100;
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(100m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_SByte()
        {
            sbyte value = -50;
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(-50m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_UInt16()
        {
            ushort value = 1000;
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(1000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_UInt32()
        {
            uint value = 100000;
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(100000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_UInt64()
        {
            ulong value = 10000000000;
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(10000000000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Int16()
        {
            short value = -1000;
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(-1000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Int32()
        {
            int value = -100000;
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(-100000m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Int64()
        {
            long value = -10000000000;
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(-10000000000m, result);

            long value2 = 583792581039233983;
            decimal result2 = DecimalConvertUtils.ConvertTo(value2);
            Assert.AreEqual(583792581039233983m, result2);
        }

        [TestMethod]
        public void Test_ConvertTo_Decimal()
        {
            decimal value = 12345.6789m;
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(12345.6789m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Double()
        {
            double value = 12345.6789;
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(12345.6789m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Single()
        {
            float value = 12345.67f;
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(12345.67m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_NonNumeric()
        {
            string value = "Not a number";
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(0m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_Double_FrenchCulture()
        {
            double value = 12345.6789;
            CultureInfo originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
                decimal result = DecimalConvertUtils.ConvertTo(value);
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
            double value = 12345.6789;
            CultureInfo originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                decimal result = DecimalConvertUtils.ConvertTo(value);
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
            string value = "12345.6789";
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(12345.6789m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_String_InvalidNumber()
        {
            string value = "Not a number";
            decimal result = DecimalConvertUtils.ConvertTo(value);
            Assert.AreEqual(0m, result);
        }

        [TestMethod]
        public void Test_ConvertTo_String_FrenchCulture()
        {
            string value = "12345,6789"; // Using comma as decimal separator
            CultureInfo originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
                decimal result = DecimalConvertUtils.ConvertTo(value);
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
            string value = "12345,6789"; // Using comma as decimal separator
            CultureInfo originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                decimal result = DecimalConvertUtils.ConvertTo(value);
                Assert.AreEqual(12345.6789m, result);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }
    }
}