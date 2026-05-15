using highspeed.framework;
using FreeSql.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace highspeed.api.Common
{
    /// <summary>
    /// 授权JWT类
    /// </summary>
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public const string HeaderKey = Consts.HeaderKey_Authorization;

        /// <summary>
        /// Token配置
        /// </summary>
        /// <param name="configuration"></param>
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 创建Token
        /// </summary>
        /// <param name="data">保存数据</param>
        /// <returns></returns>
        public string CreateToken(string data)
        {
            // 1. 定义需要使用到的Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.UserData, data)
            };

            // 2. 从 appsettings.json 中读取SecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            // 5. 根据以上，生成token
            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],           //Issuer
                _configuration["Jwt:Audience"],         //Audience
                claims,                                 //Claims,
                DateTime.Now,                           //notBefore
                DateTime.Now.AddMinutes(Consts.Auth_TimeoutMin),   //expires
                signingCredentials                      //Credentials
            );

            // 6. 将token变为string
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        /// <summary>
        /// 创建Token
        /// </summary>
        /// <param name="object">保存对象</param>
        /// <returns></returns>
        public string CreateToken(object obj)
        {
            return CreateToken(obj?.ToJson() ?? string.Empty);
        }

        /// <summary>
        /// 获取Token中保留的信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        internal string GetTokenData(string token)
        {
            //解密方法
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters varParam = new TokenValidationParameters();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            varParam.IssuerSigningKey = securityKey;
            varParam.ValidIssuer = _configuration["Jwt:Issuer"];
            varParam.ValidAudience = _configuration["Jwt:Audience"];
            varParam.ValidateIssuer = true;
            varParam.ValidateAudience = true;
            ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, varParam, out SecurityToken secToken);
            return claimsPrincipal.Claims.FirstOrDefault(i => i.Type == ClaimTypes.UserData)?.Value;
        }

        /// <summary>
        /// 获取Token中保留的信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        internal T GetTokenData<T>(string token)
        {
            var json = GetTokenData(token);
            if (string.IsNullOrEmpty(json)) return (T)typeof(T).GetDefaultValue();
            return (T)json.JsonToObject(typeof(T));
        }

        internal string RefreshToken(string old)
        {
            if (string.IsNullOrWhiteSpace(old)) return old;

            if (old.ToLower().StartsWith("bearer")) old = old.Substring(6).Trim();

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var token = tokenHandler.ReadJwtToken(old);
                if (token == null) return old;

                if (token.ValidTo.ToUniversalTime() < DateTime.Now.ToUniversalTime().AddMinutes(5)) // 在过期前5分钟生成新的Token
                {
                    return CreateToken(GetTokenData(old));
                }
            }
            catch (Exception) { }
            return old;
        }

        internal void ToBlack(string userId)
        {
            BlackTokenStore.Push(userId);
        }

        internal async Task ToWhite(string userId)
        {
           await BlackTokenStore.Clean(userId);
        }

        internal bool IsInBlack(string userId)
        {
            var task = BlackTokenStore.Find(userId);
            task.Wait();
            return task.Result != null;
        }

        private static class BlackTokenStore
        {
            private static string localDbPath;
            private static bool syncDbStructure;
            private static IFreeSql fSql;
            private static JwtHelper jwt;

            [Table(Name = "black_tokens")]
            private class BlackToken
            {
                [Column(Name = "id", IsIdentity = true, IsPrimary = true)]
                public long Id { get; set; }
                [Column(Name = "user")]
                public string UserId { get; set; }
                [Column(Name = "create_time", ServerTime = DateTimeKind.Local)]
                public DateTime CreateTime { get; set; }
            }

            static BlackTokenStore()
            {
                localDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "blacklist.db");
                syncDbStructure = true;

                if (!File.Exists(localDbPath)) File.Create(localDbPath).Dispose();

                fSql = new FreeSql.FreeSqlBuilder()
                        .UseConnectionString(FreeSql.DataType.Sqlite, $"Data Source={localDbPath};Pooling=true;Min Pool Size=1;")
                        .UseAutoSyncStructure(syncDbStructure)
                        //.UseMonitorCommand(cmd => Logger.Debug("BlackTokenStore - sql", cmd.CommandText))
                        .Build();

                if (syncDbStructure)
                {
                    fSql.CodeFirst.SyncStructure(typeof(BlackToken));
                }

                jwt = DIContainer.GetSerivce<JwtHelper>();
            }

            internal static async void Push(string userId)
            {
                await Clean(userId);
                await fSql.Insert<BlackToken>(new BlackToken { UserId = userId, CreateTime = DateTime.Now }).ExecuteAffrowsAsync();
            }

            internal static async Task<string?> Find(string userId)
            {
                Clean();
                var result = await fSql.Select<BlackToken>().Where(t => t.UserId == userId).FirstAsync();
                return result?.UserId;
            }

            internal static async Task Clean(string userId)
            {
                var now = DateTime.Now;
                await fSql.Select<BlackToken>().Where(t => t.UserId == userId).ToDelete().ExecuteAffrowsAsync();
            }

            internal static void Clean()
            {
                var now = DateTime.Now;
                fSql.Select<BlackToken>().Where(t => now.Subtract(t.CreateTime).TotalMinutes > Consts.Auth_TimeoutMin).ToDelete().ExecuteAffrowsAsync();
            }
        }
    }
}
