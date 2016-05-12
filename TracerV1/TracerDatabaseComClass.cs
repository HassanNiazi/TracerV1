using Microsoft.SqlServerCe.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TracerV1
{
    class TracerDatabaseComClass
    {
        public SqlCeConnection con;
        public string errorMessage = null;

      

        public struct TraceItem
        {
            public string rrcMessage;
            public string ueid;
            public int cellID;
            public DateTime time;
        }

        //Insert data to Database
        public void insertRow(string rrcMessage,string ueid, int cellID,DateTime time)
        {
            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = String.Format("INSERT INTO TraceDB (rrcMsgName,ueId,CellId,time) VALUES ('{0}','{1}',{2},'{3}');", rrcMessage, ueid, cellID, time);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;

            }
        }

        public void insertDataTable(DataTable dataTable)
        {
            try
            {
                if (dataTable.Columns.Count > 4)
                {
                    errorMessage = "Data Not Inserted. {reason} Column Count in input greater then 4. {Method} insertDataTable(DataTable dataTable)";
                    return;
                }
                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();
                con.Open();
                foreach (DataRow item in dataTable.Rows)
                {
                    cmd.CommandText = String.Format("INSERT INTO TraceDB (rrcMsgName,ueId,CellId,time) VALUES ('{0}','{1}',{2},'{3}');", item[0], item[1], item[2], item[3]);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        private List<TraceItem> selectDataByRRCMessage(string RRCMessage)
        {
            List<TraceItem> outputList = new List<TraceItem>();
            TraceItem traceItem = new TraceItem();
            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = String.Format("Select * FROM TraceDB WHERE rrcMsgName = '" + RRCMessage + "';");
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);

                foreach (DataRow r in dt.Rows)
                {
                    traceItem.rrcMessage = (string)r[0];
                    traceItem.ueid = (string)r[1];
                    traceItem.cellID = (int)r[2];
                    traceItem.time = (DateTime)r[3];
                    outputList.Add(traceItem);
                }

                con.Close();

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return outputList;
        }

        private List<TraceItem> selectDataByUeID(string ueid)
        {
            List<TraceItem> outputList = new List<TraceItem>();
            TraceItem traceItem = new TraceItem();
            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = String.Format("Select * FROM TraceDB WHERE ueId = '" + ueid + "';");
                sda.SelectCommand = cmd;
                DataTable dt = new DataTable();
                sda.Fill(dt);
                foreach (DataRow r in dt.Rows)
                {
                    traceItem.rrcMessage = (string)r[0];
                    traceItem.ueid = (string)r[1];
                    traceItem.cellID = (int)r[2];
                    traceItem.time = (DateTime)r[3];
                    outputList.Add(traceItem);
                }
                con.Close();

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return outputList;
        }

        private List<TraceItem> selectDataByCellID(int CellID)
        {
            List<TraceItem> outputList = new List<TraceItem>();
            TraceItem traceItem = new TraceItem();
            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();
                con.Open();
                cmd.CommandText = String.Format("Select * FROM TraceDB WHERE CellId = " + CellID + ";");
                sda.SelectCommand = cmd;
                DataTable dt = new DataTable();
                sda.Fill(dt);
                foreach (DataRow r in dt.Rows)
                {
                    traceItem.rrcMessage = (string)r[0];
                    traceItem.ueid = (string)r[1];
                    traceItem.cellID = (int)r[2];
                    traceItem.time = (DateTime)r[3];
                    outputList.Add(traceItem);
                }
                con.Close();

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return outputList;
        }

        private List<TraceItem> selectDataByDate(DateTime date)
        {
            List<TraceItem> outputList = new List<TraceItem>();
            TraceItem traceItem = new TraceItem();
            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();
                con.Open();
                cmd.CommandText = String.Format("Select * FROM TraceDB WHERE time = '" + date + "';");
                sda.SelectCommand = cmd;
                DataTable dt = new DataTable();
                sda.Fill(dt);
                foreach (DataRow r in dt.Rows)
                {
                    traceItem.rrcMessage = (string)r[0];
                    traceItem.ueid = (string)r[1];
                    traceItem.cellID = (int)r[2];
                    traceItem.time = (DateTime)r[3];
                    outputList.Add(traceItem);
                }
                con.Close();

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return outputList;
        }

        private List<string> ReturnAllRRCMessages(string RRCMessage)
        {
            List<string> outputList = new List<string>();
            
            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = String.Format("Select DISTINCT rrcMsgName FROM TraceDB;");
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);

                foreach (DataRow r in dt.Rows)
                {
                    
                    outputList.Add(r[0].ToString());
                }

                con.Close();

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return outputList;
        }

        private List<string> ReturnAllUeids(string RRCMessage)
        {
            List<string> outputList = new List<string>();

            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = String.Format("Select DISTINCT ueId FROM TraceDB;");
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);

                foreach (DataRow r in dt.Rows)
                {

                    outputList.Add(r[0].ToString());
                }

                con.Close();

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return outputList;
        }

        private List<string> ReturnAllCellIDs(string RRCMessage)
        {
            List<string> outputList = new List<string>();

            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = String.Format("Select DISTINCT CellId FROM TraceDB;");
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);

                foreach (DataRow r in dt.Rows)
                {

                    outputList.Add(r[0].ToString());
                }

                con.Close();

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return outputList;
        }

        private List<string> ReturnAlldates(string RRCMessage)
        {
            List<string> outputList = new List<string>();

            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = String.Format("Select DISTINCT time FROM TraceDB;");
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);

                foreach (DataRow r in dt.Rows)
                {

                    outputList.Add(r[0].ToString());
                }

                con.Close();

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return outputList;
        }

    }
}
