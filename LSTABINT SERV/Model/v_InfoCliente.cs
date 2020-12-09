namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_InfoCliente
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(30)]
        public string NumCliente { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(150)]
        public string Apellidos { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(30)]
        public string NumCuenta { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(20)]
        public string SaldoCuenta { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(20)]
        public string TipoCuenta { get; set; }

        [Key]
        [Column(Order = 6)]
        public bool StatusCuenta { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(50)]
        public string NumTag { get; set; }

        [StringLength(20)]
        public string SaldoTag { get; set; }

        [Key]
        [Column(Order = 8)]
        public bool StatusTag { get; set; }
    }
}
