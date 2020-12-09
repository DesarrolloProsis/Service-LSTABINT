namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Clientes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Clientes()
        {
            CuentasTelepeajes = new HashSet<CuentasTelepeajes>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(30)]
        public string NumCliente { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(150)]
        public string Apellidos { get; set; }

        [StringLength(150)]
        public string EmailCliente { get; set; }

        [StringLength(300)]
        public string AddressCliente { get; set; }

        [StringLength(50)]
        public string PhoneCliente { get; set; }

        public bool StatusCliente { get; set; }

        public DateTime DateTCliente { get; set; }

        [Required]
        [StringLength(128)]
        public string IdCajero { get; set; }

        public string Empresa { get; set; }

        public string CP { get; set; }

        public string Pais { get; set; }

        public string City { get; set; }

        public string Departamento { get; set; }

        public string NIT { get; set; }

        public string PhoneOffice { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CuentasTelepeajes> CuentasTelepeajes { get; set; }
    }
}
