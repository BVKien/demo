using Google.Apis.Auth;
using OJTEDU.Application.ApplicationServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class GoogleJsonWebSignatureValidator : IGoogleJsonWebSignatureValidator
    {
        public async Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken, GoogleJsonWebSignature.ValidationSettings settings)
        {
            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
    }
}
