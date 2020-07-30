namespace PortfolioTracker.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Position")]
    public partial class Position
    {
        public int Id { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public int AccountId { get; set; }

        [Required]
        [StringLength(16)]
        public string TickerSymbol { get; set; }

        public int Quantity { get; set; }

        public decimal CostBasis { get; set; }

        public virtual Account Account { get; set; }
    }
}
