using LiteDB;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Models.LiteDB;

namespace UZonMailService.Services.Litedb
{
    /// <summary>
    /// 树形结构的服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeLikeService<T> : CRUDService where T : TreeNode
    {
        public TreeLikeService(ILiteRepository liteRepository) : base(liteRepository)
        {
        }

        /// <summary>
        /// 添加 tree 节点
        /// 不会对节点名重复性进行判断
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<T> AddTreeNode(T node)
        {
            // 修改全路径
            node.Path = node.Name;
            // 获取父级路径
            var parentNode = await FirstOrDefault<T>(x => x.Id == node.ParentId);
            if (parentNode != null)
            {
                // 组合路径
                node.Path = parentNode.Path + "/" + node.Name;
            }
            // 新建
            await Create(node);

            return node;
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> DeleteNodeById(string nodeId)
        {
            // 找到 node
            var targetNode = await FirstOrDefault<T>(x => x.Id == nodeId);
            if (targetNode == null) return new List<string>();

            // 递归找到所有的 node 节点
            var children = await GetChildrenNodes(targetNode);
            var deletingIds = new HashSet<string>()
            {
                targetNode.Id,
            };
            children.ForEach(x => deletingIds.Add(x.Id));

            // 开始删除数据
            await DeleteMany<T>(x => deletingIds.Contains(x.Id));
            return deletingIds.ToList();
        }

        /// <summary>
        /// 获取所有的子节点
        /// </summary>
        /// <param name="startNode"></param>
        /// <returns></returns>
        private async Task<List<T>> GetChildrenNodes(T startNode)
        {
            var results = new List<T>();

            // 递归子节点
            var children = await FindAll<T>(x => x.ParentId == startNode.Id);
            results.AddRange(children);
            if (!children.Any()) return results;

            foreach (var child in children)
            {
                var childResults = await GetChildrenNodes(child);
                results.AddRange(childResults);
            }

            return results;
        }
    }
}
