using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WingTipCommon.Generators
{
    public static class RandomPasswordGenerator
    {
        public static string Generate(int length, PasswordCharacters includedCharacters = PasswordCharacters.All, IEnumerable<char> excludedCharacters = null)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "The password length must be greater than zero.");
            }

            var randomCharacters = new char[length];
            var characters = GenerateCharacters(includedCharacters, excludedCharacters);
            var charactersLength = characters.Length;
            var randomBytes = new byte[length];
            var randomNumberGenerator = new RNGCryptoServiceProvider();
            randomNumberGenerator.GetBytes(randomBytes);

            for (var index = 0; index < length; index++)
            {
                randomCharacters[index] = characters[randomBytes[index] % charactersLength];
            }

            return new string(randomCharacters);
        }

        private static char[] GenerateCharacters(PasswordCharacters includedCharacters, IEnumerable<char> excludedCharacters)
        {
            var charactersBuilder = new StringBuilder();

            foreach (var character in Characters.Where(character => (character.Key & includedCharacters) == character.Key))
            {
                if (excludedCharacters != null)
                {
                    charactersBuilder.Append(character.Value.Where(c => !excludedCharacters.Contains(c)).ToArray());
                }
                else
                {
                    charactersBuilder.Append(character.Value);
                }
            }

            return charactersBuilder.ToString().ToCharArray();
        }

        private static readonly Dictionary<PasswordCharacters, string> Characters = new Dictionary<PasswordCharacters, string>(4)
        {
            {PasswordCharacters.LowercaseLetters, "abcdefghijklmnopqrstuvwxyz"},
            {PasswordCharacters.UppercaseLetters, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"},
            {PasswordCharacters.Digits, "0123456789"},
            {PasswordCharacters.Space, " "},
            {PasswordCharacters.Symbols, @"~`!@#$%^&*()_-+={[}]|\:;""'<,>.?/"}
        };
    }
}
