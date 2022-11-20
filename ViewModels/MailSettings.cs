
namespace BlogProj_12_10_22.ViewModels
{
    public class MailSettings
    {
        //this is to configure and use an smtp server
        // from google for example
        public string? Mail { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }

    }
}
