using System;
using System.Collections.Generic;
using System.Text;

namespace Oryx.OnePublish.ConfigArchticle
{
    public class ServerStruct
    {
        public string Host { get; set; }

        public string User { get; set; }

        public string Pwd { get; set; }

        public string DeployPath { get; set; }

        public string DeployedCMD { get; set; }
    }
}
