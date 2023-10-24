using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;

namespace DailyOperation.API.Helpers
{
    public class SqlHelper : SqlDataAccessHelper
    {
        public SqlHelper(
            PTMaxstationContext context
            ) : base(context)
        {

        }

        public  List<T> RawSqlQuery<T>(string query, Func<DbDataReader, T> map)
        {
            //using (var context = new PTMaxstationContext())
            //{
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                //command.CommandType = CommandType.Text;

                context.Database.OpenConnection();

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
            //}
        }
    }
}
