using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos30.Models
{
    public class Cartera
    {
        public int Abono { get; set; }
        public string Compania { get; set; }
        public int DocumentoCliente { get; set; }
        public string TipoDocumentoCliente { get; set; }
        public string Tercero { get; set; }

        public int VencimientoFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int ValorTotal { get; set; }

        public int Saldo { get; set; }
        public int SaldoCartera { get; set; }

       
    }
}