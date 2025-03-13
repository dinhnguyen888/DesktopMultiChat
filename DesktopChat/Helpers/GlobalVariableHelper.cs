using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Windows;

namespace DesktopChat.Helpers
{
    public static class GlobalVariableHelper
    {
        private const string AccessTokenKey = "AccessToken";
        private const string RefreshTokenKey = "RefreshToken";

        public static string AccessToken
        {
            get
            {
                if (Application.Current.Properties.Contains(AccessTokenKey))
                    return Application.Current.Properties[AccessTokenKey]?.ToString() ?? string.Empty;
                return string.Empty;
            }
            set
            {
                Application.Current.Properties[AccessTokenKey] = value;
             
            }
        }

        public static string RefreshToken
        {
            get
            {
                if (Application.Current.Properties.Contains(RefreshTokenKey))
                    return Application.Current.Properties[RefreshTokenKey]?.ToString() ?? string.Empty;
                return string.Empty;
            }
            set
            {
                Application.Current.Properties[RefreshTokenKey] = value;
               
            }
        }

        public static Dictionary<string, string>? DecodeToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var claims = new Dictionary<string, string>();
                foreach (var claim in jwtToken.Claims)
                {
                    claims[claim.Type] = claim.Value;
                }
                return claims;
            }
            catch
            {
                return null;
            }
        }

        public static string GetUserInfoFromToken(string claimType)
        {
            var token = AccessToken;
            if (string.IsNullOrEmpty(token)) return string.Empty;
            var claims = DecodeToken(token);
            if (claims != null && claims.TryGetValue(claimType, out var value))
            {
                return value;
            }
            return string.Empty;
        }

        public static bool IsTokenValid()
        {
            var token = AccessToken;
            if (string.IsNullOrEmpty(token)) return false;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        public static void ClearTokens()
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
        }
    }
}