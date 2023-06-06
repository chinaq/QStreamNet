using System.Text;
using FluentAssertions;
using QStreamNet.Core.Util;

namespace QStreamNet.Test.Core.Util
{
    [TestClass]
    public class QDataHandlerTest
    {
        private QDataHandler qDataHandler;

        public QDataHandlerTest()
        {
            qDataHandler = new QDataHandler();
        }


        #region int

        [TestMethod]
        public void HexStrToInt_Positive()
        {
            string hexStr = "00 00 01 0C ";
            int result = qDataHandler.StrHexToInt(hexStr);
            Assert.AreEqual(268, result);
        }

        [TestMethod]
        public void HexStrToInt_Negitive()
        {
            string hexStr = "F0 00 01 0C ";
            int result = qDataHandler.StrHexToInt(hexStr);
            Assert.AreEqual(-268435188, result);
        }



        ///////////////////////////
        // BytesToInt
        ///////////////////////////
        // Bytes4ToInt
        [TestMethod]
        public void Bytes4ToInt()
        {
            byte[] bytes = new byte[] { 0xF0, 0x00, 0x01, 0x0C };
            int result = qDataHandler.Bytes4ToInt(bytes, 0);
            Assert.AreEqual(-268435188, result);
        }

        [TestMethod]
        public void Bytes4ToInt_Len_Gt_4()
        {
            byte[] bytes = new byte[] { 0x00, 0xF0, 0x00, 0x01, 0x0C };
            int result = qDataHandler.Bytes4ToInt(bytes, 1);
            Assert.AreEqual(-268435188, result);
        }

        // Bytes2ToInt
        [TestMethod]
        public void Bytes2ToInt()
        {
            byte[] bytes = new byte[] { 0x11, 0xaa, 0x22, 0xbb };
            int result = qDataHandler.Bytes2ToInt(bytes, 1);
            Assert.AreEqual(43554, result);
        }

        #endregion


        #region float

        ///////////////////////////
        //Bytes4ToFloat
        ///////////////////////////
        [TestMethod]
        public void Bytes4ToFloat()
        {
            byte[] bytes = new Byte[] { 0x41, 0x8E, 0x58, 0xCA };
            float result = qDataHandler.Bytes4ToFloat(bytes, 0);
            Assert.AreEqual(17.79, result, 0.1);
        }

        #endregion


        #region Chars

        ///////////////////////////
        // BytesToStrChars
        ///////////////////////////
        [TestMethod]
        public void BytesToStrChars()
        {
            byte[] bytes = new byte[] { 0x32, 0x38, 0x4A, 0x41, 0x31, 0x34, 0x31, 0x30, 0x30, 0x30, 0x30, 0x32 };
            string result = qDataHandler.BytesToStrChars(bytes);
            Assert.AreEqual("28JA14100002", result);
        }

        [TestMethod]
        public void BytesToStrChars_non_ascii()
        {
            byte[] bytes = new byte[] { 0xff, 0x38, 0x4A, 0x41, 0x31, 0x34, 0x31, 0x30, 0x30, 0x30, 0x30, 0x32 };
            string result = qDataHandler.BytesToStrChars(bytes);
            Assert.AreEqual("?8JA14100002", result);
        }

        [TestMethod]
        public void BytesToStrChars_with_len_paras()
        {
            byte[] bytes = new byte[] { 0xff, 0x38, 0x4A, 0x41, 0x31, 0x34, 0x31, 0x30, 0x30, 0x30, 0x30, 0x32 };
            string result = qDataHandler.BytesToStrChars(bytes, 1, 2);
            Assert.AreEqual("8J", result);
        }

        [TestMethod]
        public void StrCharsToBytes(){
            string str = "8J";
            byte[] result = qDataHandler.StrCharsToBytes(str);
            byte[] expected = new byte[] { 0x38, 0x4A };
            CollectionAssert.AreEqual(expected, result);
        }


        #endregion


        #region CheckSum

        ///////////////////////////
        // CheckSum
        ///////////////////////////
        [TestMethod]
        public void CheckSum()
        {
            byte[] bytes = new byte[] { 0x00, 0x11, 0x22, 0x55 };
            byte result = qDataHandler.BytesCheckSum(bytes, 1, 2);
            Assert.AreEqual(0x33, result);
        }

        #endregion


        #region long

