namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AmountConfigurations
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Concept { get; set; }

        public short Type { get; set; }

        [Key]
        [Column(Order = 1)]
        public double Amount { get; set; }
    }
}
