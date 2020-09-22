using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Agilent_6632B
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class COM_Select_Window : Window
    {
        string COM_Port = "";
        public int BaudRate = 9600;
        public int Parity = 0;
        public int StopBits = 1;
        public int DataBits = 8;
        public int Handshake = 0;
        public int WriteTimeout = 2000;
        public int ReadTimeout = 2000;
        public bool RtsEnable = false;
        public SerialPort sp;

        List<string> portList;
        public COM_Select_Window()
        {
            InitializeComponent();
            Get_COM_List();
        }

        private void Get_COM_List()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                var portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());
                portList = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains('(' + n + ')'))).ToList();
                foreach (string p in portList)
                {
                    updateList(p);
                }
            }
        }

        private void updateList(string data)
        {
            ListBoxItem itm = new ListBoxItem();
            itm.Content = data;
            COM_List.Items.Add(itm);
        }

        private void COM_Refresh_Click(object sender, RoutedEventArgs e)
        {
            COM_List.Items.Clear();
            Get_COM_List();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            COM_Port = COM_Number.Text;
            using (var sp = new SerialPort(COM_Port, BaudRate, (Parity)Parity, DataBits, (StopBits)StopBits))
            {
                sp.ReadTimeout = ReadTimeout;
                sp.WriteTimeout = WriteTimeout;
                sp.Handshake = (Handshake)Handshake;
                try
                {
                    sp.Open();
                }
                catch (Exception)
                {
                    Message.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
                    Message.Text = "Error: COM Port in use or not exists.";
                }
                try
                {
                    sp.WriteLine("SYST:LANG SCPI");
                    sp.WriteLine("*IDN?");
                    string temp = sp.ReadLine();
                    RS232_Info.DeviceID = "HP" + temp.Substring(temp.IndexOf(",") + 1, 5);

                    sp.WriteLine("VOLT? MAX");
                    temp = sp.ReadLine();
                    RS232_Info.maxVolt = Double.Parse(temp, System.Globalization.NumberStyles.Float).ToString();

                    sp.WriteLine("CURR? MAX");
                    temp = sp.ReadLine();
                    RS232_Info.maxCurr = Double.Parse(temp, System.Globalization.NumberStyles.Float).ToString();

                    //sp.WriteLine("*RST\r\n"); No longer required!

                    sp.WriteLine("VOLT:PROT:LEV? MAX");
                    temp = sp.ReadLine();
                    RS232_Info.maxOVP = Double.Parse(temp, System.Globalization.NumberStyles.Float).ToString();

                    sp.WriteLine("DISPLAY:MODE NORM");

                    sp.Close();
                    RS232_Info.COM_Port = COM_Port;
                    RS232_Info.BaudRate = BaudRate;
                    RS232_Info.Parity = Parity;
                    RS232_Info.StopBits = StopBits;
                    RS232_Info.DataBits = DataBits;
                    RS232_Info.Handshake = Handshake;
                    RS232_Info.WriteTimeout = WriteTimeout;
                    RS232_Info.ReadTimeout = ReadTimeout;
                    RS232_Info.RtsEnable = RtsEnable;
                    RS232_Info.Connected = true;
                    close_window();
                }
                catch (Exception)
                {
                    Message.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
                    Message.Text = "Error: Device not found/cannot read device.";
                    sp.Close();
                }
            }
        }

        private void close_window()
        {
            this.Close();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            COM_Port = COM_Number.Text;
            using (var sp = new SerialPort(COM_Port, BaudRate, (Parity)Parity, DataBits, (StopBits)StopBits))
            {
                sp.ReadTimeout = ReadTimeout;
                sp.WriteTimeout = WriteTimeout;
                sp.Handshake = (Handshake)Handshake;
                try
                {
                    sp.Open();
                }
                catch (Exception)
                {
                    Message.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
                    Message.Text = "Error: COM Port in use or not exists.";
                }
                try
                {
                    sp.WriteLine("SYST:LANG SCPI");
                    sp.WriteLine("*rst");
                    Message.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF008000"));
                    Message.Text = "Reset command send.";
                    sp.Close();
                }
                catch (Exception)
                {
                    Message.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
                    Message.Text = "Error: Device not found/cannot reset device.";
                    sp.Close();
                }
            }
        }

        private void Get_Info_Click(object sender, RoutedEventArgs e)
        {
            COM_Port = COM_Number.Text;
            using (var sp = new SerialPort(COM_Port, BaudRate, (Parity)Parity, DataBits, (StopBits)StopBits))
            {
                sp.ReadTimeout = ReadTimeout;
                sp.WriteTimeout = WriteTimeout;
                sp.Handshake = (Handshake)Handshake;
                try
                {
                    sp.Open();
                }
                catch (Exception)
                {
                    Message.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
                    Message.Text = "Error: COM Port in use or not exists.";
                }
                try
                {
                    sp.WriteLine("SYST:LANG SCPI");
                    sp.WriteLine("*IDN?");
                    string temp = sp.ReadLine();
                    Message.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF008000"));
                    Message.Text = temp;
                    sp.WriteLine("DISPLAY:MODE TEXT");
                    sp.WriteLine("DISP:TEXT " + "'" + COM_Number.Text + "'");
                    sp.Close();
                }
                catch (Exception)
                {
                    Message.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
                    Message.Text = "Error: Device not found/cannot read device.";
                    sp.Close();
                }
            }
        }

        private void COM_List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Message.Text = "";
            string temp = COM_List.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            string COM = temp.Substring(0, temp.IndexOf(" -"));
            COM_Number.Text = COM;
            COM_Port = COM;
            try
            {
                using (var sp = new SerialPort(COM, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One))
                {
                    sp.Open();
                    sp.Close();
                    Message.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF008000"));
                    Message.Text = (COM + " is ready. Click Device Info button.");
                }
            }
            catch (Exception)
            {
                Message.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
                Message.Text = ("Error: " + COM + " is already in use.");
            }
        }

        private void baudRate_DropDownClosed(object sender, EventArgs e)
        {
            string temp = "";
            temp = baudRate.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            BaudRate = Int32.Parse(temp);
        }

        private void bits_DropDownClosed(object sender, EventArgs e)
        {
            string temp = "";
            temp = bits.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            DataBits = Int32.Parse(temp);
        }

        private void parity_DropDownClosed(object sender, EventArgs e)
        {
            string temp = "";
            temp = parity.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            switch (temp)
            {
                case "Even":
                    Parity = 2;
                    break;
                case "Odd":
                    Parity = 1;
                    break;
                case "None":
                    Parity = 0;
                    break;
                case "Mark":
                    Parity = 3;
                    break;
                case "Space":
                    Parity = 4;
                    break;
                default:
                    Parity = 0;
                    break;
            }
        }

        private void Stop_DropDownClosed(object sender, EventArgs e)
        {
            string temp = "";
            temp = Stop.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            switch (temp)
            {
                case "1":
                    StopBits = 1;
                    break;
                case "1.5":
                    StopBits = 3;
                    break;
                case "2":
                    StopBits = 2;
                    break;
                default:
                    Parity = 1;
                    break;
            }
        }

        private void flow_DropDownClosed(object sender, EventArgs e)
        {
            string temp = "";
            temp = flow.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            switch (temp)
            {
                case "Xon/Xoff":
                    Handshake = 1;
                    break;
                case "Hardware":
                    Handshake = 2;
                    break;
                case "None":
                    Handshake = 0;
                    break;
                default:
                    Handshake = 1;
                    break;
            }
        }
    }
}
