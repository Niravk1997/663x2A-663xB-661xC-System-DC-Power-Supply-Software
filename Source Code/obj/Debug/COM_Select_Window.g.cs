﻿#pragma checksum "..\..\COM_Select_Window.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "3732DDF88275E789BFAAC0C96E57F118B61F5D931DF7A5C112EE63775F86B3F7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Agilent_6632B;
using ScottPlot;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Agilent_6632B {
    
    
    /// <summary>
    /// COM_Select_Window
    /// </summary>
    public partial class COM_Select_Window : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\COM_Select_Window.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button COM_Refresh;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\COM_Select_Window.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox COM_List;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\COM_Select_Window.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox COM_Number;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\COM_Select_Window.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox baudRate;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\COM_Select_Window.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox bits;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\COM_Select_Window.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox parity;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\COM_Select_Window.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Stop;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\COM_Select_Window.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox flow;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\COM_Select_Window.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Connect;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\COM_Select_Window.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Reset;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\COM_Select_Window.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Get_Info;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\COM_Select_Window.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Message;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/663x2A 663xB 661xC Software;component/com_select_window.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\COM_Select_Window.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.COM_Refresh = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\COM_Select_Window.xaml"
            this.COM_Refresh.Click += new System.Windows.RoutedEventHandler(this.COM_Refresh_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.COM_List = ((System.Windows.Controls.ListBox)(target));
            
            #line 17 "..\..\COM_Select_Window.xaml"
            this.COM_List.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.COM_List_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 3:
            this.COM_Number = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.baudRate = ((System.Windows.Controls.ComboBox)(target));
            
            #line 26 "..\..\COM_Select_Window.xaml"
            this.baudRate.DropDownClosed += new System.EventHandler(this.baudRate_DropDownClosed);
            
            #line default
            #line hidden
            return;
            case 5:
            this.bits = ((System.Windows.Controls.ComboBox)(target));
            
            #line 34 "..\..\COM_Select_Window.xaml"
            this.bits.DropDownClosed += new System.EventHandler(this.bits_DropDownClosed);
            
            #line default
            #line hidden
            return;
            case 6:
            this.parity = ((System.Windows.Controls.ComboBox)(target));
            
            #line 41 "..\..\COM_Select_Window.xaml"
            this.parity.DropDownClosed += new System.EventHandler(this.parity_DropDownClosed);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Stop = ((System.Windows.Controls.ComboBox)(target));
            
            #line 48 "..\..\COM_Select_Window.xaml"
            this.Stop.DropDownClosed += new System.EventHandler(this.Stop_DropDownClosed);
            
            #line default
            #line hidden
            return;
            case 8:
            this.flow = ((System.Windows.Controls.ComboBox)(target));
            
            #line 53 "..\..\COM_Select_Window.xaml"
            this.flow.DropDownClosed += new System.EventHandler(this.flow_DropDownClosed);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Connect = ((System.Windows.Controls.Button)(target));
            
            #line 58 "..\..\COM_Select_Window.xaml"
            this.Connect.Click += new System.Windows.RoutedEventHandler(this.Connect_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.Reset = ((System.Windows.Controls.Button)(target));
            
            #line 59 "..\..\COM_Select_Window.xaml"
            this.Reset.Click += new System.Windows.RoutedEventHandler(this.Reset_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.Get_Info = ((System.Windows.Controls.Button)(target));
            
            #line 60 "..\..\COM_Select_Window.xaml"
            this.Get_Info.Click += new System.Windows.RoutedEventHandler(this.Get_Info_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.Message = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

