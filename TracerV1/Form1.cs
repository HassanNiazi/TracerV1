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

using DevExpress.XtraCharts;
using System.Drawing.Imaging;
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
        DataTable traceDataTable = new DataTable();
        DataTable dt = new DataTable();
        Image img1 = null;
        string mainDir = Environment.CurrentDirectory;
        SqlCeConnection con;
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

        public Form1()
        {
            InitializeComponent();

            mapProviderList.DataSource = GMapProviders.List;


            //SaveFileDialog svff = new SaveFileDialog();
            //svff.ShowDialog();
            //Excel_Com xlC = new Excel_Com(svff.FileName);
            //OpenFileDialog opf = new OpenFileDialog();
            //opf.ShowDialog();
            //Excel_Com ex = new Excel_Com();
            //ex.openExcelBook(opf.FileName);
            //DataTable dt = ex.getWorkSheetData(1);
            //dataGridView4.DataSource = dt;


            try
            {
                System.Net.IPHostEntry e = System.Net.Dns.GetHostEntry("www.google.com");

            }
            catch
            {
                MainMap.Manager.Mode = AccessMode.CacheOnly;
                MessageBox.Show("No internet connection avaible, going to CacheOnly mode.",
                      "GMap.NET - Demo.WindowsForms", MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);
            }

            // config map
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
            //  MsgBox(mainDir);
            sqlConOpen();
            refetchData();
            barDockControlTop.Visible = false;
            barDockControlRight.Visible = false;
            //      ribbonControl1.Visible = false;
          //  MsgBox(TracerV1.Properties.Settings.Default.mocStringOfficial);
            mocMessageFilter.Text = TracerV1.Properties.Settings.Default.mocStringOfficial;
            mtcMessageFilter.Text = TracerV1.Properties.Settings.Default.mtcStringOfficial;
            callDropFilterMessage.Text = TracerV1.Properties.Settings.Default.drcStringOfficial;


        }

        private void refetchData()
        {
            try
            {
                _cellDataUpload_FromFile();
                _traceDataUpload();
                updateIMSIDB(); // Fetchs IMSI's From The tracers Database to the IMSI DB
                UpdateIMSIChkList(); // From IMSI DB to Control
                UpdateRRCMsgsList();
                updateUserList_DataVisualizer();
                started = true;
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message);
            }
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


                openFileDialog1.ShowDialog();
                // MessageBox.Show( openFileDialog1.FileName.ToString());

                string FilePath = openFileDialog1.FileName;
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
                if (!(traceDataTable.Columns.Count > 0))
                    for (int i = 0; i <= 26; i++)
                    {
                        // dt.Columns.Add(i.ToString());
                        traceDataTable.Columns.Add(headers[i]);
                    }

                rows.ForEach(x =>
                {
                    traceDataTable.Rows.Add(x);
                });


                dataGridView1.DataSource = traceDataTable;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updateUserList_DataVisualizer()
        {
            try
            {

                List<string> lis = SelectAllUserFromDB();
                foreach (string s in lis)
                {
                    UserNames_DataVisualizer.Items.Add(s);
                }

            }
            catch (Exception ex)
            {
                MsgBox(ex.Message);
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void _cellDataUpload()
        {


            try
            {

                openFileDialog1.ShowDialog();
                MessageBox.Show(openFileDialog1.FileName);

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

            string[] rrc = extractRRCMsgs(traceDataTable);
            foreach (string s in rrc)
            {
                RRCMessages.Items.Add(s);
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
                    g.Dispose();
                    img.Dispose();
                }
                else
                {
                    img.Save(svf.FileName);
                }
            }
            catch (Exception ex)
            {
                MsgBox("There is an error exporting file, Please restart the application \n " + ex.Message);
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
            traceDataTable.Clear();
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
                barDockControlTop.Visible = true;
                barDockControlRight.Visible = true;
                //    ribbonControl1.Visible = true;
            }
            else
            {
                barDockControlTop.Visible = false;
                //    ribbonControl1.Visible = false;
                barDockControlRight.Visible = false;
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
                exc.closeExcelBook(opf.FileName);

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
            openFileDialog1.ShowDialog();
            DevExpress.XtraPrinting.XlsExportOptions dxo = new DevExpress.XtraPrinting.XlsExportOptions();
            dxo.ExportMode = DevExpress.XtraPrinting.XlsExportMode.SingleFile;

            chartControl1.ExportToXlsx(openFileDialog1.FileName);
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


            UserMessageFilter umf = new UserMessageFilter(traceDataTable, mocMessageFilter.Text , mtcMessageFilter.Text , callDropFilterMessage.Text );
            DataTable graphData = new DataTable(); //rab setup , pagginging type 1 , signanling conn release 
            graphData.Clear();
            DataColumn dcGraph = new DataColumn("Date", System.Type.GetType("System.DateTime"));
            graphData.Columns.Add(dcGraph);
            dcGraph = new DataColumn("MOC", System.Type.GetType("System.Int32"));
            graphData.Columns.Add(dcGraph);
            dcGraph = new DataColumn("MTC", System.Type.GetType("System.Int32"));
            graphData.Columns.Add(dcGraph);
            dcGraph = new DataColumn("CallDrop", System.Type.GetType("System.Int32"));
            graphData.Columns.Add(dcGraph);
            dcGraph = new DataColumn("TotalCalls", System.Type.GetType("System.Int32"));
            graphData.Columns.Add(dcGraph);

            List<UserMessageFilter.countDate> dataList = new List<UserMessageFilter.countDate>();

            dataList = umf.getResult(mocMessageFilter.Text, mtcMessageFilter.Text, callDropFilterMessage.Text);
            foreach (UserMessageFilter.countDate s in dataList)
            {
                object[] str = { s.date, s.countMOC, s.countMTC, s.countDropCalls, s.countMOC + s.countMTC };

                graphData.Rows.Add(str);
            }


            //object[] str = { DateTime.Today, umf.countMOC, umf.countMTC, umf.countDropCalls };

            // graphData.Rows.Add(str);

            dataGridView5.DataSource = graphData;
            //MsgBox(umf.errorMessage);
            dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;



            DevExpress.XtraCharts.Series seriesMOC = new DevExpress.XtraCharts.Series("MOC", ViewType.Bar);
            chartControl1.Series.Add(seriesMOC);
            seriesMOC.DataSource = graphData;
            dataGridView5.DataSource = graphData;
            //                dataGridView5.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            seriesMOC.ArgumentScaleType = ScaleType.Auto;
            seriesMOC.ArgumentDataMember = "Date";
            seriesMOC.ValueScaleType = ScaleType.Numerical;
            string[] STR = new string[1];
            STR[0] = "MOC";
            //      STR[1] = "MTC";
            //      STR[2] = "CallDrop";
            seriesMOC.ValueDataMembers.AddRange(STR);

            DevExpress.XtraCharts.Series seriesMTC = new DevExpress.XtraCharts.Series("MTC", ViewType.Bar);
            chartControl1.Series.Add(seriesMTC);
            seriesMTC.DataSource = graphData;
            dataGridView5.DataSource = graphData;
            //                dataGridView5.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            seriesMTC.ArgumentScaleType = ScaleType.Auto;
            seriesMTC.ArgumentDataMember = "Date";
            seriesMTC.ValueScaleType = ScaleType.Numerical;
            STR = new string[1];
            STR[0] = "MTC";
            //      STR[1] = "MTC";
            //      STR[2] = "CallDrop";
            seriesMTC.ValueDataMembers.AddRange(STR);

            DevExpress.XtraCharts.Series seriesTotalCalls = new DevExpress.XtraCharts.Series("Total Calls", ViewType.Bar);
            chartControl1.Series.Add(seriesTotalCalls);
            seriesTotalCalls.DataSource = graphData;
            dataGridView5.DataSource = graphData;
            //                dataGridView5.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            seriesTotalCalls.ArgumentScaleType = ScaleType.Auto;
            seriesTotalCalls.ArgumentDataMember = "Date";
            seriesTotalCalls.ValueScaleType = ScaleType.Numerical;
            STR = new string[1];
            STR[0] = "TotalCalls";
            //      STR[1] = "MTC";
            //      STR[2] = "CallDrop";
            seriesTotalCalls.ValueDataMembers.AddRange(STR);




            DevExpress.XtraCharts.Series seriesCallDrop = new DevExpress.XtraCharts.Series("Call Drop", ViewType.Bar);
            chartControl1.Series.Add(seriesCallDrop);
            seriesCallDrop.DataSource = graphData;
            dataGridView5.DataSource = graphData;
            //                dataGridView5.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            seriesCallDrop.ArgumentScaleType = ScaleType.Auto;
            seriesCallDrop.ArgumentDataMember = "Date";
            seriesCallDrop.ValueScaleType = ScaleType.Numerical;
            STR = new string[1];
            STR[0] = "CallDrop";
            //      STR[1] = "MTC";
            //      STR[2] = "CallDrop";
            seriesCallDrop.ValueDataMembers.AddRange(STR);


            //.AddRange(new string[] { "Value" });

            // Set some properties to get a nice-looking chart.
            //   ((SideBySideBarSeriesView)series.View).ColorEach = true;
            //((XYDiagram)chartControl1.Diagram).AxisY.Visibility = DevExpress.Utils.DefaultBoolean.False;
            //chartControl1.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
            chartControl1.Refresh();
            chartControl1.RefreshData();



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
                barDockControlTop.Visible = true;
                barDockControlRight.Visible = true;
                //    ribbonControl1.Visible = true;
            }
            else
            {
                barDockControlTop.Visible = false;
                //    ribbonControl1.Visible = false;
                barDockControlRight.Visible = false;
            }
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
