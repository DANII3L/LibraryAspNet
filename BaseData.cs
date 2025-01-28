using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace LibraryBaseData
{
    public class BaseData
    {
        //Variable global de lista parametros
        private static List<ClassParameters> listParameters = new List<ClassParameters>();

        //Executes
        #region Executes
        //Execute Json Required
        #region ExecuteProcedureJsonRequired
        public DataSet ExecuteProcedure(string ConectionString, string Json, string NameProcedure)
        {
            //DataSet de respuesta sobre el metodo
            DataSet dtDates = new DataSet();
            DataTable RespProcedimiento = new DataTable();
            DataColumn column = RespProcedimiento.Columns.Add("MensajeProcedimiento");
            RespProcedimiento.Rows.Add(column);
            RespProcedimiento.Columns.Add("MensajeProceso");
            //RespProcedimiento.Rows.Add(column1);
            try
            {
                //Casting de JSON String a Class Token
                JToken token = JObject.Parse(Json);

                //Casting de parametros del JSON a Lista de parametros
                DesealizeParameters(token);

                //Añadir Tabla de respuesta de procedimiento actual
                RespProcedimiento.Rows[0]["MensajeProceso"] = "Proceso principal correcto";
                try
                {
                    //Generar conexión
                    SqlConnection ConDb = new SqlConnection(ConectionString);

                    // Conectar
                    ConDb.Open();

                    // Construir llamado a procedimiento
                    SqlCommand cmd = ConDb.CreateCommand();
                    cmd.CommandText = NameProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;

                    //Creación de parametros a enviar a partir de la lista llenada
                    foreach (var item in listParameters)
                    {
                        cmd.Parameters.Add(item.NameParameter, item.DbType).Value = Convert.ChangeType(item.Value, item.TypeCode);
                    }

                    // Ejecutar procedimiento y llenar dataset
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtDates);

                    // Cerrar y desechar conexion
                    ConDb.Close();
                    ConDb.Dispose();

                    //Añadir Tabla de respuesta de procedimiento actual
                    RespProcedimiento.Rows[0]["MensajeProcedimiento"] = "Operación consultada correctamente";
                }
                catch (Exception ex)
                {
                    //Añadir Tabla de respuesta de procedimiento actual
                    RespProcedimiento.Rows[0]["MensajeProcedimiento"] = "Operación erronea en procedimiento busqueda " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                //Añadir Tabla de respuesta de procedimiento actual
                RespProcedimiento.Rows[0]["MensajeProceso"] = "Operación erronea " + ex.Message;
            }

            dtDates.Tables.Add(RespProcedimiento);

            //Retorno de datos encontrados
            return dtDates;
        }
        #endregion

        //Json dont requerid
        #region ExecuteProcedureNoJson
        public DataSet ExecuteProcedure(string ConectionString, string NameProcedure)
        {
            //DataSet de respuesta sobre el metodo
            DataSet dtDates = new DataSet();
            DataTable RespProcedimiento = new DataTable();
            DataColumn column = RespProcedimiento.Columns.Add("MensajeProcedimiento");
            RespProcedimiento.Rows.Add(column);

            try
            {
                //Generar conexión
                SqlConnection ConDb = new SqlConnection(ConectionString);

                // Conectar
                ConDb.Open();

                // Construir llamado a procedimiento
                SqlCommand cmd = ConDb.CreateCommand();
                cmd.CommandText = NameProcedure;
                cmd.CommandType = CommandType.StoredProcedure;

                // Ejecutar procedimiento y llenar dataset
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtDates);

                // Cerrar y desechar conexion
                ConDb.Close();
                ConDb.Dispose();

                //Añadir Tabla de respuesta de procedimiento actual
                RespProcedimiento.Rows[0]["MensajeProcedimiento"] = "Operación consultada correctamente";
            }
            catch (Exception ex)
            {
                //Añadir Tabla de respuesta de procedimiento actual
                RespProcedimiento.Rows[0]["MensajeProcedimiento"] = "Operación erronea en procedimiento busqueda " + ex.Message;
            }

            dtDates.Tables.Add(RespProcedimiento);

            //Retorno de datos encontrados
            return dtDates;
        }
        #endregion

        #endregion

        //Deserealize parameters
        #region DesealizeParameters
        private static void DesealizeParameters(JToken token)
        {
            //Inicializar lista de parametros vacía
            listParameters = new List<ClassParameters>();

            //Casting de datos del JToken class por dato de sus valores
            foreach (JToken item in token.Children().ToList<JToken>())
            {
                JToken _material = item;

                //Metodo de identificación de tipo de dato a llenar en la lista de parametros
                IdentifyType(_material);
            }

        }
        #endregion

        //Identify types data
        #region IdentifyType
        private static void IdentifyType(JToken _material)
        {
            //Tipo de dato a validar
            string Type = ((JProperty)_material).Value.Type.ToString();
            //Clase de parametros a llenar dentro de la lista de parametros
            ClassParameters parameters = new ClassParameters();

            //Validar tipo de dato
            if (Type.Equals("Integer"))
            {
                parameters.NameParameter = ((JProperty)_material).Name;
                parameters.Value = ((JProperty)_material).Value;
                parameters.DbType = SqlDbType.Int;
                parameters.TypeCode = TypeCode.Int64;
            }
            else if (Type.Equals("String"))
            {
                parameters.NameParameter = ((JProperty)_material).Name;
                parameters.Value = ((JProperty)_material).Value;
                parameters.DbType = SqlDbType.VarChar;
                parameters.TypeCode = TypeCode.String;
            }
            else if (Type.Equals("Date"))
            {
                parameters.NameParameter = ((JProperty)_material).Name;
                parameters.Value = ((JProperty)_material).Value;
                parameters.DbType = SqlDbType.DateTime;
                parameters.TypeCode = TypeCode.DateTime;
            }
            else if (Type.Equals("Decimal"))
            {
                parameters.NameParameter = ((JProperty)_material).Name;
                parameters.Value = ((JProperty)_material).Value;
                parameters.DbType = SqlDbType.Decimal;
                parameters.TypeCode = TypeCode.Decimal;
            }
            else
            {
                parameters.NameParameter = ((JProperty)_material).Name;
                parameters.Value = ((JProperty)_material).Value;
            }

            //Llenado de lista de parametros a enviar
            listParameters.Add(parameters);
        }
        #endregion

        //Validar si dentro del Json hay otro objeto
        #region ValidateJSON
        private static bool ValidateJSON(string s)
        {
            try
            {
                JToken.Parse(s);
                return true;
            }
            catch (JsonReaderException ex)
            {
                Trace.WriteLine(ex);
                return false;
            }
        }
        #endregion

    }



}
