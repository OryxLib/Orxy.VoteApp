using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueTestMysql.Models
{
    public class VoteInfo
    {
        public int Id { get; set; }

        [JsonProperty("question")]
        public string Name { get; set; }

        [JsonProperty("voteList")]
        public virtual IList<VoteOptions> VoteOptions { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime EnableTime { get; set; }
    }
}
