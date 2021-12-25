﻿using System;
using System.IO;
using System.Windows.Forms;

namespace PwM.Utils
{
    public class ImportExport
    {
        /*Import/export vault class.*/

        private static OpenFileDialog s_openFileDialog = new OpenFileDialog();
        private static SaveFileDialog s_saveFileDialog = new SaveFileDialog();

        /// <summary>
        /// Import vault.
        /// </summary>
        /// <param name="vaultList"></param>
        /// <param name="vaultDirPath"></param>
        public static void Import(System.Windows.Controls.ListView vaultList, string vaultDirPath)
        {
            s_openFileDialog.Filter = "Vault Files (*.x)|*.x";
            s_openFileDialog.Multiselect = true;
            s_openFileDialog.Title = "Select PwM vault files to Import";
            DialogResult dr = s_openFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                int count = 0;
                string vault = string.Empty;
                string vaultOverwrite = string.Empty;
                bool copyOverwrite = false;
                bool copyClean = false;
                foreach (var vaultfile in s_openFileDialog.FileNames)
                {
                    try
                    {

                        FileInfo fileInfo = new FileInfo(vaultfile);
                        vault = fileInfo.Name;
                        string vaultPwMLocation = vaultDirPath + vault;
                        if (File.Exists(vaultPwMLocation))
                        {
                            if (VaultImportOverwriteFile(vault) == "yes")
                            {
                                count++;
                                copyOverwrite = true;
                                vaultOverwrite = vault;
                                File.Copy(vaultfile, vaultPwMLocation, true);
                                GlobalVariables.importConfirmation = "";
                            }
                        }
                        else
                        {
                            count++;
                            copyClean = true;
                            File.Copy(vaultfile, vaultPwMLocation);
                        }
                    }
                    catch (Exception e)
                    {
                        Notification.ShowNotificationInfo("red", e.Message);
                    }
                }
                NotificaitonImport(count, vault, vaultOverwrite, copyOverwrite, copyClean);
                VaultManagement.ListVaults(vaultDirPath, vaultList);
            }
        }

        
        /// <summary>
        /// Export vault.
        /// </summary>
        /// <param name="vaultList"></param>
        /// <param name="vaultDirPath"></param>
        public static void Export(System.Windows.Controls.ListView vaultList, string vaultDirPath)
        {
            if (vaultList.SelectedItem == null)
            {
                Notification.ShowNotificationInfo("orange", "You must select a vault for export!");
                return;
            }
            string vault = VaultManagement.GetVaultNameFromListView(vaultList);
            s_saveFileDialog.Filter = "Vault Files (*.x)|*.x";
            s_saveFileDialog.Title = $"Export vault: {vault}";
            DialogResult dr = s_saveFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                try
                {
                    string vaultPath = vaultDirPath + vault+".x";
                    File.Copy(vaultPath, s_saveFileDialog.FileName, true);
                    Notification.ShowNotificationInfo("green", $"Vault {vault} is exported!");
                }
                catch (Exception e)
                {
                    Notification.ShowNotificationInfo("red", e.Message);
                }
            }
        }

        /// <summary>
        /// Notification when vault import is finished.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="vaultFile"></param>
        /// <param name="vaultOverwirte"></param>
        /// <param name="copyOverwrite"></param>
        /// <param name="copyClean"></param>
        private static void NotificaitonImport(int count, string vaultFile, string vaultOverwirte, bool copyOverwrite, bool copyClean)
        {
            if (count > 1)
            {
                Notification.ShowNotificationInfo("green", "The selected vaults are imported!");
                return;
            }
            if (copyOverwrite)
            {
                vaultOverwirte = vaultOverwirte.Substring(0, vaultOverwirte.Length - 2);
                Notification.ShowNotificationInfo("green", $"{vaultOverwirte} vault was imported!");
            }
            if (copyClean)
            {
                vaultFile = vaultFile.Substring(0, vaultFile.Length - 2);
                Notification.ShowNotificationInfo("green", $"{vaultFile} vault was imported!");
            }
        }


       /// <summary>
       /// Vault overwrite notification check.
       /// </summary>
       /// <param name="vaultName"></param>
       /// <returns></returns>
        private static string VaultImportOverwriteFile(string vaultName)
        {
            GlobalVariables.vaultName = vaultName;
            ImportNotification importNotification = new ImportNotification();
            importNotification.ShowDialog();
            return GlobalVariables.importConfirmation;
        }
    }
}
