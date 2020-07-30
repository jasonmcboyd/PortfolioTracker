namespace PortfolioTracker.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Transfer")]
    public partial class Transfer
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransferDate { get; set; }

        [Required]
        [StringLength(1)]
        public string TransferType { get; set; }

        public virtual Account Account { get; set; }
    }
}
