using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos30.Models;
using WcfSyscom30.Conexion;

namespace WcfPruebas30
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {
        public int NumeroPagina = 1;
        public int ResultadoPorPagina = 10;
        public int InicioPaginacion = 0;
        public int FinPaginacion = 10;
        public int ResultadoTotal = 0;



        public RespClientes GetClientes(ObtInfoClientes obtenerConSolidado)
        {
            RespClientes ConsolidadoClientes = new RespClientes();
            ConsolidadoClientes.Registro = new Log();
            String cliente = "";
            ConexionBD Db = new ConexionBD();
            List<ClienteResponse> Cliente = new List<ClienteResponse>();
            if (obtenerConSolidado.Pagina.NumRegistroPagina > 0)
            {
                ResultadoPorPagina = ConsolidadoClientes.paginas.NumeroDePaginas;
                FinPaginacion = ResultadoPorPagina;
            }
            if (obtenerConSolidado.Pagina.Pagina > 0)
            {
                NumeroPagina = obtenerConSolidado.Pagina.Pagina;
                FinPaginacion = ResultadoPorPagina * NumeroPagina;
                InicioPaginacion = (FinPaginacion - ResultadoPorPagina) + 1;
            }
            if (obtenerConSolidado.NitCliente != null || String.IsNullOrWhiteSpace(obtenerConSolidado.NitCliente))
            {
                cliente = obtenerConSolidado.NitCliente;
                InicioPaginacion = 1;
                FinPaginacion = 1;
                NumeroPagina = 1;
            }
            return null;
            // Acceder al procedimiento de almacenamiento
            DataSet TablaClientes = new DataSet();
            List<SqlParameter> ListParametros = new List<SqlParameter>();
            ListParametros.Add(new SqlParameter("@NitCliente", cliente));
            ListParametros.Add(new SqlParameter("Inicio", InicioPaginacion));
            ListParametros.Add(new SqlParameter("Fin", FinPaginacion));

            if (Db.ejecutarQuery("WSConsolidacionClient", ListParametros, out TablaClientes, out string[] NuevoMensaje, CommandType.StoredProcedure))
            {
                ResultadoTotal = Convert.ToInt32(TablaClientes.Tables[0].Rows[0]["TotalFilas"]);
                if (ResultadoTotal > 0) {
                    if (NumeroPagina <= (int)Math.Ceiling((double)ResultadoTotal / ResultadoPorPagina))
                    {

                        IEnumerable<DataRow> data = TablaClientes.Tables[1].AsEnumerable();
                        IEnumerable<DataRow> dataFil = data.GroupBy(g => g.Field<string>("IdTercero")).Select(g => g.First());

                        dataFil.ToList().ForEach(i => Cliente.Add(new ClienteResponse
                        {
                            pmNitCliente = i.Field<string>("NitCliente"),
                            pmPaginaActual = i.Field<int>("PaginActual"),
                            pmRegistrosPorPagina = i.Field<int>("RegistrosXPagina"),
                            pmCiudad = i.Field<string>("Ciudad"),
                            pmNumLista = i.Field<int>("NumLista"),
                            pmDireccion = i.Field<string>("Direccion"),
                            pmNitVendedor = i.Field<string>("NitVendedor"),
                            pmNomVendedor = i.Field<string>("NombreVendedor")
                            

               
                        }));

                        Cliente.ForEach(c =>
                        {
                            if (data.Where(r => r.Field<string>("IdAgencia") != null).Count() > 0)
                            {
                                c.pmListaAgencia = new List<Agencia>();
                                data.Where(ca => ca.Field<string>("IdAgencia") != null).ToList().ForEach(i => c.pmListaAgencia.Add(new Agencia
                                {
                                    pmCodAge = i.Field<string>("CodAgencia"),
                                    pmNomAge = i.Field<string>("NomAge")
                                }));
                            }

                        });
                        ConsolidadoClientes.Clientes = Cliente;
                        ConsolidadoClientes.Registro = new Log { Codigo = "008", Descripcion = "Se ejecutó correctamente la consulta" };
                    }
                    else {
                        ConsolidadoClientes.Registro = new Log { Codigo = "009", Descripcion = "La Página que deseas acceder no está disponible porque solo cuentan con " + (int)Math.Ceiling((double)ResultadoTotal/ResultadoPorPagina)};
                    }


                }

            }

            return ConsolidadoClientes;

        }

     

  

        [return: MessageParameter(Name = "ConsolidacionC")]
        public RespClientes resClients(RespClientes Info)
        {
            throw new NotImplementedException();
        }

        [return: MessageParameter(Name = "CarteraResponse")]
        public CarteraReq.CarteraResp RespCartera(CarteraReq ReqCartera)
        {
            throw new NotImplementedException();
        }

        [return: MessageParameter(Name = "CarteraTotal")]
        public CarteraReq.CarteraRespTotal RespCarteraTotal(CarteraReq.ObtCarteraTotal obtCarteraTotal)
        {
            throw new NotImplementedException();
        }

        //Obtener cartera metodo para modificar
        //[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCartera", BodyStyle = WebMessageBodyStyle.Bare)]
        //[return: MessageParameter(Name = "Cartera")]
        //public ResObtenerCartera getCartera(DtCliente Modelo)
        //{
        //    ResObtenerCartera respuesta = new ResObtenerCartera();
        //    respuesta.Error = null;

        //    try
        //    {
        //        if (Modelo.Usuarios == null)
        //            respuesta.Error = new Errores { codigo = "USER_002", descripcion = "¡Todas las variables del usuario no pueden ser nulas!" };
        //        if (Modelo.Usuarios.UserName == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.UserName))
        //            respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El UserName no puede ser nulo o vacío!" };
        //        else if (Modelo.Usuarios.Password == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.Password))
        //            respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El Password no puede ser nulo o vacío!" };
        //        else if (Modelo.Cliente.NitCliente == null || String.IsNullOrWhiteSpace(Modelo.Cliente.NitCliente))
        //            respuesta.Error = new Errores { codigo = "CLIEN_001", descripcion = "¡El NitCliente no puede ser nulo o vacío!" };
        //        else if (ExisteUsuario(Modelo.Usuarios))
        //        {
        //            DatosCartera dcl = new DatosCartera();
        //            List<DatosCartera> DatCartera = new List<DatosCartera>();
        //            respuesta.Error = dcl.ConsultarCartera(Modelo.Cliente.NitCliente, out DatCartera);
        //            if (respuesta.Error == null)
        //            {
        //                if (DatCartera == null)
        //                    respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
        //                else
        //                    respuesta.Datoscartera = DatCartera;
        //            }
        //        }
        //        else
        //            respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
        //    }
        //    catch (Exception ex)
        //    {
        //        respuesta.Error = new Errores { descripcion = ex.Message };
        //    }
        //    return respuesta;
        //}

        ////[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ConsolidadoClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        ////[return: MessageParameter(Name = "cliente")]
        ////public ResObtenerConsClientes getConsClientes(DtClientes Modelo)
        ////{
        ////    ResObtenerConsClientes respuesta = new ResObtenerConsClientes();
        ////    respuesta.Error = null;

        ////    try
        ////    {
        ////        if (Modelo.Usuarios == null)
        ////            respuesta.Error = new Errores { codigo = "USER_002", descripcion = "¡Todas las variables del usuario no pueden ser nulas!" };
        ////        else
        ////        {
        ////            if (Modelo.Usuarios.UserName == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.UserName))
        ////                respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El UserName no puede ser nulo o vacío!" };
        ////            else if (Modelo.Usuarios.Password == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.Password))
        ////                respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El Password no puede ser nulo o vacío!" };
        ////            //else if (Modelo.Cliente.NitCliente == null || String.IsNullOrWhiteSpace(Modelo.Cliente.NitCliente))
        ////            //    respuesta.Error = new Errores { codigo = "CLIEN_001", descripcion = "¡El NitCliente no puede ser nulo o vacío!" };
        ////            else if (ExisteUsuario(Modelo.Usuarios))
        ////            {
        ////                DatosCliente dcl = new DatosCliente();
        ////                PaginadorCliente<DatosCliente> DatCliente = new PaginadorCliente<DatosCliente>();
        ////                respuesta.Error = dcl.ConsultarConsCliente(Modelo.Clientes, out DatCliente);
        ////                if (respuesta.Error == null)
        ////                {
        ////                    if (DatCliente == null)
        ////                        respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
        ////                    else
        ////                        respuesta.ListadoClientes = DatCliente;
        ////                }
        ////            }
        ////            else
        ////                respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        respuesta.Error = new Errores { descripcion = ex.Message };
        ////    }

        ////    return respuesta;
        ////}


        //[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarPedido", BodyStyle = WebMessageBodyStyle.Bare)]
        //[return: MessageParameter(Name = "Pedido")]
        //public ResGenerarPedido setPedido(DtPedido Modelo)
        //{
        //    ResGenerarPedido respuesta = new ResGenerarPedido();
        //    respuesta.Error = null;
        //    List<SqlParameter> _parametros = new List<SqlParameter>();

        //    try
        //    {
        //        if (Modelo.Usuarios == null)
        //            respuesta.Error = new Errores { codigo = "USER_002", descripcion = "¡Todas las variables del usuario no pueden ser nulas!" };
        //        else
        //        {
        //            if (Modelo.Usuarios.UserName == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.UserName))
        //                respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El UserName no puede ser nulo o vacío!" };
        //            else if (Modelo.Usuarios.Password == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.Password))
        //                respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El Password no puede ser nulo o vacío!" };
        //            else if (Modelo.Pedido.IdCliente == null || String.IsNullOrWhiteSpace(Modelo.Pedido.IdCliente))
        //                respuesta.Error = new Errores { codigo = "GPED_001", descripcion = "¡El IdCliente no puede ser nulo o vacío!" };
        //            else if (Modelo.Pedido.CodConcepto == null || String.IsNullOrWhiteSpace(Modelo.Pedido.CodConcepto))
        //                respuesta.Error = new Errores { codigo = "GPED_001", descripcion = "¡El CodConcepto no puede ser nulo o vacío!" };
        //            else if (Modelo.Pedido.IdVendedor == null || String.IsNullOrWhiteSpace(Modelo.Pedido.IdVendedor))
        //                respuesta.Error = new Errores { codigo = "GPED_001", descripcion = "¡El IdVendedor no puede ser nulo o vacío!" };
        //            else if (Modelo.Pedido.Observación == null || String.IsNullOrWhiteSpace(Modelo.Pedido.Observación))
        //                respuesta.Error = new Errores { codigo = "GPED_001", descripcion = "¡La Observación no puede ser nula o vacía!" };
        //            else if (Modelo.Pedido.ListaProductos.Count == 0)
        //                respuesta.Error = new Errores { codigo = "GPED_003", descripcion = "¡No existen ningún producto para generar el pedido!" };
        //            else if (ExisteUsuario(Modelo.Usuarios))
        //            {
        //                DatosPedido dpd = new DatosPedido();
        //                List<DatosPedido> DatPedido = new List<DatosPedido>();
        //                List<Pedido> ppedido = new List<Pedido>();
        //                respuesta.Error = dpd.GenerarPedido(Modelo, out DatPedido);
        //                if (respuesta.Error == null)
        //                {
        //                    if (DatPedido == null)
        //                        respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
        //                    else
        //                        respuesta.DatosPedido = DatPedido;
        //                }

        //            }
        //            else
        //                respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        respuesta.Error = new Errores { descripcion = ex.Message };
        //    }

        //    return respuesta;
        //}








    }
}
