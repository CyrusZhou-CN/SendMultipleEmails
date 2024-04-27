
using MailKit.Net.Smtp;
using MimeKit;

namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 本机发件
    /// </summary>
    public class LocalSender(SendItem sendItem) : SendMethod
    {
        private readonly SendItem _sendItem = sendItem;

        /// <summary>
        /// 使用本机进行发件
        /// </summary>
        /// <returns></returns>
        public override async Task<SentStatus> Send()
        {
            if (_sendItem == null)
            {
                throw new ArgumentNullException(nameof(_sendItem));
            }
            if (!_sendItem.Validate()) return SentStatus.Failed;

            // 参考：https://github.com/jstedfast/MailKit/tree/master/Documentation/Examples

            // 本机发件逻辑
            var message = new MimeMessage();
            // 发件人
            message.To.Add(new MailboxAddress(_sendItem.Outbox.Name, _sendItem.Outbox.Email));
            // 收件人、抄送、密送           
            foreach (var address in _sendItem.Inboxes)
            {
                message.To.Add(new MailboxAddress(address.Name, address.Email));
            }
            if (_sendItem.CC != null)
                foreach (var address in _sendItem.CC)
                {
                    message.Cc.Add(new MailboxAddress(address.Name, address.Email));
                }
            if (_sendItem.BCC != null)
                foreach (var address in _sendItem.BCC)
                {
                    message.Bcc.Add(new MailboxAddress(address.Name, address.Email));
                }

            // 主题
            message.Subject = _sendItem.Subject;

            // 正文
            BodyBuilder bodyBuilder = new()
            {
                HtmlBody = _sendItem.GetBody()
            };
            // 附件
            List<string> attachments = await _sendItem.GetAttachments();
            foreach (var attachment in attachments)
            {
                bodyBuilder.Attachments.Add(attachment);
            }
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.Connect(_sendItem.Outbox.SmtpHost, _sendItem.Outbox.SmtpPort, _sendItem.Outbox.EnableSSL);
            // Note: only needed if the SMTP server requires authentication
            if (!string.IsNullOrEmpty(_sendItem.Outbox.AuthPassword)) client.Authenticate(_sendItem.Outbox.AuthUserName, _sendItem.Outbox.AuthPassword);

            try
            {
                string sendResult = await client.SendAsync(message);
                return await UpdateSendingStatus(true, sendResult);
            }
            catch(Exception ex)
            {
                return await UpdateSendingStatus(false, ex.Message);
            }
            finally
            {
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// 发送完成后回调
        /// </summary>
        /// <param name="ok"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual async Task<SentStatus> UpdateSendingStatus(bool ok,string message)
        {
            return await _sendItem.UpdateSendingStatus(ok, message);
        }
    }
}
