using System;
using System.Collections.Generic;
using System.Text;

namespace Oryx.OnePublish.ConfigArchticle
{
    public class ConfigStruct
    {
        public ClientStruct Client { get; set; }

        public List<ServerStruct> ServerList { get; set; }
    }
}
