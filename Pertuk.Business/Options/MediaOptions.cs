namespace Pertuk.Business.Options
{
    public class MediaOptions
    {
        private string _emptyProfilePicture;

        public string SitePath { get; set; }
        public string StorageZoneName { get; set; }
        public string TemplateDirectoryPath { get; set; }
        public string DefaultsDirectoryPath { get; set; }
        public string ProfileImages { get; set; }
        public string EmptyProfilePicture
        {
            get { return _emptyProfilePicture; }
            set
            {
                string newValue = DefaultsDirectoryPath + value;
                _emptyProfilePicture = newValue;
            }
        }
        public string PostsDirectoryPath { get; set; }
    }
}