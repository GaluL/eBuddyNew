﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using eBuddyApp.Services.Azure;

namespace Template10.Samples.SearchSample.Controls
{
    public sealed partial class LoginPart : UserControl
    {
        public LoginPart()
        {
            this.InitializeComponent();
        }

        public event EventHandler HideRequested;
        public event EventHandler LoggedIn;
        public event EventHandler SignUpRequested;

        private async void LoginClicked(object sender, RoutedEventArgs e)
        {
            if (await MobileService.Instance.FacebookLogIn())
            {
                LoggedIn?.Invoke(this, EventArgs.Empty);
                HideRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            HideRequested?.Invoke(this, EventArgs.Empty);
        }

        //public Models.UserCredentials UserCredentials { get; set; }
        private async void fbSignup_OnClick(object sender, RoutedEventArgs e)
        {
            SignUpRequested?.Invoke(this, null);             
        }
    }
}
