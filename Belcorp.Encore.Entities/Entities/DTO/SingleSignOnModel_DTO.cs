using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class SingleSignOnModel_DTO
    {        
        /// <summary>
        /// Decoded text, text that is going to be encoded
        /// </summary>
        public string DecodedText { get; set; }

        /// <summary>
        /// Encoded text, text that is going to be decoded
        /// </summary>
        public string EncodedText { get; set; }

        /// <summary>
        ///     Timestamp for the SSO token
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}
