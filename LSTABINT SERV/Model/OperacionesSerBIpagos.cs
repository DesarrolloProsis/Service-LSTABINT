//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class OperacionesSerBIpagos
    {
        public long Id { get; set; }
        public string NumAutoriProveedor { get; set; }
        public string NumAutoriBanco { get; set; }
        public Nullable<double> SaldoAnterior { get; set; }
        public Nullable<double> SaldoModificar { get; set; }
        public Nullable<double> SaldoActual { get; set; }
        public bool StatusOperacion { get; set; }
        public System.DateTime DateTOpSerBI { get; set; }
        public long TagId { get; set; }
        public string Numero { get; set; }
        public string NoReferencia { get; set; }
        public string Tipo { get; set; }
        public string Concepto { get; set; }
    
        public virtual Tags Tags { get; set; }
    }
}
