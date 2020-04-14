using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.Models
{
    public class WxqLog
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string Avarta { get; set; }
        public string Msg { get; set; }
        public string MsgType { get; set; }
        public MsgStatus MsgStatus { get; set; }
    }

    public enum MsgStatus
    {
        Created,
        Proccessed,
        Checked,
        Deleted
    }
}
