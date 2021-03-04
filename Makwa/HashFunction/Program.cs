using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace HashFunction
{
    class Program
    {
        static void Main(string[] args)
        {//test vector - expected hash value C9-CE-A0-E6-EF-09-39-3A-B1-71-0A-08
            Makwa makwaTestVector = new Makwa(false, true, 4096, 12, "modulus.txt");
            byte[] salt = Utility.HexStringToByteArray("C72703C22A96D9992F3DEA876497E392");
            byte[] passwordBytes = Utility.HexStringToByteArray("4765676F206265736877616A692761616B656E20617765206D616B77613B206F6E7A61616D206E616E69697A61616E697A692E");
            string passwordText = "Gego beshwaji'aaken awe makwa; onzaam naniizaanizi."; //Don't get friendly with the bear; he's too dangerous.

            byte[] testHash1 = makwaTestVector.CreateHash(passwordBytes, salt);
            Console.WriteLine("hash text: " + BitConverter.ToString(testHash1));

            byte[] testHash2 = makwaTestVector.CreateHash(passwordText,salt);
            Console.WriteLine("hash hex: " + BitConverter.ToString(testHash2));
            
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();


            //other tests
            ModulusGenerator.GenerateRSAModulus(2048, "genModulus.txt");
            Makwa makwaExample = new Makwa(true, true, 4096, 64, "genModulus.txt");
            byte[] exampleHash1 = makwaExample.CreateHash("hasło", "sól");
            Console.WriteLine(BitConverter.ToString(exampleHash1));
            
            byte[] exampleHash2 = makwaExample.CreateHash("haslo", "sól");
            Console.WriteLine(BitConverter.ToString(exampleHash2));
            
        }
    }
}
