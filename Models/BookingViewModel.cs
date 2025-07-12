namespace tourly.Models
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public DateTime? BookingDate { get; set; }
    }

}
