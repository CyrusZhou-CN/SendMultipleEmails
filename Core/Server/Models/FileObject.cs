using Uamazing.Utils.Database.Attributes;

namespace Uamazing.UZonEmail.Server.Models
{
    /// <summary>
    /// ioFile 类
    /// 用于保存上传的文件
    /// </summary>    
    [CollectionName("FileObject")]
    public class FileObject : LinkingUserId
    {
        /// <summary>
        /// 文件名称
        /// 可阅读的，未经编码过的名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 最后修改日期
        /// </summary>
        public DateTime LastModifyDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 文件哈希码
        /// 验证文件的唯一性
        /// </summary>
        public string SHA256 { get; set; }

        /// <summary>
        /// 文件状态
        /// 只有 OK 的文件才是有效的文件
        /// </summary>
        public FileStatus FileStatus { get; set; } = FileStatus.PreCreated;
    }

    public enum FileStatus
    {
        /// <summary>
        /// 预创建
        /// </summary>
        PreCreated,

        /// <summary>
        /// 完成
        /// </summary>
        Ok,
    }
}
