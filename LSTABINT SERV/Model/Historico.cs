namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Historico")]
    public partial class Historico
    {
        public int Id { get; set; }

        [StringLength(25)]
        public string Tag { get; set; }

        [StringLength(35)]
        public string Delegacion { get; set; }

        [StringLength(35)]
        public string Plaza { get; set; }

        [StringLength(10)]
        public string Cuerpo { get; set; }

        [StringLength(10)]
        public string Carril { get; set; }

        public DateTime Fecha { get; set; }

        [StringLength(10)]
        public string Clase { get; set; }

        [StringLength(10)]
        public string Evento { get; set; }

        public double Saldo { get; set; }

        [StringLength(20)]
        public string Operador { get; set; }

        [StringLength(20)]
        public string SaldoAnterior { get; set; }

        [StringLength(20)]
        public string SaldoActualizado { get; set; }

        [StringLength(20)]
        public string NumeroCuenta { get; set; }

        [StringLength(20)]
        public string TipoCuenta { get; set; }
    }
}
