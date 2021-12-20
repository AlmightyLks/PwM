﻿using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PwM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Regex _regex = new Regex("[^!-*.-]+");
        private static string s_rootPath = Path.GetPathRoot(Environment.SystemDirectory);
        private static readonly string s_accountName = Environment.UserName;
        private static string s_passwordManagerDirectory = $"{s_rootPath}Users\\{s_accountName}\\AppData\\Local\\PwM\\";
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeVaultsDirectory(s_passwordManagerDirectory);
            Utils.VaultManagement.ListVaults(s_passwordManagerDirectory, vaultList);
            userTXB.Text = s_accountName;
            vaultsCountLBL.Text = Utils.GlobalVariables.vaultsCount.ToString();
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged; // Exit vault on suspend.
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch); // Exit vault on lock screen.
        }

        /// <summary>
        /// Create vautls directory.
        /// </summary>
        /// <param name="directoryName">Directory name.</param>
        private void InitializeVaultsDirectory(string directoryName)
        {
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
        }


        //------------------------UI Settings------------------------------
        /// <summary>
        /// Column sort click on heard for applist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppListColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort(header, appList, direction);
                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        /// <summary>
        /// Column sort click on heard for Vault List.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VaultListColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort(header, vaultList, direction);

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        /// <summary>
        /// Sort function for column header click on listview.
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="listView"></param>
        /// <param name="direction"></param>
        private void Sort(string sortBy, ListView listView, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(listView.Items);
            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
        /// <summary>
        /// Drag window on mouse click left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();

        }
        /// <summary>
        /// Close wpf form button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeBTN_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();//close the app
        }


        /// <summary>
        /// Minimizr button(label)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miniMizeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// About window open button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutBTN_Click(object sender, RoutedEventArgs e)
        {
            var aB = new about();
            aB.ShowDialog();
        }


        // Tab Switch
        private void Home_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetListViewColor(homeListVI, false);
            SetListViewColor(vaultsListVI, true);
            SetListViewColorApp(appListVI, true);
            tabControl.SelectedIndex = 0;
        }
        private void Vault_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetListViewColor(homeListVI, true);
            SetListViewColor(vaultsListVI, false);
            SetListViewColorApp(appListVI, true);
            tabControl.SelectedIndex = 1;
        }
        private void App_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetListViewColor(homeListVI, true);
            SetListViewColor(vaultsListVI, true);
            SetListViewColorApp(appListVI, false);
            tabControl.SelectedIndex = 2;
        }
        //--------------------

        /// <summary>
        /// Set Color of listView on click. 
        /// </summary>
        /// <param name="listViewItem"></param>
        /// <param name="reset"></param>
        private void SetListViewColor(ListViewItem listViewItem, bool reset)
        {
            var converter = new BrushConverter();
            if (reset)
            {
                listViewItem.Background = Brushes.Transparent;
                listViewItem.Foreground = (Brush)converter.ConvertFromString("#FFDCDCDC");
                return;
            }
            listViewItem.Background = (Brush)converter.ConvertFromString("#6f2be3");
        }

        /// <summary>
        /// Set Color of listView on click. 
        /// </summary>
        /// <param name="listViewItem"></param>
        /// <param name="reset"></param>
        private void SetListViewColorApp(ListViewItem listViewItem, bool reset)
        {
            if (listViewItem.IsEnabled)
            {
                var converter = new BrushConverter();
                if (reset)
                {
                    listViewItem.Background = Brushes.Transparent;
                    listViewItem.Foreground = (Brush)converter.ConvertFromString("#FFDCDCDC");
                    return;
                }
                listViewItem.Background = (Brush)converter.ConvertFromString("#6f2be3");
            }
        }


        /// <summary>
        /// Acceptin only custom characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdPrefixTXT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        /// <summary>
        /// check for regex match
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        /// <summary>
        /// Prevent pasting letterts 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Decrypt vault buy double click on it and populate appList on Application tab and switch to it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vaultList_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenVault();
        }


        /// <summary>
        /// Open selected vault from vault list.
        /// </summary>
        private void OpenVault()
        {
            var converter = new BrushConverter();
            if (vaultList.SelectedItem != null)
            {
                string vaultName = vaultList.SelectedItem.ToString();
                vaultName = vaultName.Split(',')[0].Replace("{ Name = ", "");
                var masterPassword = Utils.MasterPasswordLoad.LoadMasterPassword(vaultName);
                VaultClose();
                if (masterPassword != null && masterPassword.Length > 0)
                {
                    if (Utils.AppManagement.DecryptAndPopulateList(appList, vaultName, masterPassword))
                    {
                        appListVI.IsEnabled = true;
                        appListVI.Foreground = (Brush)converter.ConvertFromString("#FFDCDCDC");
                        SetListViewColor(homeListVI, true);
                        SetListViewColor(vaultsListVI, true);
                        SetListViewColorApp(appListVI, false);
                        tabControl.SelectedIndex = 2;
                        appListVaultLVL.Text = vaultName;
                    }
                }
            }
            else
            {
                appListVI.Foreground = Brushes.Red;
                appListVI.IsEnabled = false;
            }
        }

        /// <summary>
        /// Clear applist, and all passwords boxes and text boxes from applicaiton tab, closes it and moves to vault tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void vaultCloseLBL_Click(object sender, RoutedEventArgs e)
        {
            VaultClose();
        }

        // Function for clear applist, and all passwords boxes and text boxes from applicaiton tab, closes it and moves to vault tab.
        public void VaultClose()
        {
            SetListViewColor(homeListVI, true);
            SetListViewColor(vaultsListVI, false);
            SetListViewColorApp(appListVI, true);
            appList.Items.Clear();
            tabControl.SelectedIndex = 1;
            appListVI.Foreground = Brushes.Red;
            appListVI.IsEnabled = false;
            Utils.AppManagement.vaultSecure = null;
            GC.Collect();
        }

        /// <summary>
        /// Copy password from selected account for 15 seconds in clipboard. Right click context menu event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Utils.AppManagement.CopyPassToClipBoard(appList));
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 15);
            dispatcherTimer.Start();
        }

        /// <summary>
        /// Clear clipboard after timer stops.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Clipboard.Clear();
            dispatcherTimer.Stop();
        }

        /// <summary>
        /// Show password from selected account. Right click context menu event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            Utils.AppManagement.ShowPassword(appList);
        }

        /// <summary>
        /// Check if PC enters sleep or hibernate mode and lock vault.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    VaultClose();
                    break;
            }
        }

        /// <summary>
        /// Check if lock screen and close vault.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                VaultClose();
            }
        }

        /// <summary>
        /// Enter key event for open vault.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vaultList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OpenVault();
            }
        }


        /// <summary>
        /// Update account password event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateAccountPass_Click(object sender, RoutedEventArgs e)
        {
            Utils.AppManagement.UpdateSelectedItemPassword(appList, appListVaultLVL.Text);
        }

        /// <summary>
        /// Add new applicaiton icon event (+).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddAppIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddApplications addApplications = new AddApplications();
            addApplications.ShowDialog();
            if (Utils.GlobalVariables.closeAppConfirmation != "yes")
            {
                var masterPassword = Utils.MasterPasswordLoad.LoadMasterPassword(appListVaultLVL.Text);
                Utils.AppManagement.AddApplication(appList, appListVaultLVL.Text, Utils.GlobalVariables.applicationName, Utils.GlobalVariables.accountName, Utils.GlobalVariables.accountPassword, masterPassword);
                Utils.ClearVariables.VariablesClear();
            }
        }

        /// <summary>
        /// Delete applicaiton icon event (-).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelAppIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Utils.AppManagement.DeleteSelectedItem(appList, appListVaultLVL.Text);
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            Utils.AppManagement.DeleteSelectedItem(appList, appListVaultLVL.Text);
        }

        private void DeleteVault_Click(object sender, RoutedEventArgs e)
        {
            Utils.VaultManagement.DeleteVaultItem(vaultList, s_passwordManagerDirectory);
            vaultsCountLBL.Text = Utils.GlobalVariables.vaultsCount.ToString();
        }

        private void AddVaultIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddVault addVault = new AddVault();
            addVault.ShowDialog();
            if (Utils.GlobalVariables.createConfirmation == "yes")
                Utils.VaultManagement.ListVaults(s_passwordManagerDirectory, vaultList);
        }

        private void DelVaultIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Utils.VaultManagement.DeleteVaultItem(vaultList, s_passwordManagerDirectory);
            vaultsCountLBL.Text = Utils.GlobalVariables.vaultsCount.ToString();
        }
    }
}