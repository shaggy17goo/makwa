using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;


namespace HashFunction
{
    class Makwa
    {
        private bool pre_hashing;
        private bool post_hashing;
        private int m_cost;
        private int post_hashing_length;
        private BigInteger modulus;
        private byte[] modulusBytes;
        private byte[] saltBytes;
        private readonly byte[] hexzero = { 0x00 };
        private readonly byte[] hexone = { 0x01 };
        private  static HMAC HashMAC { get; } = new HMACSHA256();

        
        /// <summary>
        /// Makwa constructor
        /// </summary>
        /// <param name="preHashing">prehashing</param>
        /// <param name="postHashing">posthasing</param>
        /// <param name="mCost">work factory, squareMod times</param>
        /// <param name="postHashLength">posthashing Length</param>
        /// <param name="modulusPath">path to file with modulus(HEX)</param>
        public Makwa(bool preHashing, bool postHashing, int mCost, int postHashLength, string modulusPath)
        {
            pre_hashing = preHashing;
            post_hashing = postHashing;
            m_cost = mCost;
            post_hashing_length = postHashLength;
            modulusBytes = Utility.HexStringToByteArray(Utility.ReadFirstLineFromFile(modulusPath));
            modulus = Utility.ByteArrayToBigInteger(modulusBytes);
            if (!CheckInitialParams())
            {
                throw new Exception("Invalid initial params");
            }
        }


        /// <summary>
        /// Create password hash
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <param name="salt">salt</param>
        /// <returns>byteArray with hash byte</returns>
        public byte[] CreateHash(string password, string salt)
        {
            byte[] passBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            return CreateHash(passBytes, saltBytes);
        }
        
        /// <summary>
        /// Create password hash
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <param name="salt">salt</param>
        /// <returns>byteArray with hash byte</returns>
        public byte[] CreateHash(string password, byte[] salt)
        {
            byte[] passBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = salt;
            return CreateHash(passBytes, saltBytes);
        }
        
        
        /// <summary>
        /// Create password hash
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <param name="salt">salt</param>
        /// <returns>byteArray with hash byte</returns>
        public byte[] CreateHash(byte[] password, byte[] salt)
        {
            byte[] passBytes = password;
            byte[] saltBytes = salt;
            int u = passBytes.Length;
            int k = modulusBytes.Length;

            if (pre_hashing)
                passBytes = KDF(passBytes, 64);

            if (u > 255 || u > (k - 32))
                throw new Exception("invalid password length");

            byte[] uByte = {(byte)u};

            byte[] sb = KDF(Utility.ConcatenateByteArrays(saltBytes, passBytes, uByte), k - 2 - u);
            byte[] xb = Utility.ConcatenateByteArrays(hexzero, sb, passBytes, uByte);
            BigInteger x = Utility.ByteArrayToBigInteger(xb);
            for (int i = 0; i < m_cost + 1; i++)
                x = BigInteger.ModPow(x, 2, modulus);
            
            byte[] Y = Utility.BigIntegerToByteArray(x);
            
            if (post_hashing)
                Y = KDF(Y, post_hashing_length);

            return Y;
        }

        /// <summary>
        /// Helper Function
        /// </summary>
        /// /// <param name="data">data to hashing</param>
        /// <param name="outLength">out hash length</param>
        /// <returns>byteArray with hash byte</returns>
        private byte[] KDF(byte[] data, int outLength)
        {
            int r = HashMAC.HashSize / 8;
            byte[] V = Utility.CustomByteArray(0x01, r);
            byte[] K = Utility.CustomByteArray(0x00, r);
            HMAC hashBuffer = HashMAC;
            hashBuffer.Key = K;
            Byte[] hmacData = Utility.ConcatenateByteArrays(V, hexzero, data);
            hashBuffer.Key = hashBuffer.ComputeHash(hmacData);
            V = hashBuffer.ComputeHash(V);
            hashBuffer.Key = hashBuffer.ComputeHash(Utility.ConcatenateByteArrays(V, hexone, data));
            V = hashBuffer.ComputeHash(V);
            byte[] T = new byte[0];
            while (T.Length < outLength)
            {
                V = hashBuffer.ComputeHash(V);
                T = Utility.ConcatenateByteArrays(T, V);
            }
            byte[] output = new byte[outLength];
            Array.Copy(T, output, outLength);
            return output;
        }
        
        /// <summary>
        /// Check initial params
        /// m_cost - workFactory value > 0
        /// modulusLength >= 160
        /// </summary>
        private bool CheckInitialParams()
        {
            if (m_cost <= 0)
                return false;
            if (modulusBytes.Length < 160)
                return false;
            return true;
        }
    }
}
