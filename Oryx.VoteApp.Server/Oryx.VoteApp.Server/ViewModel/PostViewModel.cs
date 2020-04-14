using Oryx.VoteApp.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.ViewModel
{
    public class PostViewModel
    {
        public VoteLog VoteLog { get; set; }

        public string NickName { get; set; }

        public string Avatar { get; set; }
    }
}
