using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueTestMysql.Models
{
    public class UserInfo
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        [JsonProperty("openid")]
        public string WxOpenId { get; set; }

        [JsonProperty("nickname")]
        public string WxNickName { get; set; }

        [JsonProperty("avatarUrl")]
        public string WxAvatarUrl { get; set; }

        [JsonProperty("country")]
        public string WxCountry { get; set; }

        [JsonProperty("city")]
        public string WxCity { get; set; }
    }
}
