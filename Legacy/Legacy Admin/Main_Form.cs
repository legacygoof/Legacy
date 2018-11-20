using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Legacy_Admin
{
    public partial class Main_Form : Form
    {
        List<string> logger;
        int logPos = 0;
        private Timer _timer;
        public Main_Form(List<string> logger)
        {
            InitializeComponent();
            this.logger = logger;
        }

        private void Main_Form_Load(object sender, EventArgs e)
        {
            DB_Helper.InitializeDB();
            TokensGrid.DataSource = DB_Helper.getTokenInfo();
            DatabaseClientsGrid.DataSource = DB_Helper.getDbClientInfo();
            ConnectedClientsGrid.DataSource = getConnectedClientInfo();

            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            RefreshServerLog();
        }

        private void RefreshServerLog()
        {
            if(logger.Count > logPos)
            {
                listBox1.Items.Add(logger[logPos]);
                logPos++;
            }
        }

        private DataTable getConnectedClientInfo()
        {
            DataTable connectedClientTable = new DataTable();
            return connectedClientTable;
        }

        

        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }


        #region DBclient buttons
        //client refresh
        private void dbClientsRefresh_Button_Click(object sender, EventArgs e)
        {
            DatabaseClientsGrid.DataSource = DB_Helper.getDbClientInfo();
        }

        //save user changes
        private void button22_Click_1(object sender, EventArgs e)
        {
            DB_Helper.Update_Users();
        }

        //ban all
        private void button2_Click(object sender, EventArgs e)
        {

        }
        //give all time
        private void button1_Click(object sender, EventArgs e)
        {

        }
        //set all logged in
        private void button4_Click(object sender, EventArgs e)
        {

        }
        //set all logged out
        private void button5_Click(object sender, EventArgs e)
        {

        }


        //unban all
        private void button3_Click(object sender, EventArgs e)
        {
            
        }
        #endregion

        private void button22_Click(object sender, EventArgs e)
        {

        }

        private void DatabaseClientsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        






        #region Token Tab
        //TOKEN REFRESH
        private void TokensRefresh_Button_Click(object sender, EventArgs e)
        {
            TokensGrid.DataSource = DB_Helper.getTokenInfo();
        }
        //token save
        private void button23_Click(object sender, EventArgs e)
        {
            DB_Helper.Update_Tokens();
        }
        //custom generate button
        private void button7_Click(object sender, EventArgs e)
        {
            int days = Convert.ToInt32(daysNum.Value);
            int months = Convert.ToInt32(MonthsNum.Value);
            int years = Convert.ToInt32(YearsNum.Value);
            int totalDays = 0;
            totalDays += days;
            if (months > 0)
                totalDays += (months * 30);
            if (years > 0)
                totalDays += (years * 365);

            DB_Helper.Generate_Token(totalDays);
        }
        //generate lifetime button
        private void button8_Click(object sender, EventArgs e)
        {
            DB_Helper.Generate_Token(999,textBox1.Text);
        }
        //genrate trial token
        private void button9_Click(object sender, EventArgs e)
        {
            DB_Helper.Generate_Token(1, textBox2.Text);
        }
        //generate 1 day
        private void button10_Click(object sender, EventArgs e)
        {
            DB_Helper.Generate_Token(1,Convert.ToInt32(tokenNum.Value));
        }
        //generate 2 day
        private void button11_Click(object sender, EventArgs e)
        {
            DB_Helper.Generate_Token(2, Convert.ToInt32(tokenNum.Value));
        }
        //generate 1 week
        private void button12_Click(object sender, EventArgs e)
        {
            DB_Helper.Generate_Token(7, Convert.ToInt32(tokenNum.Value));
        }
        //genereate 1 month
        private void button13_Click(object sender, EventArgs e)
        {
            DB_Helper.Generate_Token(30, Convert.ToInt32(tokenNum.Value));
        }
        //generate 2 month
        private void button14_Click(object sender, EventArgs e)
        {
            DB_Helper.Generate_Token(60, Convert.ToInt32(tokenNum.Value));
        }
        //genereate 3 month
        private void button15_Click(object sender, EventArgs e)
        {
            DB_Helper.Generate_Token(90, Convert.ToInt32(tokenNum.Value));
        }
        #endregion

        
    }
}
