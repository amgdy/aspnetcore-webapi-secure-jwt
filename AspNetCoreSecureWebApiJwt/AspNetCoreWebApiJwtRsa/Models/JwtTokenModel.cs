using System;

namespace AspNetCoreWebApiJwtRsa.Models
{
    public class JwtTokenModel
    {
        public string AccessToken { get; set; }

        public DateTime ExpirationTime { get; set; }
    }
}
