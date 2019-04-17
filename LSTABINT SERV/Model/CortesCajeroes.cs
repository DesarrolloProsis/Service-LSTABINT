namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CortesCajeroes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CortesCajeroes()
        {
            OperacionesCajeroes = new HashSet<OperacionesCajeroes>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(20)]
        public string NumCorte { get; set; }

        public DateTime DateTApertura { get; set; }

        public DateTime? DateTCierre { get; set; }

        public double? MontoTotal { get; set; }

        [StringLength(300)]
        public string Comentario { get; set; }

        [StringLength(128)]
        public string IdCajero { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OperacionesCajeroes> OperacionesCajeroes { get; set; }
    }
}
