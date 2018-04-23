using Belcorp.Encore.Entities.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Decodes EncodedText with provided key ?? defaultKey and provided salt
        /// </summary>
        /// <param name="model"></param>
        void Decode(SingleSignOnModel_DTO model);
    }
}
