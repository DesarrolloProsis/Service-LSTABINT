namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ListaNegra")]
    public partial class ListaNegra
    {
        public long Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Tipo { get; set; }

        [Required]
        [StringLength(50)]
        public string Numero { get; set; }

        [Required]
        [StringLength(280)]
        public string Observacion { get; set; }

        public double? SaldoAnterior { get; set; }

        public DateTime Date { get; set; }

        [Required]
        [StringLength(128)]
        public string IdCajero { get; set; }

        [StringLength(30)]
        public string Clase { get; set; }

        [StringLength(30)]
        public string NumCliente { get; set; }

        [StringLength(30)]
        public string NumCuenta { get; set; }
    }
}
