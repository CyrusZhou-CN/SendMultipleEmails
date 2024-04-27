namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 邮件地址
    /// </summary>
    public class EmailAddress
    {
        public string Email { get; set; }

        private string _name;
        public string Name
        {
            get { return string.IsNullOrEmpty(_name) ? _name : Email; }
            set { _name = value; }
        }
    }
}
