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
        /// <summary>
        ///  Returns the name of table.
        /// </summary>
        public readonly string TableName = "TraceDB";

        public TracerDatabaseComClass(string mainDir)
        {
            firstCall(mainDir);
        }
        public struct TraceItem
        {
            public string rrcMessage;
            public string ueid;
            public string cellID;
            public DateTime time;
        }

        //Insert data to Database
        public void insertRow(string rrcMessage, string ueid, string cellID, DateTime time)
        {
            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) , "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = String.Format("INSERT INTO TraceDB (rrcMsgName,ueId,CellId,time) VALUES ('{0}','{1}','{2}','{3}');", rrcMessage, ueid, cellID, time);
                sda.SelectCommand = cmd;
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;

            }
        }



        public void firstCall(string mainDir)
        {
            try
            {

                //con = new SqlCeConnection("Data Source = C:/Users/sikan/Documents/Visual Studio 2015/Projects/TracerV1/TracerV1/bin/x64/Debug/Database1.sdf;"); // Max Database Size = 4090");
                con = new SqlCeConnection("Data Source = " + mainDir + "/Database1.sdf; Max Database Size = 4090");

                //con = new SqlCeConnection();
                //                con.ConnectionString = "Data Source = Database1.sdf; Max Database Size = 4000; Persist Security Info=  False";
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
               // SqlCeCommand cmd = con.CreateCommand();
                con.Open();
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
                    cmd.CommandText = String.Format("INSERT INTO TraceDB (rrcMsgName,ueId,CellId,time) VALUES ('{0}','{1}','{2}','{3}');", item[0], item[1], item[2], item[3]);
                    sda.SelectCommand = cmd;
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void DeleteAllDataFromDatabase()
        {
            try
            {
               
                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();
                con.Open();

                cmd.CommandText = "DELETE FROM TraceDB";
                cmd.ExecuteNonQuery();
             
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public List<TraceItem> selectDataByRRCMessage(string RRCMessage)
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
                    traceItem.cellID = (string)r[2];
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

        public List<TraceItem> selectDataByUeID(string ueid)
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
                    traceItem.cellID = (string)r[2];
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

        public List<TraceItem> selectDataByCellID(string cellID)
        {
            List<TraceItem> outputList = new List<TraceItem>();
            TraceItem traceItem = new TraceItem();
            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();
                con.Open();
                cmd.CommandText = String.Format("Select * FROM TraceDB WHERE CellId = '" + cellID + "';");
                sda.SelectCommand = cmd;
                DataTable dt = new DataTable();
                sda.Fill(dt);
                foreach (DataRow r in dt.Rows)
                {
                    traceItem.rrcMessage = (string)r[0];
                    traceItem.ueid = (string)r[1];
                    traceItem.cellID = (string)r[2];
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

        public List<TraceItem> selectDataByDate(DateTime date)
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
                    traceItem.cellID = (string)r[2];
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

        public List<string> ReturnAllRRCMessages()
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


        public List<string> ReturnAllRRCMessagesFromRRCMsgDatabase()
        {
            List<string> outputList = new List<string>();

            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = String.Format("Select DISTINCT rrcMessage FROM rrcMessageLookUp;");
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

        public List<string> ReturnAllUeids()
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

        public List<string> ReturnAllCellIDs()
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

        public List<string> ReturnAlldates()
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

        public DataTable queryByIMSIandRRCMessage(string imsi, string rrcMessage)
        {
           
            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = "Select * FROM TraceDB WHERE ueId LIKE '%" + imsi +  "%' AND rrcMsgName = '" + rrcMessage + "';";
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);

                con.Close();

                return dt;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (con.State == ConnectionState.Open)
                    con.Close();
                return null;
            }

           
        }

        public DataTable queryByRrcMessageAndTime(string rrcMessage, DateTime time)
        {

            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = "Select * FROM TraceDB WHERE rrcMsgName = '" + rrcMessage + "' AND time = '" + time + "';";
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);

                con.Close();

                return dt;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (con.State == ConnectionState.Open)
                    con.Close();
                return null;
            }


        }

        /// <summary>
        /// Method Implemented Generically. Handle Data Yourself.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable customQuery(string query)
        {


            DataTable dt = new DataTable();
            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = query;
                sda.SelectCommand = cmd;

                sda.Fill(dt);
                con.Close();

                return dt;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return dt;

        }

    }
}
