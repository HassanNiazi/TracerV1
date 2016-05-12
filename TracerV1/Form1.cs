using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using Excel = Microsoft.Office.Interop.Excel;
using DevExpress.XtraCharts;
using System.Drawing.Imaging;
using Microsoft.Office.Core;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Cache;

namespace TracerV1
{
    public partial class Form1 : Form
    {


        //TODO: Add Names for IMSI's
        //TODO: Add Snap It ---- Done
        //Add Normal Map Search 
        bool started = false;
        bool unknownCellsFlag = false;
        List<string[]> rows;
        static readonly string PasswordHash = "P@@SwAAA";
        static readonly string SaltKey = "S@LT&ZZZ";
        static readonly string VIKey = "HR$2pIjHR$2pIj12";
        DataTable traceDataTable = new DataTable();
        DataTable dt = new DataTable();
        DataTable cellNames = new DataTable();
        Image img1 = null;
        string mainDir = Environment.CurrentDirectory;
        SqlCeConnection con;
        List<string> rrcItems = new List<string>();
        object misValue = System.Reflection.Missing.Value;
        class _point
        {


            public double lat;
            public double lng;

            public _point(double _lat, double _lng)
            {
                lat = _lat;
                lng = _lng;
            }

        }
        public struct cell
        {
            public string cellID;
            public string siteName;
            public string cellName;
        }

        public struct genericDataContainer
        {
            public string imsi;
            public List<int> count;
        }

        public Form1()
        {
            InitializeComponent();
            //this.Visible = false;
            //this.Hide();
            mapProviderList.DataSource = GMapProviders.List;

            //  MsgBox(mainDir);
            //this.Visible = ;



            #region Licenscing
            //while (Properties.Settings.Default.expired)
            //{
            // this.Visible = false;
            //if (Properties.Settings.Default.expired)
            //{
            //    MessageBox.Show("Dear User! Your Licencse has expired. ");
            //    LoadLicensceFile();
            //    Environment.Exit(Environment.ExitCode);
            //    //Application.Exit();

            //}


            try
            {
                System.Net.IPHostEntry e = System.Net.Dns.GetHostEntry("www.google.com");
                toolStatus.Text = "Connected!  ";
                MainMap.Manager.Mode = AccessMode.ServerAndCache;

                //if (Properties.Settings.Default.licenseLastDate < GetNistTime())
                //{
                //    Properties.Settings.Default.expired = true;
                //    MessageBox.Show("Dear User! Your Licencse has expired. ");
                //    LoadLicensceFile();

                //    Environment.Exit(Environment.ExitCode);

                //}

            }
            catch
            {
                MainMap.Manager.Mode = AccessMode.CacheOnly;
                //  MessageBox.Show("No internet connection avaible, going to CacheOnly mode.",
                ///      "GMap.NET - Demo.WindowsForms", MessageBoxButtons.OK,
                //   MessageBoxIcon.Warning);
                toolStatus.Text = "Offline - Cache Mode Only";


                //if (Properties.Settings.Default.licenseLastDate < DateTime.Today)
                //{
                //    Properties.Settings.Default.expired = true;
                //    MessageBox.Show("Dear User! Your Licencse has expired. ");
                //    LoadLicensceFile();
                //    //Application.Exit();
                //    Environment.Exit(Environment.ExitCode);
                //}
                //this.Visible = !this.Visible;
            }
            //}
            // config map

            //  this.Visible = Properties.Settings.Default.loadStatus;
            #endregion



            MainMap.MapProvider = GMapProviders.GoogleMap;
            MainMap.Position = new PointLatLng(33.7294, 73.0931);
            MainMap.MinZoom = 0;
            MainMap.MaxZoom = 24;
            MainMap.Zoom = 9;

            #region Access Functions For Help
            // add your custom map db provider
            //GMap.NET.CacheProviders.MySQLPureImageCache ch = new GMap.NET.CacheProviders.MySQLPureImageCache();
            //ch.ConnectionString = @"server=sql2008;User Id=trolis;Persist Security Info=True;database=gmapnetcache;password=trolis;";
            //MainMap.Manager.SecondaryCache = ch;

            // set your proxy here if need
            //GMapProvider.WebProxy = new WebProxy("10.2.0.100", 8080);
            //GMapProvider.WebProxy.Credentials = new NetworkCredential("ogrenci@bilgeadam.com", "bilgeada");

            // map events
            //  {
            // MainMap.OnPositionChanged += new PositionChanged(MainMap_OnPositionChanged);

            //  MainMap.OnTileLoadStart += new TileLoadStart(MainMap_OnTileLoadStart);
            //  MainMap.OnTileLoadComplete += new TileLoadComplete(MainMap_OnTileLoadComplete);

            // MainMap.OnMapZoomChanged += new MapZoomChanged(MainMap_OnMapZoomChanged);
            //  MainMap.OnMapTypeChanged += new MapTypeChanged(MainMap_OnMapTypeChanged);

            // MainMap.OnMarkerClick += new MarkerClick(MainMap_OnMarkerClick);
            // MainMap.OnMarkerEnter += new MarkerEnter(MainMap_OnMarkerEnter);
            // MainMap.OnMarkerLeave += new MarkerLeave(MainMap_OnMarkerLeave);

            // MainMap.OnPolygonEnter += new PolygonEnter(MainMap_OnPolygonEnter);
            // MainMap.OnPolygonLeave += new PolygonLeave(MainMap_OnPolygonLeave);

            // MainMap.OnRouteEnter += new RouteEnter(MainMap_OnRouteEnter);
            //  MainMap.OnRouteLeave += new RouteLeave(MainMap_OnRouteLeave);

            // MainMap.Manager.OnTileCacheComplete += new TileCacheComplete(OnTileCacheComplete);
            // MainMap.Manager.OnTileCacheStart += new TileCacheStart(OnTileCacheStart);
            // MainMap.Manager.OnTileCacheProgress += new TileCacheProgress(OnTileCacheProgress);
            //    } 

            #endregion

            MainMap.MarkersEnabled = true;

            //GMapOverlay markersOverlay = new GMapOverlay("markers");
            //GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(54.6961334816182, 25.2985095977783),
            //GMarkerGoogleType.arrow);
            //markersOverlay.Markers.Add(marker);
            //MainMap.Overlays.Add(markersOverlay);




        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            Form2 f2 = new Form2();
            f2.ShowDialog();
            //#region Licenscing
            //    MsgBox(Properties.Settings.Default.expired.ToString());
            while (Properties.Settings.Default.expired)
            {
                // this.Visible = false;
                if (Properties.Settings.Default.expired)
                {
                    MessageBox.Show("Dear User! Your Licencse has expired. ");
                    LoadLicensceFile();
                    Environment.Exit(Environment.ExitCode);
                    //Application.Exit();

                }


                try
                {
                    System.Net.IPHostEntry el = System.Net.Dns.GetHostEntry("www.google.com");
                    toolStatus.Text = "Connected!  ";
                    MainMap.Manager.Mode = AccessMode.ServerAndCache;

                    if (Properties.Settings.Default.licenseLastDate < GetNistTime())
                    {
                        Properties.Settings.Default.expired = true;
                        MessageBox.Show("Dear User! Your Licencse has expired. ");
                        Properties.Settings.Default.Save();
                        LoadLicensceFile();

                        Environment.Exit(Environment.ExitCode);

                    }

                }
                catch
                {
                    MainMap.Manager.Mode = AccessMode.CacheOnly;
                    //  MessageBox.Show("No internet connection avaible, going to CacheOnly mode.",
                    ///      "GMap.NET - Demo.WindowsForms", MessageBoxButtons.OK,
                    //   MessageBoxIcon.Warning);
                    toolStatus.Text = "Offline - Cache Mode Only";

                    try
                    {
                        if (Properties.Settings.Default.licenseLastDate < DateTime.Today)
                        {
                            Properties.Settings.Default.expired = true;
                            Properties.Settings.Default.Save();
                            MessageBox.Show("Dear User! Your Licencse has expired. ");
                            LoadLicensceFile();
                            //Application.Exit();
                            Environment.Exit(Environment.ExitCode);
                        }
                        //this.Visible = !this.Visible;
                    }
                    catch (Exception ex)
                    {

                        MsgBox(ex.Message);
                    }
                }
            }
            //// config map

            ////  this.Visible = Properties.Settings.Default.loadStatus;
            //#endregion
            if (Properties.Settings.Default.licenseLastDate > DateTime.Today)
                expiryLic.Caption = "Licensced Till : " + Properties.Settings.Default.licenseLastDate.ToShortDateString();
            else
                expiryLic.Caption = "Licensce Expired";
            sqlConOpen();
            refetchData();
            //barDockControlTop.Visible = false;
            //barDockControlRight.Visible = false;


            //      ribbonControl1.Visible = false;
            //  MsgBox(TracerV1.Properties.Settings.Default.mocStringOfficial);




            mocMessageFilter.Text = Properties.Settings.Default.mocStringOfficial;
            mtcMessageFilter.Text = Properties.Settings.Default.mtcStringOfficial;
            callDropFilterMessage.Text = Properties.Settings.Default.drcStringOfficial;
            graphLabel1TB.Text = Properties.Settings.Default.graphLabel1;
            graphLabel2TB.Text = Properties.Settings.Default.graphLabel2;
            graphLabel3TB.Text = Properties.Settings.Default.graphLabel3;
            this.Show();
            this.WindowState = FormWindowState.Maximized;
            f2.Dispose();
            snapControl1.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;

        }

