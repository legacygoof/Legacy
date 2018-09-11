using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Legacy
{
    public partial class Login_Form : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Thread msgLoop;

        

        public Login_Form()
        {
            InitializeComponent();
        }

        private void Login_Form_Load(object sender, EventArgs e)
        {
            msgLoop = new Thread(ServerMsgLoop);
            this.panel1.MouseDown += panel1_MouseDown;
            Connect();
        }


        public void ServerMsgLoop()
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                int rec = client.Receive(buffer);
                byte[] data = new byte[rec];
                Array.Copy(buffer, data, rec);
                MessageBox.Show(Encoding.ASCII.GetString(data));
                Thread.Sleep(2);
            }
        }
        private void Login()
        {
            if(client.Connected)
            {
                byte[] data = PacketWriter.sendString(Convert.ToString(0 + " " + Username_Textbox.Text + " " + Password_Textbox.Text));
                client.Send(data);

                byte[] recBuffer = new byte[1024];
                int recSize = client.Receive(recBuffer);
                byte[] recData = new byte[recSize];
                Array.Copy(recBuffer, recData, recSize);
                MessageBox.Show(Encoding.ASCII.GetString(recData));
            }
        }

        /// <summary>
        /// this will intialize the connection for our socket(client)
        /// </summary>
        private void Connect()
        {
            //right now testing is being done on local host 
            //we'll use a while loop until a connection is made, might want to add a counter later
            while (!client.Connected)
            {
                //using a try and catch so if we get an error the program doesnt just crash
                //and we can close the program with reasoning behind it
                try
                {
                    //we'll change the ip to an md5 encryption so login server doesnt get ddosed
                    client.Connect(IPAddress.Parse("192.168.1.209"), 1234);//IPAddress.Loopback, 1234);
                }
                //here we catch an error and can do whatever we want to it, we'll probs close the program for security
                catch (SocketException ex)
                {
                    MessageBox.Show("Unable to connect to server\nError: " + ex);
                    this.Close();
                    Application.Exit();
                }
            }
        }


        private void panel1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.ExitApp_Hover_Pic;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.ExitApp_Pic;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
