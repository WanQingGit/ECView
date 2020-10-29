namespace ECView_CSharp
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public class Input_FanCount : Form
    {
        private Button button1;
        private Button button2;
        private ComboBox comboBox1;
  //      private IContainer components;
        private Label label1;

        public Input_FanCount()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
        }

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.button1 = new Button();
            this.button2 = new Button();
            this.comboBox1 = new ComboBox();
            base.SuspendLayout();
            this.label1.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label1.Location = new Point(10, 0x13);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0xb5, 0x16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please setting fan count:";
            this.button1.Location = new Point(12, 0x3d);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x71, 0x17);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.button2.Location = new Point(0x85, 0x3d);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x71, 0x17);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new EventHandler(this.button2_Click);
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] { "1", "2", "3", "4" });
            this.comboBox1.Location = new Point(0xc5, 0x16);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new Size(0x2a, 0x15);
            this.comboBox1.TabIndex = 7;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x101, 0x5e);
            base.Controls.Add(this.comboBox1);
            base.Controls.Add(this.button2);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.label1);
            base.Name = "Input_FanCount";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Fan Count";
            base.ResumeLayout(false);
        }

        const int WM_SYSCOMMAND = 0x112;
        const int SC_CLOSE = 0xF060;
        const int SC_MINIMIZE = 0xF020;
        const int SC_MAXIMIZE = 0xF030;
        protected override void WndProc(ref Message m)
        {

            if ((m.Msg == WM_SYSCOMMAND) && (((int) m.WParam) == SC_CLOSE))
            {
                if (MessageBox.Show("确定这个视窗关闭", "关闭Timer设定讯息!", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
                base.Close();
            }
            base.WndProc(ref m);
        }
        public int FanCount
        {
            get
            {
                return this.comboBox1.SelectedIndex+1;
            }
            set
            {
                this.comboBox1.SelectedIndex = Convert.ToInt16(value) - 1;
            }
        }
    }
}

