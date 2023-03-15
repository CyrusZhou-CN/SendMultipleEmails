using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Uamazing.Utils.DotNETCore.Token;
using Uamazing.Utils.Validate;

namespace Uamazing.Utils.Token.Extensions
{
    /// <summary>
    /// jwt 授权验证
    /// </summary>
    public static class JWTToken
    {
        /// <summary>
        /// 传入
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static Result<string> CreateToken(this TokenParams tokenParam, Dictionary<string, string> payload)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // token的有效时间
                Expires = DateTime.UtcNow.AddMilliseconds(tokenParam.Expire),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenParam.Secret)), SecurityAlgorithms.HmacSha256Signature),
                Issuer = tokenParam.Issuer,
                Audience = tokenParam.Audience,
            };

            if (payload != null)
            {
                List<Claim> claims = payload.ToList().ConvertAll(kv =>
                {
                    return new Claim(kv.Key,kv.Value);
                });
                tokenDescriptor.Subject = new ClaimsIdentity(claims);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return new SuccessResult<string>(token);
        }

        /// <summary>
        /// 获取token中的附带的数据
        /// </summary>
        /// <param name="tokenParam"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Result<JObject> GetTokenPayload(this TokenParams tokenParam, string token)
        {
            //校验token
            var validateParameter = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = tokenParam.Issuer,
                ValidAudience = tokenParam.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenParam.Secret))
            };
            //不校验，直接解析token
            //jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token1);
            try
            {
                //校验并解析token
                var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token, validateParameter, out SecurityToken validatedToken);//validatedToken:解密后的对象
                var jwtPayload = ((JwtSecurityToken)validatedToken).Payload.SerializeToJson(); //获取payload中的数据
                var jobj = JObject.Parse(jwtPayload);

                return new SuccessResult<JObject>(jobj);   
            }
            catch (SecurityTokenExpiredException expireException)
            {
                //表示过期
                return new ErrorResult<JObject>(null,expireException.Message);
            }
            catch (SecurityTokenException error)
            {
                //表示token错误
                return new ErrorResult<JObject>(null,error.Message);
            }
            catch(Exception ex)
            {
                // 其它错误
                return new ErrorResult<JObject>(null,ex.Message);
            }
        }
    }
}
