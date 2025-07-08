using IBM.XMS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.Services.Messaging.IbmMq.Settings
{
    public class IbmMqSettings
    {
        public (DestinationType type, string name) Destination { get; set; }
        public bool Transacted { get; set; }
        public AcknowledgeMode AcknowledgeMode { get; set; }
    }
}
