using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;

namespace HashFunction
{
	public class ModulusGenerator
	{
		private readonly static RandomNumberGenerator random = new RNGCryptoServiceProvider();

		/// <summary>
		/// Generate and save modulus to file using RSA key generator
		/// </summary>
		/// <param name="byteLength">modulus byte length</param>
		/// <param name="path">path to file to save</param>
		/// <returns>modulus as bigInteger</returns>
		public static BigInteger GenerateRSAModulus(int byteLength, string path)
		{
			RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(byteLength);
			int lengthInBase64 = (int) Math.Ceiling(RSA.KeySize / 24.0) * 4;
			string RSAKeyString = RSA.ToXmlString(false);
			string modulusStr = RSAKeyString.Substring(22, lengthInBase64);
			byte[] modulusByte = Convert.FromBase64String(modulusStr);
			BigInteger modulusBigInt = Utility.ByteArrayToBigInteger(modulusByte);
			using (StreamWriter writer = new StreamWriter(path))  
			{  
				writer.WriteLine(Utility.BigIntegerToHexString(modulusBigInt));
			}
			return modulusBigInt;
		}

		
		/// <summary>
		/// Generate and save modulus to file using primes random generator
		/// </summary>
		/// <param name="byteLength">modulus byte length</param>
		/// <param name="path">path to file to save</param>
		/// <returns>modulus as bigInteger</returns>
		public static BigInteger GenerateRandomModulus(int byteLength, string path)
		{
			BigInteger p = GeneratePrime(byteLength/2,255,true);
			BigInteger q = GeneratePrime(byteLength/2,255,true);
			BigInteger modulus = p * q;
			string strModulus=Utility.BigIntegerToHexString(modulus);
			using (StreamWriter writer = new StreamWriter(path))  
			{  
				writer.WriteLine(strModulus);
			}
			return modulus;
		}
		

		/// <summary>
		/// Generate and save modulus to file using RSA key generator
		/// </summary>
		/// <param name="byteLength">generate prime byte size</param>
		/// <param name="k">the number of rounds of testing to perform</param>
		/// <param name="mod4eq3">if true return primes which p mod4 = 3</param>
		/// <returns>prime as bigInteger</returns>
		public static BigInteger GeneratePrime(int byteLength, int k, bool mod4Eq3)
		{
			byte[] tab = new byte[byteLength];
			BigInteger n;
			while (true)
			{
				random.GetBytes(tab);
				n = Utility.ByteArrayToBigInteger(tab);
				if(!mod4Eq3 || BigInteger.ModPow(n,1,4)==3 && IsPrime(n,k))
					break;
			}
			return n;

		}
		
		
		/// <summary>
		/// It returns false if n is composite and returns true if n is probably prime.
		/// </summary>
		/// <param name="n">BigInteger prime to check</param>
		/// <param name="k">input parameter that determines accuracy level</param>
		/// <returns>true if probably prime</returns>
		public static bool IsPrime(BigInteger n, int k)
		{

			// Corner cases 
			if (n <= 1 || n == 4)
				return false;
			if (n <= 3)
				return true;

			// Find r such that n = 2^d * r + 1 
			// for some r >= 1 
			BigInteger d = n - 1;

			while (d % 2 == 0)
				d /= 2;

			// Iterate given n of 'k' times 
			for (int i = 0; i < k; i++)
				if (MillerTest(d, n) == false)
					return false;

			return true;
		}
		
		
		/// <summary>
		/// It returns false if n is composite and returns true if n is probably prime.
		/// </summary>
		/// <param name="n">BigInteger prime to check</param>
		/// <param name="d">an odd number such that d*2<sup>r</sup> = n-1 for some r >= 1 </param>
		/// <returns>true if probably true</returns>
		public static bool MillerTest(BigInteger d, BigInteger n)
		{
			byte[] tab = new byte[n.GetByteCount()+1];
			random.GetBytes(tab);
			BigInteger r = BigInteger.Abs(new BigInteger(tab));
			BigInteger a = 2 + BigInteger.ModPow(r, 1, n - 4);

			BigInteger x = BigInteger.ModPow(a, d, n);

			if (x == 1 || x == n - 1)
				return true;
			
			while (d != n - 1)
			{
				x = BigInteger.ModPow(x, 2, n);
				d = d * 2;

				if (x == 1)
					return false;
				if (x == n - 1)
					return true;
			}
			return false;
		}
	}
}
