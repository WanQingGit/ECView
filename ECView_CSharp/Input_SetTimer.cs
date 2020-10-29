namespace ECView_CSharp
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public class Input_SetTimer : Form
    {
        private Button button1;
        private Button button2;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private NumericUpDown numericHdd;
        private NumericUpDown numericFan;
        public Input_SetTimer()
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
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericHdd = new System.Windows.Forms.NumericUpDown();
            this.numericFan = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericHdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFan)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "time interval for refresh fan info:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(50, 92);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 21);
            this.button1.TabIndex = 6;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(169, 92);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(113, 21);
            this.button2.TabIndex = 7;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(288, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 24);
            this.label2.TabIndex = 9;
            this.label2.Text = "sec.";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(288, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 24);
            this.label3.TabIndex = 12;
            this.label3.Text = "sec.";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(240, 24);
            this.label4.TabIndex = 10;
            this.label4.Text = "time interval for refresh hdd:";
            // 
            // numericHdd
            // 
            this.numericHdd.DecimalPlaces = 1;
            this.numericHdd.Location = new System.Drawing.Point(240, 53);
            this.numericHdd.Name = "numericHdd";
            this.numericHdd.Size = new System.Drawing.Size(42, 21);
            this.numericHdd.TabIndex = 13;
            this.numericHdd.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numericFan
            // 
            this.numericFan.DecimalPlaces = 1;
            this.numericFan.Location = new System.Drawing.Point(240, 19);
            this.numericFan.Name = "numericFan";
            this.numericFan.Size = new System.Drawing.Size(42, 21);
            this.numericFan.TabIndex = 14;
            this.numericFan.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // Input_SetTimer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 126);
            this.Controls.Add(this.numericFan);
            this.Controls.Add(this.numericHdd);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "Input_SetTimer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Timer setting";
            ((System.ComponentModel.ISupportInitialize)(this.numericHdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFan)).EndInit();
            this.ResumeLayout(false);

        }

        protected override void WndProc(ref Message m)
        {
            if ((m.Msg == 0x112) && (((int) m.WParam) == 0xf060))
            {
                if (MessageBox.Show("确定这个视窗关闭", "关闭Timer设定讯息!!", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
                base.Close();
            }
            base.WndProc(ref m);
        }

        public decimal InterValFan
        {
            get {return this.numericFan.Value; }
            set
            {
                this.numericFan.Value = value;
            }
        }
        public decimal InterValHdd
        {
            get { return this.numericHdd.Value; }
            set
            {
                this.numericHdd.Value = value;
            }
        }
    }
}

