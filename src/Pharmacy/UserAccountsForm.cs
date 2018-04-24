using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Pharmacy
{
    public partial class UserAccountsForm : Form
    {
        public static string Entry_UserName = @"#admin#";
        public static string Entry_Modifiers = "Administrator";
        public UserAccountsForm()
        {
            InitializeComponent();
            if (Entry_UserName == "") { Entry_UserName = @"#admin#"; Entry_Modifiers = "Administrator"; }
            txtUserName.Text = Entry_UserName;
            cmbModifiers.Text = Entry_Modifiers;
        }

        #region Timers
        private void btnChangePass_Click(object sender, EventArgs e)
        {
            //
            // Understand whether the panel should be closed or open
            //
            if (panelChangePass.Size.Height < 220) // Panel is closed so open panel
            {
                btnChangePass.Image = global::Pharmacy.Properties.Resources.Uper;
                btnChangeAccounts.Image = global::Pharmacy.Properties.Resources.Downer;
                timerClosePanel_ChangeAccounts.Start();
                timerOpenPanel_ChangePass.Start();
                timerOpenPanel_ChangeAccounts.Stop();
                timerClosePanel_ChangePass.Stop();
            }
            else // Panel is opened so close panel
            {
                btnChangePass.Image = global::Pharmacy.Properties.Resources.Downer;
                timerClosePanel_ChangePass.Start();
                timerOpenPanel_ChangePass.Stop();
            }
        }

        private void btnChangeAccounts_Click(object sender, EventArgs e)
        {
            txtInfo.Visible = false;
            //
            // Understand whether the panel should be closed or open
            //
            if (panelChangeAccounts.Size.Height < 220) // Panel is closed so open panel
            {
                btnChangeAccounts.Image = global::Pharmacy.Properties.Resources.Uper;
                btnChangePass.Image = global::Pharmacy.Properties.Resources.Downer;
                timerClosePanel_ChangePass.Start();
                timerOpenPanel_ChangeAccounts.Start();
                timerOpenPanel_ChangePass.Stop();
                timerClosePanel_ChangeAccounts.Stop();
            }
            else // Panel is opened so close panel
            {
                btnChangeAccounts.Image = global::Pharmacy.Properties.Resources.Downer;
                timerClosePanel_ChangeAccounts.Start();
                timerOpenPanel_ChangeAccounts.Stop();
            }
        }

        private void timerOpenPanel_ChangePass_Tick(object sender, EventArgs e)
        {
            if (panelChangePass.Size.Height < 220)
            {
                panelChangePass.Size = new Size(270, panelChangePass.Size.Height + 10);
                panelChangeAccounts.Location = new Point(12, 12 + panelChangePass.Size.Height + 6);
            }
            else
            {
                panelChangePass.Size = new Size(270, 220);
                panelChangeAccounts.Location = new Point(12, 238);
                timerOpenPanel_ChangePass.Stop();
            }
        }

        private void timerOpenPanel_ChangeAccounts_Tick(object sender, EventArgs e)
        {
            if (panelChangeAccounts.Size.Height < 220)
            {
                panelChangeAccounts.Size = new Size(270, panelChangeAccounts.Size.Height + 10);
            }
            else
            {
                panelChangeAccounts.Size = new Size(270, 220);
                timerOpenPanel_ChangeAccounts.Stop();
            }
        }

        private void timerClosePanel_ChangePass_Tick(object sender, EventArgs e)
        {
            if (panelChangePass.Size.Height > 22)
            {
                panelChangePass.Size = new Size(270, panelChangePass.Size.Height - 10);
                panelChangeAccounts.Location = new Point(12, 12 + panelChangePass.Size.Height + 6);
            }
            else
            {
                panelChangePass.Size = new Size(270, 22);
                panelChangeAccounts.Location = new Point(12, 40);
                timerClosePanel_ChangePass.Stop();
            }
        }

        private void timerClosePanel_ChangeAccounts_Tick(object sender, EventArgs e)
        {
            if (panelChangeAccounts.Size.Height > 22)
            {
                panelChangeAccounts.Size = new Size(270, panelChangeAccounts.Size.Height - 10);
            }
            else
            {
                panelChangeAccounts.Size = new Size(270, 22);
                timerClosePanel_ChangeAccounts.Stop();
            }
        }
        #endregion

        #region Change Accounts Panel's Controls

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtUser.Text != "" && txtPass.Text != "" && txtRePass.Text != "" && cmbModifiers.Text != "")
            {
                if (txtPass.Text == txtRePass.Text) // New Pass == Confirm New Pass
                {
                    var oldUser = (from old in new LINQ_UserPassDataContext().User_Passwords
                                   where old.UserName.ToLower() == txtUser.Text.ToLower()
                                   select old).SingleOrDefault();
                    if (oldUser == null)
                    {
                        string Password = CreateMD5Hash(txtPass.Text);

                        new LINQ_UserPassDataContext().SaveUserPass(txtUser.Text, Password, cmbModifiers.Text, txtPassHint.Text);
                        MessageBox.Show("User by name (" + txtUser.Text + ") successful added to program's login");
                        // 
                        // clear any textBox
                        //
                        cmbModifiers.Text = "Administrator";
                        txtPassHint.Text = string.Empty;
                        txtRePass.Text = string.Empty;
                        txtPass.Text = string.Empty;
                        txtUser.Text = string.Empty;
                        txtUser.Focus();
                    }
                    else // this user already added to User_Passwords database
                    {
                        MessageBox.Show("This user name is already added in user_passwords database! \rPlease enter other name"
                            , "Find Duplicate User Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtUser.Text = string.Empty;
                        txtPass.Text = string.Empty;
                        txtRePass.Text = string.Empty;
                        txtUser.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("The passwords you typed do not match." +
                            " Please retype the new password in both boxes.",
                            "User Account Control Panel");
                    txtRePass.Text = string.Empty;
                    txtPass.Text = string.Empty;
                    txtPass.Focus();
                }
            }
            else
            {
                MessageBox.Show("You have empty important textBoxes. Please fill all boxes.", "Empty TextBox Error");
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (txtUser.Text != "" && txtPass.Text != "")
            {
                string Password = CreateMD5Hash(txtPass.Text);

                var dbUP = (from user in new LINQ_UserPassDataContext().User_Passwords
                            where (user.UserName.ToLower() == txtUser.Text.ToLower()) && (user.Password == Password)
                            select user.UserName).SingleOrDefault();
                if (dbUP != null)
                {
                    if (MessageBox.Show("Are you sure to delete user by name (" + txtUser.Text + ") ?", "Deleting Question's"
                        , MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        new LINQ_UserPassDataContext().DeleteUserPass(dbUP);
                        txtUser.Text = string.Empty;
                        txtPass.Text = string.Empty;
                        txtRePass.Text = string.Empty;
                        txtPassHint.Text = string.Empty;
                        cmbModifiers.SelectedItem = cmbModifiers.Items[0];
                        txtUser.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Your user name or password is incorrect");
                    txtUser.Text = string.Empty;
                    txtPass.Text = string.Empty;
                    txtUser.Focus();
                }
            }
            else
            {
                MessageBox.Show("You have empty important textBoxes. Please fill all boxes.", "Empty TextBox Error");
            }
        }

        bool Lock = true;
        private void picLocker_Click(object sender, EventArgs e)
        {
            if (Lock) // UnLock
            {
                if (txtUser.Text != "" && txtPass.Text != "" && cmbModifiers.Text != "")
                {
                    string Password = CreateMD5Hash(txtPass.Text);
                    var db = new LINQ_UserPassDataContext();
                    var arrAdmin = (from adminUser in db.User_Passwords
                                    where (adminUser.Modifiers == @"Administrator" && 
                                    adminUser.UserName.ToLower() == txtUser.Text.ToLower() && 
                                    Password == adminUser.Password)
                                    select adminUser).ToArray();

                    if (arrAdmin.Length > 0)
                    {
                        Lock = false;
                        picLocker.Image = global::Pharmacy.Properties.Resources.unlock;
                        dgvUserPass.DataSource = db.User_Passwords;
                        //
                        dgvUserPass.Columns[0].HeaderText = "User Name";
                        dgvUserPass.Columns[0].Width = 120;
                        dgvUserPass.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgvUserPass.Columns[0].ReadOnly = true;
                        //
                        dgvUserPass.Columns[1].HeaderText = "Password";
                        dgvUserPass.Columns[1].Width = 100;
                        dgvUserPass.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        //
                        dgvUserPass.Columns[2].HeaderText = "Password Hint";
                        dgvUserPass.Columns[2].Width = 100;
                        //
                        dgvUserPass.Columns[3].HeaderText = "Modifiers";
                        dgvUserPass.Columns[3].Width = 100;
                        //
                        this.Size = new Size(800, 300);
                    }
                    else return;                    
                }
            }
            else // Lock
            {
                dgvUserPass.Columns.Clear();
                this.Size = new Size(300, 300);
                picLocker.Image = global::Pharmacy.Properties.Resources.locked;
                txtUser.Text = string.Empty;
                txtPass.Text = string.Empty;
                txtRePass.Text = string.Empty;
                txtPassHint.Text = string.Empty;
                cmbModifiers.Text = Entry_Modifiers;
                txtUser.Focus();
                Lock = true;
            }
        }
        #endregion

        #region Change Password Panel's Controls
        private void txtHint_MouseDown(object sender, MouseEventArgs e)
        {
            if (txtHint.ForeColor == Color.DimGray)
            {
                txtHint.Text = string.Empty;
                txtHint.ForeColor = Color.Black;
            }
        }

        private void txtHint_MouseLeave(object sender, EventArgs e)
        {
            if (txtHint.Text == "")
            {
                txtHint.ForeColor = Color.DimGray;
                txtHint.Text = "Password Hint";
            }
        }

        private void txtHint_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtHint.ForeColor == Color.DimGray)
            {
                txtHint.Text = string.Empty;
                txtHint.ForeColor = Color.Black;
            }
        }

        private void txtHint_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtHint.Text == "")
            {
                txtHint.ForeColor = Color.DimGray;
                txtHint.Text = "Password Hint";
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (txtCurrentPassword.Text != "" && txtNewPassword.Text != "" & txtConfirmNewPassword.Text != "")
            {
                if (txtConfirmNewPassword.Text == txtNewPassword.Text) // New Pass == Confirm New Pass
                {
                    string CurrentPassword = CreateMD5Hash(txtCurrentPassword.Text);

                    string NewPassword = CreateMD5Hash(txtNewPassword.Text);

                    var db = (from UP in new LINQ_UserPassDataContext().User_Passwords
                              where (UP.UserName.ToLower() == txtUserName.Text.ToLower()) && (UP.Password == CurrentPassword)
                              select UP).SingleOrDefault();

                    if (db != null) // Correct current pass
                    {
                        new LINQ_UserPassDataContext().EditUserPass(db.UserName, NewPassword, txtHint.Text, db.Modifiers);
                        MessageBox.Show("Change password successfuly!");
                    }
                    else // Incorrect current pass
                    {
                        MessageBox.Show("Your password is incorrect!",
                            "Incorrect Password", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    // 
                    // clear any textBox
                    //
                    txtHint.ForeColor = Color.DimGray;
                    txtHint.Text = "Password Hint";
                    txtNewPassword.Text = string.Empty;
                    txtCurrentPassword.Text = string.Empty;
                    txtConfirmNewPassword.Text = string.Empty;
                    txtCurrentPassword.Focus();
                }
                else
                {
                    MessageBox.Show("The passwords you typed do not match." +
                            " Please retype the new password in both boxes.",
                            "User Account Control Panel");
                    txtNewPassword.Text = string.Empty;
                    txtConfirmNewPassword.Text = string.Empty;
                    txtNewPassword.Focus();
                }
            }
            else
            {
                MessageBox.Show("You have empty important textBoxes. Please fill all boxes.", "Empty TextBox Error");
            }
        }

        #endregion

        #region Converter Code

        private void dgvUserPass_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Cells[0].Value != null)
            {
                if (MessageBox.Show("Are you sure to delete user by name (" + e.Row.Cells[0].Value.ToString() + ") ?",
                    "User Deleting Row", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    new LINQ_UserPassDataContext().DeleteUserPass(e.Row.Cells[0].Value.ToString());
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void dgvUserPass_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                new LINQ_UserPassDataContext().EditUserPass(dgvUserPass[0, e.RowIndex].Value.ToString(),
                                                            dgvUserPass[1, e.RowIndex].Value.ToString(),
                                                            dgvUserPass[2, e.RowIndex].Value.ToString(),
                                                            dgvUserPass[3, e.RowIndex].Value.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnConvertPass_Click(object sender, EventArgs e)
        {
            if (txtOriginalPass.Text != "" && txtOriginalPass.ForeColor == Color.Black)
            {
                txtHashPass.ForeColor = Color.Black;
                txtHashPass.Text = CreateMD5Hash(txtOriginalPass.Text);
            }
        }

        public string CreateMD5Hash(string input)
        {
            //Create a byte array from source data
            byte[] inputBytes = ASCIIEncoding.ASCII.GetBytes(input);
            //
            //Compute hash based on source data
            byte[] hashBytes = new MD5CryptoServiceProvider().ComputeHash(inputBytes);
            //
            // Convert the byte array to hexadecimal string
            //
            StringBuilder sOutput = new StringBuilder(hashBytes.Length);
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sOutput.Append(hashBytes[i].ToString("X2"));
                // To force the hex string to lower-case letters instead of
                // upper-case, use he following line instead:
                // sOutput.Append(hashBytes[i].ToString("x2")); 
            }
            return sOutput.ToString();
        }
        #endregion
        #region Converter Graphics
        private void txtOriginalPass_MouseDown(object sender, MouseEventArgs e)
        {
            if (txtOriginalPass.ForeColor == Color.DimGray)
            {
                txtOriginalPass.Text = "";
                txtOriginalPass.ForeColor = Color.Black;
            }
        }

        private void txtOriginalPass_MouseLeave(object sender, EventArgs e)
        {
            if (txtOriginalPass.Text == "")
            {
                txtOriginalPass.ForeColor = Color.DimGray;
                txtOriginalPass.Text = "Original Password";
            }
        }

        private void txtOriginalPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtOriginalPass.ForeColor == Color.DimGray)
            {
                txtOriginalPass.Text = "";
                txtOriginalPass.ForeColor = Color.Black;
            }
        }

        private void txtOriginalPass_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtOriginalPass.Text == "")
            {
                txtOriginalPass.ForeColor = Color.DimGray;
                txtOriginalPass.Text = "Original Password";
            }
        }
        #endregion

        private void cmbModifiers_SelectedIndexChanged(object sender, EventArgs e)
        {            
            switch (this.cmbModifiers.Text)
            {
                case "Administrator": txtInfo.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture,
                                                    "سطح دسترسی دکتر داروخانه:" + Environment.NewLine + Environment.NewLine +
                                                    "ایجاد کاربران جدید و یا حذف کاربران" + Environment.NewLine +
                                                    "تعیین سطوح دسترسی و تغییر رمز کابران" + Environment.NewLine +
                                                    "بازیابی و یا تهیه پشتیبان از اطلاعات" + Environment.NewLine + 
                                                    "تغییر و یا حذف اطلاعات اشتباه");
                                                  
                    break;
                case "Finance User": txtInfo.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture,
                                                    "سطح دسترسی کاربر مالی:" + Environment.NewLine + Environment.NewLine +
                                                    "محاسبه کردن وجه نسخه بیمار" + Environment.NewLine +
                                                    "محاسبه کردن وجه افراد متفرقه" + Environment.NewLine +
                                                    "ثبت کردن فاکتور خرید" + Environment.NewLine +
                                                    "تهیه گذارشات مالی");
                    break;
                case "Preparation User": txtInfo.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture,
                                                    "سطح دسترسی کاربر آماده سازی:" + Environment.NewLine + Environment.NewLine +
                                                    "ثبت کردن اطلاعات دارو ها" + Environment.NewLine +
                                                    "ثبت کردن اطلاعات بیمه ها" + Environment.NewLine +
                                                    "ثبت کردن اطلاعات محصولات آرایشی و بهداشتی");
                    break;
                case "Receptionist User": txtInfo.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture,
                                                    "سطح دسترسی کاربر پذیرش:" + Environment.NewLine + Environment.NewLine +
                                                    "ثبت کردن مشخصات بیمار ها و تحویل دارو به بیمار مراجعه کننده" + Environment.NewLine +
                                                    "ثبت کردن مشخصات افراد متفرقه و تحویل درخواستهای آنها");
                    break;
            }
            
        }

        private void picInfo_Click(object sender, EventArgs e)
        {
            txtInfo.Visible = !txtInfo.Visible;
        }

        private void txtInfo_Click(object sender, EventArgs e)
        {
            this.txtInfo.Visible = false;
        }

        private void UserAccountsForm_Load(object sender, EventArgs e)
        {
            txtInfo.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture,
                                                     "سطح دسترسی دکتر داروخانه:" + Environment.NewLine + Environment.NewLine +
                                                     "ایجاد کاربران جدید و یا حذف کاربران" + Environment.NewLine +
                                                     "تعیین سطوح دسترسی و تغییر رمز کابران" + Environment.NewLine +
                                                     "بازیابی و یا تهیه پشتیبان از اطلاعات" + Environment.NewLine +
                                                     "تغییر و یا حذف اطلاعات اشتباه");
        }

        
    }
}
