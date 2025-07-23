using System.ComponentModel.DataAnnotations;

namespace jamesmvc.Models
{
    public class CustomerMessage
    {
        [Key]
        public int Id { get; set; }

        public string SenderId { get; set; }  

        public string ReceiverId { get; set; }

        [Required(ErrorMessage = "請輸入訊息內容")]
        [MaxLength(1000)]
        public string Message { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool IsFromCustomer { get; set; } = true;
    }
}
