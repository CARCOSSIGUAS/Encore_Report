using Belcorp.Encore.Application.Utilities;
using Belcorp.Encore.Entities.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        byte[] CryptoKey { get; set; }
        string Salt { get; set; }

        public AuthenticationService()
        {
            this.CryptoKey = Encoding.UTF8.GetBytes("N3tst3ps_2008");
            this.Salt = "&netsteps_salt&";
        }

        /// <summary>
        /// Decrypts the EncodedText and stores the value in AccountID
        /// </summary>
        /// <param name="model"></param>
        /// <param name="exceptionHandler"></param>
        public void Decode(SingleSignOnModel_DTO model)
        {
            if (string.IsNullOrWhiteSpace(model.EncodedText))
            {
                model.DecodedText = "";
                model.TimeStamp = DateTime.MinValue;
                return;
            }

            try
            {
                TimeStampedString decryptToken = EncryptionUtilities.DecryptTripleDES<TimeStampedString>(CryptoKey, model.EncodedText, Salt);
                model.DecodedText = decryptToken.MyString;
                model.TimeStamp = decryptToken.TimeStamp;
            }
            catch (Exception ex)
            {
            }
        }

        private class TimeStampedString
        {
            public string MyString { get; set; }

            public DateTime TimeStamp { get; set; }

            public TimeStampedString(string mystring, DateTime timeStamp)
            {
                this.MyString = mystring;
                this.TimeStamp = timeStamp;
            }
        }
    }
}
