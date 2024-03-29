namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Tags
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string NumTag { get; set; }

        [StringLength(20)]
        public string SaldoTag { get; set; }

        public bool StatusTag { get; set; }

        public bool StatusResidente { get; set; }

        public DateTime DateTTag { get; set; }

        public long CuentaId { get; set; }

        [Required]
        [StringLength(128)]
        public string IdCajero { get; set; }

        public virtual CuentasTelepeajes CuentasTelepeajes { get; set; }
    }
}
