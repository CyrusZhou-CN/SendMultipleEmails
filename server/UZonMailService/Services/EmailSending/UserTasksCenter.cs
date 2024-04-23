using UZonMailService.Models.SqlLite.EmailSending;

namespace UZonMailService.Services.EmailSending
{
    /// <summary>
    /// 用户级发件任务管理
    /// </summary>
    public class UserTasksCenter : Dictionary<int, UserTaskManager>
    {

    }
}
