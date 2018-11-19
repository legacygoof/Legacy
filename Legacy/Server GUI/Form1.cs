using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server_GUI
{
    public partial class Form1 : Form
    {
        string old_msg = "";
        int userListSize = 0;
        BackgroundWorker logwork = new BackgroundWorker();
        BackgroundWorker userWork = new BackgroundWorker();
        private Server server = new Server();
        public Form1(Server server)
        {
            InitializeComponent();
            Login_Helper.InitializeDB();
            this.server = server;
            logwork.DoWork += Logwork_DoWork;
            logwork.RunWorkerCompleted += Logwork_RunWorkerCompleted;
            userWork.DoWork += UserWork_DoWork;
            userWork.RunWorkerCompleted += UserWork_RunWorkerCompleted;
            logwork.RunWorkerAsync();
            userWork.RunWorkerAsync();
            
        }

        private void UserWork_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                if (userListSize != server.userList.Count)
                {
                    return;
                }
            }
        }

        private void UserWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            Users[] templist = new Users[server.userList.Count-1];
            server.userList.CopyTo(templist);
            

            foreach (Users u in templist)
                if (u.Name != null && u.Name != "")
                dataGridView1.Rows.Add(u.Name,u.IP,u.clientSocket,"false");
            userWork.RunWorkerAsync();
        }

        private void Logwork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Server_InfoBox.Items.Add(old_msg);
            logwork.RunWorkerAsync();
        }

        private void Logwork_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                if (!old_msg.Equals(server.message))
                {
                    old_msg = server.message;
                    return;
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            server.Start();
            
        }
    }
}
