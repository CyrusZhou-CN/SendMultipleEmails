using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using HttpMultipartParser;
using Server.Config;
using Server.Database.Extensions;
using Server.Database.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    /// <summary>
    /// 文件接口
    /// </summary>
    class Ctrler_File : BaseControllerAsync
    {
        /// <summary>
        /// 新增文件
        /// </summary>
        [Route(HttpVerbs.Post, "/file")]
        public async Task UploadFile()
        {
            // 找到当前的用户名
            var userId = Token.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                await ResponseErrorAsync("需要登陆才能上传");
                return;
            }

            // 获取文件流
            var parser = MultipartFormDataParser.Parse(Request.InputStream);

            if (parser.Files.Any() == false)
                throw new Exception("Invalid request, you need to post a file");

            // 获取子目录
            var subPaths = parser.Parameters.Where(p => p.Name == Fields.subPath);

            string subPath = "files";
            if (subPaths.Count() > 0) subPath = subPaths.First().Data;

            List<string> fileIds = new List<string>();

            // 可能会同时上传多个文件
            foreach (var file in parser.Files)
            {
                byte[] fileData;
                using (var ms = new MemoryStream())
                {
                    file.Data.CopyTo(ms);
                    fileData = ms.ToArray();
                }
                // 获取文件名
                var fileId = $"_{userId}/{subPath}/{file.FileName}";
                // 构建文件信息对象
                var fileInfo = new FileAttachment
                {
                    FileId = fileId,
                    UserId = userId,
                    SubPath = subPath,
                    FileName = file.FileName,
                    FileData = fileData
                };
                // 保存到数据库中
                SqlDb.Insert(fileInfo);
                fileIds.Add(fileId);
            }

            // 返回id
            await ResponseSuccessAsync(fileIds);
        }

        [Route(HttpVerbs.Get, "/file")]
        public async Task DownloadFile([QueryField] string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                await ResponseErrorAsync("fileId 为空");
                return;
            }

            var file = SqlDb.FindById<FileAttachment>(fileId);
            if (file != null)
            {
                using (var stream = HttpContext.OpenResponseStream())
                {
                    HttpContext.Response.ContentType = GetContentType(file.FileName);
                    stream.Write(file.FileData, 0, file.FileData.Length);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 404;
                await ResponseErrorAsync("File not found");
            }
        }
        // 获取文件的ContentType
        private string GetContentType(string fileName)
        {
            // 根据文件扩展名或MIME类型返回相应的ContentType
            // 这里可以根据实际情况进行更改
            string contentType = "application/octet-stream"; // 默认为二进制流
            string extension = Path.GetExtension(fileName)?.ToLowerInvariant();

            switch (extension)
            {
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".doc":
                case ".docx":
                    contentType = "application/msword";
                    break;
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".bmp":
                    contentType = "image/bmp";
                    break;
                case ".webp":
                    contentType = "image/webp";
                    break;
                    // 其他图片文件类型可以继续添加
            }
            return contentType;
        }
    }
}
