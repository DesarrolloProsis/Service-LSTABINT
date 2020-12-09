namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OperacionesCajeroes
    {
        public long Id { get; set; }

        [Required]
        [StringLength(70)]
        public string Concepto { get; set; }

        [StringLength(30)]
        public string TipoPago { get; set; }

        public double? Monto { get; set; }

        public DateTime DateTOperacion { get; set; }

        public long CorteId { get; set; }

        [Required]
        [StringLength(50)]
        public string Numero { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; }

        public double? CobroTag { get; set; }

        public string NoReferencia { get; set; }

        public bool StatusCancelacion { get; set; }

        public virtual CortesCajeroes CortesCajeroes { get; set; }
    }
}
