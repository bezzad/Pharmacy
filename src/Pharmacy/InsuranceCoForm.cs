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
    public partial class InsuranceCoForm : Form
    {
        public InsuranceCoForm()
        {
            InitializeComponent();
        }

        private void InsuranceCoForm_Load(object sender, EventArgs e)
        {
            var DB = new LINQ_PharmacyDataContext();
            this.dgvInsuranceCo.DataSource = DB.Health_care_insurance_Cos;
            this.dgvInsuranceCo.Columns[0].HeaderText = "کد بیمه";
            this.dgvInsuranceCo.Columns[1].HeaderText = "نام سازمان";
            this.dgvInsuranceCo.Columns[2].HeaderText = "نوع بیمه";
            this.dgvInsuranceCo.Columns[3].HeaderText = "درصد بیمه";
            this.dgvInsuranceCo.Columns[4].HeaderText = "دورنگار";
            this.dgvInsuranceCo.Columns[5].HeaderText = "تلفن اول";
            this.dgvInsuranceCo.Columns[6].HeaderText = "تلفن دوم";
            this.dgvInsuranceCo.Columns[7].HeaderText = "ایمیل";
            this.dgvInsuranceCo.Columns[8].HeaderText = "آدرس";
            this.dgvInsuranceCo.Columns[9].HeaderText = "کد پستی";
            this.dgvInsuranceCo.Columns[10].HeaderText = "آدرس سایت اینترنتی";
            //
            // set column comboBox
            //
            columnToolStripComboBox.Items.Clear();
            foreach (DataGridViewColumn cHeaderText in this.dgvInsuranceCo.Columns)
                columnToolStripComboBox.Items.Add(cHeaderText.HeaderText);

            dgvInsuranceCo.ClearSelection();
        }

        private void searchToolStripButton_Click(object sender, EventArgs e)
        {
            if (columnToolStripComboBox.Text != string.Empty)
            {
                for (int rows = 0; rows < this.dgvInsuranceCo.RowCount; rows++)
                    if (searchToolStripTextBox.Text.ToLower() == dgvInsuranceCo[columnToolStripComboBox.SelectedIndex, rows].Value.ToString().ToLower())
                        dgvInsuranceCo.Rows[rows].Selected = true;
            }
        }

        private void searchToolStripTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            dgvInsuranceCo.ClearSelection();
            searchToolStripButton_Click(sender, e);
        }
    }
}
