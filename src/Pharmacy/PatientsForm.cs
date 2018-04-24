using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pharmacy
{
    public partial class PatientsForm : Form
    {
        public PatientsForm()
        {
            InitializeComponent();
        }

        private void PatientsForm_Load(object sender, EventArgs e)
        {
            var DB = new LINQ_PharmacyDataContext();
            this.dgvPatients.DataSource = DB.Patients;
            this.dgvPatients.Columns[0].HeaderText = "کد بیمار";
            this.dgvPatients.Columns[1].HeaderText = "کد بیمه";
            this.dgvPatients.Columns[2].HeaderText = "کد بیمه درمانی";
            this.dgvPatients.Columns[3].HeaderText = "نام بیمار";
            this.dgvPatients.Columns[4].HeaderText = "تاریخ نسخه";
            this.dgvPatients.Columns[5].HeaderText = "شماره صفحه نسخه";
            this.dgvPatients.Columns[6].HeaderText = "نام پزشک";
            this.dgvPatients.Columns[7].HeaderText = "شماره نظام پزشکی";
            this.dgvPatients.Columns[8].HeaderText = "جمع قیمت فروش";
            this.dgvPatients.Columns[9].HeaderText = "جمع قیمت خرید";
            this.dgvPatients.Columns[10].HeaderText = "سهم بیمه شده";
            this.dgvPatients.Columns[11].HeaderText = "سهم سازمان";
            //
            // set column comboBox
            //
            columnToolStripComboBox.Items.Clear();
            foreach (DataGridViewColumn cHeaderText in this.dgvPatients.Columns)
                columnToolStripComboBox.Items.Add(cHeaderText.HeaderText);

            dgvPatients.ClearSelection();
        }

        private void searchToolStripButton_Click(object sender, EventArgs e)
        {
            if (columnToolStripComboBox.Text != string.Empty)
            {
                for (int rows = 0; rows < this.dgvPatients.RowCount; rows++)
                    if (searchToolStripTextBox.Text.ToLower() == dgvPatients[columnToolStripComboBox.SelectedIndex, rows].Value.ToString().ToLower())
                        dgvPatients.Rows[rows].Selected = true;
            }
        }

        private void searchToolStripTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            dgvPatients.ClearSelection();
            searchToolStripButton_Click(sender, e);
        }
    }
}