        private void refetchData()
        {
            try
            {
                _cellDataUpload_FromFile();
                _cellNamesDataUpload();
                _traceDataUpload();
                //updateUserList_DataVisualizer();
                updateIMSIDB(); // Fetchs IMSI's From The tracers Database to the IMSI DB
                UpdateIMSIChkList(); // From IMSI DB to Control
                                     // UpdateRRCMsgsList();
                insertToRRCMessageLookUpDatabase();
                //updateUserList_DataVisualizer();
                started = true;
                UpdateDataRrcMessageLookUpGridView();
                UpdateIMSIChkList(); // From IMSI DB to Control

                //// Disable Cell Highlight because it looks unprofessional in Snapshots
                dataGridView5.DefaultCellStyle.SelectionBackColor = dataGridView5.DefaultCellStyle.BackColor;
                dataGridView5.DefaultCellStyle.SelectionForeColor = dataGridView5.DefaultCellStyle.ForeColor;

                dgv4.DefaultCellStyle.SelectionBackColor = dgv4.DefaultCellStyle.BackColor;
                dgv4.DefaultCellStyle.SelectionForeColor = dgv4.DefaultCellStyle.ForeColor;

            }
            catch (Exception ex)
            {
                MsgBox(ex.Message);
            }
        }

        public static DateTime GetNistTime()
        {
            DateTime dateTime = DateTime.MinValue;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://nist.time.gov/actualtime.cgi?lzbc=siqm9b");
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore); //No caching
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader stream = new StreamReader(response.GetResponseStream());
                string html = stream.ReadToEnd();//<timestamp time=\"1395772696469995\" delay=\"1395772696469995\"/>
                string time = Regex.Match(html, @"(?<=\btime="")[^""]*").Value;
                double milliseconds = Convert.ToInt64(time) / 1000.0;
                dateTime = new DateTime(1970, 1, 1).AddMilliseconds(milliseconds).ToLocalTime();
            }

