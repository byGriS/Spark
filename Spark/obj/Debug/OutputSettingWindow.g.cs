﻿#pragma checksum "..\..\OutputSettingWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "FDB273F797A081205086258A8C689A31022CC9BD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Spark;
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


namespace Spark {
    
    
    /// <summary>
    /// OutputSettingWindow
    /// </summary>
    public partial class OutputSettingWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 40 "..\..\OutputSettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bRemoveInput;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\OutputSettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listOutput;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\OutputSettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Spark.SerialSettingControl serialSetting;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\OutputSettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Spark.EthernetSettingControl ethernetSetting;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\OutputSettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Spark.ParamsTextOutput paramsText;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\OutputSettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Spark.ParamsModbusOutput paramsModbus;
        
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
            System.Uri resourceLocater = new System.Uri("/Spark;component/outputsettingwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\OutputSettingWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            
            #line 13 "..\..\OutputSettingWindow.xaml"
            ((Spark.OutputSettingWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 38 "..\..\OutputSettingWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AddSerial_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 39 "..\..\OutputSettingWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AddEthernet_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.bRemoveInput = ((System.Windows.Controls.Button)(target));
            
            #line 40 "..\..\OutputSettingWindow.xaml"
            this.bRemoveInput.Click += new System.Windows.RoutedEventHandler(this.RemoveInput_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.listOutput = ((System.Windows.Controls.ListBox)(target));
            
            #line 45 "..\..\OutputSettingWindow.xaml"
            this.listOutput.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.listInput_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.serialSetting = ((Spark.SerialSettingControl)(target));
            return;
            case 7:
            this.ethernetSetting = ((Spark.EthernetSettingControl)(target));
            return;
            case 8:
            this.paramsText = ((Spark.ParamsTextOutput)(target));
            return;
            case 9:
            this.paramsModbus = ((Spark.ParamsModbusOutput)(target));
            return;
            case 10:
            
            #line 64 "..\..\OutputSettingWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Ok_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 65 "..\..\OutputSettingWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Cancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
