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

namespace Legacy_Admin
{
    public partial class Login_Form : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public List<string> serverLog = new List<string>();
        public List<string> command = new List<string>();
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Thread msgLoop;
        public bool loggedin = false;
        private static string version = "Admin_1.01";
        private static bool version_checked = false;
        System.Windows.Forms.Timer timer;
        public int cmdpos = 0;

        public Login_Form()
        {
            InitializeComponent();
        }

        private void Login_Form_Load(object sender, EventArgs e)
        {
            
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();
            msgLoop = new Thread(ServerMsgLoop);
            msgLoop.IsBackground = true;
            msgLoop.Start();
            this.panel1.MouseDown += panel1_MouseDown;
            Connect();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += timer_tick;
            timer.Start();
        }

        private void timer_tick(object sender, EventArgs e)
        {
            checkCommands();
        }

        private void checkCommands()
        {
            
            if (command.Count == 1)
            {
                
                byte[] data = PacketWriter.sendString(Convert.ToString(8 + " " + command[0]));
                client.Send(data);
                command.Clear();
                
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Main_Form form = new Main_Form(serverLog,command);
            form.Show();
            this.Hide();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (loggedin == false)
            {

            }
        }

        public void ServerMsgLoop()
        {
            while (true)
            {
                
                if (client.Connected)
                {
                    byte[] buffer = new byte[1024];
                    int rec = client.Receive(buffer);
                    byte[] data = new byte[rec];

                    Array.Copy(buffer, data, rec);
                    string msg = Encoding.ASCII.GetString(data);
                    string[] msgArgs = msg.Split(' ');
                    string text = "";
                    for (int i = 1; i < msgArgs.Length; i++)
                        text += msgArgs[i] + " ";
                    if (version_checked == false)
                    {
                        if (msgArgs[0] == ErrorCodes.Version_Success.ToString())
                        {
                            version_checked = true;
                        }
                        else if (msgArgs[0] == ErrorCodes.Error.ToString())
                        {
                            MessageBox.Show("PLEASE UPDATE YOUR CLIENT, YOUR USING AN OUTDATED VERSION CURRENT VERSION IS " + msgArgs[1]);
                            Application.Exit();
                        }
                    }
                    if (loggedin == false)
                    {
                        if (msgArgs[0] == ErrorCodes.Success.ToString())
                        {
                            loggedin = true;
                        }
                        else
                        {
                            MessageBox.Show(msgArgs[0]);
                        }
                    }
                    if (msgArgs[0] == ProcessCodes.Kick.ToString())
                    {
                        MessageBox.Show("Kicked: " + text);
                        Application.Exit();
                    }
                    else if (msgArgs[0] == ProcessCodes.Ban.ToString())
                    {
                        MessageBox.Show("BANNED: " + text);
                        Application.Exit();
                    }
                    else if (msgArgs[0] == ProcessCodes.Reboot.ToString())
                    {
                        MessageBox.Show("Reboot: " + text);
                        Application.Exit();
                    }
                    else if (msgArgs[0] == ProcessCodes.Message.ToString())
                    {
                        MessageBox.Show("Message: " + text);
                    }

                    if(msgArgs[0] == ProcessCodes.Logger.ToString())
                    {
                        serverLog.Add(text);
                    }
                    
                    Thread.Sleep(2);

                    Thread.Sleep(2);
                }
            }
        }
        private void Login()
        {
            if (client.Connected)
            {
                byte[] data = PacketWriter.sendString(Convert.ToString(0 + " " + Username_Textbox.Text + " " + Password_Textbox.Text));
                client.Send(data);
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
                    return;
                }
                //here we catch an error and can do whatever we want to it, we'll probs close the program for security
                catch (SocketException ex)
                {
                    DialogResult dialogResult = MessageBox.Show("Unable to connect to server, check your connection\nPress retry to try again or cancel to exit", "Unable To Connect", MessageBoxButtons.RetryCancel);
                    if (dialogResult == DialogResult.Cancel)
                    {
                        break;
                    }

                }
            }
            Application.Exit();
            if (client.Connected)
            {
                byte[] data = PacketWriter.sendString(Convert.ToString(6 + " " + version));
                client.Send(data);
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
