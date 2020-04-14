using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.Models
{
    public class JDLYOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Answer { get; set; }
        public string ImgUrl { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }

        public JDLYQuestion Question { get; set; }
    }
}