            return dateTime;
        }

        private void sqlConOpen()
        {
            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = "SELECT * FROM ImsiUsers";
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);
                dataGridView3.DataSource = dt;
                con.Close();
            }
            catch (Exception ex)
            {

                MsgBox(ex.Message);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void insertToRRCMessageLookUpDatabase()
        {
            try
            {
                UpdateRRCMsgsList();
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();
                con.Open();

                foreach (var item in rrcItems)
                {
                    try
                    {

                        cmd.CommandText = string.Format("INSERT INTO rrcMessageLookUp (rrcMessage,lookUpValue) VALUES ('{0}','{0}');", item);
                        sda.SelectCommand = cmd;
                        cmd.ExecuteNonQuery();

                    }
                    catch
                    {

                    }
                }

            }
            catch
            {

            }

            finally
            {
                sqlConClose();
            }

        }

        private void _updateRRCMessageLookUpDatabase()
        {
            try
            {
                DataTable data = (DataTable)(rrcMessageLookUpGrid.DataSource);

                foreach (DataRow item in data.Rows)
                {
                    _updateRRCMessageLookUpDB(item.ItemArray[0].ToString(), item.ItemArray[1].ToString());
                }

            }
            catch (Exception ex)
            {
                MsgBox(ex.Message);
            }
        }

        private void _updateRRCMessageLookUpDB(string rrcMessage, string lookupVal)
        {
            try
            {

                //   string[] rrc = extractRRCMsgs(traceDataTable);

                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                if (lookupVal == "")
                    lookupVal = "Null";

                cmd.CommandText = string.Format("UPDATE rrcMessageLookUp Set lookUpValue = '{0}' Where rrcMessage = '{1}'", lookupVal, rrcMessage);
                sda.SelectCommand = cmd;
                cmd.ExecuteNonQuery();


            }
            catch (Exception ex)
            {

                MsgBox(ex.Message);

            }
            finally
            {
                sqlConClose();
            }
        }

        private void UpdateDataRrcMessageLookUpGridView()
        {
            try
            {
                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = "SELECT * FROM rrcMessageLookUp";
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);
                rrcMessageLookUpGrid.DataSource = dt;
                rrcMessageLookUpGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                con.Close();
            }
            catch (Exception ex)
            {

                MsgBox(ex.Message);
                sqlConClose();
            }

        }


        private string LookupRRCMessageName(string message)
        {
            try
            {
                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = string.Format("SELECT * FROM rrcMessageLookUp WHERE rrcMessage = '{0}';", message);
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);

                con.Close();


                return dt.Rows[0][1].ToString();

            }
            catch (Exception ex)
            {

                MsgBox(ex.Message);
                sqlConClose();
                return null;
            }

        }

        private void sqlConClose()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message);
            }
        }
        /// <summary>
        /// Trace Data Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browse_Click(object sender, EventArgs e)
        {


            try
            {
                openFileDialog1.Multiselect = true;
                openFileDialog1.ShowDialog();
                // MessageBox.Show( openFileDialog1.FileName.ToString());


                foreach (var item in openFileDialog1.FileNames)
                {

                    string FilePath = item;
                    path.Text = FilePath;

                    //File.Copy(FilePath, "CTO Trace.csv", true);
                    //File.AppendAllLines()

                    if (traceDataTable.Columns.Count > 0)
                    {
                        string[] fileReadAllLines = File.ReadAllLines(FilePath);
                        string[] fileData = new string[fileReadAllLines.Length - 1];
                        for (int i = 1; i < (fileReadAllLines.Length); i++)
                        {
                            fileData[i - 1] = fileReadAllLines[i];
                        }
                        File.AppendAllLines("CTO Trace.csv", fileData);
                    }
                    else
                    {
                        File.Copy(FilePath, "CTO Trace.csv", true);
                    }

                    rows = File.ReadAllLines("CTO Trace.csv").Select(x => x.Split(',')).Where(x => x[0] != "" && x[1] != "").ToList();
                    //   traceDataTable = new DataTable();
                    string[] headers = rows[0];
                    rows.RemoveAt(0);
                    MsgBox(headers.Length.ToString());
                    if (!(traceDataTable.Columns.Count > 0))
                        for (int i = 0; i < headers.Length; i++)
                        {

                            // dt.Columns.Add(i.ToString());
                            traceDataTable.Columns.Add(headers[i]);
                        }

                    //rows.ForEach(x =>
                    //{
                    //    traceDataTable.Rows.Add(x);
                    //});
                    for (int x = 0; x < rows.Count; x++)
                    {
                        traceDataTable.Rows.Add(rows[x]);
                    }
                }
                //try
                //{
                //    dataGridView1.DataSource = traceDataTable;
                //}
                //catch (Exception ex)
                //{
                //    MsgBox(ex.Message+ " -_-");
                //}

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void updateUserList_DataVisualizer()
        //{
        //    try
        //    {

        //        List<string> lis = SelectAllUserFromDB();
        //        foreach (string s in lis)
        //        {
        //            UserNames_DataVisualizer.Items.Add(s);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        MsgBox(ex.Message);
        //    }
        //}

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void _cellDataUpload()
        {


            try
            {

                openFileDialog1.ShowDialog();
                //    MessageBox.Show(openFileDialog1.FileName);

                string FilePath = openFileDialog1.FileName.ToString();
                fpCell.Text = FilePath;
                // FilePath = "Cells Lat Longs.csv";
                File.Copy(FilePath, "Cells Lat Longs.csv", true);

                rows = File.ReadAllLines(FilePath).Select(x => x.Split(',')).Where(x => x[0] != "" && x[1] != "").ToList();

                dt = new DataTable();

                string[] headers = rows[0];
                rows.RemoveAt(0);

                if (dt.Columns.Count == 0)
                    for (int i = 0; i < headers.Length; i++)
                    {
                        // dt.Columns.Add(i.ToString());
                        dt.Columns.Add(headers[i].ToString());
                    }

                rows.ForEach(x =>
                {
                    dt.Rows.Add(x);
                });


                dataGridView2.DataSource = dt;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void _cellNamesDataUpload()
        {


            try
            {

                //    openFileDialog1.ShowDialog();
                //       MessageBox.Show(openFileDialog1.FileName);

                //     string FilePath = openFileDialog1.FileName.ToString();
                //     fpCell.Text = FilePath;
                // FilePath = "Cells Lat Longs.csv";
                //   File.Copy(FilePath, "CellNames.csv", true);
                List<string[]> fileData = new List<string[]>();
                fileData = File.ReadAllLines(mainDir + "/CellNames.csv").Select(x => x.Split(',')).Where(x => x[0] != "" && x[1] != "").ToList();
                cellNames = new DataTable();

                string[] headers = fileData[0];
                fileData.RemoveAt(0);

                if (cellNames.Columns.Count == 0)
                    for (int i = 0; i < headers.Length; i++)
                    {
                        // dt.Columns.Add(i.ToString());
                        cellNames.Columns.Add(headers[i].ToString());
                    }

                fileData.ForEach(x =>
                {
                    cellNames.Rows.Add(x);
                });


            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void _cellDataUpload_FromFile()
        {

            try
            {


                //openFileDialog1.ShowDialog();
                //MessageBox.Show(openFileDialog1.FileName.ToString());

                string FilePath;
                //FilePath = openFileDialog1.FileName.ToString();
                FilePath = "Cells Lat Longs.csv";
                //   File.Copy(FilePath, mainDir + "Cells Lat Longs.csv", true);

                rows = File.ReadAllLines(FilePath).Select(x => x.Split(',')).Where(x => x[0] != "" && x[1] != "").ToList();
                dt = new DataTable();
                string[] headers = rows[0];
                rows.RemoveAt(0);
                for (int i = 0; i <= 10; i++)
                {
                    // dt.Columns.Add(i.ToString());
                    dt.Columns.Add(headers[i].ToString());
                }

                rows.ForEach(x =>
                {
                    dt.Rows.Add(x);
                });


                dataGridView2.DataSource = dt;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updateIMSIDB()
        {
            foreach (var s in getIMSIs())
            {
                insertToImsiDB(s);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Developed by Hassan Niazi \nJunior RNO ZTE Pakistan\nHassanniazi93@gmail.com", "About");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private string[] extractUeids(DataTable table)
        {
            string[] s1 = null;

            int count = 0;
            try
            {


                //  DataRow[] result = table.Select("ueId = 'IMSI:410018147711211;TMSI:B9A5A66A'");
                //  DataRow[] result = table.Select("* ueId").Distinct().ToArray() ;

                DataView view = new DataView(table);
                DataTable distinctValues = view.ToTable(true, "ueId"); //, "CellId","rrcMsgName");
                DataRow[] result = distinctValues.Select();
                s1 = new string[result.Length];
                foreach (DataRow row in result)
                {
                    s1[count] = (string)row.ItemArray[0];
                    count++;
                }

                return s1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return s1;
            }
        }

        private string[] extractRRCMsgs(DataTable table)
        {
            string[] s1 = null;

            int count = 0;
            try
            {

                DataView view = new DataView(table);
                DataTable distinctValues = view.ToTable(true, "rrcMsgName");
                DataRow[] result = distinctValues.Select();
                s1 = new string[result.Length];
                foreach (DataRow row in result)
                {
                    s1[count] = (string)row.ItemArray[0];
                    count++;
                }

                return s1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return s1;
            }
        }

        private List<string> extractIMSI(string[] ueIDs)
        {
            List<string> IMSI = new List<string>();
            try
            {


                foreach (string s in ueIDs)
                {
                    if (!isEqualToIMSI(IMSI, s.Substring(5, 15)))
                        IMSI.Add(s.Substring(5, 15));
                }
                return IMSI;
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message);
                return IMSI;
            }
        }

        private bool isEqualToIMSI(List<string> lis, string s)
        {
            bool isEqual = false;
            foreach (var r in lis)
            {
                if (r == s)
                {
                    isEqual = true;
                }
            }

            return isEqual;
        }

        private void toolStripSplitButton2_ButtonClick(object sender, EventArgs e)
        {
            string[] s = extractUeids(traceDataTable);
            if (s != null)
                foreach (string _s in s)
                {
                    MessageBox.Show(_s);
                }

        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            try
            {
                MainMap.Zoom = trackBar1.Value;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void _traceDataUpload()
        {
            try
            {

                rows = File.ReadAllLines("CTO Trace.csv").Select(x => x.Split(',')).Where(x => x[0] != "" && x[1] != "").ToList();

                string[] headers = rows[0];
                rows.RemoveAt(0);
                if (traceDataTable.Columns.Count == 0)
                    for (int i = 0; i <= 26; i++)
                    {
                        // dt.Columns.Add(i.ToString());
                        traceDataTable.Columns.Add(headers[i].ToString());
                    }

                rows.ForEach(x =>
                {
                    traceDataTable.Rows.Add(x);
                });


                dataGridView1.DataSource = traceDataTable;

            }

            catch (Exception ex)
            {
                MsgBox(ex.Message);
            }
        }

        private void MapTab_Click(object sender, EventArgs e)
        {

        }

        private void MsgBox(string s)
        {
            MessageBox.Show(s);
        }

        private void MapPage_Click(object sender, EventArgs e)
        {

        }

        private void UpdateIMSIChkList()
        {
            IMSIChkList.Items.Clear();

            List<string> imsis = SelectAllUserFromDB();

            foreach (var s in imsis)
            {
                IMSIChkList.Items.Add(s);
                try
                {
                    // insertToImsiDB(s);
                }
                catch// (Exception ex)
                {

                    //MsgBox( ex.Message );
                }



            }
        }

        private void UpdateRRCMsgsList()
        {
            RRCMessages.Items.Clear();
            rrcItems.Clear();
            string[] rrc = extractRRCMsgs(traceDataTable);
            foreach (string s in rrc)
            {
                RRCMessages.Items.Add(s);
                messagesListBox.Items.Add(s);
                rrcChkListGrap2.Items.Add(s);
                rrcItems.Add(s);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private List<String> getIMSIs()
        {
            return extractIMSI(extractUeids(traceDataTable));
        }

        private void PlotMarkers_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            resetMap();

            try
            {

                int i = 1;
                toolStripProgressBar1.ProgressBar.Value = 0;
                unknownCellsFlag = false;
                foreach (string s in IMSIChkList.CheckedItems)
                {
                    img1 = Image.FromFile(string.Format("{0}/TrackingDot{1}.png", mainDir, i));
                    plotMarkers2(getIMSIFromUserName(s), img1);
                    i++;
                    toolStripProgressBar1.ProgressBar.Value = toolStripProgressBar1.ProgressBar.Value + (100 / IMSIChkList.CheckedItems.Count);
                }
                if (unknownCellsFlag)
                    MsgBox("Some Cell Coordinates were not available and cant be plotted. Please see the Alien Cells in Tool Config Tab for Cell Ids");
                //PointLatLng p = new PointLatLng(33.7294, 73.0931);
                // MainMap.Position = new PointLatLng(33.7294, 73.0931);
                //   img.Dispose();

            }
            catch (Exception ex)
            {
                MsgBox(ex.Message);
            }
        }

        private string getIMSIFromUserName(string names)
        {
            // List<string> imsis = new List<string>();
            string imsi = null;
            try
            {

                con = new SqlCeConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf"));
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();

                cmd.CommandText = "SELECT * FROM ImsiUsers where UserName = '" + names + "'";
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);
                imsi = dt.Rows[0][0].ToString();


                con.Close();
                return imsi;
            }
            catch (Exception ex)
            {

                MsgBox(ex.Message);
                con.Close();
                return null;
            }



        }

        private bool listConatains_point(List<_point> input, _point p)
        {
            bool hasData = false;

            foreach (var lp in input)
            {
                if (lp.lat == p.lat && lp.lng == p.lng)
                    hasData = true;
            }
            return hasData;
        }

        private void plotMarkers(string imsi, Image img)
        {
            try
            {
                DataRow[] dr = traceDataTable.Select("ueId = '" + imsi + "' And rrcMsgName = '" + RRCMessages.SelectedItem.ToString() + "'");
                PointLatLng p = new PointLatLng(33.7294, 73.0931);
                //   MainMap.Position = new PointLatLng(33.7294, 73.0931);
                GMapOverlay markersOverlay = new GMapOverlay("markers");
                List<_point> coords = new List<_point>();
                List<_point> allcoords = new List<_point>();
                _point pLocal = new _point(33.7294, 73.0931);
                foreach (DataRow d in dr)
                {

                    DataRow[] cdr = dt.Select("[cell Id] = '" + d["CellId"].ToString() + "'"); // Cell Not Found ; CDR would be Empty;

                    double latCell = double.Parse(cdr[0]["LAT"].ToString());
                    double lngCell = double.Parse(cdr[0]["LONG"].ToString());
                    pLocal = new _point(latCell, lngCell);
                    allcoords.Add(pLocal);
                    if (!listConatains_point(coords, pLocal))
                    {

                        //  MainMap.Position = new PointLatLng(latCell, lngCell);
                        p.Lat = latCell;
                        p.Lng = lngCell;
                        //  Image img;
                        //  img = Image.FromFile("TrackingDot.png");

                        //  GMapMarkerImage cusMarker = new GMapMarkerImage(p, img);


                        GMapMarkerImage cusMarker = new GMapMarkerImage(p, img);

                        markersOverlay.Markers.Add(cusMarker);
                        MainMap.Overlays.Add(markersOverlay);

                        coords.Add(pLocal);

                    }


                }

                //    MainMap.Position = new PointLatLng(pLocal.lat, pLocal.lng);

                MainMap.ZoomAndCenterMarkers(markersOverlay.Id);

                //img.Dispose();
                foreach (var r in coords)
                {
                    listBox1.Items.Add(r.lat.ToString() + "      " + r.lng.ToString());
                }
                foreach (var r in allcoords)
                {
                    listBox2.Items.Add(r.lat.ToString() + "      " + r.lng.ToString());
                }
                //listBox1.DataSource = coords;
            }
            catch
            {
                MsgBox("Please select valid arguments/KPI's to Plot or make sure the You have updated Cell Data");
            }
        }

        private void plotMarkers2(string imsi, Image img)
        {
            try
            {

                DataRow[] dr = traceDataTable.Select(string.Format("ueId LIKE '%{0}%' And rrcMsgName = '{1}'", imsi, RRCMessages.SelectedItem));
                PointLatLng p = new PointLatLng(33.7294, 73.0931);
                //   MainMap.Position = new PointLatLng(33.7294, 73.0931);
                GMapOverlay markersOverlay = new GMapOverlay("markers");
                List<_point> coords = new List<_point>();
                List<_point> allcoords = new List<_point>();
                _point pLocal = new _point(33.7294, 73.0931);
                foreach (DataRow d in dr)
                {
                    try
                    {
                        DataRow[] cdr = dt.Select("[Cell ID] = " + d["CellId"].ToString());

                        double latCell = double.Parse(cdr[0]["LAT"].ToString());
                        double lngCell = double.Parse(cdr[0]["LONG"].ToString());
                        pLocal = new _point(latCell, lngCell);
                        allcoords.Add(pLocal);
                        if (!listConatains_point(coords, pLocal))
                        {

                            p.Lat = latCell;
                            p.Lng = lngCell;

                            GMapMarkerImage cusMarker = new GMapMarkerImage(p, img);

                            markersOverlay.Markers.Add(cusMarker);
                            MainMap.Overlays.Add(markersOverlay);

                            coords.Add(pLocal);

                        }
                    }
                    catch
                    {
                        //   MsgBox(string.Format("Unable to locate Coordinates for the cell Id : {0} Please update Cell database", d["CellId"]));
                        //   coords.Add(pLocal);
                        if (!unknownCells.Items.Contains(d["CellId"].ToString()))
                            unknownCells.Items.Add(d["CellId"].ToString());

                        unknownCellsFlag = true;
                    }

                }

                //    MainMap.Position = new PointLatLng(pLocal.lat, pLocal.lng);

                MainMap.ZoomAndCenterMarkers(markersOverlay.Id);

                //img.Dispose();
                foreach (var r in coords)
                {
                    listBox1.Items.Add(r.lat.ToString() + "      " + r.lng.ToString());
                }
                foreach (var r in allcoords)
                {
                    listBox2.Items.Add(r.lat.ToString() + "      " + r.lng.ToString());
                }
                //listBox1.DataSource = coords;
            }
            catch
            {
                MsgBox("Please select valid arguments/KPI's to Plot or make sure the You have updated Cell Location Data");
            }
        }

        private void ResetMap_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            resetMap();
        }

        private void resetMap()
        {
            MainMap.Overlays.Clear();
            MainMap.Refresh();
        }

        private void mapProviderList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void mapProviderList_SelectedValueChanged(object sender, EventArgs e)
        {
            if (started == true)
            {
                MainMap.MapProvider = GMapProviders.List[mapProviderList.SelectedIndex];
            }
        }

        private void toolStripSplitButton3_ButtonClick(object sender, EventArgs e)
        {

            try
            {
                System.Net.IPHostEntry elf = System.Net.Dns.GetHostEntry("www.google.com");
                toolStatus.Text = "Connected!  ";
                MainMap.Manager.Mode = AccessMode.ServerAndCache;
            }
            catch
            {
                MainMap.Manager.Mode = AccessMode.CacheOnly;
                //  MessageBox.Show("No internet connection avaible, going to CacheOnly mode.",
                ///      "GMap.NET - Demo.WindowsForms", MessageBoxButtons.OK,
                //   MessageBoxIcon.Warning);
                toolStatus.Text = "Offline - Cache Mode Only";
            }
        }

        private void Capture_Click(object sender, EventArgs e)
        {
            //  MsgBox(Environment.CurrentDirectory.ToString());
            CaptureGraph();

        }

        private void CaptureGraph()
        {
            try
            {
                Image img = MainMap.ToImage();
                Image pxl = null;
                SaveFileDialog svf = new SaveFileDialog();

                if (JPG.Checked)
                    svf.DefaultExt = "jpg";
                else
                    svf.DefaultExt = "png";
                if (addLegend.Checked)
                {
                    Graphics g = Graphics.FromImage(img);
                    Pen blackPen = new Pen(Color.FromArgb(255, 0, 0, 0), 1);

                    RectangleF rectf = new RectangleF(10, 10, 180, 300);

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    int Textsize = 9;
                    int totalEle = IMSIChkList.CheckedItems.Count;
                    g.DrawRectangle(blackPen, 10, 10, 180, (totalEle * 30) + 20);
                    for (int i = 1; i <= IMSIChkList.CheckedItems.Count; i++)
                    {
                        pxl = Image.FromFile(mainDir + "/TrackingDot" + (i).ToString() + ".png");
                        g.DrawString(IMSIChkList.CheckedItems[i - 1].ToString(), new Font("Tahoma", Textsize, FontStyle.Bold), Brushes.CadetBlue, new RectangleF(20, 30 * i, 180, (totalEle * 30) + 20));
                        g.DrawImage(pxl, new Point(150, (30 * i) - 5));
                        pxl.Dispose();
                    }

                    g.Flush();


                    svf.AutoUpgradeEnabled = false;
                    svf.ShowDialog();

                    g.Save();

                    img.Save(svf.FileName);
                    //_addImagesToImageControls(svf.FileName);
                    g.Dispose();
                    img.Dispose();
                }
                else
                {
                    //_addImagesToImageControls(svf.FileName);
                    img.Save(svf.FileName);
                }
            }
            catch (Exception ex)
            {
                MsgBox("There is an error exporting file, Please restart the application \n " + ex.Message);
            }
        }

        private Image getMapImage()
        {
            try
            {
                Image img = MainMap.ToImage();
                Image pxl = null;
                // SaveFileDialog svf = new SaveFileDialog();

                //if (JPG.Checked)
                //    svf.DefaultExt = "jpg";
                //else
                //    svf.DefaultExt = "png";
                if (addLegend.Checked)
                {
                    Graphics g = Graphics.FromImage(img);
                    Pen blackPen = new Pen(Color.FromArgb(255, 0, 0, 0), 1);

                    RectangleF rectf = new RectangleF(10, 10, 180, 300);

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    int Textsize = 9;
                    int totalEle = IMSIChkList.CheckedItems.Count;
                    g.DrawRectangle(blackPen, 10, 10, 180, (totalEle * 30) + 20);
                    for (int i = 1; i <= IMSIChkList.CheckedItems.Count; i++)
                    {
                        pxl = Image.FromFile(string.Format("{0}/TrackingDot{1}.png", mainDir, i));
                        g.DrawString(IMSIChkList.CheckedItems[i - 1].ToString(), new Font("Tahoma", Textsize, FontStyle.Bold), Brushes.CadetBlue, new RectangleF(20, 30 * i, 180, (totalEle * 30) + 20));
                        g.DrawImage(pxl, new Point(150, (30 * i) - 5));
                        pxl.Dispose();
                    }

                    g.Flush();


                    // svf.AutoUpgradeEnabled = false;
                    // svf.ShowDialog();

                    g.Save();

                    //  img.Save(svf.FileName);
                    g.Dispose();
                    return img;
                }
                else
                {
                    return img;
                    // img.Save(svf.FileName);
                }
            }
            catch (Exception ex)
            {
                MsgBox("There is an error exporting file, Please restart the application \n " + ex.Message);
                return null;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            sqlConClose();
            Application.Exit();
        }

        private void BrowseCell_Click(object sender, EventArgs e)
        {
            _cellDataUpload();
        }

        private void rmvData_Click(object sender, EventArgs e)
        {
            traceDataTable.Columns.Clear();
            traceDataTable = new DataTable();
            DeleteImsiDB();
            File.Delete(mainDir + "/CTO Trace.csv");
            File.Create(mainDir + "/CTO Trace.csv").Dispose();

            dataGridView1.Refresh();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void UpdateIMSI_Click(object sender, EventArgs e)
        {
            try
            {
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                if (UserNameTB.Text == "")
                    UserNameTB.Text = "Null";

                cmd.CommandText = "UPDATE ImsiUsers Set UserName = '" + UserNameTB.Text + "' Where IMSI = '" + IMSITB.Text + "'";
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);
                dataGridView3.DataSource = dt;
                con.Close();
            }
            catch (Exception ex)
            {

                MsgBox(ex.Message);
            }
        }

        private void SearchIMSI_Click(object sender, EventArgs e)
        {
            try
            {
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = "SELECT * FROM ImsiUsers Where IMSI LIKE '%" + IMSITB.Text + "%'";
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);
                dataGridView3.DataSource = dt;
                con.Close();
            }
            catch (Exception ex)
            {

                MsgBox(ex.Message);
            }
        }

        private string getUserNameFromIMSI(string imsi)
        {
            try
            {
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = "SELECT * FROM ImsiUsers Where IMSI LIKE '%" + imsi + "%'";
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);
                // dataGridView3.DataSource = dt;
                con.Close();

                return dt.Rows[0][1].ToString();

            }
            catch (Exception ex)
            {

                MsgBox(ex.Message);
                return "User Not Found";
            }
        }

        private void RstButton_Click(object sender, EventArgs e)
        {
            try
            {
                sqlConOpen();
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message);
            }
        }

        private void insertToImsiDB(string imsi, string userName)
        {
            try
            {
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = "INSERT INTO ImsiUsers (IMSI,UserName) VALUES ('" + imsi + "','" + userName + "');";
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);
                dataGridView3.DataSource = dt;
                con.Close();
            }
            catch// (Exception ex)
            {
                con.Close();
                //  MsgBox(ex.Message);
            }
        }

        private void DeleteImsiDB()
        {
            try
            {
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = "DELETE FROM ImsiUsers;";
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);
                dataGridView3.DataSource = dt;
                con.Close();
            }
            catch// (Exception ex)
            {
                con.Close();
                //  MsgBox(ex.Message);
            }
        }

        private void insertToImsiDB(string imsi)
        {
            try
            {
                insertToImsiDB(imsi, "Null");
            }
            catch// (Exception ex)
            {

                //  MsgBox(ex.Message);
            }
        }

        private List<string> SelectAllUserFromDB()
        {
            List<string> users = new List<string>();

            try
            {
                SqlCeDataAdapter sda = new SqlCeDataAdapter();
                SqlCeCommand cmd = con.CreateCommand();

                con.Open();
                cmd.CommandText = "SELECT * FROM ImsiUsers";
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();
                sda.Fill(dt);
                //dataGridView3.DataSource = dt;
                IMSIChkList.Items.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    users.Add(r[1].ToString());

                }
                con.Close();

                return users;

            }
            catch (Exception ex)
            {

                MsgBox(ex.Message);
                con.Close();
                return null;
            }
        }

        private void MapTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MapTab.SelectedIndex == 0)
            {
                //   _cellDataUpload_FromFile();
                //   _traceDataUpload();
                updateIMSIDB(); // Fetchs IMSI's From The tracers Database to the IMSI DB
                UpdateIMSIChkList(); // From IMSI DB to Control
                UpdateRRCMsgsList();
                //    started = true;
            }
            if (MapTab.SelectedIndex == 4)
            {
                //barDockControlTop.Visible = true;
                //barDockControlRight.Visible = true;
                ////    ribbonControl1.Visible = true;
            }
            else
            {
                //    barDockControlTop.Visible = false;
                //    //    ribbonControl1.Visible = false;
                //    barDockControlRight.Visible = false;
            }
        }

        private void chartControl1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog opf = new OpenFileDialog();
                opf.ShowDialog();
                Excel_Com exc = new Excel_Com();
                exc.openExcelBook(opf.FileName);
                DataTable dt = exc.getWorkSheetData(1);

                //    dataGridView5.DataSource = dt;
                exc.closeExcelBook();

                /**
                 * 
                 * 
                 * 
                 * 
                 * */
                // Specify data members to bind the series.
                DevExpress.XtraCharts.Series series = new DevExpress.XtraCharts.Series("Series1", ViewType.Line);
                chartControl1.Series.Add(series);
                series.DataSource = dt;
                dataGridView5.DataSource = dt;
                //                dataGridView5.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
                dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                series.ArgumentScaleType = ScaleType.Auto;
                series.ArgumentDataMember = "UserName";
                series.ValueScaleType = ScaleType.Numerical;
                string[] STR = new string[1];
                STR[0] = "Value";
                series.ValueDataMembers.AddRange(STR);
                //.AddRange(new string[] { "Value" });

                // Set some properties to get a nice-looking chart.
                //   ((SideBySideBarSeriesView)series.View).ColorEach = true;
                //((XYDiagram)chartControl1.Diagram).AxisY.Visibility = DevExpress.Utils.DefaultBoolean.False;
                //chartControl1.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
                chartControl1.Refresh();
                chartControl1.RefreshData();
                /***
                * 
                * 
                * 
                * 
                * ***/

                //chart1.Series.Add("test");
                //chart1.Series["test"].XValueMember = "UserName";
                //chart1.Series["test"].YValueMembers = "Value";
                //chart1.DataSource = dt;
                //chart1.DataBind();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ribbonStatusBar1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog svf = new SaveFileDialog();
                svf.ShowDialog();



                chartControl1.ExportToImage(svf.FileName + ".jpeg", ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message);
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

            SaveFileDialog svf = new SaveFileDialog();
            svf.ShowDialog();
            DevExpress.XtraPrinting.XlsExportOptions dxo = new DevExpress.XtraPrinting.XlsExportOptions();
            dxo.ExportMode = DevExpress.XtraPrinting.XlsExportMode.SingleFile;
            chartControl1.ExportToXlsx(svf.FileName + ".xlsx");
        }

        private void GoTOCoord_Click(object sender, EventArgs e)
        {
            MainMap.Position = new PointLatLng(double.Parse(LAT.Text), double.Parse(LNG.Text));
        }

        private void MainMap_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng p = MainMap.FromLocalToLatLng(e.X, e.Y);
            currentCoords.Text = string.Format("Lat : {0} , Lng : {1}", p.Lat, p.Lng);
            ZoomStatusStrip.Text = string.Format(" Zoom Level : {0}", MainMap.Zoom);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            unknownCells.Items.Clear();
        }

        private void MainMap_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            plotGraphCalls(chartControl1, dataGridView5);

        }

        private void updateMessageBackup_Click(object sender, EventArgs e)
        {
            TracerV1.Properties.Settings.Default.mocStringOfficial = mocMessageFilter.Text;
            TracerV1.Properties.Settings.Default.mtcStringOfficial = mtcMessageFilter.Text;
            TracerV1.Properties.Settings.Default.drcStringOfficial = callDropFilterMessage.Text;
            TracerV1.Properties.Settings.Default.Save();
        }



        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (MapTab.SelectedIndex == 4)
            {
                //barDockControlTop.Visible = true;
                //barDockControlRight.Visible = true;
                ////    ribbonControl1.Visible = true;
            }
            else
            {
                //barDockControlTop.Visible = false;
                ////    ribbonControl1.Visible = false;
                //barDockControlRight.Visible = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string clipData = null;
            foreach (var s in unknownCells.Items)
            {
                clipData = clipData + " " + s;
            }

            Clipboard.SetText(clipData);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            TracerV1.Properties.Settings.Default.graphLabel1 = graphLabel1TB.Text;
            //    TracerV1.Properties.Settings.Default.mtcStringOfficial = mtcMessageFilter.Text;
            //   TracerV1.Properties.Settings.Default.drcStringOfficial = callDropFilterMessage.Text;
            TracerV1.Properties.Settings.Default.Save();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void splitContainer5_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void plotcc1_Click(object sender, EventArgs e)
        {
            plotGraph2(cc1, dgvV2, messagesListBox, graphLabel2TB.Text);
        }

        private void plotGraphCalls(ChartControl ch, DataGridView _dgv)
        {

            ch.Series.Clear();
            ch.Titles.Clear();
            ch.DataSource = null;


            UserMessageFilter umf = new UserMessageFilter(traceDataTable, mocMessageFilter.Text, mtcMessageFilter.Text, callDropFilterMessage.Text);
            DataTable graphData = new DataTable(); //rab setup , pagginging type 1 , signanling conn release 
            graphData.Clear();
            DataColumn dcGraph = new DataColumn("Date", System.Type.GetType("System.DateTime"));
            graphData.Columns.Add(dcGraph);
            dcGraph = new DataColumn("MOC", System.Type.GetType("System.Int32"));
            graphData.Columns.Add(dcGraph);
            dcGraph = new DataColumn("MTC", System.Type.GetType("System.Int32"));
            graphData.Columns.Add(dcGraph);
            dcGraph = new DataColumn("TotalCalls", System.Type.GetType("System.Int32"));
            graphData.Columns.Add(dcGraph);
            dcGraph = new DataColumn("CallDrop", System.Type.GetType("System.Int32"));
            graphData.Columns.Add(dcGraph);

            List<UserMessageFilter.countDate> dataList = new List<UserMessageFilter.countDate>();

            dataList = umf.getResult(mocMessageFilter.Text, mtcMessageFilter.Text, callDropFilterMessage.Text);
            if (dataList == null)
            {
                MessageBox.Show("Please Load Valid Data with valid Date Format i.e. DD/MM/YYYY. Time column should not contain Time value and should only show Date Data. \n\nError Details:\n" + umf.errorMessage);
                return;
            }

            foreach (UserMessageFilter.countDate s in dataList)
            {
                object[] str = { s.date, s.countMOC - s.countMTC, s.countMTC, s.countMOC + s.countMTC, s.countDropCalls };

                graphData.Rows.Add(str);
            }


            //object[] str = { DateTime.Today, umf.countMOC, umf.countMTC, umf.countDropCalls };

            // graphData.Rows.Add(str);

            _dgv.DataSource = graphData;
            //MsgBox(umf.errorMessage);
            _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;



            DevExpress.XtraCharts.Series seriesCallDrop = new DevExpress.XtraCharts.Series("Call Drop", ViewType.Line);
            ch.Series.Add(seriesCallDrop);
            seriesCallDrop.DataSource = graphData;
            _dgv.DataSource = graphData;
            //                _dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            seriesCallDrop.ArgumentScaleType = ScaleType.Auto;
            seriesCallDrop.ArgumentDataMember = "Date";
            seriesCallDrop.ValueScaleType = ScaleType.Numerical;
            string[] STR = new string[1];
            STR[0] = "CallDrop";
            //      STR[1] = "MTC";
            //      STR[2] = "CallDrop";
            seriesCallDrop.ValueDataMembers.AddRange(STR);


            DevExpress.XtraCharts.Series seriesMOC = new DevExpress.XtraCharts.Series("MOC", ViewType.Bar);
            ch.Series.Add(seriesMOC);

            ChartTitle myTitle = new ChartTitle();


            myTitle.Text = graphLabel1TB.Text;
            ch.Titles.Add(myTitle);
            seriesMOC.DataSource = graphData;
            _dgv.DataSource = graphData;
            //                _dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            seriesMOC.ArgumentScaleType = ScaleType.Auto;
            seriesMOC.ArgumentDataMember = "Date";
            seriesMOC.ValueScaleType = ScaleType.Numerical;
            STR = new string[1];
            STR[0] = "MOC";
            //      STR[1] = "MTC";
            //      STR[2] = "CallDrop";
            seriesMOC.ValueDataMembers.AddRange(STR);

            DevExpress.XtraCharts.Series seriesMTC = new DevExpress.XtraCharts.Series("MTC", ViewType.Bar);
            ch.Series.Add(seriesMTC);
            seriesMTC.DataSource = graphData;
            _dgv.DataSource = graphData;
            //                _dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            seriesMTC.ArgumentScaleType = ScaleType.Auto;
            seriesMTC.ArgumentDataMember = "Date";
            seriesMTC.ValueScaleType = ScaleType.Numerical;
            STR = new string[1];
            STR[0] = "MTC";
            //      STR[1] = "MTC";
            //      STR[2] = "CallDrop";
            seriesMTC.ValueDataMembers.AddRange(STR);

            DevExpress.XtraCharts.Series seriesTotalCalls = new DevExpress.XtraCharts.Series("Total Calls", ViewType.Bar);
            ch.Series.Add(seriesTotalCalls);
            seriesTotalCalls.DataSource = graphData;
            _dgv.DataSource = graphData;
            //                _dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            seriesTotalCalls.ArgumentScaleType = ScaleType.Auto;
            seriesTotalCalls.ArgumentDataMember = "Date";
            seriesTotalCalls.ValueScaleType = ScaleType.Numerical;
            STR = new string[1];
            STR[0] = "TotalCalls";
            //      STR[1] = "MTC";
            //      STR[2] = "CallDrop";
            seriesTotalCalls.ValueDataMembers.AddRange(STR);







            //.AddRange(new string[] { "Value" });

            // Set some properties to get a nice-looking chart.
            //   ((SideBySideBarSeriesView)series.View).ColorEach = true;
            //((XYDiagram)ch.Diagram).AxisY.Visibility = DevExpress.Utils.DefaultBoolean.False;
            //ch.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;


            ch.Refresh();
            ch.RefreshData();


        }

        /// <summary>
        /// Graph With Rab Setup Failures and Call Drop For Each User
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="_dgv"></param>
        private void plotGraph2(ChartControl ch, DataGridView _dgv, CheckedListBox chkList, string chartTitle)
        {
            _dgv.ScrollBars = ScrollBars.Both;

            ch.Series.Clear();
            ch.Titles.Clear();
            ch.DataSource = null;

            //// Chart Title Add

            ChartTitle myTitle = new ChartTitle();
            myTitle.Text = chartTitle;
            ch.Titles.Add(myTitle);
            /////

            UserMessageFilter umf = new UserMessageFilter(traceDataTable);
            DataTable graphData = new DataTable(); //rab setup , pagginging type 1 , signanling conn release 
            graphData.Clear();

            List<string> chkMessages = new List<string>();

            foreach (var r in chkList.CheckedItems)
            {
                chkMessages.Add(r.ToString());
            }

            List<UserMessageFilter.genericDataContainer> count = new List<UserMessageFilter.genericDataContainer>();
            List<string> str = getIMSIs();
            count = umf.getResult(chkMessages, str);
            if (count == null)
            {
                MessageBox.Show("Please Load Valid Data with valid Date Format i.e. DD/MM/YYYY. Time column should not contain Time value and should only show Date Data. \n\nError Details:\n" + umf.errorMessage);
                return;
            }


            DataColumn dcGraph;
            dcGraph = new DataColumn("User", System.Type.GetType("System.String"));
            graphData.Columns.Add(dcGraph);

            foreach (var item in chkMessages)
            {
                dcGraph = new DataColumn(LookupRRCMessageName(item), System.Type.GetType("System.Int32"));
                graphData.Columns.Add(dcGraph);
            }

            List<object> rowList = new List<object>();

            foreach (var s in count)
            {
                rowList.Clear();
                rowList.Add(getUserNameFromIMSI(s.imsi));

                foreach (var item in s.count)
                {
                    rowList.Add(item);
                }

                graphData.Rows.Add(rowList.ToArray());
            }

            //object[] rowArray = rowList.ToArray();

            //graphData.Rows.Add(rowArray); // Adding Row to datatable graphData

            _dgv.DataSource = graphData;

            //   _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            string[] STR = new string[1];

            foreach (var item in chkMessages)
            {
                string str1 = LookupRRCMessageName(item);
                DevExpress.XtraCharts.Series seriesMOC = new DevExpress.XtraCharts.Series(str1, ViewType.Bar);
                ch.Series.Add(seriesMOC);
                seriesMOC.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                seriesMOC.DataSource = graphData;
                _dgv.DataSource = graphData;
                //  _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                seriesMOC.ArgumentScaleType = ScaleType.Auto;
                seriesMOC.ArgumentDataMember = "User";
                seriesMOC.ValueScaleType = ScaleType.Numerical;
                STR[0] = str1;
                seriesMOC.ValueDataMembers.AddRange(STR);
            }
            dgv4.DataSource = graphData;
            ch.Refresh();
            ch.RefreshData();
            _dgv.Refresh();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            ListBox L = messagesListBox;
            TextBox T = textBox4;
            L.Items.Clear();

            foreach (string str in rrcItems)
            {
                if (str.StartsWith(T.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    L.Items.Add(str);
                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            ListBox L = rrcChkListGrap2;
            TextBox T = textBox5;
            L.Items.Clear();

            foreach (string str in rrcItems)
            {
                if (str.StartsWith(T.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    L.Items.Add(str);
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            plotGraph2(cc2, dgvv2g2, rrcChkListGrap2, graphLabel3TB.Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //SaveFileDialog svf = new SaveFileDialog();
            //svf.ShowDialog();
            //cc1.ExportToImage(svf.FileName + ".jpg", ImageFormat.Jpeg);
            cc1.ExportToImage(string.Format("{0}/chart2dv2.jpg", mainDir), ImageFormat.Jpeg); ;

            dgv4.DataSource = dgvV2.DataSource;
            _addImagesToImageControls(string.Format("{0}/chart2dv2.jpg", mainDir), graphLabel2TB.Text);
            gridToImage(dgv4, cc1.Width, "");
        }

        private void gridToImage(DataGridView __dgv, int widthToSync, string label)
        {
            //Resize DataGridView to full height.
            int height = __dgv.Height;
            __dgv.Height = __dgv.RowCount * __dgv.RowTemplate.Height;

            if (checkBox1.Checked)
            {
                __dgv.Width = widthToSync;
            }

            __dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //int width = __dgv.Width;
            ////int a = __dgv.DisplayedColumnCount(true);
            //int b = __dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Displayed);
            //__dgv.Width = __dgv.Columns.GetColumnCount(DataGridViewElementStates.None) * __dgv.RowHeadersWidth;


            //Create a Bitmap and draw the DataGridView on it.
            Bitmap bitmap = new Bitmap(__dgv.Width, __dgv.Height); // Replace with this referece
            __dgv.DrawToBitmap(bitmap, new Rectangle(0, 0, __dgv.Width, __dgv.Height));

            //Resize DataGridView back to original height.
            __dgv.Height = height;
            string path = mainDir + "/dgv.png";
            //Save the Bitmap to folder.

            bitmap.Save(path);
            _addImagesToImageControls(path, label);

        }

        private void button11_Click(object sender, EventArgs e)
        {
            //SaveFileDialog svf = new SaveFileDialog();
            //svf.ShowDialog();
            //cc2.ExportToImage(svf.FileName + ".jpg", ImageFormat.Jpeg);


            cc2.ExportToImage(string.Format("{0}/chart3dv2.jpg", mainDir), ImageFormat.Jpeg);

            dgv4.DataSource = dgvv2g2.DataSource;
            _addImagesToImageControls(string.Format("{0}/chart3dv2.jpg", mainDir), graphLabel3TB.Text);
            gridToImage(dgv4, cc2.Width, "");
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            // _updateRRCMessageLookUpDatabase();
        }

        private void rrcMessageLookUpGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {


            //     MsgBox(" Cell Value Changed " + e.RowIndex.ToString() + "  -  " + e.ColumnIndex.ToString() + ":::::::::::" + rrcMessageLookUpGrid[e.ColumnIndex,e.RowIndex].Value.ToString());

            _updateRRCMessageLookUpDB(rrcMessageLookUpGrid[0, e.RowIndex].Value.ToString(), rrcMessageLookUpGrid[1, e.RowIndex].Value.ToString());
            rrcMessageLookUpGrid[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.FromArgb(77, 255, 100);
        }

        private void rrcMessageLookUpGrid_RowLeave(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void generateDailyRCTTraceReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog svf = new SaveFileDialog();
                svf.Filter = "Excel Files | *.xlsx";
                svf.ShowDialog();
                PlotMarkers_Click(null, null);

                Image imgMap = getMapImage();

                string mapPath = string.Format("{0}/map.jpg", mainDir);

                imgMap.Save(mapPath);

                plotGraphCalls(chartControl1, dataGridView5);

                Image imgChart1;

                string chartPath = string.Format("{0}/chart1.jpg", mainDir);

                chartControl1.ExportToImage(chartPath, ImageFormat.Jpeg);

                imgChart1 = Image.FromFile(chartPath);


                Excel.Application xlApp = new Excel.Application();

                Excel.Workbook xlWorkBook = new Excel.Workbook();

                Excel.Worksheet xlWorksheet = new Excel.Worksheet();

                xlApp = new Excel.Application();

                xlWorkBook = xlApp.Application.Workbooks.Add(misValue);

                xlWorksheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                xlWorksheet.Shapes.AddPicture(mapPath, MsoTriState.msoFalse, MsoTriState.msoTrue, 25, 25, imgMap.Width, imgMap.Height);

                xlWorkBook.SaveAs(svf.FileName);

                closeExcelBook(xlWorkBook, xlApp, xlWorksheet);

                //Excel_Com exc = new Excel_Com(svf.FileName);

                //exc.ChangeWorkSheet(1);

                //exc.addPicture(25, 25, imgMap.Width, imgMap.Height, string.Format("{0}/map.jpg", mainDir));

                //exc.addPicture(25, 50 + imgMap.Height, imgChart1.Width, imgChart1.Height, string.Format("{0}/chart1.jpg", mainDir));

                //exc.closeExcelBook();

            }
            catch (Exception ex)
            {
                MsgBox(ex.Message);
            }
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;


            }
            finally
            {
                GC.Collect();
            }
        }
        public void closeExcelBook(Excel.Workbook xlWorkBook, Excel.Application xlApp, Excel.Worksheet xlWorkSheet)
        {

            try
            {
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
            catch (Exception ex)
            {

                MsgBox(ex.Message);
            }
        }

        private void _addImagesToImageControls(string FileName, string header)
        {
            snapControl1.Document.InsertHtmlText(snapControl1.Document.CaretPosition, string.Format("<br><b><font size=3>{0}</font></b><br>", header));
            Image IMGlOCAL = Image.FromFile(FileName);
            string tempFile = Path.GetTempFileName();
            IMGlOCAL.Save(tempFile);
            Image imgTemp = Image.FromFile(tempFile);
            int width = 0, height = 0;
            if (!((widthImage == null) || (heightImage == null)))
                if (!((widthImage.EditValue == null) || (heightImage.EditValue == null)))
                {
                    bool validWidth = int.TryParse(widthImage.EditValue.ToString(), out width);
                    bool validHeight = int.TryParse(heightImage.EditValue.ToString(), out height);

                    if (!(validWidth && validHeight))
                    {
                        width = imgTemp.Width;
                        height = imgTemp.Height;
                    }
                }
                else
                {

                    width = imgTemp.Width;
                    height = imgTemp.Height;
                }
            snapControl1.Document.Images.Insert(snapControl1.Document.CaretPosition, ScaleImage(imgTemp, width, height));
            IMGlOCAL.Dispose();
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }



        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //SaveFileDialog svf = new SaveFileDialog() { Filter = "Excel Files | *.xlsx" };
                //svf.ShowDialog();
                PlotMarkers_Click(null, null);

                Image imgMap = getMapImage();

                string mapPath = string.Format("{0}/map.jpg", mainDir);

                imgMap.Save(mapPath);

                imgMap.Dispose();

                plotGraphCalls(chartControl1, dataGridView5);

                // Image imgChart1;

                string chartPath = string.Format("{0}/chart1.jpg", mainDir);

                chartControl1.ExportToImage(chartPath, ImageFormat.Jpeg);

                //  imgChart1 = Image.FromFile(chartPath);

                _addImagesToImageControls(mapPath, "Map : ");
                _addImagesToImageControls(chartPath, "CTO Trace : ");

                dgv4.DataSource = dataGridView5.DataSource;
                gridToImage(dgv4, chartControl1.Width, "");
            }
            catch (Exception ex) { MsgBox(ex.Message); }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            LoadLicensceFile();

        }

        private void LoadLicensceFile()
        {
            try
            {


                OpenFileDialog opf = new OpenFileDialog();
                opf.ShowDialog();
                string lic = File.ReadAllText(opf.FileName);
                string licD = Decrypt(lic);
                //MsgBox(lic + " : " + licD);
                //MsgBox(DateTime.Today.ToString());
                Properties.Settings.Default.licenseLastDate = DateTime.Parse(licD);
                Properties.Settings.Default.Save();
                MsgBox(string.Format("Prodcut Licensce Extended Till : {0}", Properties.Settings.Default.licenseLastDate.Date));
                if (Properties.Settings.Default.licenseLastDate > DateTime.Today)
                {
                    Properties.Settings.Default.expired = false;
                    Properties.Settings.Default.Save();
                    expiryLic.Caption = "Licensced Till : " + Properties.Settings.Default.licenseLastDate.ToShortDateString();
                }
                else
                {
                    Properties.Settings.Default.expired = true;
                    Properties.Settings.Default.Save();
                    expiryLic.Caption = "Licensce Expired";
                }
            }
            catch (Exception ex)
            {
                MsgBox("Please select a valid Licensce File.");
                if (Properties.Settings.Default.licenseLastDate > DateTime.Today)
                {
                    Properties.Settings.Default.expired = false;
                    Properties.Settings.Default.Save();
                    expiryLic.Caption = "Licensced Till : " + Properties.Settings.Default.licenseLastDate.ToShortDateString();
                }
                else
                {
                    Properties.Settings.Default.expired = true;
                    Properties.Settings.Default.Save();
                    expiryLic.Caption = "Licensce Expired";
                }
            }

        }

        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        private void splitContainer6_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void RRCMessages_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            PlotMarkers_Click(sender, e);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            resetMap();
        }

        private void button8_Click_2(object sender, EventArgs e)
        {
            chartControl1.ExportToImage(string.Format("{0}/chartcon1.png", mainDir), ImageFormat.Png);
            dgv4.DataSource = dataGridView5.DataSource;
            _addImagesToImageControls(string.Format("{0}/chartcon1.png", mainDir), graphLabel2TB.Text);
            gridToImage(dgv4, chartControl1.Width, "");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            List<cell> siteAndCells = new List<cell>();
            siteAndCells = getSiteNameFormCellID();
            listBox3.Items.Clear();
            foreach (cell item in siteAndCells)
            {
                listBox3.Items.Add("Site : " + item.siteName + " ; Cell ID : " + item.cellID + " ; " + " Cell Name : " + item.cellName);
            }
        }

        //private void 

        private List<cell> getSiteNameFormCellID()
        {
            List<cell> output = new List<cell>();
            try
            {

                List<string> Cells = getAllActiveCells();
                cell c = new cell();
                foreach (var _cell in Cells)
                {
                    c.cellID = _cell;
                    DataRow[] _r = dt.Select("[Cell ID] = " + _cell.ToString());
                    DataRow[] _c = cellNames.Select("[Cell ID] = " + _cell.ToString());
                    c.siteName = _r[0][0].ToString();
                    c.cellName = _c[0][1].ToString();
                    output.Add(c);
                }
            }

            catch (Exception ex)
            {

                MsgBox(ex.Message);
            }

            return output;
        }



        private List<string> getAllActiveCells()
        {
            List<string> cells = new List<string>();
            DataView view = new DataView(traceDataTable);
            DataTable distinctValues = view.ToTable(true, "CellId");
            foreach (DataRow row in distinctValues.Rows)
            {
                cells.Add(row[0].ToString());
            }
            return cells;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            string clipData = null;
            foreach (var s in listBox3.Items)
            {
                clipData = clipData + " \n" + s;
            }

            Clipboard.SetText(clipData);
            MsgBox("Data Copied To Clipboard.");
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MessageBox.Show("Developed By Muhammad Hassan Niazi\nHassanniazi93@gmail.com\nRF ZTE Pakistan","About",MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }


    public class GMapMarkerImage : GMap.NET.WindowsForms.GMapMarker
    {
        private Image img;

        /// <summary>
        /// The image to display as a marker.
        /// </summary>
        public Image MarkerImage
        {
            get
            {
                return img;
            }
            set
            {
                img = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p">The position of the marker</param>
        public GMapMarkerImage(PointLatLng p, Image image)
            : base(p)
        {
            img = image;
            Size = img.Size;
            Offset = new System.Drawing.Point(-Size.Width / 2, -Size.Height / 2);
        }

        public override void OnRender(Graphics g)
        {
            g.DrawImage(img, LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
        }
    }
}