        ///////////////////////////
        // Bytes4ToLong
        ///////////////////////////
        [TestMethod]
        public void Bytes4ToLong()
        {
            byte[] bytes = new byte[] { 0x00, 0x11, 0xFD, 0x41 };
            long result = qDataHandler.Bytes4ToLong(bytes, 0);
            Assert.AreEqual(1178945, result);
        }

        [TestMethod]
        public void Bytes4ToLong_Len_Gt_4()
        {
            byte[] bytes = new byte[] { 0x00, 0x00, 0x11, 0xFD, 0x41 };
            long result = qDataHandler.Bytes4ToLong(bytes, 1);
            Assert.AreEqual(1178945, result);
        }


        ///////////////////////////
        // str hex to long
        ///////////////////////////
        [TestMethod]
        public void StrToLong()
        {
            string strHex = "34 19 29";
            long result = qDataHandler.StrDecToLong(strHex);
            Assert.AreEqual(341929, result);
        }


        ///////////////////////////
        // bytes bcd to long
        ///////////////////////////
        [TestMethod]
        public void BytesBcdToLong_with_len_paras()
        {
            byte[] bytes = new byte[] { 0x12, 0x23 };
            long result = qDataHandler.BytesBcdToLong(bytes, 0, 2);
            Assert.AreEqual(1223, result);
        }

        [TestMethod]
        public void BytesBcdToLong()
        {
            byte[] bytes = new byte[] { 0x12, 0x23 };
            long result = qDataHandler.BytesBcdToLong(bytes);
            Assert.AreEqual(1223, result);
        }

        #endregion


        #region string

        ///////////////////////////
        // str hex to bytes
        ///////////////////////////
        [TestMethod]
        public void StrHexToBytes()
        {
            var bytes = qDataHandler.StrHexToBytes("12 34 ab cd");
            CollectionAssert.AreEqual(new byte[] { 0x12, 0x34, 0xab, 0xcd }, bytes);
        }

        [TestMethod]
        public void StrSepTest()
        {
            var str = "2412345678";
            var result = qDataHandler.StrSep(str);
            Assert.AreEqual("24 12 34 56 78 ", result);
        }

        #endregion


        #region time

        /////////////////
        // Bytes5ToTime
        ////////////////
        [TestMethod]
        public void BytesBcd5ToTime()
        {
            var time = qDataHandler.BytesBcd5ToDateTime(new byte[] { 0x21, 0x12, 0x10, 0x09, 0x08 }, 0);
            Assert.AreEqual(DateTime.Parse("2021-12-10 09:08"), time);
        }


        [TestMethod]
        public void TimeToBytesBcd5()
        {
            var dateTimeBytes = qDataHandler.DateTimeToBytesBcd5(DateTime.Parse("2022-12-10 13:14"));
            dateTimeBytes.Should().BeEquivalentTo(new byte[] { 0x22, 0x12, 0x10, 0x13, 0x14 });
        }


        [TestMethod]
        public void BytesDec6ToDateTime()
        {
            var dateTime = qDataHandler.BytesDec6ToDateTime(new byte[] { 22, 12, 10, 13, 14, 15 }, 0);
            dateTime.Should().BeCloseTo(DateTime.Parse("2022-12-10 13:14:15"), TimeSpan.FromMilliseconds(1));
        }


        [TestMethod]
        public void DateTimeToBytesDec6()
        {
            var dateTimeBytes = qDataHandler.DateTimeToBytesDec6(DateTime.Parse("2022-12-10 13:14:15"));
            dateTimeBytes.Should().BeEquivalentTo(new byte[] { 22, 12, 10, 13, 14, 15 });
        }
        

        [TestMethod]
        public void BytesDec3ToTimeOnly()
        {
            var time = qDataHandler.BytesDec3ToTimeOnly(new byte[] { 13, 14, 15 }, 0);
            time.Should().Be(new TimeOnly(13, 14, 15));
        }


        [TestMethod]
        public void TimeOnlyToBytesDec3()
        {
            byte[] timeBytes = qDataHandler.TimeOnlyToBytesDec3(new TimeOnly(13, 14, 15));
            timeBytes.Should().BeEquivalentTo(new byte[] { 13, 14, 15 });
        }

        [TestMethod]
        public void TimeToBytes7()
        {
            byte[] timeBytes = qDataHandler.DateTimeToBytesBcd7(DateTime.Parse("2022-12-10 12:34:56"));
            timeBytes.Should().BeEquivalentTo(new byte[] {0x56, 0x34, 0x12, 0x10, 0x06, 0x12, 0x22});
        }

        #endregion


        #region crc

