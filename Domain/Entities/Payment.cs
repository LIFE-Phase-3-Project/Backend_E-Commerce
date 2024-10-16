﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentStatus { get; set; }

        public string PaymentMethod {  get; set; }

        public string TransactionId { get; set; } 
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        [JsonIgnore]
        public Order Order { get; set; }
    }
}
