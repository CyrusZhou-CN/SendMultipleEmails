
using MailKit.Net.Smtp;
using MimeKit;

namespace UZonMailService.Services.EmailSending.SendCore
{
    /// <summary>
    /// 本机发件
    /// </summary>
    public class SendLocal : SendMethod
    {
        private readonly SendItem _sendItem;
        public SendLocal(SendItem sendItem)
        {
            this._sendItem = sendItem;
        }

        /// <summary>
        /// 使用本机进行发件
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> Send()
        {
            if (_sendItem == null)
            {
                throw new ArgumentNullException(nameof(_sendItem));
            }

            // 参考：https://github.com/jstedfast/MailKit/tree/master/Documentation/Examples

            // 本机发件逻辑
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Joey Tribbiani", "joey@friends.com"));
            message.To.Add(new MailboxAddress("Mrs. Chanandler Bong", "chandler@friends.com"));
            message.Subject = "How you doin'?";

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = @"<b>Hey Chandler,</b><br>I just wanted to let you know that Monica and I were going to go play some paintball, you in?<br>-- Joey";
            message.Body = bodyBuilder.ToMessageBody();            

            using var client = new SmtpClient();
            client.Connect("smtp.friends.com", 587, false);

            // Note: only needed if the SMTP server requires authentication
            client.Authenticate("joey", "password");

            client.Send(message);
            client.Disconnect(true);

            return true;
        }
    }
}
