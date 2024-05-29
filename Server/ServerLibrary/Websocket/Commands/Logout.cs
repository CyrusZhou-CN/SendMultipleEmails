using ServerLibrary.Config;
using ServerLibrary.Execute;
using ServerLibrary.Http.Headers;
using ServerLibrary.Protocol;
using ServerLibrary.Websocket.Temp;
using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Websocket.Commands
{
    class Logout : IWebsocketCommand
    {
        public ILog Logger { get; set; }
        public string Name => CommandClassName.Logout.ToString();

        public void ExecuteCommand(ReceivedMessage message)
        {
            if (string.IsNullOrEmpty(message.Body.token))
            {
                message.Response(false);
                return;
            }

            var jwtToken = new JwtToken(UserConfig.Instance.TokenSecret, message.Body.token);

            SessionsCenter.Instance.RemoveAllSessions(jwtToken.UserId);

            message.Response(true);
        }
    }
}
