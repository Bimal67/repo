namespace tourly.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedAt { get; set; } = default(DateTime?);
    }
}
