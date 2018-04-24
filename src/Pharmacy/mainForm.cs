using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;

namespace Pharmacy
{
    public partial class mainForm : Form
    {
        LINQ_PharmacyDataContext DB;

        public mainForm()
        {
            Thread.CurrentThread.Name = "Main";
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
            this.Visible = false;

            InitializeComponent();

            SplashScreenForm SSF = new SplashScreenForm();
            SSF.Show();
            //
            // check and connect to UserPass and Pharmacy database.
            //
            UserPasswordForm upForm = new UserPasswordForm();
            if (new LINQ_UserPassDataContext().User_Passwords.ToArray().Length > 1)
            {
                System.Threading.Thread.Sleep(1000);
                SSF.Dispose();
                if (upForm.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                }
            }
            else upForm.Dispose();
            //
            // connect to Database by LINQ
            //
            DB = new LINQ_PharmacyDataContext();
            //
            //
            if (SSF != null)
            {
                System.Threading.Thread.Sleep(1000);
                SSF.Dispose();
            }
            this.Visible = true;
            //
            // Set Access Limit
            //
            setUserModifier(UserAccountsForm.Entry_Modifiers);
        }

        private void setUserModifier(string m)
        {
            switch (m)
            {
                case "Administrator": { /*Full Controls*/ }
                    break;
                case "Finance User":
                    {
                        userAccountsToolStripButton.Enabled = false;
                        openToolStripButton.Enabled = false;
                        exportToolStripButton.Enabled = false;
                        newToolStripButton.Enabled = false;
                        //
                        tPageDrugs.Dispose();
                        tPageCosmeticsAndToiletries.Dispose();
                        tPageInsuranceCO.Dispose();
                    }
                    break;
                case "Preparation User":
                    {
                        userAccountsToolStripButton.Enabled = false;
                        openToolStripButton.Enabled = false;
                        exportToolStripButton.Enabled = false;
                        newToolStripButton.Enabled = false;
                        //
                        tPageSales.Dispose();
                    }
                    break;
                case "Receptionist User":
                    {
                        userAccountsToolStripButton.Enabled = false;
                        openToolStripButton.Enabled = false;
                        exportToolStripButton.Enabled = false;
                        newToolStripButton.Enabled = false;
                        //
                        tPageDrugs.Dispose();
                        tPageCosmeticsAndToiletries.Dispose();
                        tPageInsuranceCO.Dispose();
                    }
                    break;
                default: break;
            }
        }

