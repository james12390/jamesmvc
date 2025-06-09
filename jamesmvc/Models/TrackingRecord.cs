using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace jamesmvc.Models
{
    public class TrackingRecord
    {
        [Key]
        public int Id { get; set; }

        public int LogisticsOrderId { get; set; } // 關聯物流單

        [ForeignKey("LogisticsOrderId")]
        public LogisticsOrder Order { get; set; }

        [Required]
        [MaxLength(100)]
        public string Status { get; set; } // 例如：「配送中」、「已簽收」、「轉運中」

        [MaxLength(200)]
        public string Location { get; set; } // 例如：「台中轉運中心」

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
