using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.PostManager
{
    public class DbOperationQueue : Queue<Object>
    {
        public DbOperationQueue() { }

        public DbOperationQueue(int capacity)
            : base(capacity)
        {

        }
    }
}
