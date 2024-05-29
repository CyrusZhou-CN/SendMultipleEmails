using Newtonsoft.Json.Linq;
using ServerLibrary.Database.Models;
using SuperSocket.WebSocket;
using Swan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Websocket.Temp
{
    /// <summary>
    /// 保存用户的 session 方便在 http 中调用
    /// </summary>
    public class SessionsCenter
    {
        private static readonly Lazy<SessionsCenter> instance = new Lazy<SessionsCenter>(() => new SessionsCenter());
        private readonly Dictionary<string, List<WebSocketSession>> userSessions = new Dictionary<string, List<WebSocketSession>>();

        public static SessionsCenter Instance => instance.Value;

        public void AddSession(string userId, WebSocketSession session)
        {
            if (!userSessions.ContainsKey(userId))
            {
                userSessions[userId] = new List<WebSocketSession>();
            }
            userSessions[userId].Add(session);
        }

        public void RemoveAllSessions(string userId)
        {
            if (userSessions.ContainsKey(userId))
            {
                foreach (var session in userSessions[userId])
                {
                    session.Send(new Protocol.Response()
                    {
                        eventName = "Logout",
                        command = "onLogout",
                    }.ToJson());
                    session.Close();
                }
                userSessions.Remove(userId);
            }
        }

        internal bool TryGetValue(string userId, out List<WebSocketSession> sessions)
        {
            return userSessions.TryGetValue(userId, out sessions);

        }
    }
}