        #region toolStrip codes
        private void userAccountsToolStripButton_Click(object sender, EventArgs e)
        {
            UserAccountsForm uaForm = new UserAccountsForm();
            uaForm.Show();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private string FileName = string.Empty;
        Thread thImport;
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofdOpenAll = new OpenFileDialog();
            ofdOpenAll.Title = "Open Database file *.mdf";
            ofdOpenAll.Filter = @"Database files|*.mdf";
            ofdOpenAll.AutoUpgradeEnabled = true;
            ofdOpenAll.CheckFileExists = true;
            ofdOpenAll.CheckPathExists = true;

            if (ofdOpenAll.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileName = ofdOpenAll.FileName;
                    thImport = new Thread(new ThreadStart(ImportMDF));
                    thImport.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "اشکال در بازیابی اطلاعات");
                }
            }
            ofdOpenAll.Dispose();
        }
        private void ImportMDF()
        {
            try
            {
                Set_Cursor_Value("WaitCursor");
                var newDB = new LINQ_PharmacyDataContext(FileName);

                string query = @"DELETE FROM dbo.Toiletries " + @"DELETE FROM dbo.Patient_toiletries_copy "
                       + @"DELETE FROM dbo.Patient_drug_copy " + @"DELETE FROM dbo.Patient "
                       + @"DELETE FROM dbo.Health_care_insurance_Co " + @"DELETE FROM dbo.Drug";
                DB.ExecuteQuery<object>(query);

                #region 1.  Drugs DATA
                //
                // Delete old Drug data and Fill it by newDB data
                //
                var aryDrug = newDB.Drugs.ToArray();
                if (aryDrug.Length > 0)
                {
                    //
                    // read and save new data
                    //
                    foreach (var row in aryDrug)
                    {
                        Drug newDrug = new Drug()
                        {
                            Drug_ID = row.Drug_ID,
                            Abbreviation_Name = row.Abbreviation_Name,
                            Subgroups_Name = row.Subgroups_Name,
                            Generic_Name_EN = row.Generic_Name_EN,
                            Generic_Name_FA = row.Generic_Name_FA,
                            Brand_Name_EN = row.Brand_Name_EN,
                            Brand_Name_FA = row.Brand_Name_FA,
                            Warning = row.Warning,
                            Dosage_FA = row.Dosage_FA,
                            Pharmaceutical_Form = row.Pharmaceutical_Form,
                            Purchase_Price = row.Purchase_Price,
                            Sale_Price = row.Sale_Price,
                            Manufacturer = row.Manufacturer,
                            Counting_Unit = row.Counting_Unit,
                            Count_on_the_Shelf = row.Count_on_the_Shelf,
                            Count_in_the_Stock = row.Count_in_the_Stock,
                            Including_insurance = row.Including_insurance,
                            Contrasting_drugs_ID_list = row.Contrasting_drugs_ID_list,
                            Similar_drugs_ID_list = row.Similar_drugs_ID_list
                        };
                        DB.Drugs.InsertOnSubmit(newDrug);
                        DB.SubmitChanges();
                    }
                }
                #endregion

                #region 2.  Toiletries DATA
                //
                // Delete old Toiletries data and Fill it by newDB data
                //
                var aryToiletry = newDB.Toiletries.ToArray();
                if (aryToiletry.Length > 0)
                {
                    //
                    // read and save new data
                    //
                    foreach (var row in aryToiletry)
                    {
                        Toiletry newToiletry = new Toiletry()
                        {
                            Toiletries_ID = row.Toiletries_ID,
                            Abbreviation_Name = row.Abbreviation_Name,
                            Subgroups_Name = row.Subgroups_Name,
                            Name_EN = row.Name_EN,
                            Name_FA = row.Name_FA,
                            Warning = row.Warning,
                            Purchase_Price = row.Purchase_Price,
                            Sale_Price = row.Sale_Price,
                            Manufacturer = row.Manufacturer,
                            Counting_Unit = row.Counting_Unit,
                            Count_on_the_Shelf = row.Count_on_the_Shelf,
                            Count_in_the_Stock = row.Count_in_the_Stock,
                            Consumption_Type = row.Consumption_Type
                        };
                        DB.Toiletries.InsertOnSubmit(newToiletry);
                        DB.SubmitChanges();
                    }
                }
                #endregion

                #region 3.  Health care insurance Co. DATA
                //
                // Delete old InsuranceCo. data and Fill it by newDB data
                //
                var aryInsurance = newDB.Health_care_insurance_Cos.ToArray();
                if (aryInsurance.Length > 0)
                {
                    //
                    // read and save new data
                    //
                    foreach (var row in aryInsurance)
                    {
                        Health_care_insurance_Co newInsuranceCo = new Health_care_insurance_Co()
                        {
                            Insurance_ID = row.Insurance_ID,
                            Organization_Name = row.Organization_Name,
                            Insurance_Type  = row.Insurance_Type,
                            Percent_Reduction = row.Percent_Reduction,
                            Fax = row.Fax,
                            Telephone1 = row.Telephone1,
                            Telephone2 = row.Telephone2,
                            Address = row.Address,
                            ZipCode = row.ZipCode,
                            Email = row.Email,
                            Site = row.Site
                        };
                        DB.Health_care_insurance_Cos.InsertOnSubmit(newInsuranceCo);
                        DB.SubmitChanges();
                    }
                }
                #endregion

                #region 4.  Patient DATA
                //
                // Delete old Patient data and Fill it by newDB data
                //
                var aryPatient = newDB.Patients.ToArray();
                if (aryPatient.Length > 0)
                {
                    //
                    // read and save new data
                    //
                    foreach (var row in aryPatient)
                    {
                        Patient newPatient = new Patient()
                        {
                            Patient_ID = row.Patient_ID,
                            Insurance_ID = row.Insurance_ID,
                            Health_Insurance_Code = row.Health_Insurance_Code,
                            Patient_Name = row.Patient_Name,
                            Date_of_copy = row.Date_of_copy,
                            Copy_Page_Number = row.Copy_Page_Number,
                            Doctor_Name = row.Doctor_Name,
                            Medical_Council_No = row.Medical_Council_No,
                            Total_Sale_Price = row.Total_Sale_Price,
                            Total_Purchase_Price = row.Total_Purchase_Price,
                            Portion_insured = row.Portion_insured,
                            Organizations_Portion = row.Organizations_Portion
                        };
                        DB.Patients.InsertOnSubmit(newPatient);
                        DB.SubmitChanges();
                    }
                }
                #endregion

                #region 5.  Patient Drug Copy DATA
                //
                // Delete old Patient Drug Copy data and Fill it by newDB data
                //
                var aryPatientDrugCopy = newDB.Patient_drug_copies.ToArray();
                if (aryPatientDrugCopy.Length > 0)
                {
                    //
                    // read and save new data
                    //
                    foreach (var row in aryPatientDrugCopy)
                    {
                        Patient_drug_copy pdc = new Patient_drug_copy()
                        {
                            Patient_ID = row.Patient_ID,
                            Drug_ID = row.Drug_ID,
                            Total_Price = row.Total_Price,
                            Number_Drug = row.Number_Drug                            
                        };
                        DB.Patient_drug_copies.InsertOnSubmit(pdc);
                        DB.SubmitChanges();
                    }
                }
                #endregion

                #region 6.  Patient Toiletries Copy DATA
                //
                // Delete old Patient Toiletries Copy data and Fill it by newDB data
                //
                var aryPatientToiletriesCopy = newDB.Patient_toiletries_copies.ToArray();
                if (aryPatientToiletriesCopy.Length > 0)
                {
                    //
                    // read and save new data
                    //
                    foreach (var row in aryPatientToiletriesCopy)
                    {
                        Patient_toiletries_copy ptc = new Patient_toiletries_copy()
                        {
                            Patient_ID = row.Patient_ID,
                            Toiletries_ID = row.Toiletries_ID,
                            Total_Price = row.Total_Price,
                            Number_Toiletries = row.Number_Toiletries
                        };
                        DB.Patient_toiletries_copies.InsertOnSubmit(ptc);
                        DB.SubmitChanges();
                    }
                }
                #endregion

                newDB.Connection.Close();
                newDB.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "اشکال در بازیابی اطلاعات");
                thImport.Abort();
                return;
            }
            finally
            {
                Thread.CurrentThread.Join(1000);
                Set_Cursor_Value("Default");
                Thread.CurrentThread.Abort();
            }
        }
        // ------------------------------------------------------------
        // This delegate enables asynchronous calls for setting
        // the Value property on a Mouse Cursor control.
        delegate void SetCursorValueCallback(string v);
        private void Set_Cursor_Value(string v)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            try
            {
                if (this.InvokeRequired)
                {
                    SetCursorValueCallback d = new SetCursorValueCallback(Set_Cursor_Value);
                    this.Invoke(d, new object[] { v });
                }
                else
                {
                    switch (v)
                    {
                        case "WaitCursor": this.Cursor = Cursors.WaitCursor;
                            break;
                        case "Default": this.Cursor = Cursors.Default;
                            break;
                        default: this.Cursor = Cursors.Default;
                            break;
                    }
                }
            }
            catch { }
        }

        private void exportToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfdExportAll = new SaveFileDialog();
            try
            {
                sfdExportAll.Title = "Export All Database file *.mdf";
                sfdExportAll.Filter = @"Database files|*.mdf";
                sfdExportAll.DefaultExt = "Backup.mdf";
                sfdExportAll.FileName = "Backup.mdf";
                sfdExportAll.AutoUpgradeEnabled = true;

                if (sfdExportAll.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        System.IO.File.WriteAllText("Copy.Log", sfdExportAll.FileName);
                        System.Diagnostics.Process.Start("DisconnectDB.exe");
                        Application.Exit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "اشکال در استخراج اطلاعات");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source); return; }
            finally
            {
                sfdExportAll.Dispose();
            }
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }
        #endregion

        #region Drugs
        private void btnShowAllDrugs_Click(object sender, EventArgs e)
        {
            DrugsForm DF = new DrugsForm();
            DF.ShowDialog();
        }

        private void btnSaveDrug_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSale_Price.Text) || txtSale_Price.Text == "قیمت فروش")
            {
                MessageBox.Show("Sale Price TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSale_Price.Select();
                return;
            }
            if (string.IsNullOrEmpty(txtCount_in_the_Stock.Text) || txtCount_in_the_Stock.Text == "موجودی انبار")
            {
                MessageBox.Show("Count in the Stock TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCount_in_the_Stock.Select();
                return;
            }
            if (string.IsNullOrEmpty(txtCount_on_the_Shelf.Text) || txtCount_on_the_Shelf.Text == "موجودی قفسه")
            {
                MessageBox.Show("Count on the Shelf TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCount_on_the_Shelf.Select();
                return;
            }
            string contrastingDrugs = "";
            foreach (DataGridViewRow row in dgvContrasting_drugs.Rows)
            {
                contrastingDrugs += row.Cells["colDrug_ID"].Value.ToString() + " ";
            }
            //
            string similarDrugs = "";
            foreach (DataGridViewRow row in dgvSimilar_drugs.Rows)
            {
                similarDrugs += row.Cells["colDrug_ID_S"].Value.ToString() + " ";
            }
            //
            // Save 
            //
            Drug newDrug = new Drug()
            {
                Abbreviation_Name = (txtAbbreviation_Name.Text != "نام اختصاری") ? txtAbbreviation_Name.Text : string.Empty,
                Subgroups_Name = (txtSubgroups_Name.Text != "زیر گروه") ? txtSubgroups_Name.Text : string.Empty,
                Generic_Name_EN = (txtGeneric_Name_EN.Text != "نام ژنریک انگلیسی") ? txtGeneric_Name_EN.Text : string.Empty,
                Generic_Name_FA = (txtGeneric_Name_FA.Text != "نام ژنریک فارسی") ? txtGeneric_Name_FA.Text : string.Empty,
                Brand_Name_EN = (txtBrand_Name_EN.Text != "نام تجاری انگلیسی") ? txtBrand_Name_EN.Text : string.Empty,
                Brand_Name_FA = (txtBrand_Name_FA.Text != "نام تجاری فارسی") ? txtBrand_Name_FA.Text : string.Empty,
                Warning = (txtWarning.Text != "هشدار") ? txtWarning.Text : string.Empty,
                Dosage_FA = (txtDosage_FA.Text != "دوزاژ فارسی") ? txtDosage_FA.Text : string.Empty,
                Pharmaceutical_Form = (txtPharmaceutical_Form.Text != "شکل دارویی") ? txtPharmaceutical_Form.Text : string.Empty,
                Purchase_Price = (txtPurchase_Price.Text != "قیمت خرید") ?
                decimal.Parse(txtPurchase_Price.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture) :
                decimal.Parse("0", NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture),
                Sale_Price = decimal.Parse(txtSale_Price.Text, NumberStyles.Currency | NumberStyles.Float, txtSale_Price.Culture),
                Manufacturer = (txtManufacturer.Text != "شرکت تولید کننده") ? txtManufacturer.Text : string.Empty,
                Counting_Unit = (txtCounting_Unit.Text != "واحد شمارش") ? int.Parse(txtCounting_Unit.Text) : 1,
                Count_in_the_Stock = int.Parse(txtCount_in_the_Stock.Text),
                Count_on_the_Shelf = int.Parse(txtCount_on_the_Shelf.Text),
                Including_insurance = chkIncluding_Insurance.Checked,
                Contrasting_drugs_ID_list = contrastingDrugs,
                Similar_drugs_ID_list = similarDrugs
            };

            DB.Drugs.InsertOnSubmit(newDrug);
            DB.SubmitChanges();

            MessageBox.Show("Drug information you succeed in pharmacy database was recorded.", "New Records", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnCancelDrug_Click(sender, e);
        }

        private void btnCancelDrug_Click(object sender, EventArgs e)
        {
            clear_DrugTextBoxs();
            txtAbbreviation_Name.Focus();
            if (editState_D)
            {
                btnSaveDrug.Enabled = true;
                btnShowAllDrugs.Enabled = true;
                txtDrug_ID.Visible = false;
                btnEditDrug.Text = "ویرایش داروی ثبت شده";
                editState_D = false;
                txtDrug_ID.clearByLabelText();
            }
        }

        private void clear_DrugTextBoxs()
        {
            txtAbbreviation_Name.clearByLabelText();
            txtSubgroups_Name.clearByLabelText();
            txtGeneric_Name_EN.clearByLabelText();
            txtGeneric_Name_FA.clearByLabelText();
            txtBrand_Name_EN.clearByLabelText();
            txtBrand_Name_FA.clearByLabelText();
            txtWarning.clearByLabelText();
            txtDosage_FA.clearByLabelText();
            txtPharmaceutical_Form.clearByLabelText();
            txtPurchase_Price.clearByLabelText();
            txtSale_Price.clearByLabelText();
            txtManufacturer.clearByLabelText();
            txtCounting_Unit.clearByLabelText();
            txtCount_in_the_Stock.clearByLabelText();
            txtCount_on_the_Shelf.clearByLabelText();
            chkIncluding_Insurance.Checked = false;
            dgvContrasting_drugs.Rows.Clear();
            dgvSimilar_drugs.Rows.Clear();
        }

        private void tPageDrugs_Enter(object sender, EventArgs e)
        {
            txtAbbreviation_Name.Focus();
        }

        bool editState_D = false;
        bool findedTrue_D = false;
        private void btnEditDrug_Click(object sender, EventArgs e)
        {
            if (!editState_D)
            {
                btnSaveDrug.Enabled = false;
                btnShowAllDrugs.Enabled = false;
                txtDrug_ID.Visible = true;
                btnEditDrug.Text = ". . . ثبت تغییرات";
                editState_D = true;
            }
            else
            {
                if (edit_DrugData())
                {
                    btnSaveDrug.Enabled = true;
                    btnShowAllDrugs.Enabled = true;
                    txtDrug_ID.Visible = false;
                    btnEditDrug.Text = "ویرایش داروی ثبت شده";
                    clear_DrugTextBoxs();
                    txtDrug_ID.clearByLabelText();
                    editState_D = false;
                }
                else btnCancelDrug_Click(sender, e);
            }
        }

        private bool edit_DrugData()
        {
            if (findedTrue_D)
            {
                int drugCode = int.Parse(txtDrug_ID.Text);
                if (string.IsNullOrEmpty(txtSale_Price.Text) || txtSale_Price.Text == "قیمت فروش")
                {
                    MessageBox.Show("Sale Price TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSale_Price.Select();
                    return false;
                }
                if (string.IsNullOrEmpty(txtCount_in_the_Stock.Text) || txtCount_in_the_Stock.Text == "موجودی انبار")
                {
                    MessageBox.Show("Count in the Stock TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCount_in_the_Stock.Select();
                    return false;
                }
                if (string.IsNullOrEmpty(txtCount_on_the_Shelf.Text) || txtCount_on_the_Shelf.Text == "موجودی قفسه")
                {
                    MessageBox.Show("Count on the Shelf TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCount_on_the_Shelf.Select();
                    return false;
                }
                string contrastingDrugs = "";
                foreach (DataGridViewRow row in dgvContrasting_drugs.Rows)
                {
                    contrastingDrugs += row.Cells["colDrug_ID"].Value.ToString() + " ";
                }
                //
                string similarDrugs = "";
                foreach (DataGridViewRow row in dgvSimilar_drugs.Rows)
                {
                    similarDrugs += row.Cells["colDrug_ID_S"].Value.ToString() + " ";
                }
                //
                // Edit
                //
                DB.DrugEdit(drugCode,
                    (txtAbbreviation_Name.Text != txtAbbreviation_Name.LabelText) ? txtAbbreviation_Name.Text : string.Empty,
                    (txtSubgroups_Name.Text != txtSubgroups_Name.LabelText) ? txtSubgroups_Name.Text : string.Empty,
                    (txtGeneric_Name_EN.Text != txtGeneric_Name_EN.LabelText) ? txtGeneric_Name_EN.Text : string.Empty,
                    (txtGeneric_Name_FA.Text != txtGeneric_Name_FA.LabelText) ? txtGeneric_Name_FA.Text : string.Empty,
                    (txtBrand_Name_EN.Text != txtBrand_Name_EN.LabelText) ? txtBrand_Name_EN.Text : string.Empty,
                    (txtBrand_Name_FA.Text != txtBrand_Name_FA.LabelText) ? txtBrand_Name_FA.Text : string.Empty,
                    (txtWarning.Text != txtWarning.LabelText) ? txtWarning.Text : string.Empty,
                    (txtDosage_FA.Text != txtDosage_FA.LabelText) ? txtDosage_FA.Text : string.Empty,
                    (txtPharmaceutical_Form.Text != txtPharmaceutical_Form.LabelText) ? txtPharmaceutical_Form.Text : string.Empty,
                    (txtPurchase_Price.Text != txtPurchase_Price.LabelText) ? decimal.Parse(txtPurchase_Price.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture) : 0,
                    decimal.Parse(txtSale_Price.Text, NumberStyles.Currency | NumberStyles.Float, txtSale_Price.Culture),
                    (txtManufacturer.Text != txtManufacturer.LabelText) ? txtManufacturer.Text : string.Empty,
                    (txtCounting_Unit.Text != txtCounting_Unit.LabelText) ? int.Parse(txtCounting_Unit.Text) : 1,
                    int.Parse(txtCount_on_the_Shelf.Text),
                    int.Parse(txtCount_in_the_Stock.Text),
                    chkIncluding_Insurance.Checked,
                    contrastingDrugs,
                    similarDrugs);

                DB.SubmitChanges();
                DB = new LINQ_PharmacyDataContext();
                MessageBox.Show("Drug information you succeed in pharmacy database was edited.", "Edit Records", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            return false;
        }

        private void btnAddContrasting_drugs_Click(object sender, EventArgs e)
        {
            DrugsForm DF = new DrugsForm();
            DF.SelectedMode = true;
            //
            if (((Button)sender).Name == "btnAddContrasting_drugs")
            {
                Dictionary<int, string> dicDrugs = new Dictionary<int, string>();
                foreach (DataGridViewRow row in dgvContrasting_drugs.Rows)
                {
                    dicDrugs.Add(int.Parse(row.Cells["colDrug_ID"].Value.ToString()), "");
                }
                DF.dicSelectedDrugID = dicDrugs;
                DF.ShowDialog();
                dgvContrasting_drugs.Rows.Clear();
                foreach (var d in DF.dicSelectedDrugID)
                {
                    dgvContrasting_drugs.Rows.Add(1);
                    dgvContrasting_drugs.Rows[dgvContrasting_drugs.RowCount - 1].Cells["colDrug_ID"].Value = d.Key;
                    dgvContrasting_drugs.Rows[dgvContrasting_drugs.RowCount - 1].Cells["colGeneric_Name_FA"].Value = d.Value.ToString();
                }
            }
            else if (((Button)sender).Name == "btnAddSimilar_drugs")
            {
                Dictionary<int, string> dicDrugs = new Dictionary<int, string>();
                foreach (DataGridViewRow row in dgvSimilar_drugs.Rows)
                {
                    dicDrugs.Add(int.Parse(row.Cells["colDrug_ID_S"].Value.ToString()), "");
                }
                DF.dicSelectedDrugID = dicDrugs;
                DF.ShowDialog();
                dgvSimilar_drugs.Rows.Clear();
                foreach (var d in DF.dicSelectedDrugID)
                {
                    dgvSimilar_drugs.Rows.Add(1);
                    dgvSimilar_drugs.Rows[dgvSimilar_drugs.RowCount - 1].Cells["colDrug_ID_S"].Value = d.Key;
                    dgvSimilar_drugs.Rows[dgvSimilar_drugs.RowCount - 1].Cells["colGeneric_Name_FA_S"].Value = d.Value.ToString();
                }
            }
        }

        private void txtDrug_ID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Tab)
            {
                findedTrue_D = false;
                int Id;
                if (txtDrug_ID.Text != txtDrug_ID.LabelText && txtDrug_ID.Text != string.Empty
                    && int.TryParse(txtDrug_ID.Text, out Id))
                {
                    var drug = (from db in DB.Drugs
                                where db.Drug_ID == Id
                                select db).ToArray();
                    if (drug.Length > 0)
                    {
                        txtAbbreviation_Name.Text = drug[0].Abbreviation_Name;

                        txtSubgroups_Name.Text = drug[0].Subgroups_Name;

                        txtGeneric_Name_EN.Text = drug[0].Generic_Name_EN;

                        txtGeneric_Name_FA.Text = drug[0].Generic_Name_FA;

                        txtBrand_Name_EN.Text = drug[0].Brand_Name_EN;

                        txtBrand_Name_FA.Text = drug[0].Brand_Name_FA;

                        txtWarning.Text = drug[0].Warning;

                        txtDosage_FA.Text = drug[0].Dosage_FA;

                        txtPharmaceutical_Form.Text = drug[0].Pharmaceutical_Form;

                        txtPurchase_Price.Text = drug[0].Purchase_Price.Value.ToString();

                        txtSale_Price.Text = drug[0].Sale_Price.ToString();

                        txtManufacturer.Text = drug[0].Manufacturer;

                        txtCounting_Unit.Text = drug[0].Counting_Unit.Value.ToString();

                        txtCount_in_the_Stock.Text = drug[0].Count_in_the_Stock.ToString();

                        txtCount_on_the_Shelf.Text = drug[0].Count_on_the_Shelf.ToString();

                        chkIncluding_Insurance.Checked = drug[0].Including_insurance;
                        //
                        // Fill any DataGridView
                        //
                        // Contrasting Drugs
                        string[] allCID = drug[0].Contrasting_drugs_ID_list.Split(" ".ToCharArray());
                        dgvContrasting_drugs.Rows.Clear();
                        foreach (string anyID in allCID)
                        {
                            int id;
                            if (int.TryParse(anyID, out id))
                            {
                                try
                                {
                                    string drugName = (from d in DB.Drugs
                                                       where id == d.Drug_ID
                                                       select d.Generic_Name_FA).Single();
                                    //
                                    // set data in DataGridView
                                    dgvContrasting_drugs.Rows.Add(1);
                                    dgvContrasting_drugs.Rows[dgvContrasting_drugs.RowCount - 1].Cells["colDrug_ID"].Value = id;
                                    dgvContrasting_drugs.Rows[dgvContrasting_drugs.RowCount - 1].Cells["colGeneric_Name_FA"].Value = drugName;
                                }
                                catch { continue; }
                            }
                            else continue;
                        }
                        //
                        // Similar Drugs
                        string[] allSID = drug[0].Similar_drugs_ID_list.Split(" ".ToCharArray());
                        dgvSimilar_drugs.Rows.Clear();
                        foreach (string anyID in allSID)
                        {
                            int id;
                            if (int.TryParse(anyID, out id))
                            {
                                try
                                {
                                    string drugName = (from d in DB.Drugs
                                                       where id == d.Drug_ID
                                                       select d.Generic_Name_FA).Single();
                                    //
                                    // set data in DataGridView
                                    dgvSimilar_drugs.Rows.Add(1);
                                    dgvSimilar_drugs.Rows[dgvSimilar_drugs.RowCount - 1].Cells["colDrug_ID_S"].Value = id;
                                    dgvSimilar_drugs.Rows[dgvSimilar_drugs.RowCount - 1].Cells["colGeneric_Name_FA_S"].Value = drugName;
                                }
                                catch { continue; }
                            }
                            else continue;
                        }
                        //
                        findedTrue_D = true;
                    }
                    else clear_DrugTextBoxs();
                }
                else clear_DrugTextBoxs();
            }
        }
        #endregion

        #region Toiletries
        private void btnShowAllToiletries_Click(object sender, EventArgs e)
        {
            ToiletriesForm TF = new ToiletriesForm();
            TF.ShowDialog();
        }

        private void btnSaveToiletries_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSale_Price_T.Text) || txtSale_Price_T.Text == txtSale_Price_T.LabelText)
            {
                MessageBox.Show("Sale Price TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSale_Price_T.Select();
                return;
            }
            if (string.IsNullOrEmpty(txtCount_in_the_Stock_T.Text) || txtCount_in_the_Stock_T.Text == txtCount_in_the_Stock_T.LabelText)
            {
                MessageBox.Show("Count in the Stock TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCount_in_the_Stock_T.Select();
                return;
            }
            if (string.IsNullOrEmpty(txtCount_on_the_Shelf_T.Text) || txtCount_on_the_Shelf_T.Text == txtCount_on_the_Shelf_T.LabelText)
            {
                MessageBox.Show("Count on the Shelf TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCount_on_the_Shelf_T.Select();
                return;
            }
            //
            // Save 
            //
            Toiletry newToiletry = new Toiletry()
            {
                Abbreviation_Name = (txtAbbreviation_Name_T.Text != txtAbbreviation_Name_T.LabelText) ? txtAbbreviation_Name_T.Text : string.Empty,
                Subgroups_Name = (txtSubgroups_Name_T.Text != txtSubgroups_Name_T.LabelText) ? txtSubgroups_Name_T.Text : string.Empty,
                Name_EN = (txtName_EN.Text != txtName_EN.LabelText) ? txtName_EN.Text : string.Empty,
                Name_FA = (txtName_FA.Text != txtName_FA.LabelText) ? txtName_FA.Text : string.Empty,
                Warning = (txtWarning_T.Text != txtWarning_T.LabelText) ? txtWarning_T.Text : string.Empty,
                Purchase_Price = (txtPurchase_Price_T.Text != txtPurchase_Price_T.LabelText) ? decimal.Parse(txtPurchase_Price_T.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price_T.Culture) : 0,
                Sale_Price = decimal.Parse(txtSale_Price_T.Text, NumberStyles.Currency | NumberStyles.Float, txtSale_Price_T.Culture),
                Manufacturer = (txtManufacturer_T.Text != txtManufacturer_T.LabelText) ? txtManufacturer_T.Text : string.Empty,
                Counting_Unit = (txtCounting_Unit_T.Text != txtCounting_Unit_T.LabelText) ? int.Parse(txtCounting_Unit_T.Text) : 1,
                Count_in_the_Stock = int.Parse(txtCount_in_the_Stock_T.Text),
                Count_on_the_Shelf = int.Parse(txtCount_on_the_Shelf_T.Text),
                Consumption_Type = (txtConsumption_Type.Text != txtConsumption_Type.LabelText) ? txtConsumption_Type.Text : string.Empty
            };

            DB.Toiletries.InsertOnSubmit(newToiletry);
            DB.SubmitChanges();

            MessageBox.Show("Toiletries information you succeed in pharmacy database was recorded.", "New Records", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnCancelToiletries_Click(sender, e);
        }

        bool editState_T = false;
        bool findedTrue_T = false;
        private void btnEditToiletries_Click(object sender, EventArgs e)
        {
            if (!editState_T)
            {
                btnSaveToiletries.Enabled = false;
                btnShowAllToiletries.Enabled = false;
                txtToiletries_ID.Visible = true;
                btnEditToiletries.Text = ". . . ثبت تغییرات";
                editState_T = true;
            }
            else
            {
                if (edit_ToiletryData())
                {
                    btnSaveToiletries.Enabled = true;
                    btnShowAllToiletries.Enabled = true;
                    txtToiletries_ID.Visible = false;
                    btnEditToiletries.Text = "ویرایش لوازم بهداشتی ثبت شده";
                    clear_ToiletriesTextBoxs();
                    txtToiletries_ID.clearByLabelText();
                    editState_T = false;
                }
                else btnCancelToiletries_Click(sender, e);
            }
        }

        private bool edit_ToiletryData()
        {
            if (findedTrue_T)
            {
                int toiletriesCode = int.Parse(txtToiletries_ID.Text);
                if (string.IsNullOrEmpty(txtSale_Price_T.Text) || txtSale_Price_T.Text == txtSale_Price_T.LabelText)
                {
                    MessageBox.Show("Sale Price TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSale_Price_T.Select();
                    return false;
                }
                if (string.IsNullOrEmpty(txtCount_in_the_Stock_T.Text) || txtCount_in_the_Stock_T.Text == txtCount_in_the_Stock_T.LabelText)
                {
                    MessageBox.Show("Count in the Stock TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCount_in_the_Stock_T.Select();
                    return false;
                }
                if (string.IsNullOrEmpty(txtCount_on_the_Shelf_T.Text) || txtCount_on_the_Shelf_T.Text == txtCount_on_the_Shelf_T.LabelText)
                {
                    MessageBox.Show("Count on the Shelf TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCount_on_the_Shelf_T.Select();
                    return false;
                }
                //
                // Edit
                //
                DB.ToiletriesEdit(toiletriesCode,
                        (txtAbbreviation_Name_T.Text != txtAbbreviation_Name_T.LabelText) ? txtAbbreviation_Name_T.Text : string.Empty,
                        (txtSubgroups_Name_T.Text != txtSubgroups_Name_T.LabelText) ? txtSubgroups_Name_T.Text : string.Empty,
                        (txtName_EN.Text != txtName_EN.LabelText) ? txtName_EN.Text : string.Empty,
                        (txtName_FA.Text != txtName_FA.LabelText) ? txtName_FA.Text : string.Empty,
                        (txtWarning_T.Text != txtWarning_T.LabelText) ? txtWarning_T.Text : string.Empty,
                        (txtPurchase_Price_T.Text != txtPurchase_Price_T.LabelText) ? decimal.Parse(txtPurchase_Price_T.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price_T.Culture) : 0,
                        decimal.Parse(txtSale_Price_T.Text, NumberStyles.Currency | NumberStyles.Float, txtSale_Price_T.Culture),
                        (txtManufacturer_T.Text != txtManufacturer_T.LabelText) ? txtManufacturer_T.Text : string.Empty,
                        (txtCounting_Unit_T.Text != txtCounting_Unit_T.LabelText) ? int.Parse(txtCounting_Unit_T.Text) : 1,
                        int.Parse(txtCount_in_the_Stock_T.Text),
                        int.Parse(txtCount_on_the_Shelf_T.Text),
                        (txtConsumption_Type.Text != txtConsumption_Type.LabelText) ? txtConsumption_Type.Text : string.Empty);

                DB.SubmitChanges();
                DB = new LINQ_PharmacyDataContext();
                MessageBox.Show("Toiletries information you succeed in pharmacy database was edited.", "Edit Records", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            return false;
        }

        private void btnCancelToiletries_Click(object sender, EventArgs e)
        {
            clear_ToiletriesTextBoxs();
            txtAbbreviation_Name_T.Focus();
            if (editState_T)
            {
                btnSaveToiletries.Enabled = true;
                btnShowAllToiletries.Enabled = true;
                txtToiletries_ID.Visible = false;
                btnEditToiletries.Text = "ویرایش لوازم بهداشتی ثبت شده";
                txtToiletries_ID.clearByLabelText();
                editState_T = false;
            }
        }

        private void clear_ToiletriesTextBoxs()
        {
            txtAbbreviation_Name_T.clearByLabelText();
            txtSubgroups_Name_T.clearByLabelText();
            txtName_EN.clearByLabelText();
            txtName_FA.clearByLabelText();
            txtWarning_T.clearByLabelText();
            txtPurchase_Price_T.clearByLabelText();
            txtSale_Price_T.clearByLabelText();
            txtManufacturer_T.clearByLabelText();
            txtCounting_Unit_T.clearByLabelText();
            txtCount_in_the_Stock_T.clearByLabelText();
            txtCount_on_the_Shelf_T.clearByLabelText();
            txtConsumption_Type.clearByLabelText();
        }

        private void tPageCosmeticsAndToiletries_Enter(object sender, EventArgs e)
        {
            txtAbbreviation_Name_T.Focus();
        }

        private void txtToiletries_ID_KeyUp(object sender, KeyEventArgs e)
        {
            findedTrue_T = false;
            int id;
            if (txtToiletries_ID.Text != txtToiletries_ID.LabelText && txtToiletries_ID.Text != string.Empty
                && int.TryParse(txtToiletries_ID.Text, out id))
            {
                var toiletries = (from db in DB.Toiletries
                                  where db.Toiletries_ID == id
                                  select db).ToArray();
                if (toiletries.Length > 0)
                {
                    txtAbbreviation_Name_T.Text = toiletries[0].Abbreviation_Name;

                    txtSubgroups_Name_T.Text = toiletries[0].Subgroups_Name;

                    txtName_EN.Text = toiletries[0].Name_EN;

                    txtName_FA.Text = toiletries[0].Name_FA;

                    txtWarning_T.Text = toiletries[0].Warning;

                    txtPurchase_Price_T.Text = toiletries[0].Purchase_Price.Value.ToString();

                    txtSale_Price_T.Text = toiletries[0].Sale_Price.ToString();

                    txtManufacturer_T.Text = toiletries[0].Manufacturer;

                    txtCounting_Unit_T.Text = toiletries[0].Counting_Unit.Value.ToString();

                    txtCount_in_the_Stock_T.Text = toiletries[0].Count_in_the_Stock.ToString();

                    txtCount_on_the_Shelf_T.Text = toiletries[0].Count_on_the_Shelf.ToString();

                    txtConsumption_Type.Text = toiletries[0].Consumption_Type;

                    findedTrue_T = true;
                }
                else clear_ToiletriesTextBoxs();
            }
            else clear_ToiletriesTextBoxs();
        }
        #endregion

        #region Insurance Co
        private void btnShowAllInsurance_Click(object sender, EventArgs e)
        {
            InsuranceCoForm ICF = new InsuranceCoForm();
            ICF.ShowDialog();
        }

        private void btnSaveInsurance_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPercent_Reduction.Text) || txtPercent_Reduction.Text == txtPercent_Reduction.LabelText)
            {
                MessageBox.Show("Percent Reduction TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPercent_Reduction.Select();
                return;
            }
            //
            // Save 
            //
            Nullable<decimal> dbuf = null;
            Health_care_insurance_Co newInsuranceCo = new Health_care_insurance_Co()
            {
                Organization_Name = (txtOrganization_Name.Text != txtOrganization_Name.LabelText) ? txtOrganization_Name.Text : string.Empty,
                Insurance_Type = (txtInsurance_Type.Text != txtInsurance_Type.LabelText) ? txtInsurance_Type.Text : string.Empty,
                Percent_Reduction = int.Parse(txtPercent_Reduction.Text),
                Fax = (txtFax.Text != txtFax.LabelText && txtFax.GetHasValidateValue) ? decimal.Parse(txtFax.Text) : dbuf,
                Telephone1 = (txtTelephone1.Text != txtTelephone1.LabelText && txtTelephone1.GetHasValidateValue) ? decimal.Parse(txtTelephone1.Text) : dbuf,
                Telephone2 = (txtTelephone2.Text != txtTelephone2.LabelText && txtTelephone2.GetHasValidateValue) ? decimal.Parse(txtTelephone2.Text) : dbuf,
                Address = (txtAddress.Text != txtAddress.LabelText && txtAddress.GetHasValidateValue) ? txtAddress.Text : string.Empty,
                Email = (txtEmail.Text != txtEmail.LabelText && txtEmail.GetHasValidateValue) ? txtEmail.Text : string.Empty,
                Site = (txtSite.Text != txtSite.LabelText && txtSite.GetHasValidateValue) ? txtSite.Text : string.Empty,
                ZipCode = (txtZipCode.Text != txtZipCode.LabelText && txtZipCode.GetHasValidateValue) ? decimal.Parse(txtZipCode.Text) : dbuf
            };

            DB.Health_care_insurance_Cos.InsertOnSubmit(newInsuranceCo);
            DB.SubmitChanges();

            MessageBox.Show("Health care insurance Co information you succeed in pharmacy database was recorded.", "New Records", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnCancelInsurance_Click(sender, e);
        }

        bool editState_I = false;
        bool findedTrue_I = false;
        private void btnEditInsurance_Click(object sender, EventArgs e)
        {
            if (!editState_I)
            {
                btnSaveInsurance.Enabled = false;
                btnShowAllInsurance.Enabled = false;
                txtInsuranceCo_ID.Visible = true;
                btnEditInsurance.Text = ". . . ثبت تغییرات";
                editState_I = true;
            }
            else
            {
                if (edit_InsuranceCoData())
                {
                    btnSaveInsurance.Enabled = true;
                    btnShowAllInsurance.Enabled = true;
                    txtInsuranceCo_ID.Visible = false;
                    btnEditInsurance.Text = "ویرایش شرکت بیمه ثبت شده";
                    clear_InsuranceTextBoxs();
                    txtInsuranceCo_ID.clearByLabelText();
                    editState_I = false;
                }
                else btnCancelInsurance_Click(sender, e);
            }
        }

        private bool edit_InsuranceCoData()
        {
            if (findedTrue_I)
            {
                int insuranceCode = int.Parse(txtInsuranceCo_ID.Text);
                if (string.IsNullOrEmpty(txtPercent_Reduction.Text) || txtPercent_Reduction.Text == txtPercent_Reduction.LabelText)
                {
                    MessageBox.Show("Percent Reduction TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPercent_Reduction.Select();
                    return false;
                }
                //
                // Edit
                //
                Nullable<decimal> dbuf = null;
                DB.InsuranceEdit(insuranceCode,
                    (txtOrganization_Name.Text != txtOrganization_Name.LabelText && txtOrganization_Name.GetHasValidateValue) ? txtOrganization_Name.Text : string.Empty,
                    (txtInsurance_Type.Text != txtInsurance_Type.LabelText) ? txtInsurance_Type.Text : string.Empty,
                    int.Parse(txtPercent_Reduction.Text),
                    (txtFax.Text != txtFax.LabelText && txtFax.GetHasValidateValue) ? decimal.Parse(txtFax.Text) : dbuf,
                    (txtTelephone1.Text != txtTelephone1.LabelText && txtTelephone1.GetHasValidateValue) ? decimal.Parse(txtTelephone1.Text) : dbuf,
                    (txtTelephone2.Text != txtTelephone2.LabelText && txtTelephone2.GetHasValidateValue) ? decimal.Parse(txtTelephone2.Text) : dbuf,
                    (txtEmail.Text != txtEmail.LabelText && txtEmail.GetHasValidateValue) ? txtEmail.Text : string.Empty,
                    (txtAddress.Text != txtAddress.LabelText && txtAddress.GetHasValidateValue) ? txtAddress.Text : string.Empty,
                    (txtZipCode.Text != txtZipCode.LabelText && txtZipCode.GetHasValidateValue) ? decimal.Parse(txtZipCode.Text) : dbuf,
                    (txtSite.Text != txtSite.LabelText && txtSite.GetHasValidateValue) ? txtSite.Text : string.Empty);

                DB.SubmitChanges();
                DB = new LINQ_PharmacyDataContext();
                MessageBox.Show("Health care insurance Co information you succeed in pharmacy database was edited.", "Edit Records", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            return false;
        }

        private void btnCancelInsurance_Click(object sender, EventArgs e)
        {
            clear_InsuranceTextBoxs();
            txtOrganization_Name.Focus();
            if (editState_I)
            {
                btnSaveInsurance.Enabled = true;
                btnShowAllInsurance.Enabled = true;
                txtInsuranceCo_ID.Visible = false;
                btnEditInsurance.Text = "ویرایش شرکتهای بیمه ثبت شده";
                txtInsuranceCo_ID.clearByLabelText();
                editState_I = false;
            }
        }

        private void clear_InsuranceTextBoxs()
        {
            txtOrganization_Name.clearByLabelText();
            txtInsurance_Type.clearByLabelText();
            txtPercent_Reduction.clearByLabelText();
            txtFax.clearByLabelText();
            txtTelephone1.clearByLabelText();
            txtTelephone2.clearByLabelText();
            txtEmail.clearByLabelText();
            txtSite.clearByLabelText();
            txtAddress.clearByLabelText();
            txtZipCode.clearByLabelText();
        }

        private void txtInsuranceCo_ID_KeyUp(object sender, KeyEventArgs e)
        {
            findedTrue_I = false;
            int id;
            if (txtInsuranceCo_ID.Text != txtInsuranceCo_ID.LabelText && txtInsuranceCo_ID.Text != string.Empty
                && int.TryParse(txtInsuranceCo_ID.Text, out id))
            {
                var Insurance = (from db in DB.Health_care_insurance_Cos
                                 where db.Insurance_ID == id
                                 select db).ToArray();
                if (Insurance.Length > 0)
                {
                    txtOrganization_Name.Text = Insurance[0].Organization_Name;

                    txtInsurance_Type.Text = Insurance[0].Insurance_Type;

                    txtPercent_Reduction.Text = Insurance[0].Percent_Reduction.ToString();

                    txtFax.Text = (Insurance[0].Fax.HasValue) ? Insurance[0].Fax.Value.ToString() : "";

                    txtAddress.Text = Insurance[0].Address;

                    txtTelephone1.Text = (Insurance[0].Telephone1.HasValue) ? Insurance[0].Telephone1.Value.ToString() : "";

                    txtTelephone2.Text = (Insurance[0].Telephone2.HasValue) ? Insurance[0].Telephone2.Value.ToString() : "";

                    txtZipCode.Text = (Insurance[0].ZipCode.HasValue) ? Insurance[0].ZipCode.Value.ToString() : "";

                    txtEmail.Text = Insurance[0].Email;

                    txtSite.Text = Insurance[0].Site;

                    findedTrue_I = true;
                }
                else clear_InsuranceTextBoxs();
            }
            else clear_InsuranceTextBoxs();
        }

        private void tPageInsuranceCO_Enter(object sender, EventArgs e)
        {
            txtOrganization_Name.Focus();
        }
        #endregion

        #region  Sale Drug or Toiletries
        private void rbtnGroupBoxFreeSale_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in grbFreeSale.Controls)
            {
                ctrl.Enabled = rbtnGroupBoxFreeSale.Checked;
            }
        }

        private void rbtnGroupBoxInsurance_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in grbInsurance.Controls)
            {
                ctrl.Enabled = rbtnGroupBoxInsurance.Checked;
            }
        }

        private void grbInsurance_LocationChanged(object sender, EventArgs e)
        {
            rbtnGroupBoxInsurance.Location = new Point(grbInsurance.Location.X - 7, grbInsurance.Location.Y);
        }

        private void grbInsurance_SizeChanged(object sender, EventArgs e)
        {
            rbtnGroupBoxInsurance.Location = new Point(grbInsurance.Location.X - 7, grbInsurance.Location.Y);
        }

        private void grbNoInsurance_SizeChanged(object sender, EventArgs e)
        {
            rbtnGroupBoxFreeSale.Location = new Point(grbFreeSale.Location.X - 7, grbFreeSale.Location.Y);
        }

        private void grbNoInsurance_LocationChanged(object sender, EventArgs e)
        {
            rbtnGroupBoxFreeSale.Location = new Point(grbFreeSale.Location.X - 7, grbFreeSale.Location.Y);
        }

        private void tPageSales_Enter(object sender, EventArgs e)
        {
            txtPatient_Name.Focus();
            rbtnGroupBoxFreeSale.Checked = true;
            rbtnGroupBoxInsurance_CheckedChanged(sender, e);
        }

        private void btnAddDrugs_Click(object sender, EventArgs e)
        {
            DrugsForm DF = new DrugsForm();
            DF.SelectedMode = true;
            //
            Dictionary<int, string> dicDrugs = new Dictionary<int, string>();
            foreach (DataGridViewRow row in dgvDrugs.Rows)
            {
                dicDrugs.Add(int.Parse(row.Cells["colDrugID"].Value.ToString()), "");
            }
            DF.dicSelectedDrugID = dicDrugs;
            DF.ShowDialog();
            dgvDrugs.Rows.Clear();
            foreach (var d in DF.dicSelectedDrugID)
            {
                dgvDrugs.Rows.Add(1);
                dgvDrugs.Rows[dgvDrugs.RowCount - 1].Cells["colDrugID"].Value = d.Key;
                dgvDrugs.Rows[dgvDrugs.RowCount - 1].Cells["colGenericNameFA"].Value = d.Value.ToString();
            }
        }

        private void btnRemoveSelectedDrug_Click(object sender, EventArgs e)
        {
            if (dgvDrugs.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow dgvRow in dgvDrugs.SelectedRows)
                {
                    dgvDrugs.Rows.Remove(dgvRow);
                }
            }
        }

        private void btnAddToiletries_Click(object sender, EventArgs e)
        {
            ToiletriesForm TF = new ToiletriesForm();
            TF.SelectedMode = true;
            //
            Dictionary<int, string> dicToiletries = new Dictionary<int, string>();
            foreach (DataGridViewRow row in dgvToiletries.Rows)
            {
                dicToiletries.Add(int.Parse(row.Cells["colToiletriesID"].Value.ToString()), "");
            }
            TF.dicSelectedToiletriesID = dicToiletries;
            TF.ShowDialog();
            dgvToiletries.Rows.Clear();
            foreach (var d in TF.dicSelectedToiletriesID)
            {
                dgvToiletries.Rows.Add(1);
                dgvToiletries.Rows[dgvToiletries.RowCount - 1].Cells["colToiletriesID"].Value = d.Key;
                dgvToiletries.Rows[dgvToiletries.RowCount - 1].Cells["colNameFA"].Value = d.Value.ToString();
            }
        }

        private void btnRemoveSelectedToiletries_Click(object sender, EventArgs e)
        {
            if (dgvToiletries.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow dgvRow in dgvToiletries.SelectedRows)
                {
                    dgvToiletries.Rows.Remove(dgvRow);
                }
            }
        }

        private void checkDrugs()
        {
            foreach (DataGridViewRow dgvRow in dgvDrugs.Rows)
            {
                try
                {
                    // set row drug's id
                    int drugId = int.Parse(dgvRow.Cells["colDrugID"].Value.ToString());
                    //
                    // search this row drug's in pharmacy
                    Drug rowDrug = (from d in DB.Drugs
                                    where d.Drug_ID == drugId
                                    select d).Single();
                    //
                    // check for count of row drug's by given drugs number
                    if ((rowDrug.Count_in_the_Stock + rowDrug.Count_on_the_Shelf) < int.Parse(dgvRow.Cells["colDrugNumNo"].Value.ToString()))
                    {
                        MessageBox.Show(string.Format("The number of drugs as '{0}. {1}' is not requested.",
                                                      rowDrug.Drug_ID, rowDrug.Generic_Name_FA),
                            "Not Available", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        //
                        // find similer drugs
                        //
                        /* no have code */
                    }
                    //
                    // check count of row drug's in 
                    else if (rowDrug.Count_on_the_Shelf < int.Parse(dgvRow.Cells["colDrugNumNo"].Value.ToString()))
                    {
                        MessageBox.Show(string.Format("The number of drugs as '{0}. {1}' is not requested in pharmacy shelf.",
                                                      rowDrug.Drug_ID, rowDrug.Generic_Name_FA),
                            "Not Available", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    //
                    // check by contrasting drug list
                    //
                    /* no have code */
                }
                catch { }
            }
        }

        private void dgvDrugs_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                int id;
                if (!int.TryParse(dgvDrugs[e.ColumnIndex, e.RowIndex].Value.ToString(), out id))
                {
                    MessageBox.Show("Please Enter just a number for count of drugs!", "Incorrect data enterd");
                    dgvDrugs[e.ColumnIndex, e.RowIndex].Value = "1";
                }
                else checkDrugs();
            }
        }

        private void dgvToiletries_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                int id;
                if (!int.TryParse(dgvToiletries[e.ColumnIndex, e.RowIndex].Value.ToString(), out id))
                {
                    MessageBox.Show("Please Enter just a number for count of toiletries!", "Incorrect data enterd");
                    dgvToiletries[e.ColumnIndex, e.RowIndex].Value = "1";
                }
                else checkToiletries();
            }
        }

        private void checkToiletries()
        {
            try
            {
                foreach (DataGridViewRow dgvRow in dgvToiletries.Rows)
                {
                    // set row Toiletries's id
                    int toiletriesId = int.Parse(dgvRow.Cells["colToiletriesID"].Value.ToString());
                    //
                    // search this row drug's in pharmacy
                    Toiletry rowToiletries = (from d in DB.Toiletries
                                              where d.Toiletries_ID == toiletriesId
                                              select d).Single();
                    //
                    // check for count of row Toiletries's by given Toiletries number
                    if ((rowToiletries.Count_in_the_Stock + rowToiletries.Count_on_the_Shelf) < int.Parse(dgvRow.Cells["colToiletriesNumNo"].Value.ToString()))
                    {
                        MessageBox.Show(string.Format("The number of Toiletries as '{0}. {1}' is not requested.",
                                                      rowToiletries.Toiletries_ID, rowToiletries.Name_FA),
                            "Not Available", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        //
                        // find similer Toiletries
                        //
                        /* no have code */
                    }
                    //
                    // check count of row Toiletries's in 
                    else if (rowToiletries.Count_on_the_Shelf < int.Parse(dgvRow.Cells["colToiletriesNumNo"].Value.ToString()))
                    {
                        MessageBox.Show(string.Format("The number of Toiletries as '{0}. {1}' is not requested in pharmacy shelf.",
                                                      rowToiletries.Toiletries_ID, rowToiletries.Name_FA),
                            "Not Available", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    //
                    // check by contrasting Toiletries list
                    //
                    /* no have code */
                }
            }
            catch { }
        }

        private System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.ToString(), true);
        private void btnAutoCalcFree_Click(object sender, EventArgs e)
        {
            decimal sum_SalePrice = 0;
            decimal sum_PurchasePrice = 0;
            int id;
            try
            {
                //
                // calc sum price of any selected drugs * counts for sale
                //
                foreach (DataGridViewRow row in dgvDrugs.Rows)
                {
                    id = int.Parse(row.Cells["colDrugID"].Value.ToString());
                    Drug drug = (from d in DB.Drugs
                                 where d.Drug_ID == id
                                 select d).Single();
                    sum_PurchasePrice += (((drug.Purchase_Price.HasValue) ? drug.Purchase_Price.Value : 0) *
                                              int.Parse(row.Cells[2].Value.ToString()));

                    sum_SalePrice += (drug.Sale_Price * int.Parse(row.Cells[2].Value.ToString()));
                }
                //
                // calc sum price of any selected toiletries * counts for sale
                //
                foreach (DataGridViewRow row in dgvToiletries.Rows)
                {
                    id = int.Parse(row.Cells["colToiletriesID"].Value.ToString());
                    Toiletry toiletry = (from t in DB.Toiletries
                                         where t.Toiletries_ID == id
                                         select t).Single();
                    sum_PurchasePrice += (((toiletry.Purchase_Price.HasValue) ? toiletry.Purchase_Price.Value : 0) *
                                              int.Parse(row.Cells["colToiletriesNumNo"].Value.ToString()));

                    sum_SalePrice += (toiletry.Sale_Price * int.Parse(row.Cells["colToiletriesNumNo"].Value.ToString()));
                }
                //
                // show in textBox
                //
                txtTotal_Purchase_Price.Text = sum_PurchasePrice.ToString("C", culture);
                txtTotal_Sale_Price_FreeI.Text = sum_SalePrice.ToString("C", culture);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source); }
        }

        private void btnSelectInsurance_Click(object sender, EventArgs e)
        {
            var insuranceCo = from i in DB.Health_care_insurance_Cos
                              select new object[] { i.Insurance_ID, i.Organization_Name };

            dgvSelectInsuranceCo.Rows.Clear();
            foreach (var anyInsurance in insuranceCo)
            {
                if (anyInsurance[0].ToString() != "1") // don't show Free InsuranceCo in rows
                    dgvSelectInsuranceCo.Rows.Add(anyInsurance);
            }
            dgvSelectInsuranceCo.Visible = true;
        }

        private void dgvSelectInsuranceCo_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvSelectInsuranceCo.SelectedRows.Count > 0)
            {
                txtOrganization_Name_I.Text = dgvSelectInsuranceCo.SelectedRows[0].Cells[1].Value.ToString();
                txtInsurance_ID_I.Text = dgvSelectInsuranceCo.SelectedRows[0].Cells[0].Value.ToString();
                dgvSelectInsuranceCo.Visible = false;
            }
        }

        private void btnAutoCalc_Click(object sender, EventArgs e)
        {
            decimal sum_SalePrice = 0;
            decimal sum_PurchasePrice = 0;
            decimal organizPortion_price = 0;
            int id;
            try
            {
                //
                // calc sum price of any selected drugs * counts for sale
                //
                foreach (DataGridViewRow row in dgvDrugs.Rows)
                {
                    id = int.Parse(row.Cells["colDrugID"].Value.ToString());
                    Drug drug = (from d in DB.Drugs
                                 where d.Drug_ID == id
                                 select d).Single();

                    sum_PurchasePrice += (((drug.Purchase_Price.HasValue) ? drug.Purchase_Price.Value : 0) *
                                              int.Parse(row.Cells["colDrugNumNo"].Value.ToString()));

                    decimal salePrice = (drug.Sale_Price * int.Parse(row.Cells["colDrugNumNo"].Value.ToString()));
                    sum_SalePrice += salePrice;

                    if (drug.Including_insurance && txtInsurance_ID_I.Text != string.Empty &&
                        txtInsurance_ID_I.Text != txtInsurance_ID_I.LabelText)
                    {
                        int insuranceCoID = int.Parse(txtInsurance_ID_I.Text);
                        int percent = (from p in DB.Health_care_insurance_Cos
                                       where p.Insurance_ID == insuranceCoID
                                       select p.Percent_Reduction).Single();
                        organizPortion_price += ((percent * salePrice) / 100);
                    }
                }
                //
                // calc sum price of any selected toiletries * counts for sale
                //
                foreach (DataGridViewRow row in dgvToiletries.Rows)
                {
                    id = int.Parse(row.Cells["colToiletriesID"].Value.ToString());
                    Toiletry toiletry = (from t in DB.Toiletries
                                         where t.Toiletries_ID == id
                                         select t).Single();
                    sum_PurchasePrice += (((toiletry.Purchase_Price.HasValue) ? toiletry.Purchase_Price.Value : 0) *
                                              int.Parse(row.Cells["colToiletriesNumNo"].Value.ToString()));

                    sum_SalePrice += (toiletry.Sale_Price * int.Parse(row.Cells["colToiletriesNumNo"].Value.ToString()));
                }
                //
                // show in textBox
                //
                txtTotal_Purchase_Price.Text = sum_PurchasePrice.ToString("C", culture);
                txtTotal_Sale_Price_I.Text = sum_SalePrice.ToString("C", culture);
                txtOrganizations_Portion.Text = organizPortion_price.ToString("C", culture);
                txtPortion_Insured.Text = (sum_SalePrice - organizPortion_price).ToString("C", culture);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source); }
        }
        #endregion

        private void btnShowAllPatient_Click(object sender, EventArgs e)
        {
            PatientsForm PF = new PatientsForm();
            PF.ShowDialog();
        }

        bool editState_P = false;
        bool findedTrue_P = false;

        private void btnCancelPatient_Click(object sender, EventArgs e)
        {
            clear_PatientTextBoxs();
            txtPatient_Name.Focus();
            if (editState_P)
            {
                btnSavePatient.Enabled = true;
                btnShowAllPatient.Enabled = true;
                txtPatient_ID.Visible = false;
                btnEditPatient.Text = "ویرایش بیماران ثبت شده";
                txtPatient_ID.clearByLabelText();
                editState_P = false;
            }
        }
        private void clear_PatientTextBoxs()
        {
            txtPatient_Name.clearByLabelText();
            fadpcDate_of_copy.ResetText();
            txtDoctor_Name.clearByLabelText();
            txtMedical_Council_No.clearByLabelText();
            txtTotal_Purchase_Price.clearByLabelText();
            txtTotal_Sale_Price_FreeI.clearByLabelText();
            txtTotal_Sale_Price_I.clearByLabelText();
            txtHealth_Insurance_Code.clearByLabelText();
            txtCopy_page_number.clearByLabelText();
            txtInsurance_ID_I.clearByLabelText();
            txtOrganization_Name_I.clearByLabelText();
            txtPortion_Insured.clearByLabelText();
            txtOrganizations_Portion.clearByLabelText();
            dgvDrugs.Rows.Clear();
            dgvToiletries.Rows.Clear();
            dgvSelectInsuranceCo.Visible = false;
            rbtnGroupBoxFreeSale.Checked = true;
        }

        private void btnSavePatient_Click(object sender, EventArgs e)
        {
            if (txtTotal_Purchase_Price.Text == txtTotal_Purchase_Price.LabelText)
            {
                MessageBox.Show("Total Purchase Price TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTotal_Purchase_Price.Select();
                return;
            }
            if (rbtnGroupBoxFreeSale.Checked)
            {
                if (string.IsNullOrEmpty(txtTotal_Sale_Price_FreeI.Text) || 
                    txtTotal_Sale_Price_FreeI.Text == txtTotal_Sale_Price_FreeI.LabelText) 
                {
                    MessageBox.Show("Total Sale Price TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTotal_Sale_Price_FreeI.Select();
                    return;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtTotal_Sale_Price_I.Text) ||
                    txtTotal_Sale_Price_I.Text == txtTotal_Sale_Price_I.LabelText)
                {
                    MessageBox.Show("Total Sale Price TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTotal_Sale_Price_I.Select();
                    return;
                }
            }
            if (dgvDrugs.Rows.Count == 0 && dgvToiletries.Rows.Count == 0)
            {
                MessageBox.Show("No products selected for purchase.!");
                return;
            }
            //
            // Save 
            //
            int? ibuf = null;
            try
            {
                Patient newPatient = new Patient()
                {
                    Insurance_ID = (rbtnGroupBoxFreeSale.Checked) ? 1 : int.Parse(txtInsurance_ID_I.Text),
                    Health_Insurance_Code = (txtHealth_Insurance_Code.GetHasValidateValue) ? txtHealth_Insurance_Code.Text : string.Empty,
                    Patient_Name = (txtPatient_Name.GetHasValidateValue) ? txtPatient_Name.Text : string.Empty,
                    Date_of_copy = fadpcDate_of_copy.SelectedDateTime.Date.ToShortDateString(),
                    Copy_Page_Number = (txtCopy_page_number.GetHasValidateValue) ? int.Parse(txtCopy_page_number.Text) : ibuf,
                    Doctor_Name = (txtDoctor_Name.Text != txtDoctor_Name.LabelText) ? txtDoctor_Name.Text : string.Empty,
                    Medical_Council_No = (txtMedical_Council_No.Text != txtMedical_Council_No.LabelText) ? txtMedical_Council_No.Text : string.Empty,
                    Total_Sale_Price = (rbtnGroupBoxFreeSale.Checked) ? 
                    decimal.Parse(txtTotal_Sale_Price_FreeI.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture)
                    : decimal.Parse(txtTotal_Sale_Price_I.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture),
                    Total_Purchase_Price = decimal.Parse(txtTotal_Purchase_Price.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture),
                    Portion_insured = (rbtnGroupBoxFreeSale.Checked) ? decimal.Parse(txtTotal_Sale_Price_FreeI.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture)
                    : decimal.Parse(txtPortion_Insured.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture),
                    Organizations_Portion = (rbtnGroupBoxFreeSale.Checked) ? 0 : decimal.Parse(txtOrganizations_Portion.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture)
                };

                DB.Patients.InsertOnSubmit(newPatient);
                DB.SubmitChanges();

                MessageBox.Show("Patient information you succeed in pharmacy database was recorded.", "New Records", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnCancelPatient_Click(sender, e);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source); }
        }

        private bool edit_PatientData()
        {
            if (findedTrue_P)
            {
                int patientCode = int.Parse(txtPatient_ID.Text);
                if (txtTotal_Purchase_Price.Text == txtTotal_Purchase_Price.LabelText)
                {
                    MessageBox.Show("Total Purchase Price TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTotal_Purchase_Price.Select();
                    return false;
                }
                if (rbtnGroupBoxFreeSale.Checked)
                {
                    if (string.IsNullOrEmpty(txtTotal_Sale_Price_FreeI.Text) ||
                        txtTotal_Sale_Price_FreeI.Text == txtTotal_Sale_Price_FreeI.LabelText)
                    {
                        MessageBox.Show("Total Sale Price TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTotal_Sale_Price_FreeI.Select();
                        return false;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(txtTotal_Sale_Price_I.Text) ||
                        txtTotal_Sale_Price_I.Text == txtTotal_Sale_Price_I.LabelText)
                    {
                        MessageBox.Show("Total Sale Price TextBox's is Empty!", "Empty TextBox Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTotal_Sale_Price_I.Select();
                        return false;
                    }
                }
                if (dgvDrugs.Rows.Count == 0 && dgvToiletries.Rows.Count == 0)
                {
                    MessageBox.Show("No products selected for purchase.!");
                    return false;
                }
                //
                // Edit
                //
                int? ibuf = null;
                DB.PatientEdit(patientCode,
                    (rbtnGroupBoxFreeSale.Checked) ? 1 : int.Parse(txtInsurance_ID_I.Text),
                    (txtHealth_Insurance_Code.GetHasValidateValue) ? txtHealth_Insurance_Code.Text : string.Empty,
                    (txtPatient_Name.GetHasValidateValue) ? txtPatient_Name.Text : string.Empty,
                    fadpcDate_of_copy.SelectedDateTime.Date.ToShortDateString(),
                    (txtCopy_page_number.GetHasValidateValue) ? int.Parse(txtCopy_page_number.Text) : ibuf,
                    (txtDoctor_Name.Text != txtDoctor_Name.LabelText) ? txtDoctor_Name.Text : string.Empty,
                    (txtMedical_Council_No.Text != txtMedical_Council_No.LabelText) ? txtMedical_Council_No.Text : string.Empty,
                    (rbtnGroupBoxFreeSale.Checked) ?
                    decimal.Parse(txtTotal_Sale_Price_FreeI.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture)
                    : decimal.Parse(txtTotal_Sale_Price_I.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture),
                    decimal.Parse(txtTotal_Purchase_Price.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture),
                    (rbtnGroupBoxFreeSale.Checked) ? decimal.Parse(txtTotal_Sale_Price_FreeI.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture)
                    : decimal.Parse(txtPortion_Insured.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture),
                    (rbtnGroupBoxFreeSale.Checked) ? 0 : decimal.Parse(txtOrganizations_Portion.Text, NumberStyles.Currency | NumberStyles.Float, txtPurchase_Price.Culture));

                DB.SubmitChanges();
                DB = new LINQ_PharmacyDataContext();
                MessageBox.Show("Patient information you succeed in pharmacy database was edited.", "Edit Records", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            return false;
        }

        private void btnEditPatient_Click(object sender, EventArgs e)
        {
            if (!editState_P)
            {
                btnSavePatient.Enabled = false;
                btnShowAllPatient.Enabled = false;
                txtPatient_ID.Visible = true;
                btnEditPatient.Text = ". . . ثبت تغییرات";
                editState_P = true;
            }
            else
            {
                if (edit_PatientData())
                {
                    btnSavePatient.Enabled = true;
                    btnShowAllPatient.Enabled = true;
                    txtPatient_ID.Visible = false;
                    btnEditPatient.Text = "ویرایش بیمار ثبت شده";
                    clear_PatientTextBoxs();
                    txtPatient_ID.clearByLabelText();
                    editState_P = false;
                }
                else btnCancelPatient_Click(sender, e);
            }
        }

        private void txtPatient_ID_KeyUp(object sender, KeyEventArgs e)
        {
            findedTrue_P = false;
            int id;
            if (txtPatient_ID.Text != txtPatient_ID.LabelText && txtPatient_ID.Text != string.Empty
                && int.TryParse(txtPatient_ID.Text, out id))
            {
                var Patient = (from db in DB.Patients
                               where db.Patient_ID == id
                               select db).ToArray();
                if (Patient.Length > 0)
                {
                    txtPatient_Name.Text = Patient[0].Patient_Name;

                    txtHealth_Insurance_Code.Text = Patient[0].Health_Insurance_Code;

                    txtDoctor_Name.Text = Patient[0].Doctor_Name;

                    txtMedical_Council_No.Text = Patient[0].Medical_Council_No;

                    txtTotal_Purchase_Price.Text = Patient[0].Total_Purchase_Price.ToString();

                    txtTotal_Sale_Price_FreeI.Text = Patient[0].Total_Sale_Price.ToString();

                    txtTotal_Sale_Price_I.Text = Patient[0].Total_Sale_Price.ToString();

                    txtOrganizations_Portion.Text = (Patient[0].Organizations_Portion.HasValue) ? Patient[0].Organizations_Portion.Value.ToString() : txtOrganizations_Portion.LabelText;

                    txtPortion_Insured.Text = (Patient[0].Portion_insured.HasValue) ? Patient[0].Portion_insured.Value.ToString() : txtPortion_Insured.LabelText;

                    txtCopy_page_number.Text = (Patient[0].Copy_Page_Number.HasValue) ? Patient[0].Copy_Page_Number.Value.ToString() : txtCopy_page_number.LabelText;

                    findedTrue_P = true;
                }
                else clear_PatientTextBoxs();
            }
            else clear_PatientTextBoxs();
        }
    }
}
