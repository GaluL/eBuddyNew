﻿#pragma checksum "C:\eBuddyNew\eBuddyApp\Views\Shell.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2943B6888F6C465F2E890EEF0D20A318"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eBuddyApp.Views
{
    partial class Shell : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.RootGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 2:
                {
                    this.MyHamburgerMenu = (global::Template10.Controls.HamburgerMenu)(target);
                }
                break;
            case 3:
                {
                    this.LoginModal = (global::Template10.Controls.ModalDialog)(target);
                }
                break;
            case 4:
                {
                    this.SignUpModal = (global::Template10.Controls.ModalDialog)(target);
                }
                break;
            case 5:
                {
                    this.SignUpPart = (global::Template10.Samples.SearchSample.Controls.SignUpPart)(target);
                    #line 115 "..\..\..\Views\Shell.xaml"
                    ((global::Template10.Samples.SearchSample.Controls.SignUpPart)this.SignUpPart).SignUpHideRequested += this.signUpHide;
                    #line 116 "..\..\..\Views\Shell.xaml"
                    ((global::Template10.Samples.SearchSample.Controls.SignUpPart)this.SignUpPart).SignedUp += this.signedUp;
                    #line default
                }
                break;
            case 6:
                {
                    this.loginPart = (global::Template10.Samples.SearchSample.Controls.LoginPart)(target);
                    #line 102 "..\..\..\Views\Shell.xaml"
                    ((global::Template10.Samples.SearchSample.Controls.LoginPart)this.loginPart).HideRequested += this.LoginHide;
                    #line 103 "..\..\..\Views\Shell.xaml"
                    ((global::Template10.Samples.SearchSample.Controls.LoginPart)this.loginPart).LoggedIn += this.LoginLoggedIn;
                    #line 104 "..\..\..\Views\Shell.xaml"
                    ((global::Template10.Samples.SearchSample.Controls.LoginPart)this.loginPart).SignUpRequested += this.LoginSignUp;
                    #line default
                }
                break;
            case 7:
                {
                    this.SettingsButton = (global::Template10.Controls.HamburgerButtonInfo)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

