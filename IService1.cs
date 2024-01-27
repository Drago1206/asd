using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos30.Models;
using static WcfPruebas30.CarteraReq;

namespace WcfPruebas30
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IService1
    {

        /// <summary>
        /// Resources the obt cart  definition.
        /// </summary>
        /// <param name="Info">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerConsolidadoClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "ConsolidacionC")]
        RespClientes resClients(RespClientes Info);

        /// <summary>
        /// Resources the obt cart  definition.
        /// </summary>
        /// <param name="Info">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCartera", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "CarteraResponse")]
        CarteraResp RespCartera(CarteraReq ReqCartera);



        /// <summary>
        /// Resources the obt cart  definition.
        /// </summary>
        /// <param name="Info">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCarteraTotal", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "CarteraTotal")]
        CarteraRespTotal RespCarteraTotal(ObtCarteraTotal obtCarteraTotal);


    }


    [DataContract]
    public class Log
    {
        string _fecha;
        Int32 _registros;
        string _codigo;
        string _mensaje;

        [DataMember]
        public string Fecha
        {
            get { return _fecha; }
            set { _fecha = value; }
        }

        [DataMember]
        public string Codigo
        {
            get { return _codigo; }
            set { _codigo = value; }
        }

        [DataMember]
        public string Descripcion
        {
            get { return _mensaje; }
            set { _mensaje = value; }
        }
    }

    [DataContract]
    public class OrganizadorPagina
    {
        [DataMember]
        public int NumeroDePaginas { get; set; }

        [DataMember]
        public int PaginaActual { get; set; }

        [DataMember]
        public int RegistroPorPagina { get; set; }

        [DataMember]
        public int RegistroTotal { get; set; }

    }

    [DataContract]
    public class PaginaAcceder
    {
        /// <summary>
        /// Pagina que desea acceder.
        /// </summary>
        /// <value>
        /// La pagina.
        /// </value>
        [DataMember]
        public int Pagina { get; set; }

        /// <summary>
        /// Numero de registro por pagina.
        /// </summary>
        /// <value>
        /// Numero de registro por pagina.
        /// </value>
        [DataMember]
        public int NumRegistroPagina { get; set; }
    }




    [DataContract]
    public class RespClientes
    {
        Log _registro;
        List<ClienteResponse> _clientes;
        OrganizadorPagina organizadorPagina;

        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }

        [DataMember]
        public List<ClienteResponse> Clientes
        {
            get { return _clientes; }
            set { _clientes = value; }
        }

        [DataMember]
        public OrganizadorPagina paginas
        {
            get { return organizadorPagina; }
            set { organizadorPagina = value; }
        }
    }

    [DataContract]
    public class ObtInfoClientes
    {
        Usuario usuario;

        [DataMember]
        public Usuario _usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }

        [DataMember]
        public string NitCliente { get; set; }

        [DataMember]
        public PaginaAcceder Pagina
        {
            get { return Pagina; }
            set { Pagina = value; }
        }
    }
    [DataContract]
    public class CarteraReq {
        [DataMember]
        public string NitCliente { get; set; }

        [DataMember]
        public Usuario _usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
        [DataContract]
        public class CarteraResp {
            [DataMember]
            public Cartera cartera
            {
                get { return cartera; }
                set { cartera = value; }
            }
        }

        [DataContract]
        public class ObtCarteraTotal {

            [DataMember]
            public Usuario _usuario
            {
                get { return _usuario; }
                set { _usuario = value; }
            }
        }
        [DataContract]
        public class CarteraRespTotal {
            [DataMember]
            public Cartera cartera {
                get { return cartera; }
                set { cartera = value; }
            }
        }




    }

    
        
    
}
