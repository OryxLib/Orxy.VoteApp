using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueTestMysql.Models
{
    public class VoteLog
    {
        public int Id { get; set; }

        public int VoteId { get; set; }

        //public int VoteOptionId { get; set; }

        public string VoteOption { get; set; }

        public string UserId { get; set; }

        public string UserKey { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

    }
}