using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.Models
{
    public class VoteLog
    { 
        public int Id { get; set; }

        public int VoteId { get; set; }

        //public int VoteOptionId { get; set; }

        public string VoteOption { get; set; } = string.Empty;

        public string UserId { get; set; }

        public string UserKey { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public string Latitude { get; set; }

        public string Longtitude { get; set; }
    }
}