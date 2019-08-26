﻿#pragma checksum "..\..\SReportWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2009C7D9CD072A16D6E07118C0C69EE2164385C6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using SReport;
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
using Xceed.Wpf.Toolkit;


namespace SReport {
    
    
    /// <summary>
    /// SReportWindow
    /// </summary>
    public partial class SReportWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 37 "..\..\SReportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lbDataBases;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\SReportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnConnect;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\SReportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lbLength;
        
        #line default
        #line hidden
        
        
        #line 90 "..\..\SReportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lbCount;
        
        #line default
        #line hidden
        
        
        #line 118 "..\..\SReportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRead;
        
        #line default
        #line hidden
        
        
        #line 146 "..\..\SReportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lStart;
        
        #line default
        #line hidden
        
        
        #line 155 "..\..\SReportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lFinish;
        
        #line default
        #line hidden
        
        
        #line 172 "..\..\SReportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.DateTimePicker dtpStart;
        
        #line default
        #line hidden
        
        
        #line 188 "..\..\SReportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.DateTimePicker dtpFinish;
        
        #line default
        #line hidden
        
        
        #line 200 "..\..\SReportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnDrawGraph;
        
        #line default
        #line hidden
        
        
        #line 206 "..\..\SReportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExport;
        
        #line default
        #line hidden
        
        
        #line 217 "..\..\SReportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WebBrowser wb;
        
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
            System.Uri resourceLocater = new System.Uri("/SReport;component/sreportwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SReportWindow.xaml"
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
            
            #line 11 "..\..\SReportWindow.xaml"
            ((SReport.SReportWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.lbDataBases = ((System.Windows.Controls.ListBox)(target));
            
            #line 40 "..\..\SReportWindow.xaml"
            this.lbDataBases.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.lbDataBases_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnConnect = ((System.Windows.Controls.Button)(target));
            
            #line 58 "..\..\SReportWindow.xaml"
            this.btnConnect.Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.lbLength = ((System.Windows.Controls.ListBox)(target));
            
            #line 64 "..\..\SReportWindow.xaml"
            this.lbLength.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.lbLength_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.lbCount = ((System.Windows.Controls.ListBox)(target));
            
            #line 93 "..\..\SReportWindow.xaml"
            this.lbCount.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.lbCount_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnRead = ((System.Windows.Controls.Button)(target));
            
            #line 122 "..\..\SReportWindow.xaml"
            this.btnRead.Click += new System.Windows.RoutedEventHandler(this.Read_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.lStart = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.lFinish = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.dtpStart = ((Xceed.Wpf.Toolkit.DateTimePicker)(target));
            return;
            case 10:
            this.dtpFinish = ((Xceed.Wpf.Toolkit.DateTimePicker)(target));
            return;
            case 11:
            this.btnDrawGraph = ((System.Windows.Controls.Button)(target));
            
            #line 204 "..\..\SReportWindow.xaml"
            this.btnDrawGraph.Click += new System.Windows.RoutedEventHandler(this.btnPreview_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.btnExport = ((System.Windows.Controls.Button)(target));
            
            #line 210 "..\..\SReportWindow.xaml"
            this.btnExport.Click += new System.Windows.RoutedEventHandler(this.btnExport_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            this.wb = ((System.Windows.Controls.WebBrowser)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
