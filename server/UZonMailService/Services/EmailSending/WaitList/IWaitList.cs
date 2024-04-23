using UZonMailService.Services.EmailSending.SendCore;

namespace UZonMailService.Services.EmailSending.WaitList
{
    public interface IWaitList
    {
        int GetEmailTypesCount();

        SendItem GetSendItem();

        /// <summary>
        /// 将发件项加入队尾
        /// </summary>
        /// <param name="sendItem"></param>
        void QueueSendItem(SendItem sendItem);
    }
}
