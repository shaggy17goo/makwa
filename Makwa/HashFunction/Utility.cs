using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace HashFunction
{
    public static class Utility
    {
        public static byte[] CustomByteArray(byte custombyte, int length)
        {
            byte[] customArray = new byte[length];
            for (int i = 0; i < customArray.Length; i++)
            {
                customArray[i] = custombyte;
            }
            return customArray;
        }

        public static byte[] ConcatenateByteArrays(params byte[][] arrays)
        {
            int outLength = 0;
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                outLength += array.Length;
            }
            byte[] output = new byte[outLength];
            foreach (byte[] data in arrays)
            {
                Buffer.BlockCopy(data, 0, output, offset, data.Length);
                offset += data.Length;
            }
            return output;
        }

        public static BigInteger ByteArrayToBigInteger(byte[] data)
        {
            BigInteger bi = 0;
            for (int i = 1; i <= data.Length; i++)
                bi += BigInteger.Pow(256, i - 1) * data[data.Length - i];
            return bi;
        }

        public static byte[] BigIntegerToByteArray(BigInteger bigInt)
        {
            int byteSize = bigInt.GetByteCount();
            byte[] tabIn = bigInt.ToByteArray();
            Stack<byte> stack = new Stack<byte>();
            for (int i = 0; i < tabIn.Length; i++)
            {
                stack.Push(tabIn[i]);
            }

            //remove 0x00 byte from left site
            while (stack.Peek() == 0)
            {
                stack.Pop();
                byteSize--;
            }

            byte[] tabOut = new byte[byteSize];
            for (int i = 0; i < byteSize; i++)
            {
                tabOut[i] = stack.Pop();
            }
            return tabOut;
        }
        
        public static byte[] HexStringToByteArray(string hexString)
        {
            byte[] retval = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
                retval[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return retval;
        }
        
        
        public static string BigIntegerToHexString(BigInteger bigInt)
        {
            string bigIntAsHex_ = BitConverter.ToString(BigIntegerToByteArray(bigInt));
            string bigIntAsHex = "";
            for (int i = 0; i < bigIntAsHex_.Length; i += 3)
            {
                bigIntAsHex += bigIntAsHex_.Substring(i, 2);
            }
            return bigIntAsHex;
        }
        
        public static string ReadFirstLineFromFile(string path)
        {
            string line = "";
            using (StreamReader reader = new StreamReader(path))  
            {  
                line = reader.ReadLine();
            }
            return line;
        }
        
    }
}