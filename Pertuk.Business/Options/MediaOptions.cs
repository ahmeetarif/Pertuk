namespace Pertuk.Business.Options
{
    public class MediaOptions
    {
        private string emptyProfilePicture;

        public string SitePath { get; set; }
        public string TemplateDirectoryPath { get; set; }
        public string DefaultsDirectoryPath { get; set; }
        public string EmptyProfilePicture
        {
            get { return emptyProfilePicture; }
            set
            {
                string newValue = SitePath + DefaultsDirectoryPath + value;
                emptyProfilePicture = newValue;
            }
        }
        public string PostsDirectoryPath { get; set; }
    }
}