using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace B.B.Components
{
    public class LabelTextBox : TextBox
    {
        private System.ComponentModel.Container components = new System.ComponentModel.Container();

        private System.Windows.Forms.ToolTip textToolTip = new ToolTip();
        private string lblText;
        private System.Drawing.Color lblColor = Color.DarkGray;
        private System.Drawing.Color textColor = Color.Black;
        private TextBoxFormat format = TextBoxFormat.Text;
        private System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.ToString(), true);
        private bool okKeyChar = true;

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                this.ForeColor = (value != this.LabelText) ? textColor : lblColor;
            }
        }

        [Category("Properties"), Description("Written label text's that is inside the TextBox.")]
        public string LabelText
        {
            get { return lblText; }
            set
            {
                lblText = value; this.Text = value;
                textToolTip.RemoveAll();
                textToolTip.SetToolTip(this, lblText);
            }
        }

        [Category("Properties"), Description("Written label color's that is inside the TextBox.")]
        public Color LabelColor
        { get { return lblColor; } set { lblColor = value; this.ForeColor = value; } }

        [Category("Properties"), Description("Written text color's that is inside the TextBox.")]
        public Color TextColor
        { get { return textColor; } set { textColor = value; } }

        [Category("Properties"), Description("Written label toolTip's that is inside the TextBox.")]
        public ToolTip TextToolTip 
        { get { return textToolTip; } set { textToolTip = value; } }

        [Category("Properties"), Description("Set text Format for TextBox.")]
        public TextBoxFormat Format 
        { get { return format; } set { format = value; } }

        [Category("Properties"), Description("Set text CultureInfo for TextBox.")]
        public System.Globalization.CultureInfo Culture 
        { get { return culture; } set { culture = value; } }

        private bool hasValidateValue = false;
        public bool GetHasValidateValue { get { return hasValidateValue; } }

        public LabelTextBox()
        {
            textToolTip.IsBalloon = true;
            textToolTip.ShowAlways = true;

            this.MouseDown += new MouseEventHandler(LabelTextBox_MouseDown);
            this.MouseLeave += new EventHandler(LabelTextBox_MouseLeave);
            this.KeyDown += new KeyEventHandler(LabelTextBox_KeyDown);
            this.KeyUp += new KeyEventHandler(LabelTextBox_KeyUp);
            this.Leave += new EventHandler(LabelTextBox_Leave);
            this.KeyPress += new KeyPressEventHandler(LabelTextBox_KeyPress);

            if (lblText != string.Empty)
            {
                this.ForeColor = lblColor;
                this.Text = lblText;
            }

            textToolTip.RemoveAll();
            textToolTip.SetToolTip(this, lblText);
        }

        void LabelTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+") && !okKeyChar)
                e.Handled = true;
        }

        void LabelTextBox_Leave(object sender, EventArgs e)
        {
            hasValidateValue = false;
            switch (this.Format)
            {
                //
                // Validate text for Email
                case TextBoxFormat.Email:
                    {
                        if (!this.Text.ValidateEmail())
                        {
                            this.ForeColor = lblColor;
                            this.Text = lblText;
                            hasValidateValue = false;
                        }
                        else
                        {
                            hasValidateValue = true;
                        }
                    } break;
                //
                // Validate text for URL
                case TextBoxFormat.URL:
                    {
                        if (!this.Text.ValidateURL())
                        {
                            this.ForeColor = lblColor;
                            this.Text = lblText;
                            hasValidateValue = false;
                        }
                        else
                        {
                            hasValidateValue = true;
                        }
                    } break;
                //
                // Validate text for Name
                case TextBoxFormat.ZipCode:
                    {
                        if (!this.Text.ValidateZipCode())
                        {
                            this.ForeColor = lblColor;
                            this.Text = lblText;
                            hasValidateValue = false;
                        }
                        else
                        {
                            hasValidateValue = true;
                        }
                    } break;
                //
                // Validate text for Password
                case TextBoxFormat.Password:
                    {
                        if (!this.Text.ValidatePassword())
                        {
                            this.ForeColor = lblColor;
                            this.Text = lblText;
                            hasValidateValue = false;
                        }
                        else
                        {
                            hasValidateValue = true;
                        }
                    } break;
                //
                // Validate text for SocialSecurityNumber
                case TextBoxFormat.SocialSecurityNumber:
                    {
                        if (!this.Text.ValidateSocialSecurityNumber())
                        {
                            this.ForeColor = lblColor;
                            this.Text = lblText;
                            hasValidateValue = false;
                        }
                        else
                        {
                            hasValidateValue = true;
                        }
                    } break;
                //
                // Validate text for PhoneNumber
                case TextBoxFormat.PhoneNumber:
                    {
                        if (!this.Text.ValidatePhoneNumber())
                        {
                            this.ForeColor = lblColor;
                            this.Text = lblText;
                            hasValidateValue = false;
                        }
                        else
                        {
                            hasValidateValue = true;
                        }
                    } break;
                //
                // Validate text for UCurrency
                case TextBoxFormat.UCurrency:
                    {
                        if (this.Text.ValidateUCurrency())
                        {
                            double num;
                            if (double.TryParse(this.Text, System.Globalization.NumberStyles.Currency | System.Globalization.NumberStyles.Number, culture, out num))
                            {
                                this.Text = num.ToString("C", culture);
                            }
                            hasValidateValue = true;
                        }
                        else hasValidateValue = false;
                    } break;
                //
                // Validate text for Currency
                case TextBoxFormat.Currency:
                    {
                        if (this.Text.ValidateCurrency())
                        {
                            double num;
                            if (double.TryParse(this.Text, System.Globalization.NumberStyles.Currency | System.Globalization.NumberStyles.Number, culture, out num))
                            {
                                this.Text = num.ToString("C", culture);
                            }
                            hasValidateValue = true;
                        }
                        else hasValidateValue = false;
                    } break;
                //
                // Validate text for Numeric (Double)
                case TextBoxFormat.Numeric:
                    {
                        if (!this.Text.ValidateNumeric())
                        {
                            this.ForeColor = lblColor;
                            this.Text = lblText;
                            hasValidateValue = false;
                        }
                        else
                        {
                            hasValidateValue = true;
                        }
                    } break;
                //
                // Validate text for Uinteger
                case TextBoxFormat.Uinteger:
                    {
                        if (!this.Text.ValidateUint())
                        {
                            this.ForeColor = lblColor;
                            this.Text = lblText;
                            hasValidateValue = false;
                        }
                        else
                        {
                            hasValidateValue = true;
                        }
                    } break;
                //
                // Text: any characters!
                default:
                    {
                        if (this.Text != this.LabelText)
                            hasValidateValue = true;
                        else hasValidateValue = false;
                    }break;
            }
        }

        void LabelTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.Text == string.Empty)
            {
                this.ForeColor = lblColor;
                this.Text = lblText;
            }
        }

        void LabelTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.Text == lblText && this.ForeColor == lblColor)
            {
                this.ForeColor = textColor;
                this.Text = string.Empty;
            }
            else this.ForeColor = textColor;

            if (format == TextBoxFormat.Currency || format == TextBoxFormat.Numeric || format == TextBoxFormat.Uinteger ||
                format == TextBoxFormat.ZipCode || format == TextBoxFormat.UCurrency || format == TextBoxFormat.SocialSecurityNumber ||
                format == TextBoxFormat.PhoneNumber)
            {
                if (e.KeyData == Keys.Delete || e.KeyData == Keys.Clear ||
                    e.KeyCode == Keys.OemMinus || e.KeyData == Keys.OemBackslash ||
                    e.KeyData == Keys.Back || e.KeyData == Keys.Decimal ||
                    e.KeyCode == Keys.Subtract)
                    okKeyChar = true;
                else okKeyChar = false;
            }
            else okKeyChar = true;
        }

        void LabelTextBox_MouseLeave(object sender, EventArgs e)
        {
            //
            // Validate text for Empty
            if (this.Text == string.Empty)
            {
                this.ForeColor = lblColor;
                this.Text = lblText;
            }
        }

        void LabelTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Text == lblText && this.ForeColor == lblColor)
            {
                this.ForeColor = textColor;
                this.Text = string.Empty;
            }
        }

        public void clearByLabelText()
        {
            this.Clear();
            this.ForeColor = lblColor;
            this.Text = lblText;
        }
    }

    /// <summary>
    ///  Specifies the formats used with text-related methods of the System.Windows.Forms.Clipboard
    ///  and System.Windows.Forms.DataObject classes.
    /// </summary>
    public enum TextBoxFormat
    {
        /// <summary>
        /// Validates for a positive or negative currency amount. 
        /// If there is a decimal point, 
        /// it requires 2 numeric characters after the decimal point.
        /// Example:    "-1.20" or "1.63"
        /// </summary>
        Currency,
        /// <summary>
        /// Validates an e-mail address.
        /// Example:    "someone@example.com"
        /// </summary>
        Email,
        /// <summary>
        /// Validates a name. Allows up to 40 uppercase and lowercase characters and 
        /// a few special characters that are common to some names. You can modify this list.
        /// Example:    "John Doe" or "O'Dell"
        /// </summary>
        Name,
        /// <summary>
        /// Validates that the field contains an double number
        /// Example:    "0" or "986" or "-36.59996"
        /// </summary>
        Numeric,
        /// <summary>
        ///Validates a strong password. It must be between 8 and 10 characters, 
        ///contain at least one digit and one alphabetic character, 
        ///and must not contain special characters.
        /// </summary>
        Password,
        /// <summary>
        /// Validates a U.S. phone number. It must consist of 3 numeric characters, 
        /// optionally enclosed in parentheses, followed by a set of 3 numeric characters
        /// and then a set of 4 numeric characters.
        /// Example:    "(425) 555-0123" or "425-555-0123" or "425 555 0123" or "1-425-555-0123"
        /// </summary>
        PhoneNumber,
        /// <summary>
        /// Validates the format, type, and length of the supplied input field. 
        /// The input must consist of 3 numeric characters followed by a dash, 
        /// then 2 numeric characters followed by a dash, and then 4 numeric characters.
        /// Example:    "111-11-1111"
        /// </summary>
        SocialSecurityNumber,
        /// <summary>
        /// Validate for any characters.
        /// Example:    "abc...123...~!@#$%^&"
        /// </summary>
        Text,
        /// <summary>
        ///  Validates a positive currency amount. 
        ///  If there is a decimal point, it requires 2 numeric characters after
        ///  the decimal point. For example, 3.00 is valid but 3.1 is not.
        ///  Example:   "1.00"
        /// </summary>
        UCurrency,
        /// <summary>
        /// Validates that the field contains an integer greater than zero.
        /// Example:    "0" or "986"
        /// </summary>
        Uinteger,
        /// <summary>
        /// Validates a URL
        /// Example:    "http://www.microsoft.com"
        /// </summary>
        URL,
        /// <summary>
        /// Validates a U.S. ZIP Code. The code must consist of 5 or 9 numeric characters.
        /// Example:    "12345"
        /// </summary>
        ZipCode
    }

    public static class MyExtensions
    {
        /// <summary>
        /// Validates an e-mail address.
        /// Example:    "someone@example.com"
        /// </summary>
        /// <param name="email">text variable</param>
        /// <returns>bool for correct or incorrect value</returns>
        public static bool ValidateEmail(this string email)
        {
            Regex exp = new Regex(@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
            return exp.IsMatch(email);
        }

        /// <summary>
        /// Validates a URL
        /// Example:    "http://www.microsoft.com"
        /// </summary>
        /// <param name="URL">text variable</param>
        /// <returns>bool for correct or incorrect value</returns>
        public static bool ValidateURL(this string URL)
        {
            string strRegex = "^(https?://)"
                + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //user@ 
                + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184 
                + "|" // allows either IP or domain 
                + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www. 
                + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\." // second level domain 
                + "[a-z]{2,6})" // first level domain- .com or .museum 
                + "(:[0-9]{1,4})?" // port number- :80 
                + "((/?)|" // a slash isn't required if there is no file name 
                + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";	

            Regex exp = new Regex(strRegex);
            return exp.IsMatch(URL);
        }

        /// <summary>
        ///  Validates a positive currency amount. 
        ///  If there is a decimal point, it requires 2 numeric characters after
        ///  the decimal point. For example, 3.00 is valid but 3.1 is not.
        ///  Example:   "1.00"
        /// </summary>
        /// <param name="currency">text variable</param>
        /// <returns>bool for correct or incorrect value</returns>
        public static bool ValidateUCurrency(this string currency)
        {
            Regex exp = new Regex(@"^\d+(\.\d\d)?$");
            return exp.IsMatch(currency);
        }

        /// <summary>
        /// Validates for a positive or negative currency amount. 
        /// If there is a decimal point, 
        /// it requires 2 numeric characters after the decimal point.
        /// Example:    "-1.20" or "1.63"
        /// </summary>
        /// <param name="currency">text variable</param>
        /// <returns>bool for correct or incorrect value</returns>
        public static bool ValidateCurrency(this string currency)
        {
            Regex exp = new Regex(@"^(-)?\d+(\.\d\d)?$");
            return exp.IsMatch(currency);
        }

        /// <summary>
        /// Validates a name. Allows up to 40 uppercase and lowercase characters and 
        /// a few special characters that are common to some names. You can modify this list.
        /// Example:    "John Doe" or "O'Dell"
        /// </summary>
        /// <param name="name">text variable</param>
        /// <returns>bool for correct or incorrect value</returns>
        public static bool ValidateName(this string name)
        {
            Regex exp = new Regex(@"^[a-zA-Z''-'\s]{1,40}$");
            return exp.IsMatch(name);
        }

        /// <summary>
        /// Validates the format, type, and length of the supplied input field. 
        /// The input must consist of 3 numeric characters followed by a dash, 
        /// then 2 numeric characters followed by a dash, and then 4 numeric characters.
        /// Example:    "111-11-1111"
        /// </summary>
        /// <param name="number">text variable</param>
        /// <returns>bool for correct or incorrect value</returns>
        public static bool ValidateSocialSecurityNumber(this string number)
        {
            Regex exp = new Regex(@"^\d{3}-\d{2}-\d{4}$");
            return exp.IsMatch(number);
        }

        /// <summary>
        /// Validates a U.S. phone number. It must consist of 3 numeric characters, 
        /// optionally enclosed in parentheses, followed by a set of 3 numeric characters
        /// and then a set of 4 numeric characters.
        /// Example:    "(425) 555-0123" or "425-555-0123" or "425 555 0123" or "1-425-555-0123"
        /// </summary>
        /// <param name="number">text variable</param>
        /// <returns>bool for correct or incorrect value</returns>
        public static bool ValidatePhoneNumber(this string number)
        {
            Regex exp = new Regex(@"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$");
            return exp.IsMatch(number);
        }

        /// <summary>
        /// Validates a U.S. ZIP Code. The code must consist of 5 or 9 numeric characters.
        /// Example:    "12345"
        /// </summary>
        /// <param name="number">text variable</param>
        /// <returns>bool for correct or incorrect value</returns>
        public static bool ValidateZipCode(this string number)
        {
            Regex exp = new Regex(@"^(\d{5}-\d{4}|\d{5}-\d{5}|\d{5}|\d{9}|\d{10})$|^([a-zA-Z]\d[a-zA-Z] \d[a-zA-Z]\d)$");
            return exp.IsMatch(number);
        }

        /// <summary>
        ///Validates a strong password. It must be between 8 and 10 characters, 
        ///contain at least one digit and one alphabetic character, 
        ///and must not contain special characters.
        /// </summary>
        /// <param name="pass">text variable</param>
        /// <returns>bool for correct or incorrect value</returns>
        public static bool ValidatePassword(this string pass)
        {
            Regex exp = new Regex(@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8,10})$");
            return exp.IsMatch(pass);
        }

        /// <summary>
        /// Validates that the field contains an integer greater than zero.
        /// Example:    "0" or "986"
        /// </summary>
        /// <param name="unumeric">text variable</param>
        /// <returns>bool for correct or incorrect value</returns>
        public static bool ValidateUint(this string unumeric)
        {
            Regex exp = new Regex(@"^\d+$");
            return exp.IsMatch(unumeric);
        }

        /// <summary>
        /// Validates that the field contains an double number
        /// Example:    "0" or "986" or "-36.59996"
        /// </summary>
        /// <param name="unumeric">text variable</param>
        /// <returns>bool for correct or incorrect value</returns>
        public static bool ValidateNumeric(this string numeric)
        {
            Regex exp = new Regex(@"^(0|(-(((0|[1-9]\d*)\.\d+)|([1-9]\d*))))$");
            return exp.IsMatch(numeric);
        }
    }
}
