﻿using Microsoft.Win32;
using PwMLib;
using System.Windows;
using System.Windows.Input;

namespace PwM
{
    /// <summary>
    /// Interaction logic for UpdateApplication.xaml
    /// </summary>
    public partial class UpdateApplication : Window
    {
        public UpdateApplication()
        {
            InitializeComponent();
            AccountNameTXT.Text = Utils.GlobalVariables.accountName;
            ApplicationNameTXT.Text = Utils.GlobalVariables.applicationName;
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged; // Exit vault on suspend.
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch); // Exit vault on lock screen.
        }
        /// <summary>
        /// Check if PC enters sleep or hibernate mode and closes window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    Utils.GlobalVariables.closeAppConfirmation = true;
                    this.Close();
                    break;
            }
        }

        /// <summary>
        /// Check if lock screen and closes window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                Utils.GlobalVariables.closeAppConfirmation = true;
                this.Close();
            }
        }

        /// <summary>
        /// Mouse window drag function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// Close button label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Utils.GlobalVariables.closeAppConfirmation = true;
            this.Close();
        }

        /// <summary>
        /// Label button function for minimize window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miniMizeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Show/hide master password from update account passwordbox using a textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowHideNewPassword(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                Utils.TextPassBoxChanges.ShowPassword(newPassAccBox, NewPasswordShow);
            }
            else if (e.ButtonState == MouseButtonState.Released)
            {
                Utils.TextPassBoxChanges.HidePassword(newPassAccBox, NewPasswordShow);
            }
        }

        /// <summary>
        /// Password generator for updated accounts password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateNewPassAcc_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            newPassAccBox.Password = PasswordGenerator.GeneratePassword(20);
        }

        /// <summary>
        /// Update account password from a application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateAccPassBTN_Click(object sender, RoutedEventArgs e)
        {
            UpdatePassNotification updatePassNotification = new UpdatePassNotification();
            updatePassNotification.ShowDialog();
            if (Utils.GlobalVariables.updatePwdConfirmation)
            {
                Utils.GlobalVariables.newAccountPassword = newPassAccBox.Password;
                this.Close();
            }
        }

        private void newPassAccBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            updateAccPassBTN.IsEnabled = (newPassAccBox.Password.Length > 0) ? true : false;
        }

        /// <summary>
        /// Hide master password when mouse is moved over from eye icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowNewPassword_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Utils.TextPassBoxChanges.HidePassword(newPassAccBox, NewPasswordShow);
        }
    }
}
