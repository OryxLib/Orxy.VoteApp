using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.Models
{
    public class JDLYLog
    {
        public int Id { get; set; }

        public int JdlyId { get; set; }

        public string UserCode { get; set; }

        public int RightNumber { get; set; }

        public string RightOption { get; set; }

        public string WrongOption { get; set; }

        public string OptionJson { get; set; }

        public string NickName { get; set; }

        public string Avarta { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