        [TestMethod]
        public void CrcOnArc()
        {
            var bytes = new byte[] { 0x12, 0x34, 0x56 };
            byte[] crc = qDataHandler.CrcOnArc(bytes, 0, bytes.Length);
            crc.Should().BeEquivalentTo(new byte[] { 0xfb, 0x36 });
        }

        #endregion


        #region aes

        [TestMethod]
        public void Decrypt_Temperature_Bytes()
        {
            var original = qDataHandler.StrHexToBytes("72 6d ec bf b4 b5 cc 20 30 cc de 21 27 bf 9a 88 ");
            // key
            var key_first_half = qDataHandler.StrHexToBytes("37 21 12 00 01");
            var key_second_half = Encoding.ASCII.GetBytes("SHANGHAIGAS"); 
            var key = qDataHandler.BytesCombine(key_first_half, key_second_half);
            // act
            var result = qDataHandler.DecryptBytes_Aes(original, key);
            result.Should().BeEquivalentTo(
                new byte[] { 0x00, 0x4C, 0x01, 0x03, 0x17, 0x15, 0x03, 0x56, 0x05, 0x20, 0x06, 0x34 }
            );
        }

        [TestMethod]
        public void Decrypt_Meter_Read_Bytes()
        {
            var original = qDataHandler.StrHexToBytes("dc 8a 95 f6 c0 ef 79 5b 78 ce e4 4e 58 b3 8d aa ");
            // key
            var key_first_half = qDataHandler.StrHexToBytes("37 17 09 80 03");
            var key_second_half = Encoding.ASCII.GetBytes("SHANGHAIGAS"); 
            var key = qDataHandler.BytesCombine(key_first_half, key_second_half);
            // Act
            var result = qDataHandler.DecryptBytes_Aes(original, key);
            result.Should().BeEquivalentTo(
                new byte[] { 0x00, 0x4C, 0x01, 0x00, 0x00, 0x00, 0x00 }
            );
        }


        [TestMethod]
        public void Encrypt_Meter_Read_Bytes()
        {
            var original = qDataHandler.StrHexToBytes("00 4C 01 00 00 00 00");
            // var original = qDataHandler.StrHexToBytes("dc 8a 95 f6 c0 ef 79 5b 78 ce e4 4e 58 b3 8d aa ");
            // key
            var key_first_half = qDataHandler.StrHexToBytes("37 17 09 80 03");
            var key_second_half = Encoding.ASCII.GetBytes("SHANGHAIGAS"); 
            var key = qDataHandler.BytesCombine(key_first_half, key_second_half);
            // Act
            // var result = DecryptBytes_Aes(original, key);
            var result = qDataHandler.EncryptBytes_Aes(original, key);
            result.Should().BeEquivalentTo(
                qDataHandler.StrHexToBytes("dc 8a 95 f6 c0 ef 79 5b 78 ce e4 4e 58 b3 8d aa ")
            );
        }



        [TestMethod]
        public void Decrypt_Aes_Length_32()
        {
            var origin = qDataHandler.StrHexToBytes("55 78 98 C6 31 48 89 5F 1A 81 B5 D2 08 DE BE C0 45 13 DC FD 00 B1 EE B3 F1 F8 C4 CD 18 43 FF C4");
            // key
            var key = qDataHandler.StrHexToBytes("35 20 07 17 90 53 48 41 4e 47 48 41 49 47 41 53");
            // Act
            var result = qDataHandler.DecryptBytes_Aes(origin, key);
            result.Should().BeEquivalentTo(
                qDataHandler.StrHexToBytes("00 10 4B 4B 1A 39 05 00 21 39 90 00 53 00 42 00 63 00 00")
            );
        }



        [TestMethod]
        public void Encrypt_Aes_Length_32()
        {
            var origin = qDataHandler.StrHexToBytes("00 10 4B 4B 1A 39 05 00 21 39 90 00 53 00 42 00 63 00 00");
            // key
            var key = qDataHandler.StrHexToBytes("35 20 07 17 90 53 48 41 4e 47 48 41 49 47 41 53");
            // Act
            var result = qDataHandler.EncryptBytes_Aes(origin, key);
            result.Should().BeEquivalentTo(
            qDataHandler.StrHexToBytes("55 78 98 C6 31 48 89 5F 1A 81 B5 D2 08 DE BE C0 45 13 DC FD 00 B1 EE B3 F1 F8 C4 CD 18 43 FF C4")
            );
        }

        #endregion
    }
}