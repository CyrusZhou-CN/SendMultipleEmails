using LiteDB;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using Uamazing.SME.Server.Models;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.FileUpload;
using Uamazing.Utils.Web.ResponseModel;

namespace Uamazing.SME.Server.Services
{
    /// <summary>
    /// 文件服务
    /// </summary>
    public class FileObjectService : CurdService
    {
        public FileObjectService(ILiteRepository liteRepository) : base(liteRepository)
        {
        }

        /// <summary>
        /// 通过 sha256 获取文件
        /// </summary>
        /// <param name="sha256"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<FileObject> GetFileObject(string sha256)
        {
            // 判断是否有文件，如果有文件，则返回，否则返回空
            var fileObj = await GetFirstOrDefault<FileObject>(x=>x.SHA256 == sha256);
            return fileObj;
        }

        /// <summary>
        /// 创建文件信息
        /// </summary>
        /// <param name="sha256"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<FileObject> CreateFileObject(string sha256, string userId)
        {
            var fileObj = new FileObject()
            {
                SHA256 = sha256,
                UserId = userId
            };
            fileObj = await Create(fileObj);
            return fileObj;
        }

        /// <summary>
        /// 更新磁盘文件信息
        /// </summary>
        /// <returns></returns>
        public async Task<FileObject> UpdatePhysicalFileInfo(string fileId, string objectName,string fileName, long fileSize)
        {
            var diskFile = await GetFirstOrDefault<PhysicalFileObject>(x => x.Id == fileId) ?? throw new NullReferenceException($"文件:{fileName} 不存在");

            // 更新文件信息
            diskFile.FileName = fileName;
            diskFile.FileSize = fileSize;
            diskFile.ObjectName = objectName;
            diskFile.FileStatus = FileStatus.Ok;
            LiteRepository.Update(diskFile);

            return diskFile;
        }
    }
}
