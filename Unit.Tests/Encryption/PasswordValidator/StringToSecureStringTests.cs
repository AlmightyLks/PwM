﻿using Xunit;

namespace Unit.Tests.Encryption.PasswordValidator
{
    public class StringToSecureStringTests
    {
        [Theory]
        [InlineData("Test32g342gh34")]
        [InlineData("LogTest321")]
        [InlineData("!fwe235423GE")]
        public void Ensure_we_get_a_string_from_secure_string(string item)
        {
            var ss = PwMLib.PasswordValidator.StringToSecureString(item);
            Assert.Equal(item, new System.Net.NetworkCredential(string.Empty, ss).Password);
        }
    }
}
