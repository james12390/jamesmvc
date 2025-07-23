using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jamesmvc.Models
{
        public class LogisticsOrder
    {
            public int Id { get; set; }

            public string OrderNumber { get; set; }


            [Required(ErrorMessage ="寄件人不得為空")]
            public string SenderName { get; set; }

            [Required(ErrorMessage = "電話不得為空")]
            public string SenderPhone { get; set; }

            [Required(ErrorMessage = "縣市不得為空")]
            public string SenderCity { get; set; }
            [Required(ErrorMessage = "區不得為空")]  
            public string SenderDistrict { get; set; }

            [Required(ErrorMessage = "地址不得為空")]
            public string SenderAddress { get; set; }
           


            [Required(ErrorMessage = "收件人不得為空")]
            public string ReceiverName { get; set; }

            [Required(ErrorMessage = "電話不得為空")]
            public string ReceiverPhone { get; set; }

            [Required(ErrorMessage = "區不得為空")]
            public string ReceiverCity { get; set; }
            [Required(ErrorMessage = "縣市不得為空")]
            public string ReceiverDistrict { get; set; }

            [Required(ErrorMessage = "地址不得為空")]
            public string ReceiverAddress { get; set; }

            [Required]
            [MaxLength(20)]
            public string DeliveryTimeSlot { get; set; }


            [Required]
            [MaxLength(20)]
            public string PackageSize { get; set; }


            [Required]
            [MaxLength(10)]
            public string Priority { get; set; }

            public string? Description { get; set; }


            public string? ItemDescription { get; set; }

            [DataType(DataType.Date)]
            public DateTime DeliveryDate { get; set; }
            public string? AssignedDriverId { get; set; }
            [ForeignKey("AssignedDriverId")]
            public IdentityUser? AssignedDriver { get; set; }
    }
    
}
