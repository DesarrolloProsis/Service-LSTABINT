namespace LSTABINT_SERV.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Parametrizables
    {
        public int id { get; set; }

        public string origen { get; set; }

        public string destino { get; set; }

        public int extension { get; set; }

        public string destinoresidente { get; set; }

        public short cruzes { get; set; }

        public short minutos { get; set; }
    }
}
