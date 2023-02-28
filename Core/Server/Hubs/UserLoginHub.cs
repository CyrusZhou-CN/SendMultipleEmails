using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Uamazing.SME.Server.Hubs
{
    /// <summary>
    /// 用户登陆
    /// </summary>
    [HubName("UserLogin")]
    public class UserLoginHub:Hub
    {
        public UserLoginHub()
        {

        }

        public override Task OnConnectedAsync()
        {
            // 向数据库保存 Hub
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
