using System;

namespace B2CAppMobile
{
    public static class Base64Url
    {
        public static byte[] Decode(string input)
        {
            var output = input;
            output = output.Replace('-', '+');
            output = output.Replace('_', '/');

            switch (output.Length % 4)
            {
                case 0:
                    break;

                case 2:
                    output += "==";
                    break;

                case 3:
                    output += "=";
                    break;

                default:
                    throw new Exception("Value is not a Base64 URL-encoded string.");
            }

            return Convert.FromBase64String(output);
        }
    }
}
