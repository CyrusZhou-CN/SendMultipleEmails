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
        public async Task<ResponseResult<FileObject>> GetFileObject(string sha256)
        {
            // 判断是否有文件，如果有文件，则返回，否则返回空
            var fileObj = await GetFirstOrDefault<FileObject>(x=>x.SHA256 == sha256);
            return fileObj.ToSuccessResponse();
        }

        public async Task<ResponseResult<FileObject>> UploadFileObject()
        {
            // 生成文件记录

            // 保存文件

            // 返回记录
        }

        
    }
}
