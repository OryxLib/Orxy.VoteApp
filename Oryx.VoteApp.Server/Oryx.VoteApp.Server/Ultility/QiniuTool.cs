using Microsoft.Extensions.Configuration;
using Qiniu.Common;
using Qiniu.IO.Model;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.Ultility
{
    public class QiniuTool
    {
        private readonly IConfiguration Configuration;

        public QiniuTool(IConfiguration _config)
        {
            Configuration = _config;
        }

        public string GenerateToken()
        {
            string AK = "vRlRmKlkqx9qY9CTWTinD-OJowSeYw5-Vg1nktUA";
            string SK = "4gGxVhemV8DENWtUAJqs1L_xTq5QlazSnnkXfwgP";

            // ZoneID zoneId = ZoneID.CN_East; 
            // [CN_East:华东] [CN_South:华南] [CN_North:华北] [US_North:北美]
            // USE_HTTPS = (true|false) 是否使用HTTPS
            Config.SetZone(ZoneID.CN_North, false);

            string bucket = Configuration["Bucket"];
            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;
            putPolicy.Scope = bucket;
            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            //putPolicy.DeleteAfterDays = 1;

            Mac mac = new Mac(AK, SK);
            Auth auth = new Auth(mac);
            string token = auth.CreateUploadToken(putPolicy.ToJsonString());

            return token;
        }

        private ZoneID GetZone()
        {
            var conf = Configuration["Location"];
            return (ZoneID)Enum.Parse(typeof(ZoneID), conf);
        }
    }
}
