using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.Models
{
    public class VoteBuff
    {
        public int Id { get; set; }

        public int VoteId { get; set; }

        public string VoteOption { get; set; }

        public string BossId { get; set; }
    }
}
