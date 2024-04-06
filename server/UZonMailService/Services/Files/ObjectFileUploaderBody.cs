namespace UZonMailService.Services.Files
{
    /// <summary>
    /// 上传的文件体
    /// </summary>
    public class ObjectFileUploaderBody
    {
        public string Sha256 { get; set; }
        public DateTime LastModifyDate { get; set; }
        public bool IsPublic { get; set; }
        public IFormFile FormFile { get; set; }
        public string? UniqueName { get; set; }
    }
}
