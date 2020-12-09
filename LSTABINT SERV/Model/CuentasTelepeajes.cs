namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CuentasTelepeajes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CuentasTelepeajes()
        {
            Tags = new HashSet<Tags>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(30)]
        public string NumCuenta { get; set; }

        [StringLength(20)]
        public string SaldoCuenta { get; set; }

        [Required]
        [StringLength(20)]
        public string TypeCuenta { get; set; }

        public bool StatusCuenta { get; set; }

        public bool StatusResidenteCuenta { get; set; }

        public DateTime DateTCuenta { get; set; }

        public long ClienteId { get; set; }

        [Required]
        [StringLength(128)]
        public string IdCajero { get; set; }

        public virtual Clientes Clientes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tags> Tags { get; set; }
    }
}
