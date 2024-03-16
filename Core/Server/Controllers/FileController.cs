using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Uamazing.UZonEmail.Server.Models;
using Uamazing.UZonEmail.Server.Modules.DotNETCore.Multipart;
using Uamazing.UZonEmail.Server.Services;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.FileUpload;
using Uamazing.Utils.Web.ResponseModel;
using Uamazing.UZonEmail.Server.Services.Settings;

namespace Uamazing.UZonEmail.Server.Controllers
{
    /// <summary>
    /// 文件模块
    /// </summary>
    public class FileController : ControllerBaseV1
    {
        private FileObjectService _fileService;
        private TokenService _tokenService;

        public FileController(ILiteRepository liteRepository, FileObjectService fileService, TokenService tokenService)
        {
            _fileService = fileService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// 预获取文件
        /// </summary>
        /// <param name="sha256"></param>
        /// <returns></returns>
        [HttpGet("presigned")]
        public async Task<ResponseResult<FileObject>> PresignedGetObject([FromQuery] string sha256)
        {
            var fileObj = await _fileService.GetFileObject(sha256);
            if (fileObj == null)
            {
                // 新建一个
                var (userId, _) = _tokenService.GetTokenInfo();
                fileObj = await _fileService.CreateFileObject(sha256, userId);
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
