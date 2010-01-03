using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using HBaseStargate;
using HBaseStargate.Helpers;
using HBaseStargate.Entities;
using System.Diagnostics;

namespace HbaseStargateUsage
{
    class Program
    {

        static StargateClient client;


        /*
         * Class for testing HBaseStargate
         *  
         * If you want run this you have to:
         * 1. run hbase and change host and port variables
         * 2. create 'users' table in hbase, for this task you can use *insertScritp.txt* in main folder
         * 
         */
        static string protocol = "http://";
        static string host = "192.168.228.128";
        static int port = 8080;
        static void Main(string[] args)
        {

            string serviceAddress = string.Format("{0}{1}:{2}", protocol, host, port);

            Uri addr = new Uri(serviceAddress);

            client = new StargateClient(serviceAddress);

            Console.WriteLine("Before inserting");
            Console.WriteLine("------------------------------------------");
            GetRowsViaStargate();

            SetRowsViaStargate();

            Console.WriteLine("After inserting");
            Console.WriteLine("------------------------------------------");
            GetRowsViaStargate();

            //delete only one cell
            client.DeleteColumns("users", "login10", "info:email");
            Console.WriteLine("After deleteing");
            Console.WriteLine("------------------------------------------");
            GetRowsViaStargate();
            client.DeleteRow("users", "login10");


            TestStargatePostPerformace(100);



        }

        private static void TestStargatePostPerformace(int count)
        {
            List<Row> rowList = new List<Row>(count);
            List<string> loginList = new List<string>(count);

            for (int i = 0; i < count; i++)
            {
                string key = string.Format("login{0}", i + 1);

                loginList.Add(key);

                rowList.Add(new Row()
                {
                    Key = key,
                    Cells = new CellList(){
                        new Cell(){ Column="info:name",Value=key},
                        new Cell(){ Column="info:email", Value=key}
                    }
                });
            }
            Console.WriteLine("Start bulk insert");
            Stopwatch sw = Stopwatch.StartNew();
            client.SetNewRows("users", rowList.ToArray());
            Console.WriteLine("Bulk insert {0}-rows takes {1}", count, sw.Elapsed);

            Console.WriteLine("Delete logins");
            sw.Reset();
            foreach (var item in loginList)
            {
                client.DeleteRow("users", item);
            }
            Console.WriteLine("Delete logins takes {0}", sw.Elapsed);
        }

        private static void SetRowsViaStargate()
        {
            Row[] rows = new Row[]{
                new Row(){ 
                    Key="login9", 
                    Cells=new CellList(){
                        new Cell(){ Column="info:name",Value="login9-"+DateTime.Now.ToString()},
                        new Cell(){ Column="info:email", Value="login9@grito.me"}
                    }
                },
                 new Row(){ 
                    Key="login10", 
                    Cells=new CellList(){
                        new Cell(){ Column="info:name",Value="login10"},
                        new Cell(){ Column="info:email", Value="login10@grito.me"},
                        new Cell(){Column="feeds:eastgroup.pl", Value=DateTime.Now.ToShortDateString()},
                        new Cell(){Column="feeds:feed1.pl/rss", Value=DateTime.Now.ToShortDateString()},
                        new Cell(){Column="feeds:feed2.pl/rss", Value=DateTime.Now.ToShortDateString()},
                    }
                }
                   
            };


            string table = "users";
            client.SetNewRows(table, rows);
        }

        private static void GetRowsViaStargate()
        {

            string table = "users";
            string key = "l*";

            string columns = "info,feeds";

            // CellSet rows = client.GetRow(table,key,columns);
            CellSet rows = client.GetRow(table, key);

            Console.WriteLine();
            foreach (var row in rows.Rows)
            {

                Console.WriteLine(row.Key.DecodeFromBase64());

                foreach (var item in row.Cells)
                {
                    Console.WriteLine("{0}=>{1} {2}", item.Column.DecodeFromBase64(), item.Value.DecodeFromBase64(), item.Timestamp);
                }
                Console.WriteLine();
            }
        }


    }


}
