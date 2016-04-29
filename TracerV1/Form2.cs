using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace TracerV1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Thread t1 = new Thread(doWork);
            t1.Start();

        }

        private void doWork()
        {
            Thread.Sleep(200);
            label1.Text = "Loading Libraries.";
            Thread.Sleep(300);
            label1.Text = "Loading Libraries..";
            Thread.Sleep(600);
            label1.Text = "Loading Libraries...";
            Thread.Sleep(500);
            label1.Text = "Initializing GUI Components";
            Thread.Sleep(700);
            label1.Text = "Connecting Server";
            Thread.Sleep(300);
            label1.Text = "Connecting Server.";
            Thread.Sleep(600);
            label1.Text = "Connecting Server..";
            Thread.Sleep(400);
            label1.Text = "Connecting Server...";
            Thread.Sleep(800);
            label1.Text = "Prefetching Database";
            Thread.Sleep(700);
            label1.Text = "Application Started";
            this.Close();
            Form1 f1 = new Form1();
            f1.ShowDialog();
            
            
        }
    }
}
