namespace ECView_CSharp
{
    using Microsoft.VisualBasic.FileIO;
    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Management;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    public class Form1 : Form
    {
        private bool bInitHddInfo;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private CheckBox checkBox4;
        private IContainer components;
        public int FAN_Amount=3;
        private ToolStripMenuItem fanToolStripMenuItem;
        private GroupBox gbFan1;
        private GroupBox gbFan2;
        private GroupBox gbFan3;
        private GroupBox gbDisk1;
        private GroupBox gbDisk2;
        private GroupBox gbDisk3;
        private GroupBox groupBox7;
        private GroupBox gbFan4;
        private GroupBox groupBox9;
        public int HDDInstance;
        private ToolStripMenuItem hddStripMenuItem;
        public int HDDTemp;
        public int hours;
        private Label labCPULocal;
        private Label labCPURemote;
        private Label labCPURPM;
        private Label LabECVersion;
        private Label label1;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label16;
        private Label label2;
        private Label label20;
        private Label label21;
        private Label label22;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label labGPU1RPM;
        private Label labGPURPM;
        private Label LabHDD1Model;
        private Label labHDD1Temp;
        private Label labHDD1UseHours;
        private Label LabHDD2Model;
        private Label labHDD2Temp;
        private Label labHDD2UseHours;
        private Label LabHDD3Model;
        private Label labHDD3Temp;
        private Label labHDD3UseHours;
        private Label labLock;
        private Label LabNBModel;
        private Label labRecord;
        private Label labSYSLocal;
        private Label labSYSRemote;
        private Label labSYSRPM;
        private Label labVGA1Local;
        private Label labVGA1Remote;
        private Label labVGA2Local;
        private Label labVGA2Remote;
        public bool Lock;
        public bool LockHDD;
        public bool LockRPM;
        private ToolStripMenuItem lockToolStripMenuItem;
        private RegistryKey LSPKey;
        private MenuStrip menuStrip1;
        private NTPort ntPort;
        private bool ntPortSucess;
        private NumericUpDown numericUpDown1;
        private NumericUpDown numericUpDown2;
        private NumericUpDown numericUpDown3;
        private NumericUpDown numericUpDown4;
        public bool RAID;
        public bool Rec;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ManagementObjectSearcher Searcher_BaseBoard = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_BaseBoard");
        public string[,] services;
        private ToolStripMenuItem settingToolStripMenuItem;
        private System.Windows.Forms.Timer timer_hdd;
        private System.Windows.Forms.Timer timer_fan;
        private ToolStripMenuItem timerToolStripMenuItem;
        public int IntervalFan
        {
            get { return timer_fan.Interval; }
            set { timer_fan.Interval = value; }
        }
        public int IntervalHdd
        {
            get { return timer_hdd.Interval; }
            set { timer_hdd.Interval = value; }
        }
        private GroupBox gbDisk4;
        private Label label23;
        private Label LabHDD4Model;
        private Label labHDD4Temp;
        public const int WM_CLOSE = 0x10;
        public int height_fan;
        public int height_ori;
        public Form1()
        {
            this.InitializeComponent();
            this.settingToolStripMenuItem.Enabled = false;
        }

        private void initValues()
        {
           CheckBox[] cbs = { null,checkBox1, checkBox2, checkBox3,checkBox4 };
            NumericUpDown[] nudSpeeds = { null,numericUpDown1, numericUpDown2, numericUpDown3,numericUpDown4 };
            bool[] FAN_Autols = { true,true, true, true };
            this.cbs = cbs;
            this.nudSpeeds = nudSpeeds;
            this.FAN_Autols = FAN_Autols;
            GroupBox[] gbDisks = {gbDisk1,gbDisk2,gbDisk3,gbDisk4 };
            Label[] labDiskModels = { LabHDD1Model, LabHDD2Model, LabHDD3Model, LabHDD4Model };
            Label[] labDiskTemps = {labHDD1Temp, labHDD2Temp, labHDD3Temp, labHDD4Temp };
            Label[] labelRemotes = { null, labCPURemote, labVGA1Remote, labVGA2Remote, labSYSRemote };
            Label[] labelLocals = { null, labCPULocal, labVGA1Local, labVGA2Local, labSYSLocal };
            Label[] labelRPMs = { null, labCPURPM, labGPURPM, labGPU1RPM, labSYSRPM };
            this.labelRemotes = labelRemotes;
            this.labelLocals = labelLocals;
            this.labelRPMs = labelRPMs;
            this.gbDisks = gbDisks;
            this.labDiskModels = labDiskModels;
            this.labDiskTemps = labDiskTemps;
        }
        GroupBox[] gbDisks;
        Label[] labelRemotes, labDiskTemps, labelRPMs, labelLocals, labDiskModels;
        CheckBox[] cbs;
        NumericUpDown[] nudSpeeds;
        bool[] FAN_Autols;
        int[] Fan_rpms = new int[5];
        double[] fl_rpms = new double[5];
        public decimal[] de_rpms = new decimal[5];
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            String name = ((CheckBox)sender).Name;
            int index = int.Parse(name.Substring(name.Length - 1));
            if (cbs[index].Checked)
            {
                this.ntPort.Set_FAN_Duty_Auto(index);
                this.nudSpeeds[index].Enabled = false;
                this.FAN_Autols[index] = true;
            }
            else
            {
                this.nudSpeeds[index].Enabled = true;
                this.nudSpeeds[index].ValueChanged += delegate (Object o, EventArgs v) { numericUpDown_ValueChanged(index); };
            //    this.nudSpeeds[index].ValueChanged += new EventHandler(this.numericUpDown1_ValueChanged);
                this.FAN_Autols[index] = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void fanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Input_FanCount count = new Input_FanCount {
                FanCount = this.FAN_Amount
            };
            if (count.ShowDialog() == DialogResult.OK)
            {
                this.FAN_Amount = count.FanCount;
            }
            initGb();
           
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        private void FixUACSettings()
        {
            try
            {
                using (this.LSPKey = Registry.LocalMachine)
                {
                    this.LSPKey = this.LSPKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true);
                    this.LSPKey.SetValue("EnableLUA", 0);
                    this.LSPKey.SetValue("ConsentPromptBehaviorAdmin", 0);
                    this.LSPKey.SetValue("ConsentPromptBehaviorUser", 1);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
            finally
            {
                this.LSPKey.Close();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.timer_fan.Dispose();
            this.timer_hdd.Dispose();
            if (this.ntPortSucess)
            {
                this.ntPort.Set_FAN_Duty_Auto(4);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("height:" + this.Height);
            this.height_fan = this.groupBox9.Location.Y+45;
            this.height_ori=this.Height;
            this.Height = this.height_fan;
            groupBox9.Visible = false;
            initGb();
            initValues();
            this.services = new string[4, 4];
            this.settingToolStripMenuItem.Enabled = true;
            this.HDDInstance = 0;
            this.ntPortSucess = false;
            this.bInitHddInfo = false;
            try
            {
                this.ntPort = new NTPort();
                this.ntPortSucess = true;
            }
            catch
            {
                MessageBox.Show("Initialize Fail", "Error", MessageBoxButtons.OK);
                this.ntPortSucess = false;
                return;
            }
            this.Lock = false;
            this.LockHDD = true;
            foreach (ManagementObject obj2 in this.Searcher_BaseBoard.Get())
            {
                this.LabNBModel.Text = Convert.ToString(obj2["Product"]);
            }
            this.LabECVersion.Text = "1." + this.ntPort.g_strEcVersion;
            
            this.timer_fan.Enabled = true;
            
            this.Rec = false;
            saveToolStripMenuItem_Click(null, null);
            
            for (int i = 1; i < cbs.Length; i++)
            {
                cbs[i].Checked = true;
                nudSpeeds[i].Enabled = false;
                cbs[i].CheckedChanged += new EventHandler(this.checkBox_CheckedChanged);
            }
            checkFanSet();
            this.timer_hdd.Enabled = this.hddStripMenuItem.Checked;
            this.timer_fan.Enabled = true;
        }

        [DllImport("SmartDisk.dll", EntryPoint="#2", CharSet=CharSet.Auto)]
        public static extern int GetHDDInstance();
        [DllImport("SmartDisk.dll", EntryPoint="#5", CharSet=CharSet.Auto)]
        public static extern bool GetHddSize(int devicePort, out int hddsize);
        [DllImport("SmartDisk.dll", EntryPoint="#4", CharSet=CharSet.Auto)]
        public static extern bool GetModelName(int devicePort, ref string name);
        [DllImport("SmartDisk.dll", EntryPoint="#3", CharSet=CharSet.Auto)]
        public static extern bool GetTemperture(int devicePort, out int temperature);
        public void d(double v) { /**Console.Write(v + " ");**/ }
        
        private void hddStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.Write("start...");
            try
            {
            bool isShowDisk = this.hddStripMenuItem.Checked;
                this.timer_hdd.Enabled = isShowDisk;
            if (isShowDisk && !this.RAID)
            {
                this.Height = this.height_ori;
                this.bInitHddInfo = InitDevice();
                if (!this.bInitHddInfo)
                {
                    isShowDisk=this.hddStripMenuItem.Checked = false;
                    MessageBox.Show("Initialize Fail", "Error", MessageBoxButtons.OK);
                }
                else
                {
                    
                    this.HDDInstance = GetHDDInstance();
                    string name = "";
                    int hddsize = 0;
                    int temperature = 0;
                    for (int i = 0; i < this.HDDInstance; i++)
                    {
                        
                        this.services[i, 0] = Convert.ToString(i);
                        GetModelName(i, ref name);
                        this.services[i, 1] = name;
                        GetHddSize(i, out hddsize);
                        this.services[i, 2] = Convert.ToString((int) (hddsize / 0x3e8));
                        GetTemperture(i, out temperature);d(7);
                        this.services[i, 3] = Convert.ToString(temperature);
                        checkDisk(i);d(9);
                       
                      
                    }
                }
            }
            else
            {
               this.Height = this.height_fan;
               
            }
            this.bInitHddInfo = isShowDisk;
            this.LockHDD = !isShowDisk;
            this.groupBox9.Visible = isShowDisk;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Console.WriteLine("end");
        }

        private void checkDisk(int id)
        {
            if (this.services[id, 0] != null)
            {
                this.gbDisks[id].Text = this.services[id, 2] + "GB";
                this.labDiskModels[id].Text = this.services[id, 1]  ;
                this.labDiskTemps[id].Text = Convert.ToString(this.services[id, 3]);
                this.gbDisks[id].Visible = true;
                this.gbDisks[id].Enabled = true;
            }
        }
        //[DllImport("SmartDisc.dll", EntryPoint="GetLastState", CharSet=CharSet.Auto)]
        [DllImport("SmartDisk.dll", EntryPoint="#1", CharSet=CharSet.Auto)]
        public static extern bool InitDevice();
//        ComponentResourceManager manager;
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hddStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.LabNBModel = new System.Windows.Forms.Label();
            this.gbFan1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labCPURPM = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labCPULocal = new System.Windows.Forms.Label();
            this.labCPURemote = new System.Windows.Forms.Label();
            this.gbFan2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.labGPURPM = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labVGA1Local = new System.Windows.Forms.Label();
            this.labVGA1Remote = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.gbFan3 = new System.Windows.Forms.GroupBox();
            this.labGPU1RPM = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labVGA2Local = new System.Windows.Forms.Label();
            this.labVGA2Remote = new System.Windows.Forms.Label();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.timer_fan = new System.Windows.Forms.Timer(this.components);
            this.label20 = new System.Windows.Forms.Label();
            this.LabECVersion = new System.Windows.Forms.Label();
            this.labRecord = new System.Windows.Forms.Label();
            this.labLock = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.gbFan4 = new System.Windows.Forms.GroupBox();
            this.labSYSRPM = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.labSYSLocal = new System.Windows.Forms.Label();
            this.labSYSRemote = new System.Windows.Forms.Label();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.timer_hdd = new System.Windows.Forms.Timer(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.gbDisk4 = new System.Windows.Forms.GroupBox();
            this.label23 = new System.Windows.Forms.Label();
            this.LabHDD4Model = new System.Windows.Forms.Label();
            this.labHDD4Temp = new System.Windows.Forms.Label();
            this.gbDisk3 = new System.Windows.Forms.GroupBox();
            this.labHDD3UseHours = new System.Windows.Forms.Label();
            this.LabHDD3Model = new System.Windows.Forms.Label();
            this.labHDD3Temp = new System.Windows.Forms.Label();
            this.gbDisk2 = new System.Windows.Forms.GroupBox();
            this.labHDD2UseHours = new System.Windows.Forms.Label();
            this.LabHDD2Model = new System.Windows.Forms.Label();
            this.labHDD2Temp = new System.Windows.Forms.Label();
            this.gbDisk1 = new System.Windows.Forms.GroupBox();
            this.labHDD1UseHours = new System.Windows.Forms.Label();
            this.LabHDD1Model = new System.Windows.Forms.Label();
            this.labHDD1Temp = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.gbFan1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.gbFan2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.gbFan3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.groupBox7.SuspendLayout();
            this.gbFan4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            this.groupBox9.SuspendLayout();
            this.gbDisk4.SuspendLayout();
            this.gbDisk3.SuspendLayout();
            this.gbDisk2.SuspendLayout();
            this.gbDisk1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(470, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingToolStripMenuItem
            // 
            this.settingToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.Grip;
            this.settingToolStripMenuItem.Checked = true;
            this.settingToolStripMenuItem.CheckOnClick = true;
            this.settingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timerToolStripMenuItem,
            this.fanToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.lockToolStripMenuItem,
            this.hddStripMenuItem});
            this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
            this.settingToolStripMenuItem.Size = new System.Drawing.Size(60, 21);
            this.settingToolStripMenuItem.Text = "Setting";
            // 
            // timerToolStripMenuItem
            // 
            this.timerToolStripMenuItem.Name = "timerToolStripMenuItem";
            this.timerToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.timerToolStripMenuItem.Text = "Timer";
            this.timerToolStripMenuItem.Click += new System.EventHandler(this.timerToolStripMenuItem_Click);
            // 
            // fanToolStripMenuItem
            // 
            this.fanToolStripMenuItem.Name = "fanToolStripMenuItem";
            this.fanToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.fanToolStripMenuItem.Text = "Fan Amount";
            this.fanToolStripMenuItem.Click += new System.EventHandler(this.fanToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.CheckOnClick = true;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // lockToolStripMenuItem
            // 
            this.lockToolStripMenuItem.CheckOnClick = true;
            this.lockToolStripMenuItem.Name = "lockToolStripMenuItem";
            this.lockToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.lockToolStripMenuItem.Text = "Lock";
            this.lockToolStripMenuItem.Click += new System.EventHandler(this.lockToolStripMenuItem_Click);
            // 
            // hddStripMenuItem
            // 
            this.hddStripMenuItem.CheckOnClick = true;
            this.hddStripMenuItem.Name = "hddStripMenuItem";
            this.hddStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.hddStripMenuItem.Text = "HDD Information";
            this.hddStripMenuItem.Click += new System.EventHandler(this.hddStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label1.Location = new System.Drawing.Point(152, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Model :";
            // 
            // LabNBModel
            // 
            this.LabNBModel.AutoSize = true;
            this.LabNBModel.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabNBModel.ForeColor = System.Drawing.Color.Red;
            this.LabNBModel.Location = new System.Drawing.Point(209, 5);
            this.LabNBModel.Name = "LabNBModel";
            this.LabNBModel.Size = new System.Drawing.Size(27, 15);
            this.LabNBModel.TabIndex = 2;
            this.LabNBModel.Text = "-----";
            // 
            // gbFan1
            // 
            this.gbFan1.Controls.Add(this.label2);
            this.gbFan1.Controls.Add(this.labCPURPM);
            this.gbFan1.Controls.Add(this.numericUpDown1);
            this.gbFan1.Controls.Add(this.checkBox1);
            this.gbFan1.Controls.Add(this.label8);
            this.gbFan1.Controls.Add(this.label3);
            this.gbFan1.Controls.Add(this.labCPULocal);
            this.gbFan1.Controls.Add(this.labCPURemote);
            this.gbFan1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFan1.Location = new System.Drawing.Point(10, 16);
            this.gbFan1.Name = "gbFan1";
            this.gbFan1.Size = new System.Drawing.Size(105, 123);
            this.gbFan1.TabIndex = 3;
            this.gbFan1.TabStop = false;
            this.gbFan1.Text = "CPU";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 15);
            this.label2.TabIndex = 16;
            this.label2.Text = "RPM";
            // 
            // labCPURPM
            // 
            this.labCPURPM.AutoSize = true;
            this.labCPURPM.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labCPURPM.Location = new System.Drawing.Point(55, 70);
            this.labCPURPM.Name = "labCPURPM";
            this.labCPURPM.Size = new System.Drawing.Size(19, 15);
            this.labCPURPM.TabIndex = 15;
            this.labCPURPM.Text = "---";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Enabled = false;
            this.numericUpDown1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1.Location = new System.Drawing.Point(52, 90);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(44, 22);
            this.numericUpDown1.TabIndex = 3;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.Location = new System.Drawing.Point(4, 93);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(47, 19);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "auto";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(55, 47);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 14);
            this.label8.TabIndex = 8;
            this.label8.Text = "Local";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 14);
            this.label3.TabIndex = 1;
            this.label3.Text = "Remote";
            // 
            // labCPULocal
            // 
            this.labCPULocal.AutoSize = true;
            this.labCPULocal.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labCPULocal.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labCPULocal.Location = new System.Drawing.Point(52, 17);
            this.labCPULocal.Name = "labCPULocal";
            this.labCPULocal.Size = new System.Drawing.Size(36, 24);
            this.labCPULocal.TabIndex = 7;
            this.labCPULocal.Text = "00";
            // 
            // labCPURemote
            // 
            this.labCPURemote.AutoSize = true;
            this.labCPURemote.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labCPURemote.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labCPURemote.Location = new System.Drawing.Point(6, 17);
            this.labCPURemote.Name = "labCPURemote";
            this.labCPURemote.Size = new System.Drawing.Size(36, 24);
            this.labCPURemote.TabIndex = 0;
            this.labCPURemote.Text = "00";
            // 
            // gbFan2
            // 
            this.gbFan2.Controls.Add(this.label5);
            this.gbFan2.Controls.Add(this.labGPURPM);
            this.gbFan2.Controls.Add(this.numericUpDown2);
            this.gbFan2.Controls.Add(this.label16);
            this.gbFan2.Controls.Add(this.label10);
            this.gbFan2.Controls.Add(this.label4);
            this.gbFan2.Controls.Add(this.labVGA1Local);
            this.gbFan2.Controls.Add(this.labVGA1Remote);
            this.gbFan2.Controls.Add(this.checkBox2);
            this.gbFan2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFan2.Location = new System.Drawing.Point(122, 16);
            this.gbFan2.Name = "gbFan2";
            this.gbFan2.Size = new System.Drawing.Size(105, 123);
            this.gbFan2.TabIndex = 4;
            this.gbFan2.TabStop = false;
            this.gbFan2.Text = "VGA 1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(7, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 15);
            this.label5.TabIndex = 22;
            this.label5.Text = "RPM";
            // 
            // labGPURPM
            // 
            this.labGPURPM.AutoSize = true;
            this.labGPURPM.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labGPURPM.Location = new System.Drawing.Point(59, 70);
            this.labGPURPM.Name = "labGPURPM";
            this.labGPURPM.Size = new System.Drawing.Size(19, 15);
            this.labGPURPM.TabIndex = 21;
            this.labGPURPM.Text = "---";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Enabled = false;
            this.numericUpDown2.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown2.Location = new System.Drawing.Point(53, 88);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(44, 22);
            this.numericUpDown2.TabIndex = 11;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(110, 115);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(25, 19);
            this.label16.TabIndex = 20;
            this.label16.Text = "%";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(55, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(33, 14);
            this.label10.TabIndex = 10;
            this.label10.Text = "Local";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 14);
            this.label4.TabIndex = 8;
            this.label4.Text = "Remote";
            // 
            // labVGA1Local
            // 
            this.labVGA1Local.AutoSize = true;
            this.labVGA1Local.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labVGA1Local.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labVGA1Local.Location = new System.Drawing.Point(54, 17);
            this.labVGA1Local.Name = "labVGA1Local";
            this.labVGA1Local.Size = new System.Drawing.Size(36, 24);
            this.labVGA1Local.TabIndex = 9;
            this.labVGA1Local.Text = "00";
            // 
            // labVGA1Remote
            // 
            this.labVGA1Remote.AutoSize = true;
            this.labVGA1Remote.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labVGA1Remote.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labVGA1Remote.Location = new System.Drawing.Point(8, 17);
            this.labVGA1Remote.Name = "labVGA1Remote";
            this.labVGA1Remote.Size = new System.Drawing.Size(36, 24);
            this.labVGA1Remote.TabIndex = 7;
            this.labVGA1Remote.Text = "00";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Enabled = false;
            this.checkBox2.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox2.Location = new System.Drawing.Point(6, 91);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(47, 19);
            this.checkBox2.TabIndex = 16;
            this.checkBox2.Text = "auto";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // gbFan3
            // 
            this.gbFan3.Controls.Add(this.labGPU1RPM);
            this.gbFan3.Controls.Add(this.label7);
            this.gbFan3.Controls.Add(this.numericUpDown3);
            this.gbFan3.Controls.Add(this.label12);
            this.gbFan3.Controls.Add(this.label6);
            this.gbFan3.Controls.Add(this.labVGA2Local);
            this.gbFan3.Controls.Add(this.labVGA2Remote);
            this.gbFan3.Controls.Add(this.checkBox3);
            this.gbFan3.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFan3.Location = new System.Drawing.Point(234, 16);
            this.gbFan3.Name = "gbFan3";
            this.gbFan3.Size = new System.Drawing.Size(105, 123);
            this.gbFan3.TabIndex = 5;
            this.gbFan3.TabStop = false;
            this.gbFan3.Text = "VGA 2";
            // 
            // labGPU1RPM
            // 
            this.labGPU1RPM.AutoSize = true;
            this.labGPU1RPM.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labGPU1RPM.Location = new System.Drawing.Point(52, 70);
            this.labGPU1RPM.Name = "labGPU1RPM";
            this.labGPU1RPM.Size = new System.Drawing.Size(19, 15);
            this.labGPU1RPM.TabIndex = 23;
            this.labGPU1RPM.Text = "---";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(7, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 15);
            this.label7.TabIndex = 23;
            this.label7.Text = "RPM";
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Enabled = false;
            this.numericUpDown3.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown3.Location = new System.Drawing.Point(54, 90);
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(44, 22);
            this.numericUpDown3.TabIndex = 11;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(52, 47);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(33, 14);
            this.label12.TabIndex = 10;
            this.label12.Text = "Local";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 14);
            this.label6.TabIndex = 8;
            this.label6.Text = "Remote";
            // 
            // labVGA2Local
            // 
            this.labVGA2Local.AutoSize = true;
            this.labVGA2Local.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labVGA2Local.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labVGA2Local.Location = new System.Drawing.Point(49, 17);
            this.labVGA2Local.Name = "labVGA2Local";
            this.labVGA2Local.Size = new System.Drawing.Size(36, 24);
            this.labVGA2Local.TabIndex = 9;
            this.labVGA2Local.Text = "00";
            // 
            // labVGA2Remote
            // 
            this.labVGA2Remote.AutoSize = true;
            this.labVGA2Remote.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labVGA2Remote.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labVGA2Remote.Location = new System.Drawing.Point(8, 17);
            this.labVGA2Remote.Name = "labVGA2Remote";
            this.labVGA2Remote.Size = new System.Drawing.Size(36, 24);
            this.labVGA2Remote.TabIndex = 7;
            this.labVGA2Remote.Text = "00";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Enabled = false;
            this.checkBox3.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox3.Location = new System.Drawing.Point(6, 93);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(47, 19);
            this.checkBox3.TabIndex = 16;
            this.checkBox3.Text = "auto";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // timer_fan
            // 
            this.timer_fan.Interval = 4000;
            this.timer_fan.Tick += new System.EventHandler(this.timerFan_Tick);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label20.Location = new System.Drawing.Point(339, 5);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(71, 15);
            this.label20.TabIndex = 6;
            this.label20.Text = "EC Version :";
            // 
            // LabECVersion
            // 
            this.LabECVersion.AutoSize = true;
            this.LabECVersion.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabECVersion.ForeColor = System.Drawing.Color.Red;
            this.LabECVersion.Location = new System.Drawing.Point(409, 5);
            this.LabECVersion.Name = "LabECVersion";
            this.LabECVersion.Size = new System.Drawing.Size(27, 15);
            this.LabECVersion.TabIndex = 7;
            this.LabECVersion.Text = "-----";
            // 
            // labRecord
            // 
            this.labRecord.Location = new System.Drawing.Point(109, 7);
            this.labRecord.Name = "labRecord";
            this.labRecord.Size = new System.Drawing.Size(42, 13);
            this.labRecord.TabIndex = 11;
            this.labRecord.Text = "AP initialization...";
            // 
            // labLock
            // 
            this.labLock.AutoSize = true;
            this.labLock.ForeColor = System.Drawing.Color.Red;
            this.labLock.Location = new System.Drawing.Point(109, 32);
            this.labLock.Name = "labLock";
            this.labLock.Size = new System.Drawing.Size(0, 12);
            this.labLock.TabIndex = 12;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.gbFan4);
            this.groupBox7.Controls.Add(this.gbFan1);
            this.groupBox7.Controls.Add(this.gbFan2);
            this.groupBox7.Controls.Add(this.gbFan3);
            this.groupBox7.Location = new System.Drawing.Point(5, 30);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(459, 146);
            this.groupBox7.TabIndex = 13;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Fan:";
            // 
            // gbFan4
            // 
            this.gbFan4.Controls.Add(this.labSYSRPM);
            this.gbFan4.Controls.Add(this.label11);
            this.gbFan4.Controls.Add(this.numericUpDown4);
            this.gbFan4.Controls.Add(this.label21);
            this.gbFan4.Controls.Add(this.label22);
            this.gbFan4.Controls.Add(this.labSYSLocal);
            this.gbFan4.Controls.Add(this.labSYSRemote);
            this.gbFan4.Controls.Add(this.checkBox4);
            this.gbFan4.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFan4.Location = new System.Drawing.Point(345, 16);
            this.gbFan4.Name = "gbFan4";
            this.gbFan4.Size = new System.Drawing.Size(105, 123);
            this.gbFan4.TabIndex = 24;
            this.gbFan4.TabStop = false;
            this.gbFan4.Text = "SYS";
            // 
            // labSYSRPM
            // 
            this.labSYSRPM.AutoSize = true;
            this.labSYSRPM.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labSYSRPM.Location = new System.Drawing.Point(53, 70);
            this.labSYSRPM.Name = "labSYSRPM";
            this.labSYSRPM.Size = new System.Drawing.Size(19, 15);
            this.labSYSRPM.TabIndex = 23;
            this.labSYSRPM.Text = "---";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(8, 70);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 15);
            this.label11.TabIndex = 23;
            this.label11.Text = "RPM";
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.Enabled = false;
            this.numericUpDown4.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown4.Location = new System.Drawing.Point(55, 90);
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(44, 22);
            this.numericUpDown4.TabIndex = 11;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(53, 47);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(33, 14);
            this.label21.TabIndex = 10;
            this.label21.Text = "Local";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(6, 47);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(43, 14);
            this.label22.TabIndex = 8;
            this.label22.Text = "Remote";
            // 
            // labSYSLocal
            // 
            this.labSYSLocal.AutoSize = true;
            this.labSYSLocal.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labSYSLocal.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labSYSLocal.Location = new System.Drawing.Point(52, 17);
            this.labSYSLocal.Name = "labSYSLocal";
            this.labSYSLocal.Size = new System.Drawing.Size(28, 24);
            this.labSYSLocal.TabIndex = 9;
            this.labSYSLocal.Text = "--";
            // 
            // labSYSRemote
            // 
            this.labSYSRemote.AutoSize = true;
            this.labSYSRemote.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labSYSRemote.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labSYSRemote.Location = new System.Drawing.Point(9, 17);
            this.labSYSRemote.Name = "labSYSRemote";
            this.labSYSRemote.Size = new System.Drawing.Size(28, 24);
            this.labSYSRemote.TabIndex = 7;
            this.labSYSRemote.Text = "--";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Enabled = false;
            this.checkBox4.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox4.Location = new System.Drawing.Point(11, 93);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(47, 19);
            this.checkBox4.TabIndex = 16;
            this.checkBox4.Text = "auto";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // timer_hdd
            // 
            this.timer_hdd.Interval = 6000;
            this.timer_hdd.Tick += new System.EventHandler(this.timer_hdd_tick);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label9.Location = new System.Drawing.Point(78, 5);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 15);
            this.label9.TabIndex = 14;
            this.label9.Text = "Log:";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.gbDisk4);
            this.groupBox9.Controls.Add(this.gbDisk3);
            this.groupBox9.Controls.Add(this.gbDisk2);
            this.groupBox9.Controls.Add(this.gbDisk1);
            this.groupBox9.Location = new System.Drawing.Point(5, 175);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(459, 89);
            this.groupBox9.TabIndex = 15;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "HDD:";
            // 
            // gbDisk4
            // 
            this.gbDisk4.Controls.Add(this.label23);
            this.gbDisk4.Controls.Add(this.LabHDD4Model);
            this.gbDisk4.Controls.Add(this.labHDD4Temp);
            this.gbDisk4.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbDisk4.Location = new System.Drawing.Point(345, 10);
            this.gbDisk4.Name = "gbDisk4";
            this.gbDisk4.Padding = new System.Windows.Forms.Padding(0);
            this.gbDisk4.Size = new System.Drawing.Size(105, 72);
            this.gbDisk4.TabIndex = 14;
            this.gbDisk4.TabStop = false;
            this.gbDisk4.Visible = false;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(60, 65);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(0, 15);
            this.label23.TabIndex = 2;
            // 
            // LabHDD4Model
            // 
            this.LabHDD4Model.Font = new System.Drawing.Font("Times New Roman", 8F);
            this.LabHDD4Model.Location = new System.Drawing.Point(40, 17);
            this.LabHDD4Model.Name = "LabHDD4Model";
            this.LabHDD4Model.Size = new System.Drawing.Size(59, 48);
            this.LabHDD4Model.TabIndex = 1;
            // 
            // labHDD4Temp
            // 
            this.labHDD4Temp.AutoSize = true;
            this.labHDD4Temp.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold);
            this.labHDD4Temp.ForeColor = System.Drawing.Color.Green;
            this.labHDD4Temp.Location = new System.Drawing.Point(1, 24);
            this.labHDD4Temp.Name = "labHDD4Temp";
            this.labHDD4Temp.Size = new System.Drawing.Size(36, 24);
            this.labHDD4Temp.TabIndex = 0;
            this.labHDD4Temp.Text = "00";
            // 
            // gbDisk3
            // 
            this.gbDisk3.Controls.Add(this.labHDD3UseHours);
            this.gbDisk3.Controls.Add(this.LabHDD3Model);
            this.gbDisk3.Controls.Add(this.labHDD3Temp);
            this.gbDisk3.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbDisk3.Location = new System.Drawing.Point(234, 10);
            this.gbDisk3.Name = "gbDisk3";
            this.gbDisk3.Padding = new System.Windows.Forms.Padding(0);
            this.gbDisk3.Size = new System.Drawing.Size(105, 72);
            this.gbDisk3.TabIndex = 13;
            this.gbDisk3.TabStop = false;
            this.gbDisk3.Visible = false;
            // 
            // labHDD3UseHours
            // 
            this.labHDD3UseHours.AutoSize = true;
            this.labHDD3UseHours.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labHDD3UseHours.Location = new System.Drawing.Point(60, 65);
            this.labHDD3UseHours.Name = "labHDD3UseHours";
            this.labHDD3UseHours.Size = new System.Drawing.Size(0, 15);
            this.labHDD3UseHours.TabIndex = 2;
            // 
            // LabHDD3Model
            // 
            this.LabHDD3Model.Font = new System.Drawing.Font("Times New Roman", 8F);
            this.LabHDD3Model.Location = new System.Drawing.Point(40, 17);
            this.LabHDD3Model.Name = "LabHDD3Model";
            this.LabHDD3Model.Size = new System.Drawing.Size(59, 48);
            this.LabHDD3Model.TabIndex = 1;
            // 
            // labHDD3Temp
            // 
            this.labHDD3Temp.AutoSize = true;
            this.labHDD3Temp.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold);
            this.labHDD3Temp.ForeColor = System.Drawing.Color.Green;
            this.labHDD3Temp.Location = new System.Drawing.Point(2, 24);
            this.labHDD3Temp.Name = "labHDD3Temp";
            this.labHDD3Temp.Size = new System.Drawing.Size(36, 24);
            this.labHDD3Temp.TabIndex = 0;
            this.labHDD3Temp.Text = "00";
            // 
            // gbDisk2
            // 
            this.gbDisk2.Controls.Add(this.labHDD2UseHours);
            this.gbDisk2.Controls.Add(this.LabHDD2Model);
            this.gbDisk2.Controls.Add(this.labHDD2Temp);
            this.gbDisk2.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbDisk2.Location = new System.Drawing.Point(122, 10);
            this.gbDisk2.Name = "gbDisk2";
            this.gbDisk2.Padding = new System.Windows.Forms.Padding(0);
            this.gbDisk2.Size = new System.Drawing.Size(105, 72);
            this.gbDisk2.TabIndex = 12;
            this.gbDisk2.TabStop = false;
            this.gbDisk2.Visible = false;
            // 
            // labHDD2UseHours
            // 
            this.labHDD2UseHours.AutoSize = true;
            this.labHDD2UseHours.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labHDD2UseHours.Location = new System.Drawing.Point(60, 65);
            this.labHDD2UseHours.Name = "labHDD2UseHours";
            this.labHDD2UseHours.Size = new System.Drawing.Size(0, 15);
            this.labHDD2UseHours.TabIndex = 2;
            // 
            // LabHDD2Model
            // 
            this.LabHDD2Model.Font = new System.Drawing.Font("Times New Roman", 8F);
            this.LabHDD2Model.Location = new System.Drawing.Point(36, 17);
            this.LabHDD2Model.Name = "LabHDD2Model";
            this.LabHDD2Model.Size = new System.Drawing.Size(62, 48);
            this.LabHDD2Model.TabIndex = 1;
            // 
            // labHDD2Temp
            // 
            this.labHDD2Temp.AutoSize = true;
            this.labHDD2Temp.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold);
            this.labHDD2Temp.ForeColor = System.Drawing.Color.Green;
            this.labHDD2Temp.Location = new System.Drawing.Point(1, 27);
            this.labHDD2Temp.Name = "labHDD2Temp";
            this.labHDD2Temp.Size = new System.Drawing.Size(36, 24);
            this.labHDD2Temp.TabIndex = 0;
            this.labHDD2Temp.Text = "00";
            // 
            // gbDisk1
            // 
            this.gbDisk1.Controls.Add(this.labHDD1UseHours);
            this.gbDisk1.Controls.Add(this.LabHDD1Model);
            this.gbDisk1.Controls.Add(this.labHDD1Temp);
            this.gbDisk1.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbDisk1.Location = new System.Drawing.Point(9, 10);
            this.gbDisk1.Name = "gbDisk1";
            this.gbDisk1.Padding = new System.Windows.Forms.Padding(0);
            this.gbDisk1.Size = new System.Drawing.Size(105, 72);
            this.gbDisk1.TabIndex = 11;
            this.gbDisk1.TabStop = false;
            this.gbDisk1.Visible = false;
            // 
            // labHDD1UseHours
            // 
            this.labHDD1UseHours.AutoSize = true;
            this.labHDD1UseHours.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labHDD1UseHours.Location = new System.Drawing.Point(60, 65);
            this.labHDD1UseHours.Name = "labHDD1UseHours";
            this.labHDD1UseHours.Size = new System.Drawing.Size(0, 15);
            this.labHDD1UseHours.TabIndex = 2;
            // 
            // LabHDD1Model
            // 
            this.LabHDD1Model.Font = new System.Drawing.Font("Times New Roman", 8F);
            this.LabHDD1Model.Location = new System.Drawing.Point(39, 17);
            this.LabHDD1Model.Name = "LabHDD1Model";
            this.LabHDD1Model.Size = new System.Drawing.Size(62, 48);
            this.LabHDD1Model.TabIndex = 1;
            // 
            // labHDD1Temp
            // 
            this.labHDD1Temp.AutoSize = true;
            this.labHDD1Temp.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold);
            this.labHDD1Temp.ForeColor = System.Drawing.Color.Green;
            this.labHDD1Temp.Location = new System.Drawing.Point(1, 27);
            this.labHDD1Temp.Name = "labHDD1Temp";
            this.labHDD1Temp.Size = new System.Drawing.Size(36, 24);
            this.labHDD1Temp.TabIndex = 0;
            this.labHDD1Temp.Text = "00";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(470, 267);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.labLock);
            this.Controls.Add(this.labRecord);
            this.Controls.Add(this.LabECVersion);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.LabNBModel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox7);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FanControl";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbFan1.ResumeLayout(false);
            this.gbFan1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.gbFan2.ResumeLayout(false);
            this.gbFan2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.gbFan3.ResumeLayout(false);
            this.gbFan3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.gbFan4.ResumeLayout(false);
            this.gbFan4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.gbDisk4.ResumeLayout(false);
            this.gbDisk4.PerformLayout();
            this.gbDisk3.ResumeLayout(false);
            this.gbDisk3.PerformLayout();
            this.gbDisk2.ResumeLayout(false);
            this.gbDisk2.PerformLayout();
            this.gbDisk1.ResumeLayout(false);
            this.gbDisk1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void initGb()
        {
            if (this.FAN_Amount == 0 || this.FAN_Amount > 4)
            {
                this.FAN_Amount = 1;
            }
            this.gbFan2.Enabled = false;
            this.gbFan3.Enabled = false;
            this.gbFan4.Enabled = false;
            if (this.FAN_Amount >= 2)
            {
                this.gbFan2.Enabled = true;
            }
            if (this.FAN_Amount >= 3)
            {
                this.gbFan3.Enabled = true;
            }
            if (this.FAN_Amount >= 4)
            {
                this.gbFan4.Enabled = true;
            }
        }

        private void KillMessageBox()
        {
            IntPtr hWnd = FindWindow(null, "MessageBox");
            if (hWnd != IntPtr.Zero)
            {
                PostMessage(hWnd, 0x10, IntPtr.Zero, IntPtr.Zero);
            }
        }


        private void lockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkFanSet();
        }

        private void checkFanSet()
        {
            this.Lock = this.lockToolStripMenuItem.Checked;
            bool enabled = !Lock;
            for (int i = 1; i < cbs.Length; i++)
            {
                if (cbs[i].Checked)
                {
                    nudSpeeds[i].Enabled = false;
                }
                cbs[i].Enabled = enabled;
            }
            this.labLock.Text = Lock? "Lock":"";
        }
        private void numericUpDown_ValueChanged(int index)
        {
           
            if (!this.FAN_Autols[index] && !this.Lock)
            {
                double num = ((double)this.nudSpeeds[index].Value) * 2.55;
                if (this.nudSpeeds[index].Value == 100M)
                {
                    num = 255.0;
                }
                this.ntPort.Set_FAN_Duty(index, (int)num);
            }
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.saveToolStripMenuItem.Checked)
            {
                this.Rec = true;
                this.labRecord.Text = "ON";
            }
            else
            {
                this.Rec = false;
                this.labRecord.Text = "OFF";
            }
        }

        private void timer_hdd_tick(object sender, EventArgs e)
        {
            Console.Write("timer_hdd_tick...");
            if (this.bInitHddInfo)
            {
                //if (this.RAID)
                //{
                //    this.labHDD1Temp.Text = "";
                //    this.labHDD2Temp.Text = "";
                //    this.labHDD3Temp.Text = "";
                //}
                 if (!this.LockHDD && UpdateInfo())
                {
                    int numArray = 0;
                    for (int i = 0; i < this.HDDInstance; i++)
                    {
                        if (!GetTemperture(i, out numArray)) d(1);
                        {
                            break;
                        }
                        this.labDiskTemps[i].Text= Convert.ToString(numArray);d(1);
                    }
                }
            }
            Console.WriteLine("end");
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.KillMessageBox();
            ((System.Windows.Forms.Timer) sender).Stop();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            Console.Write("Form1_Deactivate...");
            if (this.WindowState == FormWindowState.Minimized)
            {
                timer_fan.Enabled = false;
                timer_hdd.Enabled = false;
                Console.Write("this.WindowState = FormWindowState.Minimized and timer_fan.unEnabled...");

            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (!timer_fan.Enabled)
            {
            //    Console.WriteLine("\nFanCount:" + CallingVariations.GetFANCounter());
                timer_fan.Enabled = true;
                timer_hdd.Enabled = this.hddStripMenuItem.Checked;
                Console.Write("Form1_Activated and timer_fan.Enabled...");
            }

        }



        private void timerFan_Tick(object sender, EventArgs e)
        {
            try {
                Console.Write("timer1_Tick...");d(0);
                FanShow(1);d(1);
                if (this.FAN_Amount > 1)
                {

                    FanShow(2);d(2);
                }
                if (this.FAN_Amount > 2)
                {
                    FanShow(3);d(3);
                }
                if (this.FAN_Amount>=4)
                {
                    FanShow(4);d(4);
                }

                if (this.Rec)
                {

                    string file = @"C:\ECView\ECView.txt";d(5);
                    
                        string str2 = string.Concat(new object[] { DateTime.Now, "  CPU-> ", Convert.ToString(this.labCPURemote.Text), "℃ ", Convert.ToString(this.labCPULocal.Text), "℃ ",  "-", Convert.ToString(this.numericUpDown1.Value), "%-", this.labCPURPM.Text });
                        if (this.FAN_Amount >= 2)
                        {
                            str2 += ",  VGA1-> " + Convert.ToString(this.labVGA1Remote.Text) + "℃ " + Convert.ToString(this.labVGA1Local.Text) + "℃ " + "-" + Convert.ToString(this.numericUpDown2.Value) + "%-" + this.labGPURPM.Text;
                        }
                        if (this.FAN_Amount >= 3)
                        {
                            str2 += ",  VGA2-> " + Convert.ToString(this.labVGA2Remote.Text) + "℃ " + Convert.ToString(this.labVGA2Local.Text) + "℃ "  + "-" + Convert.ToString(this.numericUpDown3.Value) + "%-" + this.labGPU1RPM.Text;
                        }
                        if (this.FAN_Amount == 4)
                        {
                            str2 += ",  SYS-> " + Convert.ToString(this.labSYSRemote.Text) + "℃ " + Convert.ToString(this.labSYSLocal.Text) + "℃ "  + "-" + Convert.ToString(this.numericUpDown4.Value) + "%-" + this.labSYSRPM.Text;
                        }
                        str2 += "\r\n";
                        if (!this.RAID)
                        {
                            if (this.HDDInstance >= 1)
                            {
                                str2 += " HDD " + this.services[0, 0] + " ; " + Convert.ToString(this.labHDD1Temp.Text);
                            }
                            if (this.HDDInstance >= 2)
                            {
                                str2 += " ; HDD " + this.services[1, 0] + " ; " + Convert.ToString(this.labHDD2Temp.Text);
                            }
                            if (this.HDDInstance >= 3)
                            {
                                str2 += " ; HDD " + this.services[2, 0] + " ; " + Convert.ToString(this.labHDD3Temp.Text);
                            }
                            str2 += "\r\n";
                        }

                        FileSystem.WriteAllText(file, str2, true);
                   
                    
                }
               
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);d(6);
            }d(7);
            Console.WriteLine("done!");
        }

        private void FanShow(int id)
        {
            NTPort.ECData data = this.ntPort.Get_TempFanDuty(id);
            int local = data.Local;
            if ((local == 0x90) || (local < 0))
            {
                labelRemotes[id].Text = "---";
            }
            else
            {
                labelRemotes[id].Text = Convert.ToString(local);
            }
            int remote = data.Remote;
            if ((remote == 0x90) || (remote < 0))
            {
                labelLocals[id].Text = "---";
            }
            else
            {
                labelLocals[id].Text = Convert.ToString(remote);
            }
            int fanDuty = data.FanDuty;
            if (cbs[id].Checked)
            {
                nudSpeeds[id].Text = Convert.ToString(Math.Ceiling((double)(((double)fanDuty) / 2.55)));
            //    fanDuty = (fanDuty * 100) / 0xff;
            }
            if (this.LockRPM) return;
                this.Fan_rpms[id] = this.ntPort.Get_FAN_RPM(id);
            if (this.Fan_rpms[id] != 0)
            {
                this.fl_rpms[id] = 60.0 / ((1.391304347826087E-05 * this.Fan_rpms[id]) * 4.0)*2;
                this.de_rpms[id] = Math.Round(Convert.ToDecimal(this.fl_rpms[id]), 0);
            }
            else
            {
                this.de_rpms[id] = 0M;
            }
            this.labelRPMs[id].Text = Convert.ToString(this.de_rpms[id]);
        }

        private void timerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Input_SetTimer timer = new Input_SetTimer();
            timer.InterValFan = this.timer_fan.Interval/1000;
            timer.InterValHdd = this.timer_hdd.Interval / 1000;
            if (timer.ShowDialog() == DialogResult.OK)
            {
                    this.timer_fan.Interval = (int)(timer.InterValFan*1000);
                    this.timer_hdd.Interval = Convert.ToInt32(timer.InterValHdd * 1000);
            }
        }


        [DllImport("SmartDisk.dll", EntryPoint="#6", CharSet=CharSet.Auto)]
        public static extern bool UpdateInfo();
    }
}

