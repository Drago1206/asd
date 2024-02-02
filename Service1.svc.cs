using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos30.Model;
using WcfPedidos30.Models;
using WcfPruebas30.Models;
using WcfSyscom30.Conexion;
using static WcfPruebas30.CarteraReq;

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





        public CarteraResp RespCartera(CarteraReq ReqCartera)
        {
            ConexionBD con = new ConexionBD();


            CarteraResp respuesta = new CarteraResp();
            try
            {
                respuesta.Error = null;
                if (ReqCartera.usuario == null)
                    respuesta.Error = new Errores { codigo = "user_002", descripcion = "¡todas las variables del usuario no pueden ser nulas!" };
                if (ReqCartera.usuario.UserName == null || string.IsNullOrWhiteSpace(ReqCartera.usuario.UserName))
                    respuesta.Error = new Errores { codigo = "user_003", descripcion = "¡el username no puede ser nulo o vacío!" };
                else if (ReqCartera.usuario.Password == null || string.IsNullOrWhiteSpace(ReqCartera.usuario.Password))
                    respuesta.Error = new Errores { codigo = "user_003", descripcion = "¡el password no puede ser nulo o vacío!" };
                else if (ReqCartera.NitCliente == null || string.IsNullOrWhiteSpace(ReqCartera.NitCliente))
                    respuesta.Error = new Errores { codigo = "clien_001", descripcion = "¡el nitcliente no puede ser nulo o vacío!" };

                DataSet Tablainfo = new DataSet();
                Cartera cart = new Cartera();
                ItemCartera cartItem = new ItemCartera();
                cartItem.Detalle = new List<Cartera>();
                List<ItemCartera> datItemCart = new List<ItemCartera>();

                try
                {
                    con.setConnection("SyscomDBSAL");
                    List<SqlParameter> parametros = new List<SqlParameter>();

                    parametros.Add(new SqlParameter("@NitCliente", ReqCartera.NitCliente));
                    con.addParametersProc(parametros);

                    //Ejecuta procedimiento almacenado
                    DataTable DT = new DataTable();
                    con.resetQuery();

                    if (con.ejecutarQuery("ConsultarCartera", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                    {
                        IEnumerable<DataRow> data = Tablainfo.Tables[0].AsEnumerable()
                                                           .Where(row => row.Field<string>("Tercero") == ReqCartera.NitCliente);

                        foreach (DataRow row in data)
                        {
                            cartItem.Detalle.Add(new Cartera
                            {
                                Abono = Convert.ToInt32(row.Field<Decimal>("Abono")),
                                Documento = row.Field<int>("Documento"),
                                TipoDocumento = row.Field<string>("Tipodocumento"),
                                Compania = row.Field<string>("Compañia"),
                                Vencimiento = Convert.ToInt32(row.Field<Int16>("Vencimiento")),
                                ValorTotal = Convert.ToInt32(row.Field<Decimal>("ValorTotal")),
                                FechaEmision = row.Field<DateTime>("FechaEmision"),
                                FechaVencimiento = row.Field<DateTime>("Fechavencimiento"),
                                Saldo = Convert.ToInt32(row.Field<Decimal>("Saldo"))

                            });
                            datItemCart.Add(new ItemCartera
                            {
                                SaldoCartera = Convert.ToInt32(row.Field<Decimal>("SaldoCartera")),
                                Tercero = row.Field<string>("Tercero")
                            });
                            respuesta.DatosCartera = datItemCart;
                            respuesta.DatosCartera.Add(cartItem);
                        }



                    }

                }
                catch (Exception e)
                {
                    respuesta.Error = new Errores { descripcion = e.Message };
                }
            }
            catch (IOException ex)
            {
                respuesta.Error = new Errores { descripcion = ex.Message };
            }
            return respuesta;
        }


        public RespClientes resClients(ObtInfoClientes obtenerConSolidado)
        {
            RespClientes respuesta = new RespClientes();
            ClienteResponse agencia = new ClienteResponse();
            respuesta.Error = null;
            ConexionBD con = new ConexionBD();
            string cliente = "";
            List<ClienteResponse> clientes = new List<ClienteResponse>();



            try
            {
                if (obtenerConSolidado._usuario == null || String.IsNullOrWhiteSpace(obtenerConSolidado._usuario.UserName))
                    respuesta.Error = new Errores { codigo = "USER_002", descripcion = "¡Todas las variables del usuario no pueden ser nulas!" };
                else
                {
                    if (obtenerConSolidado._usuario.UserName == null || String.IsNullOrWhiteSpace(obtenerConSolidado._usuario.UserName))
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El UserName no puede ser nulo o vacío!" };
                    else if (obtenerConSolidado._usuario.Password == null || String.IsNullOrWhiteSpace(obtenerConSolidado._usuario.Password))
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El Password no puede ser nulo o vacío!" };
                    else if (obtenerConSolidado.NitCliente == null || String.IsNullOrWhiteSpace(obtenerConSolidado.NitCliente))
                        respuesta.Error = new Errores { codigo = "CLIEN_001", descripcion = "¡El NitCliente no puede ser nulo o vacío!" };
                    cliente = obtenerConSolidado.NitCliente;

                    try
                    {
                        con.setConnection("Syscom");
                        DataSet TablaCliente = new DataSet();
                        List<SqlParameter> parametros = new List<SqlParameter>();
                        parametros.Add(new SqlParameter("@NitCliente", obtenerConSolidado.NitCliente));
                        con.addParametersProc(parametros);
                        DataTable DT = new DataTable();
                        con.resetQuery();

                        if (con.ejecutarQuery("ConsolidacionClientes", parametros, out TablaCliente, out string[] NuevoMensaje, CommandType.StoredProcedure))
                        {
                            // Calcula el número total de elementos en la tabla 1 de TablaCliente
                            int ResultadoTotal = TablaCliente.Tables[0].Rows.Count;


                            int totalPaginas = (int)Math.Ceiling((double)ResultadoTotal / ResultadoPorPagina);

                            // Verifica si la página solicitada es válida
                            if (NumeroPagina <= totalPaginas)
                            {
                                // Filtrar los datos por el NIT del cliente
                                IEnumerable<DataRow> data = TablaCliente.Tables[0].AsEnumerable()
                                                            .Where(row => row.Field<string>("NitCliente") == obtenerConSolidado.NitCliente);

                                
                                clientes = con.DataTableToList<ClienteResponse>("NitCliente,NombreCliente,Direccion,Ciudad,Telefono,NumLista,NitVendedor,NomVendedor".Split(','), TablaCliente);
                                DataTable lista = TablaCliente.Tables[0];
                                clientes.ForEach(m =>
                                {
                                    m.ListaAgencia = new List<Agencia>();
                                    m.ListaAgencia = con.DataTableToList<Agencia>(lista.Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("NitCliente").Equals(m.NitCliente)).CopyToDataTable().AsDataView().ToTable(true, "CodAge,NomAge".Split(',')));
                                });

                                //// Procesar los datos filtrados
                                //foreach (DataRow row in data)
                                //{

                                //    clientes.Add(new ClienteResponse
                                //    {
                                //        Ciudad = row.Field<string>("Ciudad"),
                                //        Direccion = row.Field<string>("Direccion"),
                                //        NitCliente = row.Field<string>("NitCliente"),
                                //        NombreCliente = row.Field<string>("NombreCliente"),
                                //        NitVendedor = row.Field<string>("NitVendedor"),
                                //        NomVendedor = row.Field<string>("NomVendedor"),
                                //        NumLista = row.Field<int>("NumLista"),

                                //    });

                                //    agencia.ListaAgencia.Add(new Agencia
                                //    {
                                //        pmNomAge = row.Field<string>("NomAge"),
                                //        pmCodAge = row.Field<string>("CodAge")
                                //    });

                                //}

                                // Asignar la lista filtrada a RespClientes


                                respuesta.ListadoClientes = new PaginadorCliente<ClienteResponse> { Resultado = clientes };


                                respuesta.Error = new Errores { codigo = "008", descripcion = "Se ejecutó correctamente la consulta" };

                            }
                        }

                    }

                    catch (Exception e)
                    {

                        respuesta.Error = new Errores { descripcion = e.Message };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Error = new Errores { descripcion = ex.Message };
            }

            return respuesta;
        }

       
        //public CarteraRespTotal RespCarteraTotal(ObtCarteraTotal obtCarteraTotal)
        //{
        //    ConexionBD con = new ConexionBD();
        //    ConexionBD Conex = new ConexionBD();
        //    CarteraRespTotal respuesta = new CarteraRespTotal();
        //    respuesta.Error = null;
        //    if (obtCarteraTotal.usuario == null)
        //        respuesta.Error = new Errores { codigo = "user_002", descripcion = "¡todas las variables del usuario no pueden ser nulas!" };
        //    if (obtCarteraTotal.usuario.UserName == null || string.IsNullOrWhiteSpace(obtCarteraTotal.usuario.UserName))
        //        respuesta.Error = new Errores { codigo = "user_003", descripcion = "¡el username no puede ser nulo o vacío!" };
        //    else if (obtCarteraTotal.usuario.Password == null || string.IsNullOrWhiteSpace(obtCarteraTotal.usuario.Password))
        //        respuesta.Error = new Errores { codigo = "user_003", descripcion = "¡el password no puede ser nulo o vacío!" };
        //    else if (obtCarteraTotal.NitCliente == null || string.IsNullOrWhiteSpace(obtCarteraTotal.NitCliente))
        //        respuesta.Error = new Errores { codigo = "clien_001", descripcion = "¡el nitcliente no puede ser nulo o vacío!" };

        //    try {
                
        //        DataSet Tablainfo = new DataSet();
        //        con.setConnection("SyscomDBSYSCOMSOPORTE");
        //        Conex.setConnection("SyscomDBSAL");
        //        List<SqlParameter> parametros = new List<SqlParameter>();

        //        parametros.Add(new SqlParameter("@NitCliente", obtCarteraTotal.NitCliente));
        //        con.addParametersProc(parametros);

        //        //Ejecuta procedimiento almacenado
        //        DataTable DT = new DataTable();
        //        con.resetQuery();
                

        //    } catch {


        //    }

        //    return respuesta;
        //}


    }   
}
