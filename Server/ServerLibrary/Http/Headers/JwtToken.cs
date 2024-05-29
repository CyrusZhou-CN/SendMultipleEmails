using EmbedIO.Sessions;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Http.Headers
{
    public class JwtToken
    {
        public string Secret { get; private set; }
        public string UserId { get; private set; }
        public string SessionId { get; private set; }
        /// <summary>
        /// 过期暖意
        /// </summary>
        public long Expire { get; private set; }
        /// <summary>
        /// token 是否有效
        /// </summary>
        public TokenValidState TokenValidState { get; private set; } = TokenValidState.Valid;

        public string Token { get; private set; }
        private string GenerateSessionId()
        {
            return Guid.NewGuid().ToString("N");
        }

        public JwtToken(string secret, string token)
        {
            Secret = secret;
            Token = token;
            SessionId = GenerateSessionId();
            DecodeToken(token);
        }

        public JwtToken(string secret, string userId, long exp)
        {
            Secret = secret;
            UserId = userId;
            SessionId = GenerateSessionId();
            Expire = exp;
            CreateToken();
        }

        private void CreateToken()
        {
            var token = JwtBuilder.Create()
                      .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                      .WithSecret(Secret)
                      .AddClaim("exp", Expire)
                      .AddClaim("userId", UserId)
                      .AddClaim("sessionId", SessionId)
                      .Encode();

            Token = token;
        }

        private void DecodeToken(string token)
        {
            try
            {
                var payload = JwtBuilder.Create()
                            .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                            .WithSecret(Secret)
                            .MustVerifySignature()
                            .Decode<IDictionary<string, object>>(token);
                UserId = payload["userId"].ToString();
                SessionId = payload["sessionId"].ToString();
                TokenValidState = TokenValidState.Valid;
            }
            catch (TokenExpiredException)
            {
                //表示过期
                TokenValidState = TokenValidState.Expired;
            }
            catch (SignatureVerificationException)
            {
                //表示验证不通过
                TokenValidState = TokenValidState.Invalid;
            }
            catch (Exception e)
            {
                TokenValidState = TokenValidState.Error;
            }
        }

        public static long DefaultExp()
        {
            return DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds();
        }
    }

    public enum TokenValidState
    {
        Valid,
        Expired,
        Invalid,
        Error,
    }
}
