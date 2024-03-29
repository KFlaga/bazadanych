﻿#pragma checksum "..\..\ConnectionSettingsWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CED42DCE2C69CA8144819239CD2F5A6E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace BazaDanych {
    
    
    /// <summary>
    /// ConnectionSettingsWindow
    /// </summary>
    public partial class ConnectionSettingsWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\ConnectionSettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal BazaDanych.ConnectionSettingsWindow connectionSettingsWindow;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\ConnectionSettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button butCancel;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\ConnectionSettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button butOK;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\ConnectionSettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkDefault;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\ConnectionSettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbUser;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\ConnectionSettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbPassword;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\ConnectionSettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbDataSource;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\ConnectionSettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbHost;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\ConnectionSettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbPort;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\ConnectionSettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbSid;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\ConnectionSettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkTNS;
        
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
            System.Uri resourceLocater = new System.Uri("/BazaDanych;component/connectionsettingswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ConnectionSettingsWindow.xaml"
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
            this.connectionSettingsWindow = ((BazaDanych.ConnectionSettingsWindow)(target));
            return;
            case 2:
            this.butCancel = ((System.Windows.Controls.Button)(target));
            
            #line 35 "..\..\ConnectionSettingsWindow.xaml"
            this.butCancel.Click += new System.Windows.RoutedEventHandler(this.butCancel_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.butOK = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\ConnectionSettingsWindow.xaml"
            this.butOK.Click += new System.Windows.RoutedEventHandler(this.butOK_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.checkDefault = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.tbUser = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.tbPassword = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.tbDataSource = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.tbHost = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.tbPort = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.tbSid = ((System.Windows.Controls.TextBox)(target));
            return;
            case 11:
            this.checkTNS = ((System.Windows.Controls.CheckBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

