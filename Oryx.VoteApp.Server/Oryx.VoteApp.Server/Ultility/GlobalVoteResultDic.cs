using Oryx.VoteApp.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.Ultility
{
    /// <summary>
    /// key : voteId
    /// value : voteOption
    /// </summary>
    public class GlobalVoteResultDic : ConcurrentDictionary<int, BlockingCollection<GlobalVoteResultDicItem>>
    {
        public int VoteId { get; set; }

        public List<VoteLog> VoteLogList { get; set; } = new List<VoteLog>();
    }
    public class GlobalVoteResultDicItem
    {
        public string voteOption { get; set; }

        public int voteSum { get; set; }
    }
}
