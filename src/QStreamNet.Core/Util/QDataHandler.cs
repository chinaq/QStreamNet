using System.Data.HashFunction.CRC;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace QStreamNet.Core.Util
{
    public class QDataHandler
    {
         public int StrHexToInt(string strHex)
        {
            return Convert.ToInt32(strHex.Replace(" ", ""), 16);
        }

        // 16 进制 string 转 long
        public long StrDecToLong(string strHex)
        {
            return long.Parse(strHex.Replace(" ", ""));
        }


        // 16 进制 字符串 转 byte 数组
        public byte[] StrHexToBytes(string hexStr)
        {
            byte[] bytes = hexStr.Trim().Split(" ".ToCharArray())
                .Select(x => byte.Parse(x, NumberStyles.HexNumber))
                .ToArray();
            return bytes;
        }

        public byte[] StrSepHexToBytes(string concatHexStr)
        {
            return StrHexToBytes(StrSep(concatHexStr));
        }

        public byte[] StrDecToBytes(string decStr)
        {
            byte[] bytes = decStr.Split(" ".ToCharArray())
                .Select(x => byte.Parse(x))
                .ToArray();
            return bytes;
        }

        public string StrSep(string str)
        {
            return Regex.Replace(str, ".{2}", "$0 ");
        }

        public byte[] StrChunkHexToBytesBcd(string hexStr)
        {
            return StrHexToBytes(StrSep(hexStr));
        }




        // 16 进制 bcd 转 long
        public long BytesBcdToLong(byte[] bytes, int start, int len)
        {
            return StrDecToLong(BytesToStrHex(bytes, start, len));
        }

        public long BytesBcdToLong(byte[] bytes)
        {
            return StrDecToLong(BytesToStrHex(bytes));
        }


        // 4 Bytes 转 int，大端在前
        public int Bytes4ToInt(byte[] bytes, int start)
        {
            byte[] newBytes = ResetBytes(bytes, start, 4);
            return BitConverter.ToInt32(newBytes, 0);
        }

        // 4 Bytes 转 uint，大端在前
        public uint Bytes4ToUInt(byte[] bytes, int start)
        {
            byte[] newBytes = ResetBytes(bytes, start, 4);
            return BitConverter.ToUInt32(newBytes, 0);
        }

        // 4 bytes 转 long，大端在前
        public long Bytes4ToLong(byte[] bytes, int start)
        {
            byte[] newBytes = ResetBytes(bytes, start, 4);
            return BitConverter.ToUInt32(newBytes, 0);
        }

        // 4 bytes 转 float, 大端在前
        public float Bytes4ToFloat(byte[] bytes, int start)
        {
            byte[] newBytes = ResetBytes(bytes, start, 4);
            return BitConverter.ToSingle(newBytes, 0);
        }

        // 2 bytes 转 int， 大端在前
        public int Bytes2ToInt(byte[] bytes, int start)
        {
            byte[] newBytes = ResetBytes(bytes, start, 2);
            return BitConverter.ToUInt16(newBytes, 0);
        }


        private byte[] ResetBytes(byte[] bytes, int start, int len)
        {
            var newBytes = CopyBytes(bytes, start, len);
            IfLittleEndianThenRevers(ref newBytes);
            return newBytes;
        }

        private byte[] CopyBytes(byte[] bytes, int start, int len)
        {
            byte[] newBytes = bytes.Skip(start).Take(len).ToArray();
            return newBytes;
        }

        private void IfLittleEndianThenRevers(ref byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
        }




        // bytes 转 char 字符串
        public string BytesToStrChars(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }



        // bytes 转 char 字符串
        public string BytesToStrChars(byte[] bytes, int start, int len)
        {
            return Encoding.ASCII.GetString(bytes, start, len);
        }



        // 校验和
        public byte CheckSum(byte[] bytes, int start, int len)
        {
            byte checkByte = 0;
            for (int i = start; i < start + len; i++)
            {
                checkByte += bytes[i];
            }
            return checkByte;
        }



        // // 子数组
        // public byte[] SubArray(byte[] bytes, int start, int len)
        // {
        //     return bytes.Skip(start).Take(len).ToArray();
        // }


        // 数组转字符串
        public string BytesToStrHex(byte[] bytes, int start, int len, string separator = " ")
        {
            string hexStr = BitConverter.ToString(bytes, start, len);
            if (separator != "-")
                hexStr = hexStr.Replace("-", separator);
            return hexStr;
        }

        public string BytesToStrHex(byte[] bytes, string separator = " ")
        {
            string hexStr = BitConverter.ToString(bytes);
            hexStr = ReplaceSeparator(hexStr, separator);
            return hexStr;
        }


        public string BytesToStrDec(byte[] bytes, int start, int len, string separator = " ")
        {
            // BitConverter.to(bytes, start, len);
            // string hexStr = BitConverter.ToString(bytes, start, len);
            var decStr = string.Join(separator, BytesSub(bytes, start, len));
            // if (separator != "-")
                // decStr = decStr.Replace("-", separator);
            return decStr;
        }

        private string ReplaceSeparator(string hexStr, string separator)
        {
            if (separator != "-")
                hexStr = hexStr.Replace("-", separator);
            return hexStr;
        }










        public byte[] BytesCombine(params byte[][] listOfByteArrays)
        {
            int len = listOfByteArrays.Sum(c => c.Length);
            byte[] result = new byte[len];

            int lastLen = 0;
            for (int i = 0; i < listOfByteArrays.Length; i++)
            {
                byte[] bytes = listOfByteArrays[i];
                Buffer.BlockCopy(bytes, 0, result, lastLen, bytes.Length);
                lastLen += bytes.Length;
            }
            return result;
        }



        public byte[] StrDecToBytes(string decStr, string separator = " ")
        {
            byte[] bytes = decStr.Split(separator.ToCharArray())
                .Select(x => byte.Parse(x))
                .ToArray();
            return bytes;
        }


        public byte BytesCheckSum(byte[] bytes, int start, int len)
        {
            byte checkByte = 0;
            for (int i = start; i < start + len; i++)
            {
                checkByte += bytes[i];
            }
            return checkByte;
        }

        public byte[] BytesSub(byte[] bytes, int start, int len)
        {
            return bytes.Skip(start).Take(len).ToArray();
        }

        public DateTime BytesBcd5ToDateTime(byte[] bytes, int start=0)
        {
            var time = BytesToStrHex(bytes, start, 5);
            string format = "yy MM dd HH mm";
            return DateTime.ParseExact(time, format, CultureInfo.InvariantCulture);
        }

        public DateTime BytesBcd6ToDateTime(byte[] bytes, int start=0)
        {
            var time = BytesToStrHex(bytes, start, 6);
            string format = "yy MM dd HH mm ss";
            return DateTime.ParseExact(time, format, CultureInfo.InvariantCulture);
        }

        public byte[] DateTimeToBytesBcd5(DateTime dateTime)
        {
            var dateStr = dateTime.ToString("yy MM dd HH mm");
            return StrHexToBytes(dateStr);
        }

        public DateTime BytesDec6ToDateTime(byte[] bytes, int start)
        {
            return new DateTime(
                2000 +  bytes[start + 0],
                        bytes[start + 1],
                        bytes[start + 2],
                        bytes[start + 3],
                        bytes[start + 4],
                        bytes[start + 5]
            );
        }

        public byte[] DateTimeToBytesBcd7(DateTime dateTime)
        {
            var dateStr = dateTime.ToString($"ss mm HH dd {(int)dateTime.DayOfWeek} MM yy");
            return StrHexToBytes(dateStr);
        }

        public byte[] DateTimeToBytesDec6(DateTime dateTime)
        {
            var dateStr = dateTime.ToString("yy MM dd HH mm ss");
            return StrDecToBytes(dateStr);
        }

        public TimeOnly BytesDec3ToTimeOnly(byte[] bytes, int start)
        {
            return new TimeOnly(
                bytes[start + 0],
                bytes[start + 1],
                bytes[start + 2]
            );
        }


        public byte[] TimeOnlyToBytesDec3(TimeOnly timeOnly)
        {
            var timeBytes = new byte[3];
            timeBytes[0] = (byte)timeOnly.Hour;
            timeBytes[1] = (byte)timeOnly.Minute;
            timeBytes[2] = (byte)timeOnly.Second;
            return timeBytes;
        }



        public byte[] CrcOnArc(byte[] bytes, int start, int len)
        {
            var data = bytes.Skip(start).Take(len).ToArray();
            var crc = CRCFactory.Instance.Create(CRCConfig.ARC);
            var crcResult = crc.ComputeHash(data).Hash; 
            Array.Reverse(crcResult);
            return crcResult;
        }


        public int GetStrEndPos(byte[] data, int startIndex)
        {
            return Array.IndexOf(data, (byte)0x00, startIndex, data.Length-startIndex);
        }


        public byte[] DecryptBytes_Aes(byte[] cryptBytes, byte[] key)
        {
            // Check arguments.
            if (cryptBytes == null || cryptBytes.Length <= 0)
                throw new ArgumentNullException("cryptBytes");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            // Create an Aes object
            using Aes aesAlg = Aes.Create();
            aesAlg.KeySize = 128;
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Key = key;
            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            // Create the streams used for decryption.
            using MemoryStream msDecrypt = new MemoryStream(cryptBytes);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using MemoryStream memstream = new MemoryStream();
            // to array
            csDecrypt.CopyTo(memstream);
            var bytes = memstream.ToArray();
            return bytes;
        }


        public byte[] EncryptBytes_Aes(byte[] oData, byte[] key)
        {
            // Check arguments.
            if (oData == null || oData.Length <= 0)
                throw new ArgumentNullException("oData");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using Aes aesAlg = Aes.Create();
            aesAlg.KeySize = 128;
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Key = key;
            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using MemoryStream msEncrypt = new MemoryStream();
            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            //Write all data to the stream.
            // csEncrypt.Write(oData, 0, oData.Length);
            csEncrypt.Write(oData);
            csEncrypt.FlushFinalBlock();
            encrypted = msEncrypt.ToArray();
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
    }
}
