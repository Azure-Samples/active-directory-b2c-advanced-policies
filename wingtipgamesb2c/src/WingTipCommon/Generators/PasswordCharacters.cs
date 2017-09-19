using System;

namespace WingTipCommon.Generators
{
    [Flags]
    public enum PasswordCharacters
    {
        LowercaseLetters = 0x01,
        UppercaseLetters = 0x02,
        Digits = 0x04,
        Symbols = 0x08,
        Space = 0x10,
        AllLetters = LowercaseLetters | UppercaseLetters,
        Alphanumeric = AllLetters | Digits,
        All = AllLetters | Digits | Symbols | Space,
    }
}
