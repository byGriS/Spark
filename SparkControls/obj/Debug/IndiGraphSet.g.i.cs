﻿#pragma checksum "..\..\IndiGraphSet.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "5B7E0F8C867ECA346AF901AE6D55D389967D237808412A6D97564F9FA8C580FC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using SparkControls;
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


namespace SparkControls {
    
    
    /// <summary>
    /// IndiGraphSet
    /// </summary>
    public partial class IndiGraphSet : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\IndiGraphSet.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel spHistory;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\IndiGraphSet.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbHistory;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\IndiGraphSet.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listParams;
        
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
            System.Uri resourceLocater = new System.Uri("/SparkControls;component/indigraphset.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\IndiGraphSet.xaml"
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
            
            #line 12 "..\..\IndiGraphSet.xaml"
            ((SparkControls.IndiGraphSet)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.spHistory = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 3:
            this.tbHistory = ((System.Windows.Controls.TextBox)(target));
            
            #line 24 "..\..\IndiGraphSet.xaml"
            this.tbHistory.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.tbHistory_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 25 "..\..\IndiGraphSet.xaml"
            this.tbHistory.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.tbHistory_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.listParams = ((System.Windows.Controls.ListBox)(target));
            return;
            case 5:
            
            #line 40 "..\..\IndiGraphSet.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

