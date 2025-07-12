namespace tourly.Models
{
    public class Package
    {
        public int Id { get; set; }
        public string Destination { get; set; }
        public string Price{ get; set; }
        public string Image{ get; set; }
        public string Creator {  get; set; }
        public string CreatorName { get; set; }
        public string Date{ get; set; }
        public string Description { get; set; }
    }
}
