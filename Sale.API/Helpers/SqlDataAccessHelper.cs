using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using MaxStation.Entities.Models;

namespace Sale.API.Helpers
{
    public class SqlDataAccessHelper : DataModelHelper
    {
        //readonly string connectionString;
        //readonly AppSettings appSettings;
        protected PTMaxstationContext context;

        public SqlDataAccessHelper(PTMaxstationContext context)
        {
            this.context = context;
        }

        //public void ExecuteTransaction(List<SqlCommand> command)
        //{
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        if (connection.State == ConnectionState.Closed)
        //        {
        //            connection.Open();
        //        }

        //        using (SqlTransaction transaction = connection.BeginTransaction())
        //        {
        //            try
        //            {
        //                foreach (var x in command)
        //                {
        //                    x.Connection = connection;
        //                    x.CommandTimeout = 1000;
        //                    x.Transaction = transaction;
        //                    x.ExecuteNonQuery();
        //                }

        //                transaction.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //                throw ex;
        //            }
        //            finally
        //            {
        //                foreach (var x in command)
        //                {
        //                    x.Dispose();
        //                }
        //            }
        //        }
        //    }
        //}

        //public void ExecuteNonQuery(SqlCommand command)
        //{
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        if (connection.State == ConnectionState.Closed)
        //        {
        //            connection.Open();
        //        }

        //        command.Connection = connection;
        //        command.CommandTimeout = 1000;
        //        command.ExecuteNonQuery();
        //    }
        //}

        //public T GetDataObject<T>(SqlCommand command)
        //{
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        if (connection.State == ConnectionState.Closed)
        //        {
        //            connection.Open();
        //        }

        //        command.Connection = connection;
        //        command.CommandTimeout = 1000;
        //        return this.CreateObject<T>(command.ExecuteReader());
        //    }
        //}

        //public List<T> GetDataList<T>(SqlCommand command)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            if (connection.State == ConnectionState.Closed)
        //            {
        //                connection.Open();
        //            }

        //            command.Connection = connection;
        //            command.CommandTimeout = 1000;
        //            return this.CreateList<T>(command.ExecuteReader());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
