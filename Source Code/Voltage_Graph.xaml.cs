using ScottPlot;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Agilent_6632B
{
    /// <summary>
    /// Interaction logic for Voltage_Graph.xaml
    /// </summary>
    public partial class Voltage_Graph : Window
    {

        Boolean Enabler_AutoAxis = true;
        Boolean MouseTracker = false;

        Boolean titleUpdater = false;

        double current_voltage;
        double max_read_voltage = 0;
        public double min_read_voltage;

        double setVoltage = 0;
        double setOvervolt = 0;
        double setUndervolt = 0;

        int negative_Samples;
        int max_samples_allowed;

        public double[] Samples;
        public string[] Dates;
        public int[] Intstatus;

        int SampleCounter = 0;
        int Total_sample;

        PlottableSignal VoltagePlot;
        PlottableHLine set_VLine, set_OLine, set_ULine;

        PlottableVLine xLine;

        Boolean isOnceV = false;
        Boolean isOnceO = false;
        Boolean isOnceU = false;

        Boolean isOnceMouse = false;

        SolidColorBrush Disable = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
        SolidColorBrush Enable = new SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 139, 34));

        public Voltage_Graph(int samples)
        {
            InitializeComponent();
            Init_Sample_Arrays(samples);
            Initialize_Graph();

            max_samples_allowed = samples;
            MaxSamples.Content = samples.ToString();
        }

        public void Init_Sample_Arrays(int maxSamples)
        {
            Samples = new double[maxSamples];
            Dates = new string[maxSamples];
            Intstatus = new int[maxSamples];
        }

        public void Initialize_Graph()
        {
            VoltagePlot = V_Graph.plt.PlotSignal(Samples);

            V_Graph.plt.YLabel("Voltage ( V )");
            V_Graph.plt.XLabel("Sample Number");

            VoltagePlot.color = ColorTranslator.FromHtml("#FF00C0FF");
            System.Drawing.Pen Line = new System.Drawing.Pen(System.Drawing.Brushes.LightSkyBlue);
            VoltagePlot.penLD = Line;

        }

        public void Manually_Graph_Updater()
        {

            if (Enabler_AutoAxis == true)
            {
                V_Graph.plt.AxisAuto();
            }

            V_Graph.Render(skipIfCurrentlyRendering: true, lowQuality: true);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                V_Graph.plt.Clear();
                dataShare.isVoltGraphClosed = true;
                this.Close();
            }
            catch (Exception) { }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                V_Graph.plt.Clear();
                dataShare.isVoltGraphClosed = true;
                this.Close();
            }
            catch (Exception) { }
        }

        public void dataPasser(double measuredVoltage, double setVoltage, double setOVP, double setUVP, string status, string date)
        {

            if (Total_sample != max_samples_allowed)
            {
                Total_sample++;
                TotalSamples.Content = Total_sample.ToString();

                if (measuredVoltage < 0) { negative_Samples++; NegativeSamples.Content = negative_Samples.ToString(); }

                currentV(measuredVoltage);
                setVolt(setVoltage);
                setOVP_Method(setOVP);
                setUVP_Method(setUVP);
                maxVolt();
                minVolt();
                storeData(status, date);
                Manually_Graph_Updater();
                Draw_setVolt_marker(setVoltage);
                Draw_setOVP_marker(setOVP);
                Draw_setUVP_marker(setUVP);
                if (titleUpdater == false)
                {
                    TitleUpdater();
                }
            }
            else { TotalSamples.Content = "Max Samples Reached!"; }
        }

        public void TitleUpdater()
        {
            this.Title = RS232_Info.DeviceID + " " + RS232_Info.COM_Port + " Voltage Graph";
            titleUpdater = true;
        }

        public void currentV(double measuredVoltage)
        {
            current_voltage = Math.Round(measuredVoltage, 3, MidpointRounding.AwayFromZero);
            CurrentVoltage.Content = current_voltage.ToString() + "V";
        }

        public void minVolt()
        {
            if (current_voltage < min_read_voltage)
            {
                min_read_voltage = current_voltage;
                MinVoltage.Content = min_read_voltage.ToString() + "V";
            }
        }

        public void maxVolt()
        {
            if (current_voltage > max_read_voltage)
            {
                max_read_voltage = current_voltage;
                MaxVoltage.Content = current_voltage.ToString() + "V";
            }
        }

        public void setVolt(double setVoltage)
        {
            SetVolt.Content = setVoltage.ToString() + "V";
        }

        public void setOVP_Method(double setOVP)
        {
            SetOVP.Content = setOVP.ToString() + "V";
        }

        public void setUVP_Method(double setUVP)
        {
            SetUVP.Content = setUVP.ToString() + "V";
        }


        public void storeData(string Sstatus, string date)
        {
            Samples[SampleCounter] = current_voltage;
            VoltagePlot.maxRenderIndex = (SampleCounter - 1);

            Dates[SampleCounter] = date;
            string temp = Sstatus.TrimEnd();
            switch (Sstatus)
            {
                case "Dis":
                    Intstatus[SampleCounter] = 1;
                    break;
                case "CV":
                    Intstatus[SampleCounter] = 2;
                    break;
                case "CC":
                    Intstatus[SampleCounter] = 3;
                    break;
                case "CVCC":
                    Intstatus[SampleCounter] = 4;
                    break;
                case "-CC":
                    Intstatus[SampleCounter] = 5;
                    break;
                case "?":
                    Intstatus[SampleCounter] = 6;
                    break;
                default:
                    Intstatus[SampleCounter] = 7;
                    break;
            }
            SampleCounter++;
        }

        public void Draw_setVolt_marker(double setV)
        {
            if (setVoltage != setV)
            {
                setVoltage = setV;
                if (isOnceV == true)
                {
                    set_VLine.position = setVoltage;
                }
            }
        }

        public void Draw_setOVP_marker(double setOV)
        {
            if (setOvervolt != setOV)
            {
                setOvervolt = setOV;
                if (isOnceO == true)
                {
                    set_OLine.position = setOvervolt;
                }
            }
        }

        public void Draw_setUVP_marker(double setUV)
        {
            if (setUndervolt != setUV)
            {
                setUndervolt = setUV;
                if (isOnceU == true)
                {
                    set_ULine.position = setUndervolt;
                }
            }
        }

        private void VGreen_Text_Click(object sender, RoutedEventArgs e)
        {
            VGreen_Text.IsChecked = true;
            VBlue_Text.IsChecked = false;
            VRed_Text.IsChecked = false;
            VYellow_Text.IsChecked = false;
            VOrange_Text.IsChecked = false;
            VBlack_Text.IsChecked = false;
            VPink_Text.IsChecked = false;
            VViolet_Text.IsChecked = false;
            VoltagePlot.color = ColorTranslator.FromHtml("#FF00FF17");
            System.Drawing.Pen Line = new System.Drawing.Pen(System.Drawing.Brushes.LightGreen);
            VoltagePlot.penLD = Line;
            VoltagePlot.penHD = Line;
        }

        private void VBlue_Text_Click(object sender, RoutedEventArgs e)
        {
            VGreen_Text.IsChecked = false;
            VBlue_Text.IsChecked = true;
            VRed_Text.IsChecked = false;
            VYellow_Text.IsChecked = false;
            VOrange_Text.IsChecked = false;
            VBlack_Text.IsChecked = false;
            VPink_Text.IsChecked = false;
            VViolet_Text.IsChecked = false;
            VoltagePlot.color = ColorTranslator.FromHtml("#FF00C0FF");
            System.Drawing.Pen Line = new System.Drawing.Pen(System.Drawing.Brushes.LightSkyBlue);
            VoltagePlot.penLD = Line;
            VoltagePlot.penHD = Line;
        }

        private void VRed_Text_Click(object sender, RoutedEventArgs e)
        {
            VGreen_Text.IsChecked = false;
            VBlue_Text.IsChecked = false;
            VRed_Text.IsChecked = true;
            VYellow_Text.IsChecked = false;
            VOrange_Text.IsChecked = false;
            VBlack_Text.IsChecked = false;
            VPink_Text.IsChecked = false;
            VViolet_Text.IsChecked = false;
            VoltagePlot.color = System.Drawing.Color.Red;
            System.Drawing.Pen Line = new System.Drawing.Pen(System.Drawing.Brushes.Red);
            VoltagePlot.penLD = Line;
            VoltagePlot.penHD = Line;
        }

        private void VYellow_Text_Click(object sender, RoutedEventArgs e)
        {
            VGreen_Text.IsChecked = false;
            VBlue_Text.IsChecked = false;
            VRed_Text.IsChecked = false;
            VYellow_Text.IsChecked = true;
            VOrange_Text.IsChecked = false;
            VBlack_Text.IsChecked = false;
            VPink_Text.IsChecked = false;
            VViolet_Text.IsChecked = false;
            VoltagePlot.color = ColorTranslator.FromHtml("#FFFFFF00");
            System.Drawing.Pen Line = new System.Drawing.Pen(System.Drawing.Brushes.Yellow);
            VoltagePlot.penLD = Line;
            VoltagePlot.penHD = Line;
        }

        private void VOrange_Text_Click(object sender, RoutedEventArgs e)
        {
            VGreen_Text.IsChecked = false;
            VBlue_Text.IsChecked = false;
            VRed_Text.IsChecked = false;
            VYellow_Text.IsChecked = false;
            VOrange_Text.IsChecked = true;
            VBlack_Text.IsChecked = false;
            VPink_Text.IsChecked = false;
            VViolet_Text.IsChecked = false;
            VoltagePlot.color = System.Drawing.Color.DarkOrange;
            System.Drawing.Pen Line = new System.Drawing.Pen(System.Drawing.Brushes.DarkOrange);
            VoltagePlot.penLD = Line;
            VoltagePlot.penHD = Line;
        }

        private void VBlack_Text_Click(object sender, RoutedEventArgs e)
        {
            VGreen_Text.IsChecked = false;
            VBlue_Text.IsChecked = false;
            VRed_Text.IsChecked = false;
            VYellow_Text.IsChecked = false;
            VOrange_Text.IsChecked = false;
            VBlack_Text.IsChecked = true;
            VPink_Text.IsChecked = false;
            VViolet_Text.IsChecked = false;
            VoltagePlot.color = System.Drawing.Color.Black;
            System.Drawing.Pen Line = new System.Drawing.Pen(System.Drawing.Brushes.Black);
            VoltagePlot.penLD = Line;
            VoltagePlot.penHD = Line;
        }

        private void VPink_Text_Click(object sender, RoutedEventArgs e)
        {
            VGreen_Text.IsChecked = false;
            VBlue_Text.IsChecked = false;
            VRed_Text.IsChecked = false;
            VYellow_Text.IsChecked = false;
            VOrange_Text.IsChecked = false;
            VBlack_Text.IsChecked = false;
            VPink_Text.IsChecked = true;
            VViolet_Text.IsChecked = false;
            VoltagePlot.color = System.Drawing.Color.DeepPink;
            System.Drawing.Pen Line = new System.Drawing.Pen(System.Drawing.Brushes.DeepPink);
            VoltagePlot.penLD = Line;
            VoltagePlot.penHD = Line;
        }

        private void VViolet_Text_Click(object sender, RoutedEventArgs e)
        {
            VGreen_Text.IsChecked = false;
            VBlue_Text.IsChecked = false;
            VRed_Text.IsChecked = false;
            VYellow_Text.IsChecked = false;
            VOrange_Text.IsChecked = false;
            VBlack_Text.IsChecked = false;
            VPink_Text.IsChecked = false;
            VViolet_Text.IsChecked = true;
            VoltagePlot.color = System.Drawing.Color.DarkViolet;
            System.Drawing.Pen Line = new System.Drawing.Pen(System.Drawing.Brushes.DarkViolet);
            VoltagePlot.penLD = Line;
            VoltagePlot.penHD = Line;
        }

        private void bgGreen_Text_Click(object sender, RoutedEventArgs e)
        {
            bgGreen_Text.IsChecked = true;
            bgBlue_Text.IsChecked = false;
            bgRed_Text.IsChecked = false;
            bgYellow_Text.IsChecked = false;
            bgOrange_Text.IsChecked = false;
            bgBlack_Text.IsChecked = false;
            bgPink_Text.IsChecked = false;
            bgViolet_Text.IsChecked = false;
            bgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(dataBg: ColorTranslator.FromHtml("#FFAAFFB2"));
        }

        private void bgBlue_Text_Click(object sender, RoutedEventArgs e)
        {
            bgGreen_Text.IsChecked = false;
            bgBlue_Text.IsChecked = true;
            bgRed_Text.IsChecked = false;
            bgYellow_Text.IsChecked = false;
            bgOrange_Text.IsChecked = false;
            bgBlack_Text.IsChecked = false;
            bgPink_Text.IsChecked = false;
            bgViolet_Text.IsChecked = false;
            bgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(dataBg: ColorTranslator.FromHtml("#FFA1E7FF"));
        }

        private void bgRed_Text_Click(object sender, RoutedEventArgs e)
        {
            bgGreen_Text.IsChecked = false;
            bgBlue_Text.IsChecked = false;
            bgRed_Text.IsChecked = true;
            bgYellow_Text.IsChecked = false;
            bgOrange_Text.IsChecked = false;
            bgBlack_Text.IsChecked = false;
            bgPink_Text.IsChecked = false;
            bgViolet_Text.IsChecked = false;
            bgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(dataBg: ColorTranslator.FromHtml("#FFFF8989"));
        }

        private void bgYellow_Text_Click(object sender, RoutedEventArgs e)
        {
            bgGreen_Text.IsChecked = false;
            bgBlue_Text.IsChecked = false;
            bgRed_Text.IsChecked = false;
            bgYellow_Text.IsChecked = true;
            bgOrange_Text.IsChecked = false;
            bgBlack_Text.IsChecked = false;
            bgPink_Text.IsChecked = false;
            bgViolet_Text.IsChecked = false;
            bgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(dataBg: ColorTranslator.FromHtml("#FFFFFF93"));
        }

        private void bgOrange_Text_Click(object sender, RoutedEventArgs e)
        {
            bgGreen_Text.IsChecked = false;
            bgBlue_Text.IsChecked = false;
            bgRed_Text.IsChecked = false;
            bgYellow_Text.IsChecked = false;
            bgOrange_Text.IsChecked = true;
            bgBlack_Text.IsChecked = false;
            bgPink_Text.IsChecked = false;
            bgViolet_Text.IsChecked = false;
            bgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(dataBg: ColorTranslator.FromHtml("#FFFFCB8C"));
        }

        private void bgBlack_Text_Click(object sender, RoutedEventArgs e)
        {
            bgGreen_Text.IsChecked = false;
            bgBlue_Text.IsChecked = false;
            bgRed_Text.IsChecked = false;
            bgYellow_Text.IsChecked = false;
            bgOrange_Text.IsChecked = false;
            bgBlack_Text.IsChecked = true;
            bgPink_Text.IsChecked = false;
            bgViolet_Text.IsChecked = false;
            bgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(dataBg: ColorTranslator.FromHtml("#FF6E6E6E"));
        }

        private void bgPink_Text_Click(object sender, RoutedEventArgs e)
        {
            bgGreen_Text.IsChecked = false;
            bgBlue_Text.IsChecked = false;
            bgRed_Text.IsChecked = false;
            bgYellow_Text.IsChecked = false;
            bgOrange_Text.IsChecked = false;
            bgBlack_Text.IsChecked = false;
            bgPink_Text.IsChecked = true;
            bgViolet_Text.IsChecked = false;
            bgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(dataBg: ColorTranslator.FromHtml("#FFFF9ED2"));
        }

        private void bgViolet_Text_Click(object sender, RoutedEventArgs e)
        {
            bgGreen_Text.IsChecked = false;
            bgBlue_Text.IsChecked = false;
            bgRed_Text.IsChecked = false;
            bgYellow_Text.IsChecked = false;
            bgOrange_Text.IsChecked = false;
            bgBlack_Text.IsChecked = false;
            bgPink_Text.IsChecked = false;
            bgViolet_Text.IsChecked = true;
            bgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(dataBg: ColorTranslator.FromHtml("#FFE6ACFF"));
        }

        private void bgWhite_Text_Click(object sender, RoutedEventArgs e)
        {
            bgGreen_Text.IsChecked = false;
            bgBlue_Text.IsChecked = false;
            bgRed_Text.IsChecked = false;
            bgYellow_Text.IsChecked = false;
            bgOrange_Text.IsChecked = false;
            bgBlack_Text.IsChecked = false;
            bgPink_Text.IsChecked = false;
            bgViolet_Text.IsChecked = false;
            bgWhite_Text.IsChecked = true;
            V_Graph.plt.Style(dataBg: ColorTranslator.FromHtml("#FFFFFFFF"));
        }

        private void fgGreen_Text_Click(object sender, RoutedEventArgs e)
        {
            fgGreen_Text.IsChecked = true;
            fgBlue_Text.IsChecked = false;
            fgRed_Text.IsChecked = false;
            fgYellow_Text.IsChecked = false;
            fgOrange_Text.IsChecked = false;
            fgBlack_Text.IsChecked = false;
            fgPink_Text.IsChecked = false;
            fgViolet_Text.IsChecked = false;
            fgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(figBg: ColorTranslator.FromHtml("#FFAAFFB2"));
        }

        private void fgBlue_Text_Click(object sender, RoutedEventArgs e)
        {
            fgGreen_Text.IsChecked = false;
            fgBlue_Text.IsChecked = true;
            fgRed_Text.IsChecked = false;
            fgYellow_Text.IsChecked = false;
            fgOrange_Text.IsChecked = false;
            fgBlack_Text.IsChecked = false;
            fgPink_Text.IsChecked = false;
            fgViolet_Text.IsChecked = false;
            fgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(figBg: ColorTranslator.FromHtml("#FFA1E7FF"));
        }

        private void fgRed_Text_Click(object sender, RoutedEventArgs e)
        {
            fgGreen_Text.IsChecked = false;
            fgBlue_Text.IsChecked = false;
            fgRed_Text.IsChecked = true;
            fgYellow_Text.IsChecked = false;
            fgOrange_Text.IsChecked = false;
            fgBlack_Text.IsChecked = false;
            fgPink_Text.IsChecked = false;
            fgViolet_Text.IsChecked = false;
            fgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(figBg: ColorTranslator.FromHtml("#FFFF8989"));
        }

        private void fgYellow_Text_Click(object sender, RoutedEventArgs e)
        {
            fgGreen_Text.IsChecked = false;
            fgBlue_Text.IsChecked = false;
            fgRed_Text.IsChecked = false;
            fgYellow_Text.IsChecked = true;
            fgOrange_Text.IsChecked = false;
            fgBlack_Text.IsChecked = false;
            fgPink_Text.IsChecked = false;
            fgViolet_Text.IsChecked = false;
            fgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(figBg: ColorTranslator.FromHtml("#FFFFFF93"));
        }

        private void fgOrange_Text_Click(object sender, RoutedEventArgs e)
        {
            fgGreen_Text.IsChecked = false;
            fgBlue_Text.IsChecked = false;
            fgRed_Text.IsChecked = false;
            fgYellow_Text.IsChecked = false;
            fgOrange_Text.IsChecked = true;
            fgBlack_Text.IsChecked = false;
            fgPink_Text.IsChecked = false;
            fgViolet_Text.IsChecked = false;
            fgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(figBg: ColorTranslator.FromHtml("#FFFFCB8C"));
        }

        private void fgBlack_Text_Click(object sender, RoutedEventArgs e)
        {
            fgGreen_Text.IsChecked = false;
            fgBlue_Text.IsChecked = false;
            fgRed_Text.IsChecked = false;
            fgYellow_Text.IsChecked = false;
            fgOrange_Text.IsChecked = false;
            fgBlack_Text.IsChecked = true;
            fgPink_Text.IsChecked = false;
            fgViolet_Text.IsChecked = false;
            fgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(figBg: ColorTranslator.FromHtml("#FF6E6E6E"));
        }


        private void fgPink_Text_Click(object sender, RoutedEventArgs e)
        {
            fgGreen_Text.IsChecked = false;
            fgBlue_Text.IsChecked = false;
            fgRed_Text.IsChecked = false;
            fgYellow_Text.IsChecked = false;
            fgOrange_Text.IsChecked = false;
            fgBlack_Text.IsChecked = false;
            fgPink_Text.IsChecked = true;
            fgViolet_Text.IsChecked = false;
            fgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(figBg: ColorTranslator.FromHtml("#FFFF9ED2"));
        }

        private void fgViolet_Text_Click(object sender, RoutedEventArgs e)
        {
            fgGreen_Text.IsChecked = false;
            fgBlue_Text.IsChecked = false;
            fgRed_Text.IsChecked = false;
            fgYellow_Text.IsChecked = false;
            fgOrange_Text.IsChecked = false;
            fgBlack_Text.IsChecked = false;
            fgPink_Text.IsChecked = false;
            fgViolet_Text.IsChecked = true;
            fgWhite_Text.IsChecked = false;
            V_Graph.plt.Style(figBg: ColorTranslator.FromHtml("#FFE6ACFF"));
        }

        private void fgWhite_Text_Click(object sender, RoutedEventArgs e)
        {
            fgGreen_Text.IsChecked = false;
            fgBlue_Text.IsChecked = false;
            fgRed_Text.IsChecked = false;
            fgYellow_Text.IsChecked = false;
            fgOrange_Text.IsChecked = false;
            fgBlack_Text.IsChecked = false;
            fgPink_Text.IsChecked = false;
            fgViolet_Text.IsChecked = false;
            fgWhite_Text.IsChecked = true;
            V_Graph.plt.Style(figBg: ColorTranslator.FromHtml("#FFFFFFFF"));
        }

        private void Auto_Axis_Click(object sender, RoutedEventArgs e)
        {
            Enabler_AutoAxis = !Enabler_AutoAxis;
        }

        private void Mouse_Click(object sender, RoutedEventArgs e)
        {
            enableOnce_MouseTracker();
            MouseTracker = !MouseTracker;
            if (MouseTracker == false)
            {
                xLine.visible = false;
                Bar_Mouse.Background = Disable;
            }
            else
            {
                xLine.visible = true;
                Bar_Mouse.Background = Enable;
            }
        }

        private void enableOnce_MouseTracker()
        {
            if (isOnceMouse == false)
            {
                xLine = V_Graph.plt.PlotVLine(0, color: System.Drawing.Color.Red, lineStyle: LineStyle.Dash);
                xLine.visible = false;
                isOnceMouse = true;
                Bar_Mouse.Background = Enable;
            }
        }

        private void Set_Marker_Click(object sender, RoutedEventArgs e)
        {
            enableOnce_SetVMark();
            set_VLine.visible = !set_VLine.visible;
        }

        private void enableOnce_SetVMark()
        {
            if (isOnceV == false)
            {
                set_VLine = V_Graph.plt.PlotHLine(setVoltage, System.Drawing.Color.LightBlue, lineStyle: LineStyle.Dash);
                set_VLine.position = setVoltage;
                set_VLine.visible = false;
                isOnceV = true;
            }
        }

        private void OVP_Marker_Click(object sender, RoutedEventArgs e)
        {
            enableOnce_SetOMark();
            set_OLine.visible = !set_OLine.visible;
        }

        private void enableOnce_SetOMark()
        {
            if (isOnceO == false)
            {
                set_OLine = V_Graph.plt.PlotHLine(setOvervolt, System.Drawing.Color.Red, lineStyle: LineStyle.Dash);
                set_OLine.position = setOvervolt;
                set_OLine.visible = false;
                isOnceO = true;
            }
        }

        private void UVP_Marker_Click(object sender, RoutedEventArgs e)
        {
            enableOnce_SetUMark();
            set_ULine.visible = !set_ULine.visible;
        }

        private void enableOnce_SetUMark()
        {
            if (isOnceU == false)
            {
                set_ULine = V_Graph.plt.PlotHLine(setUndervolt, System.Drawing.Color.Black, lineStyle: LineStyle.Dash);
                set_ULine.position = setUndervolt;
                set_ULine.visible = false;
                isOnceU = true;
            }
        }

        private void Graph_save_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string temp = DateTime.Now.ToString("yyyy/MM/dd") + " Voltage_Graph" + ".png";
                V_Graph.plt.SaveFig(temp);
            }
            catch (Exception) { }
        }

        private void Graph_data_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = "Graph_data-" + DateTime.Now.ToString("yyyy/MM/dd") + "-" + RS232_Info.DeviceID + "-" + RS232_Info.COM_Port + "-Output(date,V)" + ".txt";
                TextWriter graph_datatotxt = new StreamWriter(@fileName, true);
                for (int i = 0; i < max_samples_allowed; i++)
                {
                    graph_datatotxt.WriteLine(Dates[i] + "," + Samples[i]);
                }
                graph_datatotxt.Close();
            }
            catch (Exception) { }
        }


        private void V_Graph_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseTracker == true)
            {
                int pixelX = (int)e.MouseDevice.GetPosition(V_Graph).X;
                (double coordinateX, _) = V_Graph.GetMouseCoordinates();

                xLine.position = coordinateX;
                V_Graph.Render(skipIfCurrentlyRendering: true, lowQuality: true);

                if (coordinateX > 0)
                {
                    Mouse_Hover_data((int)coordinateX);
                }
            }
            else { }
        }

        private void Mouse_Hover_data(int x)
        {
            try
            {
                HoverVolt.Content = Samples[x].ToString() + "V";
                HoverDate.Content = Dates[x];

                switch (Intstatus[x])
                {
                    case 1:
                        HoverStatus.Content = "Dis";
                        break;
                    case 2:
                        HoverStatus.Content = "CV";
                        break;
                    case 3:
                        HoverStatus.Content = "CC";
                        break;
                    case 4:
                        HoverStatus.Content = "CVCC";
                        break;
                    case 5:
                        HoverStatus.Content = "-CC";
                        break;
                    case 6:
                        HoverStatus.Content = "?";
                        break;
                    default:
                        HoverStatus.Content = "Fail";
                        break;
                }
            }
            catch (Exception)
            {

            }
        }

        private void Default_Theme_Click(object sender, RoutedEventArgs e)
        {
            Default_Theme.IsChecked = true;
            Black_Theme.IsChecked = false;
            Blue_Theme.IsChecked = false;
            Gray_Theme.IsChecked = false;
            GrayBlack_Theme.IsChecked = false;
            V_Graph.plt.Style(ScottPlot.Style.Default);
        }

        private void Black_Theme_Click(object sender, RoutedEventArgs e)
        {
            Default_Theme.IsChecked = false;
            Black_Theme.IsChecked = true;
            Blue_Theme.IsChecked = false;
            Gray_Theme.IsChecked = false;
            GrayBlack_Theme.IsChecked = false;
            V_Graph.plt.Style(ScottPlot.Style.Black);
        }

        private void Blue_Theme_Click(object sender, RoutedEventArgs e)
        {
            Default_Theme.IsChecked = false;
            Black_Theme.IsChecked = false;
            Blue_Theme.IsChecked = true;
            Gray_Theme.IsChecked = false;
            GrayBlack_Theme.IsChecked = false;
            V_Graph.plt.Style(ScottPlot.Style.Blue1);
        }

        private void Gray_Theme_Click(object sender, RoutedEventArgs e)
        {
            Default_Theme.IsChecked = false;
            Black_Theme.IsChecked = false;
            Blue_Theme.IsChecked = false;
            Gray_Theme.IsChecked = true;
            GrayBlack_Theme.IsChecked = false;
            V_Graph.plt.Style(ScottPlot.Style.Gray1);
        }

        private void GrayBlack_Theme_Click(object sender, RoutedEventArgs e)
        {
            Default_Theme.IsChecked = false;
            Black_Theme.IsChecked = false;
            Blue_Theme.IsChecked = false;
            Gray_Theme.IsChecked = false;
            GrayBlack_Theme.IsChecked = true;
            V_Graph.plt.Style(ScottPlot.Style.Gray2);
        }

    }
}

