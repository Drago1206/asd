using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos30.Models
{
    [DataContract]
    public class Cartera
    {
        public int pmAbono { get; set; }
        public string pmCompania { get; set; }
        public int pmDocumento { get; set; }
        public string pmTipoDocumento { get; set; }


        public int pmVencimiento { get; set; }
        public DateTime pmFechaEmision { get; set; }
        public DateTime pmFechaVencimiento { get; set; }
        public int pmValorTotal { get; set; }

        public int pmSaldo { get; set; }


        [DataMember]
        public int Abono { get { return pmAbono; } set { pmAbono = value; } }
        [DataMember]
        public string Compania { get { return pmCompania; } set { pmCompania = value; } }
        [DataMember]
        public int Documento { get { return pmDocumento; } set { pmDocumento = value; } }
        [DataMember]
        public string TipoDocumento { get { return pmTipoDocumento; } set { pmTipoDocumento = value; } }

        [DataMember]
        public int Vencimiento { get { return pmVencimiento; } set { pmVencimiento = value; } }
        [DataMember]
        public int ValorTotal { get { return pmValorTotal; } set { pmValorTotal = value; } }
        [DataMember]
        public DateTime FechaEmision { get { return pmFechaEmision; } set { pmFechaEmision = value; } }
        [DataMember]
        public DateTime FechaVencimiento { get { return pmFechaVencimiento; } set { pmFechaVencimiento = value; } }
        [DataMember]
        public int Saldo { get { return pmSaldo; } set { pmSaldo = value; } }

    }
    [DataContract]
    public class ItemCartera
    {

        [DataMember]
        public string Tercero { get; set; }
        [DataMember]
        public int SaldoCartera { get; set; }
        [DataMember]
        public List<Cartera> Detalle { get; set; }

    }
}