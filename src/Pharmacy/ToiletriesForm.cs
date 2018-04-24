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
    public partial class ToiletriesForm : Form
    {
        /// <summary>
        /// Set the Form mode and datagridview mode to checked moding;
        /// default value is false.
        /// </summary>
        public bool SelectedMode = false;

        /// <summary>
        /// List of Toiletries ID List and Farsi Name
        /// </summary>
        public Dictionary<int, string> dicSelectedToiletriesID = new Dictionary<int, string>();


        public ToiletriesForm()
        {
            InitializeComponent();
        }

        private void ToiletriesForm_Load(object sender, EventArgs e)
        {
            var DB = new LINQ_PharmacyDataContext();
            this.dgvToiletries.DataSource = DB.Toiletries;
            this.dgvToiletries.Columns[0].HeaderText = "کد لوازم بهداشتی";
            this.dgvToiletries.Columns[1].HeaderText = "نام اختصاری";
            this.dgvToiletries.Columns[2].HeaderText = "زیر گروه";
            this.dgvToiletries.Columns[3].HeaderText = "نام انگلیسی";
            this.dgvToiletries.Columns[4].HeaderText = "نام فارسی";
            this.dgvToiletries.Columns[5].HeaderText = "هشدار";
            this.dgvToiletries.Columns[6].HeaderText = "قیمت خرید";
            this.dgvToiletries.Columns[7].HeaderText = "قیمت فروش";
            this.dgvToiletries.Columns[8].HeaderText = "شرکت سازنده";
            this.dgvToiletries.Columns[9].HeaderText = "واحد شمارش";
            this.dgvToiletries.Columns[10].HeaderText = "موجودی قفسه";
            this.dgvToiletries.Columns[11].HeaderText = "موجودی انبار";
            this.dgvToiletries.Columns[12].HeaderText = "نوع مصرف";
            //
            // set column comboBox
            //
            columnToolStripComboBox.Items.Clear();
            foreach (DataGridViewColumn cHeaderText in this.dgvToiletries.Columns)
                columnToolStripComboBox.Items.Add(cHeaderText.HeaderText);

            dgvToiletries.ClearSelection();
            //
            //
            if (SelectedMode)
            {
                DataGridViewCheckBoxColumn colSelectToiletries = new DataGridViewCheckBoxColumn();
                colSelectToiletries.Name = "colSelectToiletries";
                colSelectToiletries.HeaderText = "انتخاب لوازم بهداشتی";
                colSelectToiletries.FillWeight = 120;
                dgvToiletries.Columns.Add(colSelectToiletries);
                dgvToiletries.CellClick += new DataGridViewCellEventHandler(dgvToiletries_CellClick);
                //
                // set Selected Data
                //
                if (dicSelectedToiletriesID.Count > 0) // if exist any selected Toiletries in list
                {
                    for (int row = 0; row < dgvToiletries.RowCount; row++) // search all row in list
                    {
                        int id = int.Parse(dgvToiletries["Toiletries_ID", row].Value.ToString()); // find row id
                        if (dicSelectedToiletriesID.ContainsKey(id)) // search this id in list
                        {
                            dgvToiletries["colSelectToiletries", row].Value = true; // checked finded row because it is exist in list
                        }
                        else dgvToiletries["colSelectToiletries", row].Value = false; // unchecked finded row because it isn't exist in list

                    }
                }
            }
        }

        void dgvToiletries_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvToiletries.ColumnCount - 1 && e.RowIndex >= 0 && e.RowIndex < dgvToiletries.RowCount)
            {
                dgvToiletries[e.ColumnIndex, e.RowIndex].Value = 
                    !((bool)((dgvToiletries[e.ColumnIndex, e.RowIndex].Value == null) ?
                              false : dgvToiletries[e.ColumnIndex, e.RowIndex].Value));
            }
        }

        private void searchToolStripButton_Click(object sender, EventArgs e)
        {
            if (columnToolStripComboBox.Text != string.Empty)
            {
                for (int rows = 0; rows < this.dgvToiletries.RowCount; rows++)
                    if (searchToolStripTextBox.Text.ToLower() == dgvToiletries[columnToolStripComboBox.SelectedIndex, rows].Value.ToString().ToLower()) 
                        dgvToiletries.Rows[rows].Selected = true;
            }
        }

        private void searchToolStripTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            dgvToiletries.ClearSelection();
            searchToolStripButton_Click(sender, e);
        }

        private void ToiletriesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SelectedMode)
            {
                dicSelectedToiletriesID.Clear();
                foreach (DataGridViewRow dgvRow in dgvToiletries.Rows)
                {
                    if (dgvRow.Cells["colSelectToiletries"].Value != null)
                        if ((bool)dgvRow.Cells["colSelectToiletries"].Value == true)
                        {
                            dicSelectedToiletriesID.Add((int)dgvRow.Cells["Toiletries_ID"].Value, (string)dgvRow.Cells["Name_FA"].Value);
                        }
                }
            }
        }
    }
}
