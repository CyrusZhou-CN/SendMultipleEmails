using Microsoft.AspNetCore.SignalR;

namespace Uamazing.UZonEmail.Server.Hubs
{
    /// <summary>
    /// 用户登陆
    /// </summary>
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
