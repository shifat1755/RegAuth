using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Security;
namespace RegAuth.Services
{
    public class CommonService
    {   
        public string DoHashing(string s)
        {
            const int hashSize = 256;
            IDigest digest=new Sha3Digest(hashSize);
            byte[] inputBytes = Encoding.UTF8.GetBytes(s);
            digest.BlockUpdate(inputBytes,0, inputBytes.Length);
            byte[] bytes = new byte[digest.GetDigestSize()];
            digest.DoFinal(bytes, 0);
            string hashedString = BitConverter.ToString(bytes).Replace("-", "");
            return hashedString;

        }
    }
}
