using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.Models
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

        public double EnableStartTime { get; set; }

        public bool IsEnable { get; set; } = false;

        public int CloseSeconds { get; set; } = 60;

        public string VoteType { get; set; }

        public bool Expired { get; set; } = false;

        public bool IsOpen { get; set; } = true;

        public bool ShouldLogin { get; set; } = false;
    }
}
