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
        public Main_Form()
        {
            InitializeComponent();
        }

        private void Main_Form_Load(object sender, EventArgs e)
        {
            DB_Helper.InitializeDB();
            TokensGrid.DataSource = DB_Helper.getTokenInfo();
            DatabaseClientsGrid.DataSource = DB_Helper.getDbClientInfo();
            ConnectedClientsGrid.DataSource = getConnectedClientInfo();
        }

        private DataTable getConnectedClientInfo()
        {
            DataTable connectedClientTable = new DataTable();
            return connectedClientTable;
        }

        private void dbClientsRefresh_Button_Click(object sender, EventArgs e)
        {
            //DatabaseClientsGrid.Rows.Clear();
            DatabaseClientsGrid.DataSource = DB_Helper.getDbClientInfo();
        }

        private void TokensRefresh_Button_Click(object sender, EventArgs e)
        {
            TokensGrid.Rows.Clear();
            TokensGrid.DataSource = DB_Helper.getTokenInfo();
        }

        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button22_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void DatabaseClientsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        //save user changes
        private void button22_Click_1(object sender, EventArgs e)
        {
            DB_Helper.Update_Users();
        }
    }
}
