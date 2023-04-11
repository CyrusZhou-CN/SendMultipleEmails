namespace Uamazing.SME.Server.Models
{
    /// <summary>
    /// ioFile 类
    /// 用于保存上传的文件
    /// </summary>
    public class FileObject:LinkingUserId
    {
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 最后修改日期
        /// </summary>
        public DateTime LastModifyDate { get; set; }

        /// <summary>
        /// 文件哈希码
        /// 验证文件的唯一性
        /// </summary>
        public string SHA256 { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string ObjectName { get; set; }
    }
}
