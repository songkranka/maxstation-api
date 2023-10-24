using MaxStation.Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Repositories
{
    public class BaseRepositories
    {
        protected PTMaxstationContext Context;
        public BaseRepositories(PTMaxstationContext context)
        {
            this.Context = context;
        }

        public  List<T> RawSqlQuery<T>(string query, Func<DbDataReader, T> map)
        {
          
            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                Context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var entities = new List<T>();

                    while (result.Read())
                    {
                        entities.Add(map(result));
                    }

                    return entities;
                }
            }
            
        }

        public async Task<T> GetEntityFromSql<T>(PTMaxstationContext context, string pStrSql)
        {
            T result = default(T);
            using (DataTable dt = await GetDataTable(context, pStrSql))
            {
                result = GetEntityFromDataTable<T>(dt);
            }
            return result;
        }

        public  async Task<DataTable> GetDataTable(PTMaxstationContext pContext, string pStrSql)
        {
            var conDb = pContext.Database.GetDbConnection();
            if (!(conDb is SqlConnection))
            {
                return null;
            }
            pStrSql = GetString(pStrSql);
            if (0.Equals(pStrSql.Length))
            {
                return null;
            }
            DataTable result = new DataTable();
            using (var da = new SqlDataAdapter(pStrSql, conDb as SqlConnection))
            {
                await Task.Run(() => da.Fill(result));
            }
            return result;
        }

        public T GetEntityFromDataTable<T>(DataTable pDataTable)
        {
            if (pDataTable == null || 0.Equals(pDataTable.Rows.Count))
            {
                return default(T);
            }
            Func<string, string> funcMapColName = null;
            funcMapColName = x =>
            {
                string[] arrSplit = x.Split("_");
                for (int i = 0; i < arrSplit.Length; i++)
                {
                    arrSplit[i] = arrSplit[i].ToLower();
                    char[] arrChar = arrSplit[i].ToCharArray();
                    arrChar[0] = arrChar[0].ToString().ToUpper()[0];
                    arrSplit[i] = new string(arrChar);
                }
                return string.Join(string.Empty, arrSplit);
            };
            List<DataColumn> arrOriginalCol = pDataTable.Columns.OfType<DataColumn>().ToList();
            foreach (DataColumn item in pDataTable.Columns)
            {
                item.ColumnName = funcMapColName(item.ColumnName);
            }
            T result = ConvertObject<T>(pDataTable);
            return result;
        }

        public T ConvertObject<T>(object pObjInput)
        {
            if (pObjInput == null)
            {
                return default(T);
            }
            var serialized = JsonConvert.SerializeObject(pObjInput);
            var result = JsonConvert.DeserializeObject<T>(serialized);
            return result;
        }
        public string GetString(object pInput, string pStrDefault = "")
        {
            if (pInput == null || Convert.IsDBNull(pInput))
            {
                return pStrDefault;
            }
            string result = (pInput?.ToString() ?? string.Empty).Trim();
            if (0.Equals(result.Length))
            {
                return pStrDefault;
            }
            return result;
        }

    }
}
