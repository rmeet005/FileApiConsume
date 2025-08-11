namespace FileApiConsume.Models
{
    public class File
    {

        public int Id { get; set; }
        public string ImgName { get; set; }
        public string ImgPath { get; set; }
        public bool Isdeleted { get; set; } = false;
    }
}
