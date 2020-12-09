namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_Recargas
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string Numero { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Tipo { get; set; }

        public double? Monto { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime DateTOperacion { get; set; }

        [StringLength(30)]
        public string TipoPago { get; set; }
    }
}
