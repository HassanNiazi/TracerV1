using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;

namespace TracerV1
{
    class UserMessageFilter
    {
        /// <summary>
        /// This should filter data based on dates. and return an array of counts. or dictionary
        /// </summary>
        /// 
        public DataTable localDataTable = new DataTable();

        public struct countDate
        {
            public int countMOC;
            public int countMTC;
            public int countDropCalls;
            public DateTime date;
        }

        //public List<countDate> countMOC = new List<countDate>();

        //public List<countDate> countMTC = new List<countDate>();

        //public List<countDate> countDropCalls = new List<countDate>();

        DateTime currentMOC;

        DateTime currentMTC;

        DateTime currentDropCalls;



        //public int countMTC = 0;

        //public int countDropCalls = 0;

        public string errorMessage = null;



        public List<countDate> getResult(string messageMOC, string messageMTC, string messageDropCalls)
        {
            List<countDate> count = new List<countDate>();
            countDate cdLocal = new countDate();
            DataView view = new DataView(localDataTable);
            DataTable distinctValue = view.ToTable(true, "time");
            DataRow[] timeRows = distinctValue.Select();
            foreach (DataRow r in timeRows)
            {
                cdLocal = new countDate();
                errorMessage = r[0].ToString();
            //    string[] formats = { "DD/MM/YYYY" };
                // cdLocal.date = DateTime.ParseExact(r[0].ToString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                cdLocal.date =DateTime.Parse( r[0].ToString());
                DataRow[] dataRowsMOC = localDataTable.Select(string.Format("rrcMsgName = '{0}' AND time = '{1}'", messageMOC, r[0].ToString() ));
                DataRow[] dataRowsMTC = localDataTable.Select(string.Format("rrcMsgName = '{0}' AND time = '{1}'", messageMTC , r[0].ToString()));
                DataRow[] dataRowsDropCalls = localDataTable.Select(string.Format("rrcMsgName = '{0}' AND time = '{1}'", messageDropCalls, r[0].ToString()));

                cdLocal.countMOC = dataRowsMOC.Length;
                cdLocal.countMTC = dataRowsMTC.Length;
                cdLocal.countDropCalls = dataRowsDropCalls.Length;
                count.Add(cdLocal);
            }

            return count;
        }

        /// <summary>
        /// Constructor for the UserMessageFilter Class
        /// </summary>
        /// <param name="dt">Datatable holding all the user messages</param>
        /// <param name="messageMOC">Message string to filter the Mobile Originating Calls</param>
        /// <param name="messageMTC">Message string to filter the Mobile Terminating Calls</param>
        /// <param name="messageDropCalls">Message string to filter the Drop Calls</param>
        /// 
        public UserMessageFilter(DataTable dt, string messageMOC, string messageMTC, string messageDropCalls)
        {
            try
            {
                localDataTable = dt;
                localDataTable.DefaultView.Sort = "time ASC";
                //getMTC(messageMTC);
                //getMOC(messageMOC);
                //getDropCalls(messageDropCalls);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

        }

        public void reassginDataTable(DataTable dt)
        {
            localDataTable = dt;
        }

        //private void getMOC(string messageMOC)
        //{
        //    try
        //    {

        //        ////// IF Encountring Errors Implement the other algorithum with datatable distnict for date and 
        //        //then for each for each date and then count the counter

        //        DataRow[] dr = localDataTable.Select(string.Format("rrcMsgName = '{0}'", messageMOC));
        //        countDate cdItem = new countDate();
        //        foreach (DataRow r in dr)
        //        {
        //            // countDate cdItem = new countDate();
        //            if (!(currentMOC == DateTime.Parse(r["time"].ToString())))
        //            {
        //                countMOC.Add(cdItem);
        //                cdItem = new countDate();
        //                cdItem.count = cdItem.count + 1;
        //                cdItem.date = DateTime.Parse(r["time"].ToString());
        //                currentMOC = DateTime.Parse(r["time"].ToString());

        //            }
        //            else
        //            {
        //                cdItem.count = cdItem.count + 1;
        //            }

        //        }
        //        countMOC.Add(cdItem);
        //    }
        //    catch (Exception ex)
        //    {

        //        errorMessage = ex.Message;
        //    }
        //}
        //private void getMTC(string messageMTC)
        //{

        //    try
        //    {

        //        DataRow[] dr = localDataTable.Select(string.Format("rrcMsgName = '{0}'", messageMTC));
        //        countDate cdItem = new countDate();
        //        foreach (DataRow r in dr)
        //        {
        //            // countDate cdItem = new countDate();
        //            if (!(currentMTC == DateTime.Parse(r["time"].ToString())))
        //            {
        //                countMTC.Add(cdItem);
        //                cdItem = new countDate();
        //                cdItem.count = cdItem.count + 1;
        //                cdItem.date = DateTime.Parse(r["time"].ToString());
        //                currentMTC = DateTime.Parse(r["time"].ToString());

        //            }
        //            else
        //            {
        //                cdItem.count = cdItem.count + 1;
        //            }

        //        }
        //        countMTC.Add(cdItem);
        //    }
        //    catch (Exception ex)
        //    {

        //        errorMessage = ex.Message;
        //    }
        //}
        //private void getDropCalls(string messageDropCalls)
        //{

        //    try
        //    {

        //        DataRow[] dr = localDataTable.Select(string.Format("rrcMsgName = '{0}'", messageDropCalls));
        //        countDate cdItem = new countDate();
        //        foreach (DataRow r in dr)
        //        {
        //            // countDate cdItem = new countDate();
        //            if (!(currentDropCalls == DateTime.Parse(r["time"].ToString())))
        //            {
        //                countDropCalls.Add(cdItem);
        //                cdItem = new countDate();
        //                cdItem.count = cdItem.count + 1;
        //                cdItem.date = DateTime.Parse(r["time"].ToString());
        //                currentDropCalls = DateTime.Parse(r["time"].ToString());

        //            }
        //            else
        //            {
        //                cdItem.count = cdItem.count + 1;
        //            }

        //        }
        //        countDropCalls.Add(cdItem);
        //    }
        //    catch (Exception ex)
        //    {

        //        errorMessage = ex.Message;
        //    }
        //}



    }
}
