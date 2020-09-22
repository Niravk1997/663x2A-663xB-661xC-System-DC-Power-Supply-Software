using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Agilent_6632B
{
    public static class dataShare
    {
        public static Boolean isVoltGraphClosed = false;
        public static Boolean isCurrGraphClosed = false;
        public static Boolean isPowerGraphClosed = false;
    }
    public class Serial_Interface
    {
        public string COMPort;
        public int BaudRate;
        public int Parity;
        public int StopBits;
        public int DataBits;
        public int Handshake;
        public int WriteTimeout;
        public int ReadTimeout;
        public bool RtsEnable;
        public SerialPort sp;

        public Serial_Interface(string com, int baudrate, int parity, int stopbits, int databits, int handshake, int writetimeout, int readtimeout, bool rtsenable)
        {
            this.COMPort = com;
            this.BaudRate = baudrate;
            this.Parity = parity;
            this.StopBits = stopbits;
            this.DataBits = databits;
            this.Handshake = handshake;
            this.WriteTimeout = writetimeout;
            this.ReadTimeout = readtimeout;
            this.RtsEnable = rtsenable;
        }

        public Serial_Interface(string com, int baudrate, int parity, int stopbits, int databits, int handshake, int writetimeout, int readtimeout)
        {
            this.COMPort = com;
            this.BaudRate = baudrate;
            this.Parity = parity;
            this.StopBits = stopbits;
            this.DataBits = databits;
            this.Handshake = handshake;
            this.WriteTimeout = writetimeout;
            this.ReadTimeout = readtimeout;
            this.RtsEnable = false;
        }

        public void ConfigCOM()
        {
            sp = new SerialPort(COMPort);
            sp.BaudRate = BaudRate;
            sp.Parity = (Parity)Parity;
            sp.StopBits = (StopBits)StopBits;
            sp.DataBits = DataBits;
            sp.Handshake = (Handshake)Handshake;
            sp.WriteTimeout = WriteTimeout;
            sp.ReadTimeout = ReadTimeout;
            sp.RtsEnable = RtsEnable;
        }

        public void OpenCOM()
        {
            sp.Open();
        }

        public void Write(string command)
        {
            try
            {
                sp.WriteLine(command);
            }
            catch (Exception)
            {

            }
        }

        public string Read()
        {
            string DataRead = "Null";
            try
            {
                DataRead = sp.ReadLine();
            }
            catch (Exception)
            {

            }
            return DataRead;
        }

        public void CloseCOM()
        {
            sp.Close();
        }

    }
    public static class RS232_Info
    {
        public static Boolean Connected = false;

        //RS232 Connection Info
        public static string COM_Port;
        public static int BaudRate;
        public static int Parity;
        public static int StopBits;
        public static int DataBits;
        public static int Handshake;
        public static int WriteTimeout;
        public static int ReadTimeout;
        public static bool RtsEnable;

        //Device Info
        public static string DeviceID;
        public static string maxVolt;
        public static string maxCurr;
        public static string maxOVP;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Serial Connecting
        public Serial_Interface Serial;
        Boolean Serial_Enabled = false;

        //Model Info Update
        Boolean Get_Model_ID = false;

        //Reading Data from Device
        decimal Voltage_Reading = 0;
        int V_Counter = 0;

        decimal Current_Reading = 0;
        int C_Counter = 0;

        decimal Power_reading = 0;

        Boolean Set_C_Range_Low = false;
        Boolean Set_C_Range_High = false;
        Boolean Allow_Range_Switch = false;

        double UpdateRate = 1;
        int seconds;

        Boolean Output_writetxt = false;
        Boolean Output_writecsv = false;

        Boolean autoscroll = true;
        Boolean clearList = false;
        Boolean UpdateList = true;

        DispatcherTimer updater;
        DispatcherTimer timer_update;

        COM_Select_Window Select_COM;
        General_Help_Window GeneralHelp;
        RS232_Help Serial_Help;
        Credits About_Me;
        Voltage_Graph V_Plot;
        Current_Graph C_Plot;
        Power_Graph P_Plot;


        int beeper = 0;

        double Set_voltage_value = 0;
        double actual_setVoltage = 0;
        double V_increment = 1;
        bool mV_Range = false;
        static double maxVoltage = 0;
        static double minVoltage = 0;
        double upper_V_limit = maxVoltage;
        double lower_V_limit = 0;

        double Set_current_value = 0;
        double actual_setCurrent = 0;
        double A_increment = 1;
        bool mA_Range = false;
        static double maxCurrent = 0;
        static double minCurrent = 0;
        double upper_A_limit = maxCurrent;
        double lower_A_limit = 0;

        //Switches for writting to device

        //Set Voltage
        Boolean Set_V = false;

        //Set Current
        Boolean Set_C = false;

        //Set UVP (Under Voltage Protection)
        Boolean Set_UVP = false;
        decimal UVP_value = 0;

        //Set UCP (Under Current Protection)
        Boolean Set_UCP = false;
        decimal UCP_value = 0;

        //Set OVP (Over Voltage Protection)
        Boolean Set_OVP = false;
        decimal OVP_value = 0;

        //Set OCP (Over Current Protection)
        Boolean Set_OCP = false;
        Boolean Ready_OCP = false;

        //Panel Range
        decimal v_range = 1;
        Boolean v_range_auto = true;

        decimal c_range = 1;
        Boolean c_range_auto = true;

        decimal p_range = 1;
        Boolean p_range_auto = true;

        Boolean Output_Set = false;
        Boolean Output_Off = true;

        Boolean Clear_Protect = false;

        Boolean Reset_Switch = false;

        Boolean Read_Error = false;

        Boolean Read_Set_VC = true;

        Boolean Display_set = true;
        Boolean Display_Switch = false;
        Boolean Display_text = false;
        Boolean Display_Clear = false;

        Boolean Runtime_start = false;

        Boolean Status_writetxt = false;

        Boolean MeasureV = true;
        Boolean MeasureC = true;
        Boolean GetStatus = true;
        Boolean GetProtect = true;
        Boolean GetSetVC = true;

        Boolean Invalid_Measured_Volt = false;
        Boolean Invalid_Measured_Curr = false;

        Boolean Add_MeasuredVC_List = false;

        int progress = 0;
        Boolean ProgressBar_Halt = false;

        Boolean Get_OVP_value_OCP_state = true;


        Boolean isVoltGraph = false;
        Boolean isCurrGraph = false;
        Boolean isPowerGraph = false;
        string accurate_date;
        string accurate_Date_Time;

        public MainWindow()
        {
            InitializeComponent();
            Voltage_Text_Box.Text = lower_V_limit.ToString();
            Current_Text_Box.Text = lower_A_limit.ToString();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                if (RS232_Info.Connected == true)
                {
                    Serial.CloseCOM();
                }
            }
            catch (Exception) { }
            Application.Current.Shutdown();
        }

        private void dataUpdater(object sender, RoutedEventArgs e)
        {
            updater = new DispatcherTimer();
            updater.Interval = TimeSpan.FromSeconds(UpdateRate);
            updater.Tick += dataRefresh;
            updater.Start();

            timer_update = new DispatcherTimer();
            timer_update.Interval = TimeSpan.FromSeconds(1);
            timer_update.Tick += runtime_Update;
            timer_update.Start();

        }

        private void unlock_controls()
        {
            VoltageBox.IsEnabled = true;
            CurrentBox.IsEnabled = true;
            Output.IsEnabled = true;
            Protect.IsEnabled = true;
            Display.IsEnabled = true;
            Info_Box.IsEnabled = true;
            Data_Logger.IsEnabled = true;
            List.IsEnabled = true;
            Measurements.IsEnabled = true;
            Reset.IsEnabled = true;
            Edit.IsEnabled = true;

        }

        private void Serial_Connect()
        {
            if (Serial_Enabled == false)
            {
                Serial = new Serial_Interface(RS232_Info.COM_Port, RS232_Info.BaudRate, RS232_Info.Parity, RS232_Info.StopBits, RS232_Info.DataBits, RS232_Info.Handshake, RS232_Info.WriteTimeout, RS232_Info.ReadTimeout);
                Serial.ConfigCOM();
                Serial.OpenCOM();
                maxVoltage = Convert.ToDouble(RS232_Info.maxVolt);
                upper_V_limit = maxVoltage;
                maxCurrent = Convert.ToDouble(RS232_Info.maxCurr);
                upper_A_limit = maxCurrent;
                Serial_Enabled = true;
                this.Title = RS232_Info.DeviceID + " System DC Power Supply";
            }
        }

        private void dataRefresh(object sender, EventArgs e)
        {
            if (RS232_Info.Connected == true)
            {
                if (Get_Model_ID == false)
                {
                    Info_Updater();
                    Get_Model_ID = true;
                    Runtime_start = true;
                    unlock_controls();
                }

                if (Serial_Enabled == false)
                {
                    Serial_Connect();
                }

                Setting_Voltage();
                Setting_Current();
                Setting_Output();
                Clear_Protection();
                Setting_OCP();
                Setting_OVP();
                Reset_Device();
                Read_Error_FIFO();
                Display_Visibility();
                Display_Text();
                Display_Text_Clear();
                Current_Range_Switch();

                if (MeasureV == true) { Measure_Volt(); Setting_UVP(); }
                else
                {
                    Voltage_Reading = 0;
                    Measure_Voltage.Text = "?";
                }

                if (MeasureC == true) { Measure_Curr(); Setting_UCP(); }
                else
                {
                    Current_Reading = 0;
                    Measure_Current.Text = "?";
                }

                if (MeasureC == true && MeasureV == true) { Measure_Power(); }
                else
                {
                    Power_reading = 0;
                    Calculate_Power.Text = "?";
                }

                if (GetStatus == true) { Check_Output_Status(); }
                else
                {
                    CC_CV_Status.Text = "?";
                    CC_CV_Status.FontSize = 60;
                }

                if (GetProtect == true) { Check_Protection_Status(); }
                else
                {
                    Protect_Ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
                }
                if (GetSetVC == true) { Measure_Set_VC(); }
                if (Get_OVP_value_OCP_state == true) { Get_Set_OVP_OCP_Values(); }

                progressBar_updater();
                if ((Invalid_Measured_Curr == false) && (Invalid_Measured_Volt == false))
                {
                    accurate_date = DateTime.Now.ToString("yyyy/MM/dd h:mm:ss tt");
                    Output_writeTotxt(accurate_date + "," + (Math.Round(Voltage_Reading, 4, MidpointRounding.AwayFromZero)).ToString() + "," + (Math.Round(Current_Reading, 5, MidpointRounding.AwayFromZero)).ToString());
                    accurate_Date_Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    Output_writeTocsv(accurate_Date_Time, (Math.Round(Voltage_Reading, 5, MidpointRounding.AwayFromZero)).ToString(), (Math.Round(Current_Reading, 5, MidpointRounding.AwayFromZero)).ToString());
                    Add_Measurements_List();
                    if (MeasureV == true)
                    {
                        Volt_To_Graph();
                    }
                    if (MeasureC == true)
                    {
                        Curr_To_Graph();
                    }
                    if (MeasureC == true && MeasureV == true)
                    {
                        Power_To_Graph();
                    }
                }

            }
        }

        private void Volt_To_Graph()
        {
            if (dataShare.isVoltGraphClosed == false)
            {
                if (isVoltGraph == true)
                {
                    V_Plot.dataPasser(Decimal.ToDouble(Voltage_Reading), actual_setVoltage, Decimal.ToDouble(OVP_value), Decimal.ToDouble(UVP_value), CC_CV_Status.Text, accurate_date);
                }
            }
            else if (dataShare.isVoltGraphClosed == true)
            {
                V_Graph.IsEnabled = true;
                dataShare.isVoltGraphClosed = false;
                isVoltGraph = false;
            }
        }

        private void Curr_To_Graph()
        {
            if (dataShare.isCurrGraphClosed == false)
            {
                if (isCurrGraph == true)
                {
                    C_Plot.dataPasser(Decimal.ToDouble(Current_Reading), actual_setCurrent, actual_setCurrent, Decimal.ToDouble(UCP_value), CC_CV_Status.Text, accurate_date);
                }
            }
            else if (dataShare.isCurrGraphClosed == true)
            {
                C_Graph.IsEnabled = true;
                dataShare.isCurrGraphClosed = false;
                isCurrGraph = false;
            }
        }

        private void Power_To_Graph()
        {
            if (dataShare.isPowerGraphClosed == false)
            {
                if (isPowerGraph == true)
                {
                    P_Plot.dataPasser(Decimal.ToDouble(Power_reading), CC_CV_Status.Text, accurate_date);
                }
            }
            else if (dataShare.isPowerGraphClosed == true)
            {
                P_Graph.IsEnabled = true;
                dataShare.isPowerGraphClosed = false;
                isPowerGraph = false;
            }
        }



        private void Get_Set_OVP_OCP_Values()
        {
            if (Get_OVP_value_OCP_state == true)
            {
                string temp;
                Serial.Write("VOLT:PROT:LEV?");
                temp = Serial.Read();
                if (temp != "Null")
                {
                    try
                    {
                        decimal temp_3 = Decimal.Parse(temp, System.Globalization.NumberStyles.Float);
                        OVP_value = temp_3;
                        Over_vlabel.Text = (Math.Round(temp_3, 2, MidpointRounding.AwayFromZero)).ToString() + "V";
                    }
                    catch (Exception) { Over_vlabel.Text = "?"; }
                }

                Serial.Write("CURR:PROT:STAT?");
                temp = Serial.Read();
                if (temp != "Null")
                {
                    switch (temp.TrimEnd())
                    {
                        case "1":
                            Over_clabel.Text = "Enabled";
                            OCP_Set.Content = "Enabled";
                            break;
                        case "0":
                            Over_clabel.Text = "Disabled";
                            OCP_Set.Content = "Disabled";
                            break;
                        default:
                            Over_clabel.Text = "?";
                            break;
                    }
                }
                else { Over_clabel.Text = "?"; }
            }
        }

        private void Add_Measurements_List()
        {
            if (Add_MeasuredVC_List == true)
            {
                string v_temp = (Math.Round(Voltage_Reading, 3, MidpointRounding.AwayFromZero)).ToString();
                string c_temp = (Math.Round(Current_Reading, 4, MidpointRounding.AwayFromZero)).ToString();
                listboxUpdater(v_temp + "V   " + c_temp + "A" + "   " + "," + DateTime.Now.ToString("h:mm:ss tt"));
            }
        }

        private void Setting_UVP()
        {
            if (Set_UVP == true)
            {
                if (Invalid_Measured_Volt == false)
                {
                    if (Math.Round(UVP_value, 3, MidpointRounding.AwayFromZero) > (Math.Round(Voltage_Reading, 3, MidpointRounding.AwayFromZero)))
                    {
                        Serial.Write("OUTPut OFF");
                        listboxUpdater("UVP Tripped " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                        listboxUpdater("T: " + (Math.Round(Voltage_Reading, 3, MidpointRounding.AwayFromZero)).ToString() + "V " + (Math.Round(Current_Reading, 4, MidpointRounding.AwayFromZero)).ToString() + "A " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                        Under_vlabel.Text = "Dis";
                        Output_Off = true;
                        Output_x.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        Output_Box.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        Output_Status.Content = "Output Disabled";
                        if (beeper == 1) { Console.Beep(); }
                        Set_UVP = false;
                    }
                }
            }
        }

        private void Setting_UCP()
        {
            if (Set_UCP == true)
            {
                if (Invalid_Measured_Curr == false)
                {
                    if (Math.Round(UCP_value, 4, MidpointRounding.AwayFromZero) > (Math.Round(Current_Reading, 4, MidpointRounding.AwayFromZero)))
                    {
                        Serial.Write("OUTPut OFF");
                        listboxUpdater("UCP Tripped " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                        listboxUpdater("T: " + (Math.Round(Voltage_Reading, 3, MidpointRounding.AwayFromZero)).ToString() + "V " + (Math.Round(Current_Reading, 4, MidpointRounding.AwayFromZero)).ToString() + "A " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                        Under_clabel.Text = "Dis";
                        Output_Off = true;
                        Output_x.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        Output_Box.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        Output_Status.Content = "Output Disabled";
                        if (beeper == 1) { Console.Beep(); }
                        Set_UCP = false;
                    }
                }
            }
        }

        private void Current_Range_Switch()
        {
            if (Allow_Range_Switch == true)
            {
                if (Set_C_Range_High == true)
                {
                    Serial.Write("SENSe:CURRent:RANGe MAX");
                    listboxUpdater("C Meas Range High " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                }
                else if (Set_C_Range_Low == true)
                {
                    Serial.Write("SENSe:CURRent:RANGe MIN");
                    listboxUpdater("C Meas Range Low " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                }
                Allow_Range_Switch = false;
            }
        }

        private void progressBar_updater()
        {
            if (ProgressBar_Halt == false)
            {
                if (ProgressBar.Value == ProgressBar.Maximum)
                {
                    progress = 0;
                    ProgressBar.Value = 0;
                }
                else
                {
                    ProgressBar.Value = progress;
                }
            }
        }

        public void Display_Visibility()
        {
            if (Display_Switch == true)
            {
                if (Display_set == true)
                {
                    Serial.Write("DISP OFF");
                    Display_set = false;
                    listboxUpdater("Display Off " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                }
                else
                {
                    Serial.Write("DISP ON");
                    Display_set = true;
                    listboxUpdater("Display On " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                }
            }
            Display_Switch = false;
        }

        public void Display_Text()
        {
            if (Display_text == true)
            {
                Serial.Write("DISPLAY:MODE TEXT");
                Serial.Write("DISP:TEXT " + "'" + Send_Text.Text + "'");
                listboxUpdater(Send_Text.Text + " " + "," + DateTime.Now.ToString("h:mm:ss tt"));
            }
            Display_text = false;
        }

        public void Display_Text_Clear()
        {
            if (Display_Clear == true)
            {
                Serial.Write("DISPLAY:MODE NORM");
                listboxUpdater("Display Normal " + "," + DateTime.Now.ToString("h:mm:ss tt"));
            }
            Display_Clear = false;
        }

        public void Measure_Set_VC()
        {
            if (Read_Set_VC == true)
            {
                string temp;
                Serial.Write("VOLT:LEV?");
                temp = Serial.Read();
                if (temp != "Null")
                {
                    try
                    {
                        decimal temp_2 = Decimal.Parse(temp, System.Globalization.NumberStyles.Float);
                        Set_Voltage.Text = "Set: " + (Math.Round(temp_2, 3, MidpointRounding.AwayFromZero)).ToString() + "V";
                        Set_voltage_value = Decimal.ToDouble(temp_2);
                        actual_setVoltage = Set_voltage_value;
                    }
                    catch (Exception) { Set_voltage_value = 0; }
                }
                else
                { Set_Voltage.Text = "Set: Invalid!"; }

                Serial.Write("CURR:LEV?");
                temp = Serial.Read();
                if (temp != "Null")
                {
                    try
                    {
                        decimal temp_3 = Decimal.Parse(temp, System.Globalization.NumberStyles.Float);
                        Set_Current.Text = "Set: " + (Math.Round(temp_3, 4, MidpointRounding.AwayFromZero)).ToString() + "A";
                        Set_current_value = Decimal.ToDouble(temp_3);
                        actual_setCurrent = Set_current_value;
                    }
                    catch (Exception) { Set_current_value = 0; }
                }
                else
                { Set_Current.Text = "Set: Invalid!"; }
            }
        }

        public void Measure_Volt()
        {
            string temp;
            Serial.Write("MEAS:VOLT?");
            temp = Serial.Read();
            if (temp != "Null")
            {
                if ((temp.Length >= 10) && (temp.Length <= 12) && (temp.Contains("E")) && (temp.Contains(".")))
                {
                    try
                    {
                        Voltage_Reading = Decimal.Parse(temp, System.Globalization.NumberStyles.Float);
                        Invalid_Measured_Volt = false;
                    }
                    catch (Exception) { Voltage_Reading = 0; Invalid_Measured_Volt = true; }
                    V_Counter++;
                    VoltSamples.Content = V_Counter.ToString() + "V";
                    if (ProgressBar_Halt == false) { progress++; }
                }
                if (v_range_auto == true)
                {
                    if ((Voltage_Reading > 0) && (Voltage_Reading < v_range))
                    {
                        Measure_Voltage.Text = (Math.Round((Voltage_Reading * 1000), 0, MidpointRounding.AwayFromZero)).ToString() + "mV";
                    }
                    else
                    {
                        Measure_Voltage.Text = (Math.Round(Voltage_Reading, 3, MidpointRounding.AwayFromZero)).ToString() + "V";
                    }
                }
                else
                {
                    Measure_Voltage.Text = (Math.Round(Voltage_Reading, 3, MidpointRounding.AwayFromZero)).ToString() + "V";
                }
            }
            else { Voltage_Reading = 0; Invalid_Measured_Volt = true; }
        }

        public void Measure_Curr()
        {
            string temp;
            Serial.Write("MEAS:CURR?");
            temp = Serial.Read();
            if (temp != "Null")
            {
                if ((string.Compare(temp.TrimEnd(), "9.91000E+37")) == 0)
                {
                    Measure_Current.Text = "OVLD.mA";
                    Invalid_Measured_Curr = false;
                    Current_Reading = 0;
                    if (ProgressBar_Halt == false) { progress++; }
                }
                else if ((temp.Length >= 10) && (temp.Length <= 12) && (temp.Contains("E")) && (temp.Contains(".")))
                {
                    try
                    {
                        Current_Reading = decimal.Parse(temp, System.Globalization.NumberStyles.Float);
                        Invalid_Measured_Curr = false;
                    }
                    catch (Exception) { Current_Reading = 0; Invalid_Measured_Curr = true; }
                    C_Counter++;
                    CurrSamples.Content = C_Counter.ToString() + "C";
                    progress++;
                    if (c_range_auto == true)
                    {
                        if ((Current_Reading > 0) && (Current_Reading < c_range))
                        {
                            Measure_Current.Text = (Math.Round((Current_Reading * 1000), 3, MidpointRounding.AwayFromZero)).ToString() + "mA";
                        }
                        else
                        {
                            Measure_Current.Text = (Math.Round(Current_Reading, 4, MidpointRounding.AwayFromZero)).ToString() + "A";
                        }
                    }
                    else
                    {
                        Measure_Current.Text = (Math.Round(Current_Reading, 4, MidpointRounding.AwayFromZero)).ToString() + "A";
                    }
                }
            }
            else { Current_Reading = 0; Invalid_Measured_Curr = true; }
        }

        public void Measure_Power()
        {
            try
            {
                Power_reading = (Voltage_Reading * Current_Reading);
            }
            catch (Exception) { Power_reading = 0; }
            if (p_range_auto == true)
            {
                if ((Power_reading > 0) && (Power_reading < p_range))
                {
                    Calculate_Power.Text = (Math.Round((Power_reading * 1000), 1, MidpointRounding.AwayFromZero)).ToString() + "mW";
                }
                else
                {
                    Calculate_Power.Text = (Math.Round(Power_reading, 3, MidpointRounding.AwayFromZero)).ToString() + "W";
                }
            }
            else
            {
                Calculate_Power.Text = (Math.Round(Power_reading, 3, MidpointRounding.AwayFromZero)).ToString() + "W";
            }

        }

        public void Check_Output_Status()
        {
            string temp;
            Serial.Write("STAT:OPER:COND?");
            temp = Serial.Read();
            if (temp != "Null")
            {
                switch (temp.TrimEnd())
                {
                    case "+0":
                        CC_CV_Status.FontSize = 60;
                        CC_CV_Status.Text = "Dis";
                        Output_x.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        Output_Box.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        Output_Status.Content = "Output Disabled";
                        Output_Off = true;
                        break;
                    case "+256":
                        CC_CV_Status.FontSize = 60;
                        CC_CV_Status.Text = "CV";
                        Output_x.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        Output_Box.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        Output_Status.Content = "Output Enabled";
                        break;
                    case "+1024":
                        CC_CV_Status.FontSize = 60;
                        CC_CV_Status.Text = "CC";
                        Output_x.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        Output_Box.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        Output_Status.Content = "Output Enabled";
                        break;
                    case "+1280":
                        CC_CV_Status.FontSize = 30;
                        CC_CV_Status.Text = "CVCC";
                        Output_x.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        Output_Box.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        Output_Status.Content = "Output Enabled";
                        break;
                    case "+2048":
                        CC_CV_Status.FontSize = 50;
                        CC_CV_Status.Text = "-CC";
                        Output_x.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        Output_Box.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        Output_Status.Content = "Output Enabled";
                        break;
                    default:
                        CC_CV_Status.FontSize = 60;
                        CC_CV_Status.Text = "?";
                        listboxUpdater(temp + " " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                        Output_x.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        Output_Box.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        Output_Status.Content = "Output Enabled";
                        break;
                }
            }
        }

        public void Check_Protection_Status()
        {
            string temp;
            Serial.Write("STAT:QUES:COND?");
            temp = Serial.Read();
            if (temp != "Null")
            {
                switch (temp.TrimEnd())
                {
                    case "+0":
                        Protect_Ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        break;
                    case "+1":
                        listboxUpdater("OV Tripped " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                        Protect_Ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        Output_Off = true;
                        if (beeper == 1) { Console.Beep(); }
                        break;
                    case "+2":
                        listboxUpdater("OCP Tripped " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                        Protect_Ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        Output_Off = true;
                        if (beeper == 1) { Console.Beep(); }
                        break;
                    case "+4":
                        listboxUpdater("FS Tripped " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                        Protect_Ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        Output_Off = true;
                        if (beeper == 1) { Console.Beep(); }
                        break;
                    case "+16":
                        listboxUpdater("OT Tripped " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                        Protect_Ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        Output_Off = true;
                        if (beeper == 1) { Console.Beep(); }
                        break;
                    default:
                        listboxUpdater(temp + " " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                        Protect_Ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        Output_Off = true;
                        if (beeper == 1) { Console.Beep(); }
                        break;
                }
            }
        }

        public void Setting_Voltage()
        {
            if (Set_V == true)
            {
                if (mV_Range == true)
                {
                    Serial.Write("VOLT " + (Set_voltage_value / 1000).ToString());
                    listboxUpdater("Set " + (Set_voltage_value / 1000) + "V " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                    actual_setVoltage = (Set_voltage_value / 1000);
                    Set_V = false;
                }
                else
                {
                    Serial.Write("VOLT " + Set_voltage_value.ToString());
                    listboxUpdater("Set " + (Set_voltage_value) + "V " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                    actual_setVoltage = (Set_voltage_value);
                    Set_V = false;
                }
            }
        }

        public void Setting_Current()
        {
            if (Set_C == true)
            {
                if (mA_Range == true)
                {
                    Serial.Write("CURR " + (Set_current_value / 1000).ToString());
                    listboxUpdater("Set " + (Set_current_value / 1000) + "A " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                    actual_setCurrent = (Set_current_value / 1000);
                    Set_C = false;
                }
                else
                {
                    Serial.Write("CURR " + Set_current_value.ToString());
                    listboxUpdater("Set " + (Set_current_value) + "A " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                    actual_setCurrent = (Set_current_value);
                    Set_C = false;
                }
            }
        }

        public void Setting_Output()
        {
            if (Output_Set == true)
            {
                if (Output_Off == true)
                {
                    Serial.Write("OUTPut ON");
                    listboxUpdater("Set Output On " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                    Output_x.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                    Output_Box.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                    Output_Status.Content = "Output Enabled";
                    Output_Off = false;
                }
                else
                {
                    Serial.Write("OUTPut OFF");
                    listboxUpdater("Set Output Off " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                    Output_x.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                    Output_Box.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                    Output_Status.Content = "Output Disabled";
                    Output_Off = true;
                }
                Output_Set = false;
            }
        }

        public void Clear_Protection()
        {
            if (Clear_Protect == true)
            {
                Serial.Write("OUTP:PROT:CLE");
                listboxUpdater("Protect Clear  " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                Clear_Protect = false;
            }
        }

        public void Setting_OCP()
        {
            if (Set_OCP == true)
            {
                if (Ready_OCP == false)
                {
                    Serial.Write("CURR:PROT:STAT 1");
                    listboxUpdater("OCP Enabled " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                    Ready_OCP = true;
                    Over_clabel.Text = "Enabled";
                    OCP_Set.Content = "Enabled";
                }
                else
                {
                    Serial.Write("CURR:PROT:STAT 0");
                    listboxUpdater("OCP Disabled " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                    Ready_OCP = false;
                    Over_clabel.Text = "Disabled";
                    OCP_Set.Content = "Disabled";
                }
                Set_OCP = false;
            }
        }

        public void Setting_OVP()
        {
            string temp;
            if (Set_OVP == true)
            {
                try
                {
                    double temp_2 = Convert.ToDouble(OVP.Text);
                    if (temp_2 < maxVoltage)
                    {
                        Serial.Write("VOLT:PROT " + temp_2.ToString());
                        listboxUpdater("Set OVP: " + temp_2.ToString() + "V " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                    }
                    Serial.Write("VOLT:PROT:LEV?");
                    temp = Serial.Read();
                    if (temp != "Null")
                    {
                        try
                        {
                            decimal temp_3 = Decimal.Parse(temp, System.Globalization.NumberStyles.Float);
                            OVP_value = temp_3;
                            Over_vlabel.Text = (Math.Round(temp_3, 2, MidpointRounding.AwayFromZero)).ToString() + "V";
                        }
                        catch (Exception) { Over_vlabel.Text = "?"; listboxUpdater("Got Bad OVP value " + "," + DateTime.Now.ToString("h:mm:ss tt")); }
                    }
                    else { Over_vlabel.Text = "?"; listboxUpdater("Got Bad OVP value " + "," + DateTime.Now.ToString("h:mm:ss tt")); }
                }
                catch (Exception) { listboxUpdater("Invalid OVP value" + "," + DateTime.Now.ToString("h:mm:ss tt")); }
                Set_OVP = false;
            }
        }

        public void Reset_Device()
        {
            if (Reset_Switch == true)
            {
                string temp;
                Serial.Write("*RST");
                listboxUpdater("Send Reset " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                Read_Set_VC = true;
                Measure_Set_VC();
                Read_Set_VC = false;

                Serial.Write("VOLT:PROT:LEV?");
                temp = Serial.Read();
                if (temp != "Null")
                {
                    try
                    {
                        decimal temp_3 = Decimal.Parse(temp, System.Globalization.NumberStyles.Float);
                        OVP_value = temp_3;
                        Over_vlabel.Text = (Math.Round(temp_3, 2, MidpointRounding.AwayFromZero)).ToString() + "V";
                    }
                    catch (Exception) { Over_vlabel.Text = "?"; }
                }
                else { Over_vlabel.Text = "?"; }
                Over_clabel.Text = "Disabled";
                OCP_Set.Content = "Disabled";
                Reset_Switch = false;
            }
        }

        public void Read_Error_FIFO()
        {
            string temp;
            if (Read_Error == true)
            {
                Serial.Write("SYST:ERR?");
                temp = Serial.Read();
                if (temp != "Null")
                {
                    Error_Text.Content = temp;
                    listboxUpdater(temp.TrimEnd() + " " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                }
                else { Error_Text.Content = "?"; }
                Read_Error = false;
            }
        }

        private void Info_Updater()
        {
            Get_Model.Content = ("Model: " + RS232_Info.DeviceID);
            Get_Max_Volt.Content = ("Max Voltage: " + RS232_Info.maxVolt);
            Get_Max_Curr.Content = ("Max Current: " + RS232_Info.maxCurr);
            COM_Config.Content = ("Connection: " + RS232_Info.COM_Port);
            Over_vlabel.Text = RS232_Info.maxOVP + "V";
            Info_Ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
        }

        private void Output_txtSwitch(object sender, RoutedEventArgs e)
        {
            Output_writetxt = !Output_writetxt;
        }
        private void Output_writeTotxt(string data)
        {
            try
            {
                if (Output_writetxt == true)
                {
                    string fileName = DateTime.Now.ToString("yyyy/MM/dd") + "-" + RS232_Info.DeviceID + "-" + RS232_Info.COM_Port + "-Output(date,V,A)" + ".txt";
                    TextWriter datatotxt = new StreamWriter(@fileName, true);
                    datatotxt.WriteLine(data);
                    datatotxt.Close();
                }
            }
            catch (Exception) { Message.Content = "Output text write fail."; }
        }

        private void Output_csvSwitch(object sender, RoutedEventArgs e)
        {
            Output_writecsv = !Output_writecsv;
        }
        private void Output_writeTocsv(string voltage, string current, string date_time)
        {
            try
            {
                if (Output_writecsv == true)
                {
                    string fileName = DateTime.Now.ToString("yyyy/MM/dd") + "-" + RS232_Info.DeviceID + "-" + RS232_Info.COM_Port + "-Output(date,V,A)" + ".csv";
                    TextWriter datatotxt = new StreamWriter(@fileName, true);
                    datatotxt.WriteLine(voltage + "," + current + "," + date_time);
                    datatotxt.Close();
                }
            }
            catch (Exception) { Message.Content = "Output CSV write fail."; }
        }

        private void Status_writeTotxt(string data)
        {
            try
            {
                if (Status_writetxt == true)
                {
                    string fileName = DateTime.Now.ToString("yyyy/MM/dd") + "-" + RS232_Info.DeviceID + "-" + RS232_Info.COM_Port + "-Status" + ".txt";
                    TextWriter datatotxt = new StreamWriter(@fileName, true);
                    datatotxt.WriteLine(data);
                    datatotxt.Close();
                }
            }
            catch (Exception) { Message.Content = "Output text write fail."; }
        }

        private void runtime_Update(object sender, EventArgs e)
        {
            if (Runtime_start == true)
            {
                seconds++;
                TimeSpan time = TimeSpan.FromSeconds(seconds);
                string str = time.ToString(@"hh\:mm\:ss");
                Runtime_timer.Content = str;
                if (str.TrimEnd() == "23:59:59")
                {
                    listboxUpdater("24h Runtime Reset " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                    seconds = 0;
                }
            }
        }

        private void Select_COM_Menu(object sender, RoutedEventArgs e)
        {
            COM_Port.IsEnabled = false;
            Select_COM = new COM_Select_Window();
            if (Select_COM.IsActive == false)
            {
                Select_COM.Show();
            }
        }

        private void Exit_App_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RS232_Info.Connected == true)
                {
                    Serial.CloseCOM();
                }
            }
            catch (Exception) { }
            Application.Current.Shutdown();
        }

        private void Black_Panel_Click(object sender, RoutedEventArgs e)
        {
            White_Panel.IsChecked = false;
            Output_MainPanel.Fill = new SolidColorBrush(Colors.Black);
            Black_Panel.IsChecked = true;
        }

        private void White_Panel_Click(object sender, RoutedEventArgs e)
        {
            Black_Panel.IsChecked = false;
            Output_MainPanel.Fill = new SolidColorBrush(Colors.White);
            White_Panel.IsChecked = true;
        }

        private void Green_Text_Click(object sender, RoutedEventArgs e)
        {
            Green_Text.IsChecked = true;
            Blue_Text.IsChecked = false;
            Red_Text.IsChecked = false;
            Yellow_Text.IsChecked = false;
            Orange_Text.IsChecked = false;
            White_Text.IsChecked = false;
            Black_Text.IsChecked = false;
            Measure_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF17"));
            Measure_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF17"));
            Set_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF17"));
            Set_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF17"));
            CC_CV_Status.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF17"));
            Calculate_Power.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF17"));
            Under.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF17"));
            Over.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF17"));
            Under_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF17"));
            Under_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF17"));
            Over_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF17"));
            Over_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF17"));
        }

        private void Blue_Text_Click(object sender, RoutedEventArgs e)
        {
            Green_Text.IsChecked = false;
            Blue_Text.IsChecked = true;
            Red_Text.IsChecked = false;
            Yellow_Text.IsChecked = false;
            Orange_Text.IsChecked = false;
            White_Text.IsChecked = false;
            Black_Text.IsChecked = false;
            Measure_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00C0FF"));
            Measure_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00C0FF"));
            Set_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00C0FF"));
            Set_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00C0FF"));
            CC_CV_Status.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00C0FF"));
            Calculate_Power.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00C0FF"));
            Under.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00C0FF"));
            Over.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00C0FF"));
            Under_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00C0FF"));
            Under_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00C0FF"));
            Over_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00C0FF"));
            Over_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00C0FF"));
        }

        private void Red_Text_Click(object sender, RoutedEventArgs e)
        {
            Green_Text.IsChecked = false;
            Blue_Text.IsChecked = false;
            Red_Text.IsChecked = true;
            Yellow_Text.IsChecked = false;
            Orange_Text.IsChecked = false;
            White_Text.IsChecked = false;
            Black_Text.IsChecked = false;
            Measure_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            Measure_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            Set_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            Set_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            CC_CV_Status.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            Calculate_Power.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            Under.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            Over.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            Under_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            Under_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            Over_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            Over_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
        }

        private void Yellow_Text_Click(object sender, RoutedEventArgs e)
        {
            Green_Text.IsChecked = false;
            Blue_Text.IsChecked = false;
            Red_Text.IsChecked = false;
            Yellow_Text.IsChecked = true;
            Orange_Text.IsChecked = false;
            White_Text.IsChecked = false;
            Black_Text.IsChecked = false;
            Measure_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF00"));
            Measure_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF00"));
            Set_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF00"));
            Set_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF00"));
            CC_CV_Status.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF00"));
            Calculate_Power.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF00"));
            Under.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF00"));
            Over.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF00"));
            Under_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF00"));
            Under_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF00"));
            Over_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF00"));
            Over_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF00"));
        }

        private void Orange_Text_Click(object sender, RoutedEventArgs e)
        {
            Green_Text.IsChecked = false;
            Blue_Text.IsChecked = false;
            Red_Text.IsChecked = false;
            Yellow_Text.IsChecked = false;
            Orange_Text.IsChecked = true;
            White_Text.IsChecked = false;
            Black_Text.IsChecked = false;
            Measure_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
            Measure_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
            Set_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
            Set_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
            CC_CV_Status.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
            Calculate_Power.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
            Under.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
            Over.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
            Under_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
            Under_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
            Over_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
            Over_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
        }

        private void White_Text_Click(object sender, RoutedEventArgs e)
        {
            Green_Text.IsChecked = false;
            Blue_Text.IsChecked = false;
            Red_Text.IsChecked = false;
            Yellow_Text.IsChecked = false;
            Orange_Text.IsChecked = false;
            White_Text.IsChecked = true;
            Black_Text.IsChecked = false;
            Measure_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            Measure_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            Set_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            Set_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            CC_CV_Status.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            Calculate_Power.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            Under.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            Over.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            Under_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            Under_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            Over_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            Over_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
        }

        private void Black_Text_Click(object sender, RoutedEventArgs e)
        {
            Green_Text.IsChecked = false;
            Blue_Text.IsChecked = false;
            Red_Text.IsChecked = false;
            Yellow_Text.IsChecked = false;
            Orange_Text.IsChecked = false;
            White_Text.IsChecked = false;
            Black_Text.IsChecked = true;
            Measure_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            Measure_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            Set_Voltage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            Set_Current.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            CC_CV_Status.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            Calculate_Power.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            Under.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            Over.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            Under_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            Under_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            Over_vlabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            Over_clabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
        }

        private void clearListbox(object sender, RoutedEventArgs e)
        {
            clearList = !clearList;
            if (clearList == true)
            {
                clearList = false;
                Logger.ItemsSource = null;
                Logger.Items.Clear();
                Clear.IsChecked = false;
            }
        }

        private void switchAutoscroll(object sender, RoutedEventArgs e)
        {
            autoscroll = !autoscroll;
        }
        private void listboxUpdater(string data)
        {
            Status_writeTotxt(data);

            if (UpdateList == true)
            {
                ListBoxItem itm = new ListBoxItem();
                itm.Content = data;
                Logger.Items.Add(itm);
            }

            if (autoscroll == true)
            {
                Logger.Items.MoveCurrentToLast();
                Logger.ScrollIntoView(Logger.Items.CurrentItem);
            }
        }
        private void Voltage_Up_Click(object sender, RoutedEventArgs e)
        {
            double number;
            if (Voltage_Text_Box.Text != "") number = Convert.ToDouble(Voltage_Text_Box.Text);
            else number = 0;
            if (number < upper_V_limit)
                Voltage_Text_Box.Text = Convert.ToString(number + 1);
            Set_voltage_value = Convert.ToDouble(Voltage_Text_Box.Text);
        }

        private void Voltage_Down_Click(object sender, RoutedEventArgs e)
        {
            double number;
            if (Voltage_Text_Box.Text != "") number = Convert.ToDouble(Voltage_Text_Box.Text);
            else number = 0;
            if (number > lower_V_limit)
                Voltage_Text_Box.Text = Convert.ToString(number - 1);
            Set_voltage_value = Convert.ToDouble(Voltage_Text_Box.Text);
        }

        private void Voltage_Input_KeyDOWN(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Up)
            {
                Voltage_Input_UP.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Voltage_Input_UP, new object[] { true });
            }


            if (e.Key == Key.Down)
            {
                Voltage_Input_Down.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Voltage_Input_Down, new object[] { true });
            }
        }

        private void Voltage_Input_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Voltage_Input_UP, new object[] { false });

            if (e.Key == Key.Down)
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Voltage_Input_Down, new object[] { false });
        }

        private void Voltage_Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            double number = 0;
            if (Voltage_Text_Box.Text != "")
                if (!double.TryParse(Voltage_Text_Box.Text, out number)) Voltage_Text_Box.Text = lower_V_limit.ToString();
            if (number > upper_V_limit) Voltage_Text_Box.Text = upper_V_limit.ToString();
            if (number < lower_V_limit) Voltage_Text_Box.Text = lower_V_limit.ToString();
            Voltage_Text_Box.SelectionStart = Voltage_Text_Box.Text.Length;
        }

        private void Clear_Pro_Click(object sender, RoutedEventArgs e)
        {
            Clear_Protect = true;
        }

        private void Current_Up_Click(object sender, RoutedEventArgs e)
        {
            double number;
            if (Current_Text_Box.Text != "") number = Convert.ToDouble(Current_Text_Box.Text);
            else number = 0;
            if (number < upper_A_limit)
                Current_Text_Box.Text = Convert.ToString(number + 1);
            Set_current_value = Convert.ToDouble(Current_Text_Box.Text);
        }

        private void Current_Down_Click(object sender, RoutedEventArgs e)
        {
            double number;
            if (Current_Text_Box.Text != "") number = Convert.ToDouble(Current_Text_Box.Text);
            else number = 0;
            if (number > minCurrent)
                Current_Text_Box.Text = Convert.ToString(number - 1);
            Set_current_value = Convert.ToDouble(Current_Text_Box.Text);
        }

        private void Current_Input_KeyDOWN(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Up)
            {
                Current_Input_Up.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Current_Input_Up, new object[] { true });
            }


            if (e.Key == Key.Down)
            {
                Current_Input_Down.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Current_Input_Down, new object[] { true });
            }
        }

        private void Current_Input_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Current_Input_Up, new object[] { false });

            if (e.Key == Key.Down)
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Current_Input_Down, new object[] { false });
        }

        private void Current_Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            double number = 0;
            if (Current_Text_Box.Text != "")
                if (!double.TryParse(Current_Text_Box.Text, out number)) Current_Text_Box.Text = lower_A_limit.ToString();
            if (number > upper_A_limit) Current_Text_Box.Text = upper_A_limit.ToString();
            if (number < minCurrent) Current_Text_Box.Text = lower_A_limit.ToString();
            Current_Text_Box.SelectionStart = Current_Text_Box.Text.Length;
        }

        private void TextBlock_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Set_voltage_value = Convert.ToDouble(Voltage_Text_Box.Text);
            if (e.Delta > 0)
            {
                if (Set_voltage_value < upper_V_limit)
                {
                    if (mV_Range == true)
                    {
                        switch (V_increment)
                        {
                            case 2:
                                Set_voltage_value = Set_voltage_value + 200;
                                break;
                            case 1:
                                Set_voltage_value = Set_voltage_value + 100;
                                break;
                            case 0.5:
                                Set_voltage_value = Set_voltage_value + 50;
                                break;
                            case 0.25:
                                Set_voltage_value = Set_voltage_value + 25;
                                break;
                            default:
                                Set_voltage_value = Set_voltage_value + 100;
                                break;
                        }
                        if (Set_voltage_value > (maxVoltage * 1000))
                        {
                            Set_voltage_value = (maxVoltage * 1000);
                        }
                    }
                    else
                    {
                        switch (V_increment)
                        {
                            case 2:
                                Set_voltage_value = Set_voltage_value + 2;
                                break;
                            case 1:
                                Set_voltage_value = Set_voltage_value + 1;
                                break;
                            case 0.5:
                                Set_voltage_value = Set_voltage_value + 0.5;
                                break;
                            case 0.25:
                                Set_voltage_value = Set_voltage_value + 0.25;
                                break;
                            default:
                                Set_voltage_value = Set_voltage_value + 1;
                                break;
                        }
                        if (Set_voltage_value > maxVoltage)
                        {
                            Set_voltage_value = maxVoltage;
                        }
                    }
                }
            }
            else
            {
                if (Set_voltage_value > lower_V_limit)

                {
                    if (mV_Range == true)
                    {
                        switch (V_increment)
                        {
                            case 2:
                                Set_voltage_value = Set_voltage_value - 200;
                                break;
                            case 1:
                                Set_voltage_value = Set_voltage_value - 100;
                                break;
                            case 0.5:
                                Set_voltage_value = Set_voltage_value - 50;
                                break;
                            case 0.25:
                                Set_voltage_value = Set_voltage_value - 25;
                                break;
                            default:
                                Set_voltage_value = Set_voltage_value - 100;
                                break;
                        }
                    }
                    else
                    {
                        switch (V_increment)
                        {
                            case 2:
                                Set_voltage_value = Set_voltage_value - 2;
                                break;
                            case 1:
                                Set_voltage_value = Set_voltage_value - 1;
                                break;
                            case 0.5:
                                Set_voltage_value = Set_voltage_value - 0.5;
                                break;
                            case 0.25:
                                Set_voltage_value = Set_voltage_value - 0.25;
                                break;
                            default:
                                Set_voltage_value = Set_voltage_value - 1;
                                break;
                        }
                    }
                }
            }
            if (Set_voltage_value < minVoltage)
            {
                Set_voltage_value = minVoltage;
            }
            Voltage_Text_Box.Text = Convert.ToString(Set_voltage_value);
        }

        private void TextBlock_MouseWheel2(object sender, MouseWheelEventArgs e)
        {
            Set_current_value = Convert.ToDouble(Current_Text_Box.Text);
            if (e.Delta > 0)
            {
                if (Set_current_value < upper_A_limit)
                {
                    if (mA_Range == true)
                    {
                        switch (A_increment)
                        {
                            case 2:
                                Set_current_value = Set_current_value + 200;
                                break;
                            case 1:
                                Set_current_value = Set_current_value + 100;
                                break;
                            case 0.5:
                                Set_current_value = Set_current_value + 50;
                                break;
                            case 0.25:
                                Set_current_value = Set_current_value + 25;
                                break;
                            default:
                                Set_current_value = Set_current_value + 100;
                                break;
                        }
                        if (Set_current_value > (maxCurrent * 1000))
                        {
                            Set_current_value = (maxCurrent * 1000);
                        }
                    }
                    else
                    {
                        switch (A_increment)
                        {
                            case 2:
                                Set_current_value = Set_current_value + 2;
                                break;
                            case 1:
                                Set_current_value = Set_current_value + 1;
                                break;
                            case 0.5:
                                Set_current_value = Set_current_value + 0.5;
                                break;
                            case 0.25:
                                Set_current_value = Set_current_value + 0.25;
                                break;
                            default:
                                Set_current_value = Set_current_value + 1;
                                break;
                        }
                        if (Set_current_value > maxCurrent)
                        {
                            Set_current_value = maxCurrent;
                        }
                    }
                }
            }
            else
            {
                if (Set_current_value > lower_A_limit)

                {
                    if (mA_Range == true)
                    {
                        switch (A_increment)
                        {
                            case 2:
                                Set_current_value = Set_current_value - 200;
                                break;
                            case 1:
                                Set_current_value = Set_current_value - 100;
                                break;
                            case 0.5:
                                Set_current_value = Set_current_value - 50;
                                break;
                            case 0.25:
                                Set_current_value = Set_current_value - 25;
                                break;
                            default:
                                Set_current_value = Set_current_value - 100;
                                break;
                        }
                    }
                    else
                    {
                        switch (A_increment)
                        {
                            case 2:
                                Set_current_value = Set_current_value - 2;
                                break;
                            case 1:
                                Set_current_value = Set_current_value - 1;
                                break;
                            case 0.5:
                                Set_current_value = Set_current_value - 0.5;
                                break;
                            case 0.25:
                                Set_current_value = Set_current_value - 0.25;
                                break;
                            default:
                                Set_current_value = Set_current_value - 1;
                                break;
                        }
                    }
                }
            }
            if (Set_current_value < minCurrent)
            {
                Set_current_value = minCurrent;
            }
            Current_Text_Box.Text = Convert.ToString(Set_current_value);
        }

        private void Set_Update_Rate(object sender, RoutedEventArgs e)
        {
            string time = Update_Rate.Text;
            try
            {
                UpdateRate = Convert.ToDouble(time);
                updater.Interval = TimeSpan.FromSeconds(UpdateRate);
                listboxUpdater("Update Rate " + UpdateRate + "s " + "," + DateTime.Now.ToString("h:mm:ss tt"));
            }
            catch (Exception) { listboxUpdater("Invalid Update Rate " + "," + DateTime.Now.ToString("h:mm:ss tt")); }
        }

        private void Voltage_Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Set_voltage_value = Convert.ToDouble(Voltage_Text_Box.Text);
                Set_voltage_value = Math.Round(Set_voltage_value, 3, MidpointRounding.AwayFromZero);
                Voltage_Text_Box.Text = Set_voltage_value.ToString();

                if (mV_Range == true)
                {
                    Set_voltage_value = Math.Round(Set_voltage_value, 0, MidpointRounding.AwayFromZero);
                    Voltage_Text_Box.Text = Set_voltage_value.ToString();
                    Set_Voltage.Text = "Set: " + Set_voltage_value + "mV";
                }
                else
                {
                    Set_Voltage.Text = "Set: " + Set_voltage_value + "V";
                }
                Set_V = true;
            }
        }

        private void Enter_V_Click(object sender, RoutedEventArgs e)
        {
            Set_voltage_value = Convert.ToDouble(Voltage_Text_Box.Text);
            Set_voltage_value = Math.Round(Set_voltage_value, 3, MidpointRounding.AwayFromZero);
            Voltage_Text_Box.Text = Set_voltage_value.ToString();

            if (mV_Range == true)
            {
                Set_voltage_value = Math.Round(Set_voltage_value, 0, MidpointRounding.AwayFromZero);
                Voltage_Text_Box.Text = Set_voltage_value.ToString();
                Set_Voltage.Text = "Set: " + Set_voltage_value + "mV";
            }
            else
            {
                Set_Voltage.Text = "Set: " + Set_voltage_value + "V";
            }
            Set_V = true;
        }

        private void Current_Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Set_current_value = Convert.ToDouble(Current_Text_Box.Text);
                Set_current_value = Math.Round(Set_current_value, 4, MidpointRounding.AwayFromZero);
                Current_Text_Box.Text = Set_current_value.ToString();

                if (mA_Range == true)
                {
                    Set_current_value = Math.Round(Set_current_value, 1, MidpointRounding.AwayFromZero);
                    Current_Text_Box.Text = Set_current_value.ToString();
                    Set_Current.Text = "Set: " + Set_current_value + "mA";
                }
                else
                {
                    Set_Current.Text = "Set: " + Set_current_value + "A";
                }
                Set_C = true;
            }
        }

        private void Enter_C_Click(object sender, RoutedEventArgs e)
        {
            Set_current_value = Convert.ToDouble(Current_Text_Box.Text);
            Set_current_value = Math.Round(Set_current_value, 4, MidpointRounding.AwayFromZero);
            Current_Text_Box.Text = Set_current_value.ToString();

            if (mA_Range == true)
            {
                Set_current_value = Math.Round(Set_current_value, 1, MidpointRounding.AwayFromZero);
                Current_Text_Box.Text = Set_current_value.ToString();
                Set_Current.Text = "Set: " + Set_current_value + "mA";
            }
            else
            {
                Set_Current.Text = "Set: " + Set_current_value + "A";
            }
            Set_C = true;
        }

        private void UVP_Set_Click(object sender, RoutedEventArgs e)
        {
            Set_UVP = true;
            try
            {
                decimal temp = Convert.ToDecimal(UVP.Text);
                if ((temp < (decimal)maxVoltage) && (temp > 0))
                {
                    UVP_value = Math.Round(temp, 3, MidpointRounding.AwayFromZero);
                    Under_vlabel.Text = UVP_value.ToString() + "V";
                    listboxUpdater("UVP Set: " + UVP_value.ToString() + "V " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                }
            }
            catch (Exception)
            {
                Message.Content = "Error: Invalid UVP value";
                UVP_value = 0;
                Under_vlabel.Text = "Dis";
            }
        }

        private void UVP_Clear_Click(object sender, RoutedEventArgs e)
        {
            Set_UVP = false;
            UVP_value = 0;
            Under_vlabel.Text = "Dis";
            listboxUpdater("UVP Cleared " + "," + DateTime.Now.ToString("h:mm:ss tt"));
        }

        private void UCP_Set_Click(object sender, RoutedEventArgs e)
        {
            Set_UCP = true;
            try
            {
                decimal temp = Convert.ToDecimal(UCP.Text);
                if ((temp < (decimal)maxCurrent) && (temp > 0))
                {
                    UCP_value = Math.Round(temp, 3, MidpointRounding.AwayFromZero);
                    Under_clabel.Text = UCP_value.ToString() + "A";
                    listboxUpdater("UVP Set: " + UCP_value.ToString() + "A " + "," + DateTime.Now.ToString("h:mm:ss tt"));
                }
            }
            catch (Exception)
            {
                Message.Content = "Error: Invalid UCP value";
                UCP_value = 0;
                Under_clabel.Text = "Dis";
            }
        }

        private void UCP_Clear_Click(object sender, RoutedEventArgs e)
        {
            Set_UCP = false;
            UCP_value = 0;
            Under_clabel.Text = "Dis";
            listboxUpdater("UVP Cleared " + "," + DateTime.Now.ToString("h:mm:ss tt"));
        }

        private void Output_On_Off_Click(object sender, RoutedEventArgs e)
        {
            Output_Set = true;
        }

        private void OVP_Set_Click(object sender, RoutedEventArgs e)
        {
            Set_OVP = true;
        }

        private void OCP_Set_Click(object sender, RoutedEventArgs e)
        {
            Set_OCP = true;
        }

        private void Text_Send_Click(object sender, RoutedEventArgs e)
        {
            Display_text = true;
        }

        private void Display_On_Off_Click(object sender, RoutedEventArgs e)
        {
            Display_Switch = true;
        }

        private void Get_Error_Click(object sender, RoutedEventArgs e)
        {
            Read_Error = true;
        }

        private void VRange_DropDownClosed(object sender, EventArgs e)
        {
            string temp = "";
            temp = VRange.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            switch (temp)
            {
                case "mV":
                    mV_Range = true;
                    upper_V_limit = (maxVoltage * 1000);
                    lower_V_limit = 0;
                    Voltage_Text_Box.Text = "0";
                    Set_voltage_value = 0;
                    Message.Content = "Voltage unit set to mV";
                    break;
                case "V":
                    mV_Range = false;
                    upper_V_limit = (maxVoltage);
                    lower_V_limit = 0;
                    Voltage_Text_Box.Text = "0";
                    Set_voltage_value = 0;
                    Message.Content = "Voltage unit set to V";
                    break;
                default:
                    mV_Range = false;
                    upper_V_limit = (maxVoltage);
                    lower_V_limit = 0;
                    Voltage_Text_Box.Text = "0";
                    Set_voltage_value = 0;
                    Message.Content = "Voltage unit set to V";
                    break;
            }
        }


        private void VIncrement_DropDownClosed(object sender, EventArgs e)
        {
            string temp = "";
            temp = VIncrement.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            switch (temp)
            {
                case "2":
                    V_increment = 2;
                    Message.Content = "Volt increment by 2";
                    break;
                case "1":
                    V_increment = 1;
                    Message.Content = "Volt increment by 1";
                    break;
                case "0.5":
                    V_increment = 0.5;
                    Message.Content = "Volt increment by 0.5";
                    break;
                case "0.25":
                    V_increment = 0.25;
                    Message.Content = "Volt increment by 0.25";
                    break;
                default:
                    V_increment = 1;
                    Message.Content = "Volt increment by 1";
                    break;
            }
        }


        private void ARange_DropDownClosed(object sender, EventArgs e)
        {
            string temp = "";
            temp = ARange.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            switch (temp)
            {
                case "mA":
                    mA_Range = true;
                    upper_A_limit = (maxCurrent * 1000);
                    lower_A_limit = 0;
                    Current_Text_Box.Text = "0";
                    Set_current_value = 0;
                    Message.Content = "Current unit set to mA";
                    break;
                case "A":
                    mA_Range = false;
                    upper_A_limit = maxCurrent;
                    lower_A_limit = 0;
                    Current_Text_Box.Text = "0";
                    Set_current_value = 0;
                    Message.Content = "Current unit set to A";
                    break;
                default:
                    mA_Range = false;
                    upper_A_limit = maxCurrent;
                    lower_A_limit = 0;
                    Current_Text_Box.Text = "0";
                    Set_current_value = 0;
                    Message.Content = "Current unit set to A";
                    break;
            }
        }


        private void AIncrement_DropDownClosed(object sender, EventArgs e)
        {
            string temp = "";
            temp = AIncrement.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            switch (temp)
            {
                case "2":
                    A_increment = 2;
                    Message.Content = "Curr increment by 2";
                    break;
                case "1":
                    A_increment = 1;
                    Message.Content = "Curr increment by 1";
                    break;
                case "0.5":
                    A_increment = 0.5;
                    Message.Content = "Curr increment by 0.5";
                    break;
                case "0.25":
                    A_increment = 0.25;
                    Message.Content = "Curr increment by 0.25";
                    break;
                default:
                    A_increment = 1;
                    Message.Content = "Curr increment by 1";
                    break;
            }
        }


        private void Beeper_DropDownClosed(object sender, EventArgs e)
        {
            string temp = "";
            temp = Beeper.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            switch (temp)
            {
                case "Yes":
                    beeper = 1;
                    Message.Content = "Beeper Enabled";
                    break;
                case "No":
                    beeper = 0;
                    Message.Content = "Beeper Disabled";
                    break;
                default:
                    beeper = 0;
                    Message.Content = "Beeper Disabled";
                    break;
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Reset_Switch = true;
        }

        private void ListUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateList = !UpdateList;
        }

        private void Text_Clear_Click(object sender, RoutedEventArgs e)
        {
            Display_Clear = true;
        }

        private void Status_Text_File_Click(object sender, RoutedEventArgs e)
        {
            Status_writetxt = !Status_writetxt;
        }

        private void VRange_2(object sender, RoutedEventArgs e)
        {
            VDis.IsChecked = false;
            VR2.IsChecked = true;
            VR4.IsChecked = false;
            VR5.IsChecked = false;
            VR8.IsChecked = false;
            VR1.IsChecked = false;
            v_range_auto = true;
            v_range = 0.2M;
        }
        private void VRange_4(object sender, RoutedEventArgs e)
        {
            VDis.IsChecked = false;
            VR2.IsChecked = false;
            VR4.IsChecked = true;
            VR5.IsChecked = false;
            VR8.IsChecked = false;
            VR1.IsChecked = false;
            v_range_auto = true;
            v_range = 0.4M;
        }
        private void VRange_5(object sender, RoutedEventArgs e)
        {
            VDis.IsChecked = false;
            VR2.IsChecked = false;
            VR4.IsChecked = false;
            VR5.IsChecked = true;
            VR8.IsChecked = false;
            VR1.IsChecked = false;
            v_range_auto = true;
            v_range = 0.5M;
        }
        private void VRange_8(object sender, RoutedEventArgs e)
        {
            VDis.IsChecked = false;
            VR2.IsChecked = false;
            VR4.IsChecked = false;
            VR5.IsChecked = false;
            VR8.IsChecked = true;
            VR1.IsChecked = false;
            v_range_auto = true;
            v_range = 0.8M;
        }
        private void VRange_1(object sender, RoutedEventArgs e)
        {
            VDis.IsChecked = false;
            VR2.IsChecked = false;
            VR4.IsChecked = false;
            VR5.IsChecked = false;
            VR8.IsChecked = false;
            VR1.IsChecked = true;
            v_range_auto = true;
            v_range = 1;
        }

        private void VRang_Dis(object sender, RoutedEventArgs e)
        {
            VDis.IsChecked = true;
            VR2.IsChecked = false;
            VR4.IsChecked = false;
            VR5.IsChecked = false;
            VR8.IsChecked = false;
            VR1.IsChecked = false;
            v_range_auto = false;
            v_range = 0;
        }

        private void CRange_Dis(object sender, RoutedEventArgs e)
        {
            CDis.IsChecked = true;
            AR2.IsChecked = false;
            AR4.IsChecked = false;
            AR5.IsChecked = false;
            AR8.IsChecked = false;
            AR1.IsChecked = false;
            c_range_auto = false;
            c_range = 0;
        }

        private void CRange_2(object sender, RoutedEventArgs e)
        {
            CDis.IsChecked = false;
            AR2.IsChecked = true;
            AR4.IsChecked = false;
            AR5.IsChecked = false;
            AR8.IsChecked = false;
            AR1.IsChecked = false;
            c_range_auto = true;
            c_range = 0.2M;
        }

        private void CRange_4(object sender, RoutedEventArgs e)
        {
            CDis.IsChecked = false;
            AR2.IsChecked = false;
            AR4.IsChecked = true;
            AR5.IsChecked = false;
            AR8.IsChecked = false;
            AR1.IsChecked = false;
            c_range_auto = true;
            c_range = 0.4M;
        }

        private void CRange_5(object sender, RoutedEventArgs e)
        {
            CDis.IsChecked = false;
            AR2.IsChecked = false;
            AR4.IsChecked = false;
            AR5.IsChecked = true;
            AR8.IsChecked = false;
            AR1.IsChecked = false;
            c_range_auto = true;
            c_range = 0.5M;
        }

        private void CRange_8(object sender, RoutedEventArgs e)
        {
            CDis.IsChecked = false;
            AR2.IsChecked = false;
            AR4.IsChecked = false;
            AR5.IsChecked = false;
            AR8.IsChecked = true;
            AR1.IsChecked = false;
            c_range_auto = true;
            c_range = 0.8M;
        }

        private void CRange_1(object sender, RoutedEventArgs e)
        {
            CDis.IsChecked = false;
            AR2.IsChecked = false;
            AR4.IsChecked = false;
            AR5.IsChecked = false;
            AR8.IsChecked = false;
            AR1.IsChecked = true;
            c_range_auto = true;
            c_range = 1;
        }

        private void PRange_Dis(object sender, RoutedEventArgs e)
        {
            PDis.IsChecked = true;
            PR2.IsChecked = false;
            PR4.IsChecked = false;
            PR5.IsChecked = false;
            PR8.IsChecked = false;
            PR1.IsChecked = false;
            p_range_auto = false;
            p_range = 0;

        }

        private void PRange_2(object sender, RoutedEventArgs e)
        {
            PDis.IsChecked = false;
            PR2.IsChecked = true;
            PR4.IsChecked = false;
            PR5.IsChecked = false;
            PR8.IsChecked = false;
            PR1.IsChecked = false;
            p_range_auto = true;
            p_range = 0.2M;
        }

        private void PRange_4(object sender, RoutedEventArgs e)
        {
            PDis.IsChecked = false;
            PR2.IsChecked = false;
            PR4.IsChecked = true;
            PR5.IsChecked = false;
            PR8.IsChecked = false;
            PR1.IsChecked = false;
            p_range_auto = true;
            p_range = 0.4M;
        }

        private void PRange_5(object sender, RoutedEventArgs e)
        {
            PDis.IsChecked = false;
            PR2.IsChecked = false;
            PR4.IsChecked = false;
            PR5.IsChecked = true;
            PR8.IsChecked = false;
            PR1.IsChecked = false;
            p_range_auto = true;
            p_range = 0.5M;
        }

        private void PRange_8(object sender, RoutedEventArgs e)
        {
            PDis.IsChecked = false;
            PR2.IsChecked = false;
            PR4.IsChecked = false;
            PR5.IsChecked = false;
            PR8.IsChecked = true;
            PR1.IsChecked = false;
            p_range_auto = true;
            p_range = 0.8M;
        }
        private void PRange_1(object sender, RoutedEventArgs e)
        {
            PDis.IsChecked = false;
            PR2.IsChecked = false;
            PR4.IsChecked = false;
            PR5.IsChecked = false;
            PR8.IsChecked = false;
            PR1.IsChecked = true;
            p_range_auto = true;
            p_range = 1;
        }

        private void Set_Device_C_Range_Low(object sender, RoutedEventArgs e)
        {
            Low_Range.IsChecked = true;
            High_Range.IsChecked = false;
            Set_C_Range_Low = true;
            Set_C_Range_High = false;
            Allow_Range_Switch = true;
        }

        private void Set_Device_C_Range_High(object sender, RoutedEventArgs e)
        {
            Low_Range.IsChecked = false;
            High_Range.IsChecked = true;
            Set_C_Range_Low = false;
            Set_C_Range_High = true;
            Allow_Range_Switch = true;
        }

        private void Get_Voltage_Click(object sender, RoutedEventArgs e)
        {
            MeasureV = !MeasureV;
        }

        private void Get_Current_Click(object sender, RoutedEventArgs e)
        {
            MeasureC = !MeasureC;
        }

        private void Get_CVCC_Status_Click(object sender, RoutedEventArgs e)
        {
            GetStatus = !GetStatus;
        }

        private void Get_Protect_Status_Click(object sender, RoutedEventArgs e)
        {
            GetProtect = !GetProtect;
        }

        private void Get_SetVC_Click(object sender, RoutedEventArgs e)
        {
            GetSetVC = !GetSetVC;
        }

        private void General_Help_Click(object sender, RoutedEventArgs e)
        {
            GeneralHelp = new General_Help_Window();
            if (GeneralHelp.IsActive == false)
            {
                GeneralHelp.Show();
            }
        }

        private void RS232_Help_Click(object sender, RoutedEventArgs e)
        {
            Serial_Help = new RS232_Help();
            if (Serial_Help.IsActive == false)
            {
                Serial_Help.Show();
            }
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            About_Me = new Credits();
            if (About_Me.IsActive == false)
            {
                About_Me.Show();
            }
        }

        private void Reset_time_Click(object sender, RoutedEventArgs e)
        {
            try { seconds = 0; } catch (Exception) { }
        }

        private void Reset_vSamples_Click(object sender, RoutedEventArgs e)
        {
            try { V_Counter = 0; } catch (Exception) { }
        }

        private void Reset_cSamples_Click(object sender, RoutedEventArgs e)
        {
            try { C_Counter = 0; } catch (Exception) { }
        }

        private void Samples100_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar_Halt = true;
            try
            {
                ProgressBar.Maximum = 100;
                ProgressBar.Minimum = 0;
                ProgressBar.Value = 0;
                progress = 0;
                Samples100.IsChecked = true;
                Samples1000.IsChecked = false;
                Samples10000.IsChecked = false;
            }
            catch (Exception) { }
            ProgressBar_Halt = false;
        }

        private void Samples1000_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar_Halt = true;
            try
            {
                ProgressBar.Maximum = 1000;
                ProgressBar.Minimum = 0;
                ProgressBar.Value = 0;
                progress = 0;
                Samples100.IsChecked = false;
                Samples1000.IsChecked = true;
                Samples10000.IsChecked = false;
            }
            catch (Exception) { }
            ProgressBar_Halt = false;
        }

        private void Samples10000_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar_Halt = true;
            try
            {
                ProgressBar.Maximum = 10000;
                ProgressBar.Minimum = 0;
                ProgressBar.Value = 0;
                progress = 0;
                Samples100.IsChecked = false;
                Samples1000.IsChecked = false;
                Samples10000.IsChecked = true;
            }
            catch (Exception) { }
            ProgressBar_Halt = false;
        }

        private void Add_Meas_VC_Click(object sender, RoutedEventArgs e)
        {
            Add_MeasuredVC_List = !Add_MeasuredVC_List;
        }

        private void Get_SetOV_SetOC_Click(object sender, RoutedEventArgs e)
        {
            Get_OVP_value_OCP_state = !Get_OVP_value_OCP_state;
        }

        private void VSamples_50K_Click(object sender, RoutedEventArgs e)
        {
            activateVoltGraph(50_000);
        }

        private void VSamples_100K_Click(object sender, RoutedEventArgs e)
        {
            activateVoltGraph(100_000);
        }

        private void VSamples_200K_Click(object sender, RoutedEventArgs e)
        {
            activateVoltGraph(200_000);
        }

        private void VSamples_500k_Click(object sender, RoutedEventArgs e)
        {
            activateVoltGraph(500_000);
        }

        private void VSamples_1M_Click(object sender, RoutedEventArgs e)
        {
            activateVoltGraph(1_000_000);
        }

        private void VSamples_2M_Click(object sender, RoutedEventArgs e)
        {
            activateVoltGraph(2_000_000);
        }

        private void activateVoltGraph(int max_Samples)
        {
            V_Plot = new Voltage_Graph(max_Samples);
            if (V_Plot.IsActive == false)
            {
                V_Plot.Show();
                V_Graph.IsEnabled = false;
                V_Plot.min_read_voltage = maxVoltage;
                isVoltGraph = true;
            }
        }

        private void CSamples_50K_Click(object sender, RoutedEventArgs e)
        {
            activateCurrGraph(50_000);
        }

        private void CSamples_100K_Click(object sender, RoutedEventArgs e)
        {
            activateCurrGraph(100_000);
        }

        private void CSamples_200K_Click(object sender, RoutedEventArgs e)
        {
            activateCurrGraph(200_000);
        }

        private void CSamples_500k_Click(object sender, RoutedEventArgs e)
        {
            activateCurrGraph(500_000);
        }

        private void CSamples_1M_Click(object sender, RoutedEventArgs e)
        {
            activateCurrGraph(1_000_000);
        }

        private void CSamples_2M_Click(object sender, RoutedEventArgs e)
        {
            activateCurrGraph(2_000_000);
        }

        private void activateCurrGraph(int max_Samples)
        {
            C_Plot = new Current_Graph(max_Samples);
            if (C_Plot.IsActive == false)
            {
                C_Plot.Show();
                C_Graph.IsEnabled = false;
                C_Plot.min_read_current = maxCurrent;
                isCurrGraph = true;
            }
        }

        private void PSamples_50K_Click(object sender, RoutedEventArgs e)
        {
            activatePowerGraph(50_000);
        }

        private void PSamples_100K_Click(object sender, RoutedEventArgs e)
        {
            activatePowerGraph(100_000);
        }

        private void PSamples_200K_Click(object sender, RoutedEventArgs e)
        {
            activatePowerGraph(200_000);
        }

        private void PSamples_500k_Click(object sender, RoutedEventArgs e)
        {
            activatePowerGraph(500_000);
        }

        private void PSamples_1M_Click(object sender, RoutedEventArgs e)
        {
            activatePowerGraph(1_000_000);
        }

        private void PSamples_2M_Click(object sender, RoutedEventArgs e)
        {
            activatePowerGraph(2_000_000);
        }

        private void activatePowerGraph(int max_Samples)
        {
            P_Plot = new Power_Graph(max_Samples);
            if (P_Plot.IsActive == false)
            {
                P_Plot.Show();
                P_Graph.IsEnabled = false;
                P_Plot.min_read_power = (maxVoltage * maxCurrent);
                isPowerGraph = true;
            }
        }
    }
}
