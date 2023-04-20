using LiteDB;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net;
using Uamazing.SME.Server.Helpers.DotNETCore.Multipart;
using Uamazing.SME.Server.Models;
using Uamazing.SME.Server.Services;
using Uamazing.Utils.DotNETCore.Token;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.FileUpload;
using Uamazing.Utils.Web.ResponseModel;

namespace Uamazing.SME.Server.Controllers
{
    /// <summary>
    /// 文件模块
    /// </summary>
    public class FileController : SMEControllerBase
    {
        private FileObjectService _fileService;
        private TokenParams _tokenParams;

        public FileController(ILiteRepository liteRepository,FileObjectService fileService, IOptions<TokenParams> options)
        {
            _fileService = fileService;
            _tokenParams = options.Value;
        }

        /// <summary>
        /// 预获取文件
        /// </summary>
        /// <param name="sha256"></param>
        /// <returns></returns>
        [HttpGet("presigned")]
        public async Task<ResponseResult<FileObject>> PresignedGetObject([FromQuery] string sha256)
        {
            var fileObj =await _fileService.GetFileObject(sha256);
            if(fileObj == null)
            {
                // 新建一个
                var (userId, _) = GetTokenInfo(_tokenParams);
                fileObj =await _fileService.CreateFileObject(sha256, userId);
            }

            return fileObj.ToSuccessResponse();
        }

        /// <summary>
        /// 上传文件，仅支持上传一个文件
        /// 上传文件之前，通过 <see cref="PresignedGetObject"/> 来获取 fileId
        /// </summary>
        /// <returns></returns>
        [HttpPost("multipart")]
        [DisableFormValueModelBinding]
        public async Task<ResponseResult<FileObject>> UploadPhysicalFile()
        {
            // 获取保存的目录
            var dir = Path.Combine(PhysicalFileObject.DefaultBucketName, DateTime.Now.ToString("yyyy/MM/dd"));
            var formValueProvider = await FileStreamingHelper.StreamFiles(Request, dir);
            // 获取 fileId,用于保存到数据库中
            var fileId = formValueProvider.GetValue("fileId").FirstValue;
            var fileName = formValueProvider.GetValue("fileName").FirstValue;
            var safeFileName = formValueProvider.GetValue("safeFileName").FirstValue;
            var objectName = Path.Combine(DateTime.Now.ToString("yyyy/MM/dd"), safeFileName);
            var fileSize = formValueProvider.GetValue("fileSize").FirstValue;
            // 更新数据库
            var fileObj = await _fileService.UpdatePhysicalFileInfo(fileId, objectName, fileName, long.Parse(fileSize));

            return fileObj.ToSuccessResponse();
        }
    }
}
