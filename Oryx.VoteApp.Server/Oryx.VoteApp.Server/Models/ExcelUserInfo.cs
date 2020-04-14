using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Oryx.VoteApp.Server.Models
{
    public class ExcelUserInfo
    { 
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Description { get; set; }
    }
}
