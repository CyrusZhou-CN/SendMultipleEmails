﻿using SuperSocket.SocketBase.Logging;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.SME.Server.Execute
{
   internal interface IWebsocketCommand
    {
        ILog Logger { get; set; }

        string Name { get;}       

        void ExecuteCommand(ReceivedMessage message);
    }
}
