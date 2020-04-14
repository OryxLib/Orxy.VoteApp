using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueTestMysql.Models
{
    public class VoteOptions
    {
        public int Id { get; set; }
         
        public string Key { get; set; }
         
        public string Value { get; set; }

        public string Type { get; set; } = "img";

        public virtual VoteInfo VoteInfo { get; set; }
    }
}
