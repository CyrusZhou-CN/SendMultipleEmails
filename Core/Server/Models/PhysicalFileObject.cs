namespace Uamazing.UZonEmail.Server.Models
{
    /// <summary>
    /// 磁盘文件
    /// </summary>
    public class PhysicalFileObject:FileObject
    {
        /// <summary>
        /// 默认桶名称
        /// </summary>
        public static string DefaultBucketName { get; } = "Files";
        /// <summary>
        /// 默认在 data/files
        /// </summary>
        private string _bucketName = DefaultBucketName;
        /// <summary>
        /// 存储桶名称,格式化成 /
        /// bucketName 与 ObjectName 共同组成了文件保存的路径
        /// </summary>
        public string BucketName
        {
            get => _bucketName;
            set
            {
                _bucketName = value.Replace("\\", "/").Trim('/');
            }
        }

        private string _objectName;
        /// <summary>
        /// 文件名称,格式化成 / 形式
        /// </summary>
        public string ObjectName
        {
            get => _objectName; 
            set
            {
               _objectName =  value.Replace("\\", "/").Trim('/');
            }
        }
    }
}
