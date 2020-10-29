namespace ECView_CSharp
{
    using System;
    using System.Runtime.InteropServices;

    internal class NTPort
    {
        public string g_strEcVersion = "";
        public string g_strProject = "";

        public NTPort()
        {
            InitIo();
            IntPtr eCVersion = GetECVersion();
            string str = Marshal.PtrToStringAnsi(eCVersion);
            this.g_strEcVersion = str;
            Marshal.FreeHGlobal(eCVersion);
        }

        public int Get_CPU_FAN_RPM() => 
            GetCpuFanRpm();

        public int Get_GPU_FAN_RPM() => 
            GetGpuFanRpm();

        public int Get_GPU1_FAN_RPM() => 
            GetGpu1FanRpm();

        public int Get_X72_FAN_RPM() =>
           GetX72FanRpm();

        public int Get_FAN_RPM(int id)
        {
            switch (id) {
                case 2:return Get_GPU_FAN_RPM();
                case 3:return Get_GPU1_FAN_RPM();
                case 4:return Get_X72_FAN_RPM();
                default: return Get_CPU_FAN_RPM();
            }
            
        }
        public ECData Get_TempFanDuty(int index)
        {
            ECData data = new ECData();
            uint tempFanDuty = 0;
            tempFanDuty = GetTempFanDuty(index);
           // int i=GetFanCount();
           // Console.WriteLine("GetFanCount:" + i);
            data.Local = ((int) tempFanDuty) & 0xff;
            data.Remote = ((int) (tempFanDuty >> 8)) & 0xff;
            data.FanDuty = (int) (tempFanDuty >> 0x10);
//            Console.WriteLine(index);
//            Console.WriteLine("l:" + data.Local);
//            Console.WriteLine("r:" + data.Remote);
//            Console.WriteLine("f:" + data.FanDuty+"\n");
           
            if ((data.Local > 80) || (data.Remote > 80))
            {
                Console.WriteLine("Check");
            }
            return data;
        }

       

        [DllImport("ClevoEcInfo.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetCpuFanRpm();
        [DllImport("ClevoEcInfo.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr GetECVersion();
        [DllImport("ClevoEcInfo.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetFanCount();
        [DllImport("ClevoEcInfo.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetGpu1FanRpm();
        [DllImport("ClevoEcInfo.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetGpuFanRpm();
        [DllImport("ClevoEcInfo.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetOptionModual();
        [DllImport("ClevoEcInfo.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetTempFanDuty(int index);
        [DllImport("ClevoEcInfo.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetX72FanRpm();
        [DllImport("ClevoEcInfo.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void InitIo();
        public void Set_FAN_Duty(int index, int duty)
        {
            SetFanDuty(index, duty);
        }

        public void Set_FAN_Duty_Auto(int index)
        {
            SetFanDutyAuto(index);
        }

        [DllImport("ClevoEcInfo.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetFanDuty(int index, int duty);
        [DllImport("ClevoEcInfo.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetFanDutyAuto(int index);

        [StructLayout(LayoutKind.Sequential)]
        public struct ECData
        {
            public int Remote;
            public int Local;
            public int FanDuty;
        }
    }
}

