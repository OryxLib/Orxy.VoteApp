using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.Models
{
    public class JDLYQuestion
    {
        public int Id { get; set; }

        public bool IsEnable { get; set; }

        public double EnableStartTime { get; set; }

        public int CloseSeconds { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public virtual IList<JDLYOption> Options { get; set; }
    }
}
