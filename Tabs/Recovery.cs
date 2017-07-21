﻿#region Copyright (C)

// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Recovery.cs" company="Smurf-IV">
//
//  Copyright (C) 2015 Simon Coghlan (Aka Smurf-IV)
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//   any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, see http://www.gnu.org/licenses/.
//  </copyright>
//  <summary>
//  Url: http://Elucidate.codeplex.com/
//  Email: http://www.codeplex.com/site/users/view/smurfiv
//  </summary>
// --------------------------------------------------------------------------------------------------------------------

#endregion Copyright (C)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Shared;

namespace Elucidate
{
   public sealed partial class Elucidate
   {
      #region Recovery options pane

      private void tabControl1_Selected(object sender, TabControlEventArgs e)
      {
         if (e.TabPage == RecoveryOperations)
         {
            WindowState = FormWindowState.Maximized;
            webBrowser1.Url = new Uri("http://snapraid.sourceforge.net/manual.html#9", UriKind.Absolute);
         }
         else if (e.TabPage == coveragePage)
         {
            if (Properties.Settings.Default.ConfigFileIsValid)
            {
               try
               {
                  ConfigFileHelper cfg = new ConfigFileHelper(Properties.Settings.Default.ConfigFileLocation);
                  cfg.Read();
                  List<string> displayLines = cfg.SnapShotSources;
                  // Add the Parity lines to show the amount of drive space currently occupied by SnapRaid
                  displayLines.Add(new FileInfo(cfg.ParityFile).DirectoryName);
                  if (!string.IsNullOrEmpty(cfg.QParityFile))
                  {
                     displayLines.Add(new FileInfo(cfg.QParityFile).DirectoryName);
                  }
                  driveSpace.StartProcessing(displayLines);
               }
               catch
               {
               }
            }
         }
      }

      private void tabControl1_Deselected(object sender, TabControlEventArgs e)
      {
         if (e.TabPage == RecoveryOperations)
         {
            if (WindowState == FormWindowState.Maximized)
            {
               WindowState = FormWindowState.Normal;
            }
         }
      }

      private void btnRemoveOutput_Click(object sender, EventArgs e)
      {
         if (DialogResult.Yes == MessageBoxExt.Show(this, "Are you sure you want to perform this task ?",
                        "Remove SnapRAID Output files.", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            try
            {
               ConfigFileHelper cfg = new ConfigFileHelper(Properties.Settings.Default.ConfigFileLocation);
               string readResult;
               if (!string.IsNullOrEmpty(readResult = cfg.Read()))
               {
                  MessageBoxExt.Show(this, readResult, "Config Read Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
               }
               else
               {
                  FileInfo fi;
                  if (!string.IsNullOrEmpty(cfg.ParityFile))
                  {
                     fi = new FileInfo(cfg.ParityFile);
                     if (fi.Exists)
                     {
                        fi.Delete();
                     }
                  }
                  if (!string.IsNullOrEmpty(cfg.QParityFile))
                  {
                     fi = new FileInfo(cfg.QParityFile);
                     if (fi.Exists)
                     {
                        fi.Delete();
                     }
                  }
                  if (cfg.ContentFiles != null)
                  {
                     foreach (string contentFile in cfg.ContentFiles.Where(contentFile => !string.IsNullOrEmpty(contentFile)))
                     {
                        fi = new FileInfo(contentFile);
                        if (fi.Exists)
                        {
                           fi.Delete();
                        }
                     }
                  }
               }
            }
            catch (Exception ex)
            {
               Log.Error("btnRemoveOutput_Click has thrown: ", ex);
               MessageBox.Show(this, ex.Message, "Remove SnapRAID Output files.");
            }
      }

      #endregion Recovery options pane

   }
}
