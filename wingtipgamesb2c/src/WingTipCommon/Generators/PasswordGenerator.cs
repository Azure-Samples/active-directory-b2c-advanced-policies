using System.Linq;

namespace WingTipCommon.Generators
{
    public class PasswordGenerator : IPasswordGenerator
    {
        public PasswordGenerator(
            int length,
            PasswordCharacters includedCharacters = PasswordCharacters.All,
            char[] excludedCharacters = null)
        {
            Length = length;
            IncludedCharacters = includedCharacters;
            ExcludedCharacters = excludedCharacters;
            RequiresStrongPassword = true;
        }

        public char[] ExcludedCharacters { get; }

        public PasswordCharacters IncludedCharacters { get; }

        public int Length { get; }

        public bool RequiresStrongPassword { get; set; }

        public string GeneratePassword()
        {
            string password;

            do
            {
                password = RandomPasswordGenerator.Generate(Length, IncludedCharacters, ExcludedCharacters);
            } while (RequiresStrongPassword && !IsStrongPassword(password));

            return password;
        }

        private static bool IsStrongPassword(string secret)
        {
            return secret.Any(char.IsUpper)
                && secret.Any(char.IsLower)
                && secret.Any(char.IsDigit);
        }
    }
}
