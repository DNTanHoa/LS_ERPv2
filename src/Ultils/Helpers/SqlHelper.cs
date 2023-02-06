﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultils.Helpers
{
    public static class SqlHelper
    {
        public static string ExecuteNonQuery(string connectionString,
            string storedProcedureName, params SqlParameter[] parameters)
        {
            string result = string.Empty;
            try
            {

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 900;
                        cmd.Parameters.AddRange(parameters);
                        conn.Open();
                        result = cmd.ExecuteNonQuery().ToString();
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;
        }

        public static async Task<string> ExecuteNonQueryAsync(string connectionString,
            string storedProcedureName, params SqlParameter[] parameters)
        {
            string result = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);
                        await conn.OpenAsync();
                        int result01 = await cmd.ExecuteNonQueryAsync();
                        result = result01.ToString();
                        await conn.CloseAsync();
                    }
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;
        }

        public static object ExecuteScalar(string connectionString,
            string storedProcedureName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    conn.Close();
                    return result;
                }
            }
        }

        public static async Task<object> ExecuteScalarAsync(string connectionString,
            string storedProcedureName, params SqlParameter[] parameters)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);
                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    await conn.CloseAsync();
                    return result;
                }
            }
        }

        public static SqlDataReader ExecuteReader(string connectionString,
            string storedProcedureName, params SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                conn.Open();
                SqlDataReader result = cmd.ExecuteReader();
                conn.Close();
                return result;

            }
        }

        public static async Task<SqlDataReader> ExecuteReaderAsync(string connectionString,
            string storedProcedureName, params SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                await conn.OpenAsync();
                var result = await cmd.ExecuteReaderAsync();
                await conn.CloseAsync();
                return result;
            }
        }

        public static DataTable Fill(string connectionString,
            string storedProcedureName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            string result = string.Empty;
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    conn.Open();
                    adapter.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return dt;
        }

        public static DataTable FillByReader(string connectionString,
            string storedProcedureName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(storedProcedureName, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 900;
                    cmd.Parameters.AddRange(parameters);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
            }
            return dt;
        }

        public static async Task<DataTable> FillAsync(string connectionString,
            string storedProcedureName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            string result = string.Empty;
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    await conn.OpenAsync();
                    adapter.Fill(dt);
                    await conn.CloseAsync();
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return dt;
        }

        public static DataSet FillDataSet(string connectionString,
            string storedProcedureName, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            string result = string.Empty;
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    conn.Open();
                    adapter.Fill(ds);
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return ds;
        }

        public static async Task<DataSet> FillDataSetAsync(string connectionString,
            string storedProcedureName, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            string result = string.Empty;
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    await conn.OpenAsync();
                    adapter.Fill(ds);
                    await conn.CloseAsync();
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return ds;
        }

        /// Using create item master job & size sort index
        public static DataTable FillByReader_ItemMasterJob(string connectionString,
            string storedProcedureName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(storedProcedureName, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);
                    cmd.CommandTimeout = 900;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
            }
            return dt;
        }
    }
}
