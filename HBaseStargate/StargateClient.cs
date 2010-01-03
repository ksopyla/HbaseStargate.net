using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Http;
using HBaseStargate.Entities;
using HBaseStargate.Helpers;
using System.Runtime.Serialization.Json;
using System.IO;

namespace HBaseStargate
{
    public class StargateClient
    {
        private readonly HttpClient stargateHttpClient;
        private string UserAgent="Stargate .net client";
        private string AcceptHeader="application/json";

        public StargateClient(string serviceAddress)
        {
            stargateHttpClient = new HttpClient(serviceAddress);

            SetDefaultHeaders();
        }

        //ustawia domyślne nagłówki dla blipa
        private void SetDefaultHeaders()
        {
           TimeSpan WebGetTimout = TimeSpan.FromSeconds(30);
           stargateHttpClient.TransportSettings.ConnectionTimeout = WebGetTimout;
            stargateHttpClient.TransportSettings.ReadWriteTimeout = WebGetTimout;
            //To było ustawiane, nie wiem dlaczego, zbadać
            System.Net.ServicePointManager.Expect100Continue = false;

            stargateHttpClient.DefaultHeaders.Accept.Add(
                new Microsoft.Http.Headers.StringWithOptionalQuality(AcceptHeader));

            stargateHttpClient.DefaultHeaders.Add("User-Agent", UserAgent);

           // stargateHttpClient.TransportSettings.SendChunked = true;
        }

        /// <summary>
        /// Get row or rows from table
        /// </summary>
        /// <remarks>
        /// You can use "*" for retrving many rows as follows
        /// GetRow("users","k*") - retrives all rows from table useres which keys starts from 'k'
        /// </remarks>
        /// <param name="table"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public Entities.CellSet GetRow(string table, string rowKey)
        {
            return GetColumns(table, rowKey, string.Empty);
        }

        /// <summary>
        /// Get spcified columns from paritcular row or rows
        /// </summary>
        /// <remarks>
        /// You can specified column families or columns, if you want many columns you should 
        /// 
        /// GetColumns("users","userlogin","info:name,info:email,friends") - retrives only 
        /// columns info:name , info:email and all things in column family
        /// 
        /// You can also use "*" for rows to return many rows
        /// GetColumns("users","k*","info:name,info:email,friends") - return  all users whose login starts form k 
        /// but if some user doesn't have one of specified columns the stargete returns error "http 404"
        /// </remarks>
        /// <param name="table"></param>
        /// <param name="rowKey"></param>
        /// <param name="columns">list of comma separated column or column family names</param>
        /// <returns></returns>
        public CellSet GetColumns(string table, string rowKey, string columns)
        {
            string getRowAddress = string.Format("{0}/{1}/{2}?v=1", table, rowKey,columns);
            HttpResponseMessage response = stargateHttpClient.Get(getRowAddress);

            response.EnsureStatusIsSuccessful();

            CellSet rows = response.Content.ReadAsJsonDataContract<CellSet>();

            return rows;
        }


        public void SetNewRow(string table, Row row)
        {
            SetNewRows(table, new Row[] { row });
        }

        public void SetNewRows(string table, Row[] rows)
        {
            string falseRowKey = Guid.NewGuid().ToString();
            string postUrl= string.Format("{0}/{1}",table,falseRowKey);


            CellSet c = new CellSet();
            c.Rows = rows;

            //encode fields to Base64
            foreach (var row in c.Rows)
            {
                row.Key = row.Key.EncodeToBase64();

                foreach (var cell in row.Cells)
                {
                    cell.Column = cell.Column.EncodeToBase64();
                    cell.Value = cell.Value.EncodeToBase64();
                   //cell.Timestamp = DateTime.Now.ToUnixTimestamp();
                }
            }

            string cellSetJson = Serializers.ToJson<CellSet>(c);
            /*
            {"Row":[{"key":"bG9naW43","Cell":[{"column":"aW5mbzpuYW1l","$":"bG9naW43","timestamp":1262170650}]}]}
            {"Row":[{"key":"bG9naW41","Cell":[{"timestamp":1262165074260,"column":"aW5mbzpuYW1l","$":"bG9naW41"}]}]}
             */
            var hc = HttpContent.Create(cellSetJson);
            

            var response= stargateHttpClient.Post(postUrl, "application/json",hc);

            response.EnsureStatusIsSuccessful();
        }

        public void DeleteRow(string table, string rowKey)
        {
            DeleteColumns(table, rowKey, string.Empty);
        }

        public void DeleteColumns(string table, string rowKey, string columns)
        {
            string rowAddress = string.Format("{0}/{1}/{2}", table, rowKey, columns);

            HttpResponseMessage response = stargateHttpClient.Delete(rowAddress);

            response.EnsureStatusIsSuccessful();
        }
    }
}
