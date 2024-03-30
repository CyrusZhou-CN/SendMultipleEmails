namespace UZonMailService.Models.LiteDB
{
    /// <summary>
    /// 树形数据结构
    /// </summary>
    public class TreeNode : LinkingUserId
    {
        /// <summary>
        /// 序号
        /// </summary>
        public long Order { get; set; }

        /// <summary>
        /// 父级 ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 全路径
        /// </summary>
        public string Path { get; set; }
    }
}
