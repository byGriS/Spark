﻿#pragma checksum "..\..\IndiColumnSet.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "2C7E84A53D11E6D4D277EAEEDBA5F2C6339C93FEB64D102DAEB5733B55D05D00"
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
    /// IndiColumnSet
    /// </summary>
    public partial class IndiColumnSet : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\IndiColumnSet.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbParam;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\IndiColumnSet.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbCountDot;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\IndiColumnSet.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbMax;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\IndiColumnSet.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbMin;
        
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
            System.Uri resourceLocater = new System.Uri("/SparkControls;component/indicolumnset.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\IndiColumnSet.xaml"
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
            
            #line 12 "..\..\IndiColumnSet.xaml"
            ((SparkControls.IndiColumnSet)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.cbParam = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.tbCountDot = ((System.Windows.Controls.TextBox)(target));
            
            #line 42 "..\..\IndiColumnSet.xaml"
            this.tbCountDot.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.tbCountDot_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 43 "..\..\IndiColumnSet.xaml"
            this.tbCountDot.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.tbCountDot_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.tbMax = ((System.Windows.Controls.TextBox)(target));
            
            #line 57 "..\..\IndiColumnSet.xaml"
            this.tbMax.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.tbCountDot_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 58 "..\..\IndiColumnSet.xaml"
            this.tbMax.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.tbCountDot_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.tbMin = ((System.Windows.Controls.TextBox)(target));
            
            #line 72 "..\..\IndiColumnSet.xaml"
            this.tbMin.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.tbCountDot_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 73 "..\..\IndiColumnSet.xaml"
            this.tbMin.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.tbCountDot_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 81 "..\..\IndiColumnSet.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

