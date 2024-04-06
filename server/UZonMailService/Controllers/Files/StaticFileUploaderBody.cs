namespace UZonMailService.Controllers.Files
{
    public class StaticFileUploaderBody
    {
        /// <summary>
        /// 子路径
        /// </summary>
        public string SubPath { get; set; } = "default-upload";
        public IFormFile FormFile { get; set; }
    }
}
