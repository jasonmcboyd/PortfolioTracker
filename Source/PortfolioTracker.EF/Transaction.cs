namespace PortfolioTracker.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Transaction")]  
    public partial class Transaction
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        [Required]
        [StringLength(16)]
        public string TickerSymbol { get; set; }

        public DateTime TransactionDate { get; set; }

        [Required]
        [StringLength(1)]
        public string TransactionType { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public virtual Account Account { get; set; }
    }
}
