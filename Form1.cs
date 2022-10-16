using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRC_Game.Server; //To be added in server.cs

namespace VRC_Game
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FSDServer server = new();
            string Path = filePath.Text;
            server.Start(Path);
        }

        private void filePath_TextChanged(object sender, EventArgs e)
        {
            //Not used
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            // Not Used
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                DefaultExt = "txt",
                CheckFileExists = true,
                CheckPathExists = true
            };
            
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath.Text = openFileDialog1.FileName;
            }

        }
    }
}
