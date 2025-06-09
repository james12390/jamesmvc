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

            [Required]
            public string SenderName { get; set; }

            [Required]
            public string SenderPhone { get; set; }


            [Required]
            public string SenderCity { get; set; }
            [Required]  
            public string SenderDistrict { get; set; }

            [Required]
            public string SenderAddress { get; set; }
           
            [Required]
            public string ReceiverName { get; set; }

            [Required]
            public string ReceiverPhone { get; set; }

        [Required]
            public string ReceiverCity { get; set; }
            [Required]
            public string ReceiverDistrict { get; set; }

            [Required]
            public string ReceiverAddress { get; set; }


            public string ItemDescription { get; set; }

            [DataType(DataType.Date)]
            public DateTime DeliveryDate { get; set; }
            public string? AssignedDriverId { get; set; }
            [ForeignKey("AssignedDriverId")]
            public IdentityUser? AssignedDriver { get; set; }
    }
    
}
