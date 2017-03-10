﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using eBuddyApp.Models;
using Microsoft.WindowsAzure.MobileServices;

namespace eBuddyApp.Services.Azure
{
    class MobileService
    {
        // This MobileServiceClient has been configured to communicate with the Azure Mobile Service and
        // Azure Gateway using the application key. You're all set to start working with your Mobile Service!
        private MobileServiceClient Service = new MobileServiceClient(
            "https://ebuddy.azurewebsites.net"
        );

        private static MobileService _Instance;
        public static MobileService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MobileService();    
                }

                return _Instance;
            }
        }

        private UserItem _UserData;
        public UserItem UserData
        {
            get { return _UserData; }
        }

        private ObservableCollection<RunItem> _FinishedRuns;
        public ObservableCollection<RunItem> FinishedRuns
        {
            get { return _FinishedRuns; }
        }

        private ObservableCollection<ScheduledRunItem> _ScheduledRuns;
        public ObservableCollection<ScheduledRunItem> ScheduledRuns
        {
            get { return _ScheduledRuns; }
        }

        internal event EventHandler UserDataLoaded; 

        private MobileService()
        {
            _FinishedRuns = new ObservableCollection<RunItem>();
            _ScheduledRuns = new ObservableCollection<ScheduledRunItem>();
        }

        internal async Task<bool> AuthenticateWithFacebook()
        {
            MobileServiceUser user;
            string message;
            bool success = false;

            // This sample uses the Facebook provider.
            var provider = MobileServiceAuthenticationProvider.Facebook;

            // Use the PasswordVault to securely store and access credentials.
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = null;

            try
            {
                // Try to get an existing credential from the vault.
                credential = vault.FindAllByResource(provider.ToString()).FirstOrDefault();
            }
            catch (Exception)
            {
                // When there is no matching resource an error occurs, which we ignore.
            }

            if (credential != null)
            {
                // Create a user from the stored credentials.
                user = new MobileServiceUser(credential.UserName);
                credential.RetrievePassword();
                user.MobileServiceAuthenticationToken = credential.Password;

                // Set the user from the stored credentials.
                Service.CurrentUser = user;

                // Consider adding a check to determine if the token is 
                // expired, as shown in this post: http://aka.ms/jww5vp.

                success = true;
                message = string.Format("Cached credentials for user - {0}", user.UserId);
            }
            else
            {
                try
                {
                    // Login with the identity provider.
                    user = await Service.LoginAsync(provider);

                    // Create and store the user credentials.
                    credential = new PasswordCredential(provider.ToString(),
                        user.UserId, user.MobileServiceAuthenticationToken);
                    vault.Add(credential);
                    
                    success = true;
                    message = string.Format("You are now logged in - {0}", user.UserId);
                }
                catch (MobileServiceInvalidOperationException)
                {
                    message = "You must log in. Login Required";
                }
            }

            //if (!success)
            //{
            //    var dialog = new MessageDialog(message);
            //    dialog.Commands.Add(new UICommand("OK"));
            //    await dialog.ShowAsync();
            //}

            return success;
        }

        public async Task<bool> IsRegistered()
        {
            var userItems = await Service.GetTable<UserItem>().Where(x => x.FacebookId == Service.CurrentUser.UserId).ToCollectionAsync();

            if (userItems.Count == 0)
            {
                return false;
            }

            _UserData = userItems[0];

            return true;
        }

        public async Task CollectUserData()
        {
            await CollectFinishedRuns();
            await CollectScheduledRuns();
        }

        private async Task CollectScheduledRuns()
        {
            IEnumerable<ScheduledRunItem> scheduledRuns;

            try
            {
                scheduledRuns = await Service.GetTable<ScheduledRunItem>().Where(
                    x => x.User1FacebookId == Service.CurrentUser.UserId ||
                         x.User2FacebookId == Service.CurrentUser.UserId).ToEnumerableAsync();

                foreach (var run in scheduledRuns)
                {
                    ScheduledRuns.Add(run);
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        private async Task CollectFinishedRuns()
        {
            IEnumerable<RunItem> finishedRuns;

            try
            {
                finishedRuns =
                    await Service.GetTable<RunItem>()
                        .Where(x => x.FacebookId == Service.CurrentUser.UserId)
                        .ToEnumerableAsync();

                foreach (var run in finishedRuns)
                {
                    FinishedRuns.Add(run);
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        public async Task<bool> FacebookLogIn()
        {
            if (!await AuthenticateWithFacebook())
            {
                return false;
            }

            if (!await IsRegistered())
            {
                return false;
            }

            await CollectUserData();

            UserDataLoaded?.Invoke(this, null);

            return true;
        }


        public async void RegisterUser(UserItem userData)
        {
            userData.FacebookId = Service.CurrentUser.UserId;

            await Service.GetTable<UserItem>().InsertAsync(userData);

            _UserData = userData;
        }
		
		public async void SaveRunData(RunItem runData)
        {
            await Service.GetTable<RunItem>().InsertAsync(runData);
        }
    }
}
