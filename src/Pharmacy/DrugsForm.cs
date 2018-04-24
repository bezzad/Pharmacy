using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using B.B.Components;

namespace Pharmacy
{
    public partial class DrugsForm : Form
    {
        /// <summary>
        /// Set the Form mode and datagridview mode to checked moding;
        /// default value is false.
        /// </summary>
        public bool SelectedMode = false;

        /// <summary>
        /// List of Drug ID List and Generic Farsi Name
        /// </summary>
        public Dictionary<int, string> dicSelectedDrugID = new Dictionary<int, string>();

        public DrugsForm()
        {
            InitializeComponent();
        }

        private void DrugsForm_Load(object sender, EventArgs e)
        {
            var DB = new LINQ_PharmacyDataContext();
            this.dgvDrugs.DataSource = DB.Drugs;
            this.dgvDrugs.Columns[0].HeaderText = "کد دارو";
            this.dgvDrugs.Columns[1].HeaderText = "نام اختصاری";
            this.dgvDrugs.Columns[2].HeaderText = "زیر گروه";
            this.dgvDrugs.Columns[3].HeaderText = "نام ژنریک انگلیسی";
            this.dgvDrugs.Columns[4].HeaderText = "نام ژنریک فارسی";
            this.dgvDrugs.Columns[5].HeaderText = "نام تجاری انگلیسی";
            this.dgvDrugs.Columns[6].HeaderText = "نام تجاری فارسی";
            this.dgvDrugs.Columns[7].HeaderText = "هشدار";
            this.dgvDrugs.Columns[8].HeaderText = "دوزاژ فارسی";
            this.dgvDrugs.Columns[9].HeaderText = "شکل دارویی";
            this.dgvDrugs.Columns[10].HeaderText = "قیمت خرید";
            this.dgvDrugs.Columns[11].HeaderText = "قیمت فروش";
            this.dgvDrugs.Columns[12].HeaderText = "شرکت سازنده";
            this.dgvDrugs.Columns[13].HeaderText = "واحد شمارش";
            this.dgvDrugs.Columns[14].HeaderText = "موجودی قفسه";
            this.dgvDrugs.Columns[15].HeaderText = "موجودی انبار";
            this.dgvDrugs.Columns[16].HeaderText = "شامل بیمه خدمات درمانی بودن";
            this.dgvDrugs.Columns[17].HeaderText = "لیست کد داروهای متضاد";
            this.dgvDrugs.Columns[18].HeaderText = "لیست کد داروهای مشابه";
            //
            // set column comboBox
            //
            columnToolStripComboBox.Items.Clear();
            foreach (DataGridViewColumn cHeaderText in this.dgvDrugs.Columns)
                columnToolStripComboBox.Items.Add(cHeaderText.HeaderText);

            dgvDrugs.ClearSelection();
            //
            //
            if (SelectedMode)
            {
                DataGridViewCheckBoxColumn colSelectDrug = new DataGridViewCheckBoxColumn();
                colSelectDrug.Name = "colSelectDrug";
                colSelectDrug.HeaderText = "انتخاب دارو";
                colSelectDrug.FillWeight = 80;
                dgvDrugs.Columns.Add(colSelectDrug);
                dgvDrugs.CellClick += new DataGridViewCellEventHandler(dgvDrugs_CellClick);
                //
                // set Selected Data
                //
                if (dicSelectedDrugID.Count > 0) // if exist any selected drug in list
                {
                    for (int row = 0; row < dgvDrugs.RowCount; row++) // search all row in list
                    {
                        int id = int.Parse(dgvDrugs["Drug_ID", row].Value.ToString()); // find row id
                        if (dicSelectedDrugID.ContainsKey(id)) // search this id in list
                        {
                            dgvDrugs["colSelectDrug", row].Value = true; // checked finded row because it is exist in list
                        }
                        else dgvDrugs["colSelectDrug", row].Value = false; // unchecked finded row because it isn't exist in list

                    }
                }
            }
        }

        void dgvDrugs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvDrugs.ColumnCount - 1 && e.RowIndex >= 0 && e.RowIndex < dgvDrugs.RowCount)
            {
                dgvDrugs[e.ColumnIndex, e.RowIndex].Value = !((bool)((dgvDrugs[e.ColumnIndex, e.RowIndex].Value == null) ? 
                                                                      false : dgvDrugs[e.ColumnIndex, e.RowIndex].Value));
            }
        }

        private void searchToolStripButton_Click(object sender, EventArgs e)
        {
            if (columnToolStripComboBox.Text != string.Empty)
            {
                for (int rows = 0; rows < this.dgvDrugs.RowCount; rows++)
                    if (searchToolStripTextBox.Text.ToLower() == dgvDrugs[columnToolStripComboBox.SelectedIndex, rows].Value.ToString().ToLower()) 
                        dgvDrugs.Rows[rows].Selected = true;
            }
        }

        private void searchToolStripTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            dgvDrugs.ClearSelection();
            searchToolStripButton_Click(sender, e);
        }

        private void DrugsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(SelectedMode)
            {
                dicSelectedDrugID.Clear();
                foreach (DataGridViewRow dgvRow in dgvDrugs.Rows)
                {
                    if (dgvRow.Cells["colSelectDrug"].Value != null)
                        if ((bool)dgvRow.Cells["colSelectDrug"].Value == true)
                        {
                            dicSelectedDrugID.Add((int)dgvRow.Cells["Drug_ID"].Value, (string)dgvRow.Cells["Generic_Name_FA"].Value);
                        }
                }
            }
        }
    }
}
