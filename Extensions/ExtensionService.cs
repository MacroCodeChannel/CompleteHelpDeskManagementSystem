using System.Security.Cryptography;
using System.Text;

namespace HelpDeskSystem.Extensions
{
    public interface IExtensionService
    {

        string GeneratePassword(int maxSize);
    }

    public class ExtensionService :IExtensionService
    {
        public ExtensionService()
        {
            
        }

        public string GeneratePassword(int maxSize)
        {
             var passwords = string.Empty;
            var chArray1 = new Char[52];
            var chArray2 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^*()_+".ToCharArray();
            var data = new byte[1];
            using (var cryptoservice = new RNGCryptoServiceProvider())
            {
                cryptoservice.GetNonZeroBytes(data);
                var data1 = new byte[maxSize];
                var stringBulder = new StringBuilder(maxSize);
                foreach (var item in data1)
                {
                    stringBulder.Append(chArray2[(int)item % chArray2.Length]);
                }

                passwords = stringBulder.ToString();
                return Shuffle(Random("N") + passwords + Random("S") + Random("l"));
            }
        }
        public string Shuffle(string list)
        {
            Random R = new Random();
            int index;
            List<char> chars = new List<char>(list);
            StringBuilder sb = new StringBuilder();
            while (chars.Count > 0)
            {
                index = R.Next(chars.Count);
                sb.Append(chars[index]);
                chars.RemoveAt(index);
            }
            return sb.ToString();
        }

        public string Random(string type)
        {
            var data2 = new byte[2];
            var passwords = string.Empty;
            switch (type)
            {
                case "N":
                    {
                        var charArray = "0123456789";
                        var stringBuilder = new StringBuilder(2);
                        foreach (var num in data2)
                            stringBuilder.Append(charArray[(int)num % charArray.Length]);
                        passwords = stringBuilder.ToString();
                        return passwords;
                    }

                case "l":
                    {
                        var charArray = "abcdefghijklmnopqrstuvwxyz";

                        var stringBuilder = new StringBuilder(2);
                        foreach (var num in data2)
                            stringBuilder.Append(charArray[(int)num % charArray.Length]);
                        passwords = stringBuilder.ToString();
                        return passwords;
                    }

                case "C":
                    {
                        var charArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

                        var stringBuilder = new StringBuilder(2);
                        foreach (var num in data2)
                            stringBuilder.Append(charArray[(int)num % charArray.Length]);
                        passwords = stringBuilder.ToString();
                        return passwords;
                    }

                case "S":
                    {
                        var charArray = "!@#$%^&*()_+-={}|[]:;<>?,./";
                        var stringBuilder = new StringBuilder(2);
                        foreach (var num in data2)
                            stringBuilder.Append(charArray[(int)num % charArray.Length]);
                        passwords = stringBuilder.ToString();
                        return passwords;
                    }
            }

            return string.Empty;
        }

    }
}
