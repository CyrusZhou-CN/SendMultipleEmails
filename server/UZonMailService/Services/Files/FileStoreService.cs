﻿using Microsoft.EntityFrameworkCore;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.Files;
using UZonMailService.Services.UserInfos;
using UZonMailService.Utils.DotNETCore.Exceptions;

namespace UZonMailService.Services.Files
{
    /// <summary>
    /// 文件存储服务
    /// </summary>
    public class FileStoreService(SqlContext db, UserService userService, IWebHostEnvironment env) : IScopedService
    {
        /// <summary>
        /// 获取存在的文件对象
        /// </summary>
        /// <param name="sha256"></param>
        /// <returns></returns>
        public async Task<FileObject?> GetExistFileObject(string sha256)
        {
            return await db.FileObjects.FirstOrDefaultAsync(x => x.Sha256 == sha256);
        }

        private async Task<FileBucket> GetDefaultBucket()
        {
            var bucket = await db.FileBuckets.FirstOrDefaultAsync(x => x.IsDefault);
            if (bucket == null) throw new KnownException("未找到默认的存储位置");
            return bucket;
        }

        /// <summary>
        /// 第一个是相对路径，第二个是绝对路径
        /// </summary>
        /// <param name="fileBucket"></param>
        /// <param name="sha256"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        private Tuple<string, string> GetStorePath(FileBucket fileBucket, string sha256, string extension)
        {
            // 年/月/日/文件名
            string relativePath = DateTime.Now.ToString("yyyy/MM/dd") + $"/{sha256}{extension}";
            string fullPath = Path.Combine(fileBucket.RootDir, relativePath);
            return new Tuple<string, string>(relativePath, fullPath);
        }

        /// <summary>
        /// 上传文件对象
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sha256"></param>
        /// <param name="isPublic"></param>
        /// <param name="formFile"></param>
        /// <param name="lastModifyDate"></param>
        /// <returns></returns>
        public async Task<FileUsage> UploadFileObject(int userId, ObjectFileUploaderBody fileParams)
        {
            if (!string.IsNullOrEmpty(fileParams.UniqueName))
            {
                await DeleteFileObject(fileParams.Sha256);
            }

            var result = await db.RunTransaction(async ctx =>
            {
                // 判断是否存在文件
                FileObject? fileObject = await GetExistFileObject(fileParams.Sha256);
                if (fileObject == null)
                {
                    // 保存文件
                    var defaultBucket = await GetDefaultBucket();

                    // 计算保存位置
                    var pathResult = GetStorePath(defaultBucket, fileParams.Sha256, Path.GetExtension(fileParams.FormFile.FileName));

                    // 说明文件不存在
                    fileObject = new FileObject()
                    {
                        Sha256 = fileParams.Sha256,
                        Path = pathResult.Item1,
                        Size = fileParams.FormFile.Length,
                        LastModifyDate = fileParams.LastModifyDate,
                    };
                    db.FileObjects.Add(fileObject);
                }

                // 先保存使用
                FileUsage fileUsage = new FileUsage()
                {
                    OwnerUserId = userId,
                    FileObjectId = fileObject.Id,
                    IsPublic = fileParams.IsPublic,
                    FileName = fileParams.FormFile.FileName,
                };
                db.FileUsages.Add(fileUsage);
                fileObject.LinkCount += 1;

                await db.SaveChangesAsync();

                return fileUsage;
            });

            return result;
        }

        /// <summary>
        /// 删除文件，该接口仅在文件引用小于等于 1 时有效
        /// </summary>
        /// <param name="sha256"></param>
        private async Task DeleteFileObject(string sha256, int maxLinkCount = 1)
        {
            // 移除文件
            FileObject? fileObject = await db.FileObjects.Where(x => x.Sha256 == sha256 && x.LinkCount <= maxLinkCount)
                .Include(x => x.FileBucket)
                .FirstOrDefaultAsync();
            if (fileObject == null) return;

            // 删除文件
            string fullPath = Path.Combine(fileObject.FileBucket.RootDir, fileObject.Path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            db.FileObjects.Remove(fileObject);
        }

        /// <summary>
        /// 读取文件流
        /// </summary>
        /// <param name="fileUsageId"></param>
        /// <param name="onlyPublic">仅返回public文件</param>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public async Task<string> GetFileFullPath(int fileUsageId, bool onlyPublic = true)
        {
            FileUsage? fileUsage = await db.FileUsages
                .Include(x => x.FileObject)
                .ThenInclude(x => x.FileBucket)
                .FirstOrDefaultAsync(x => x.Id == fileUsageId);
            if (fileUsage == null) throw new KnownException("文件不存在");

            // 拼接文件路径
            if (onlyPublic && !fileUsage.IsPublic) throw new KnownException("无权访问该文件");

            string fullPath = Path.Combine(fileUsage.FileObject.FileBucket.RootDir, fileUsage.FileObject.Path);
            return fullPath;
        }

        /// <summary>
        /// 获取或新建一个文件使用记录 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileName"></param>
        /// <param name="sha256">传入时，必须要保证 sha256 是存在的</param>
        /// <returns></returns>
        public async Task<FileUsage> GetOrCreateFileUsage(int userId, string fileName, string sha256)
        {
            // 判断 sha256 是否存在
            FileObject? fileObject = await GetExistFileObject(sha256) ?? throw new FileNotFoundException();

            // 获取源文件
            var fileUsage = await db.FileUsages.FirstOrDefaultAsync(x => x.OwnerUserId == userId && x.FileObjectId == fileObject.Id);
            if (fileUsage != null) return fileUsage;

            // 若不存在，则要新建
            var result = await db.RunTransaction(async ctx =>
            {
                var fileUsageTemp = new FileUsage()
                {
                    FileName = fileName,
                    OwnerUserId = userId,
                    FileObjectId = fileObject.Id,
                    IsPublic = false,
                };
                ctx.FileUsages.Add(fileUsageTemp);

                // 更新文件引用次数
                fileObject.LinkCount += 1;
                return fileUsageTemp;
            });
            return result;
        }

        /// <summary>
        /// 获取静态根目录
        /// </summary>
        /// <returns></returns>
        public (string,string) GetStaticFileDirectory()
        {
            return (Path.Combine(env.ContentRootPath, "public"), "public");
        }

        /// <summary>
        /// 生成静态文件目录
        /// 第一个值是绝对路径
        /// 第二个值是 url 路径
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public (string,string) GenerateStaticFilePath(params string[] paths)
        {
            var root = GetStaticFileDirectory();
            var relativePath = Path.Combine(paths);
            var fullPath = Path.Combine(root.Item1, relativePath);

            string baseDir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);

            return (fullPath, $"public/{relativePath}".Replace('\\', '/'));
        }
    }
}
