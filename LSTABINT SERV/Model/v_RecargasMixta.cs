namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_RecargasMixta
    {
        [StringLength(50)]
        public string Numero { get; set; }

        [StringLength(50)]
        public string Tipo { get; set; }

        public double? Monto { get; set; }

        [Key]
        [Column(Order = 0)]
        public DateTime FechaOperacion { get; set; }

        [StringLength(30)]
        public string TipoPago { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string Origen { get; set; }
    }
}
