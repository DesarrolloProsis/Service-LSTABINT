namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HistoricoListas
    {
        public long Id { get; set; }

        public string Extension { get; set; }

        public string Tamaño { get; set; }

        public DateTime Fecha_Creacion { get; set; }

        public string Tipo { get; set; }
    }
}
