using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
namespace TPCH_QUERIES
{

    class Program
    {

        void dummy()
        {
            /* con = new System.Data.SqlClient.SqlConnection(connString);
             * SqlCommand command = new SqlCommand(sql,con);
             * SqlDataReader  dataReader = command.ExecuteReader();
  while (dataReader.Read())
  {
      Console.WriteLine(dataReader.GetValue(0) + " - " + dataReader.GetValue(1) + " - " + dataReader.GetValue(2));
  }
  dataReader.Close();
  command.Dispose();*/



        }

   
        void GroupBySingleAttribute()
        {

            var ds = new DataSet();
            String connString = "Server=NAVYA\\TPCHINSTANCENEW;Database=TPCH;Integrated Security=SSPI;Persist Security Info=False;";
            SqlConnection con = new SqlConnection(connString);
            con.Open();
            String sql = "select * from dbo.lineitem where L_ShipMode='MAIL'";
            var adapter = new SqlDataAdapter(sql, con);
            int no=adapter.Fill(ds);
            var lineitems = ds.Tables[0].AsEnumerable();
            var res=  from p in lineitems
                       group p by p.Field<string>("L_ShipMode") into g
                      select new { L_ShipMode = g.Key, wholegroup = g };
            foreach (var g in res)
                {
                    Console.WriteLine("L_ShipMode: {0}", g.L_ShipMode);
                   // Console.WriteLine("\t : {0}",g.wholegroup.Count());
                    List<int> l = new List<int>();
                
                    foreach (var w in g.wholegroup)
                    {
                        int i=w.Field<int>("L_Quantity");
                        if (l.Contains(i))
                            continue;
                        l.Add(i);
                       Console.WriteLine("\t" + i);
                    }
                    l.Sort();
               
                /*    
                iterator item=l.GetEnumerator();
                    while (l.Count() != 0)
                    {
                        l.RemoveAt(0);
                        Console.WriteLine();
                        l.Remove();
                    }*/
                }
            
            con.Close();
            while (true) { };
         }

        /*funciton to group by 2 attributes and use aggregate functions */
        void GroupByMultipleAttributes()
        {
            var ds = new DataSet();
            String connString = "Server=NAVYA\\TPCHINSTANCENEW;Database=TPCH;Integrated Security=SSPI;Persist Security Info=False;";
            SqlConnection con = new SqlConnection(connString);
            con.Open();
            String sql = "select L_ShipMode,L_ShipInstruct,L_Quantity from dbo.lineitem";
            var adapter = new SqlDataAdapter(sql, con);
            int no = adapter.Fill(ds);
            var lineitems = ds.Tables[0].AsEnumerable();
            var res = from p in lineitems
                      group p by p.Field<string>("L_ShipMode") into og
                      orderby og.Key
                      select new
                      {
                          L_ShipMode = og.Key,
                          outergroup = from q in og
                                       group q by q.Field<string>("L_ShipInstruct") into ig
                                       orderby ig.Key
                                       select new { L_ShipInstruct = ig.Key, L_QuantitySum = ig.Sum(x => x.Field<int>("L_Quantity")) } //ig.Count()
                      };
            foreach (var g in res)
            {
                Console.WriteLine("L_ShipMode: {0}", g.L_ShipMode);             
                foreach (var h in g.outergroup)
                {
                    Console.WriteLine("L_ShipInstruct: {0}",h.L_ShipInstruct);
                    Console.WriteLine("Sum {0}",h.L_QuantitySum);
                   /* foreach (var i in h.innergroup)
                    {
                        Console.WriteLine("L_Quantity: {0}", i.Field<int>("L_Quantity"));
                    }*/
                    
                }
                

            }

            con.Close();
            while (true) { };

        }


        static void Main(string[] args)
        {
            Program obj= new Program();
           // obj.GroupBySingleAttribute();
            obj.GroupByMultipleAttributes();
        }
    }
}
