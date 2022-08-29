﻿/// editorMainWindow.cs file released under GNU GPL v3 licence.
/// Originally used in the SRScenarioCreatorEnhanced project: https://github.com/r20de20/SRScenarioCreatorEnhanced

using SRScenarioCreatorEnhanced.UserControls;
using System;
using System.Deployment.Application;
using System.Reflection;
using System.Windows.Forms;

// ***NOTE***
// It took total of 10h to find a way to connect editorMainWindow form to UC instances
// Needed to disable buttons, which user won't use and to allow data transport to the whole app
// If you ever find a way to change it (passing Form as arg) to events' system -- go ahead
// - Michael '8', 2022/08/12 14:29

namespace SRScenarioCreatorEnhanced
{
    public partial class editorMainWindow : Form
    {
        #region movableToolBarPanelAssets
        // Movable toolbarPanel assets
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        public ScenarioContent currentScenario;

        public editorMainWindow()
        {
            InitializeComponent();

            currentScenario = new ScenarioContent();

            // Load Scenario tab
            UC_Scenario uc = new UC_Scenario(this);
            addUserControl(uc);
        }

        #region generalWindowControls
        // Make window movable by grabbing toolbar
        private void toolbarPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        // ... or titleLabel
        private void titleLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        // Check if status (enabled/disabled) was changed for any button
        // And update (invert) their status accordingly
        public void updateTabButtonsStatus()
        {
            if (Globals.isSettingsActive != tabSettingsBtn.Enabled) tabSettingsBtn.Enabled = !tabSettingsBtn.Enabled;
            if (Globals.isTheatersActive != tabTheatersBtn.Enabled) tabTheatersBtn.Enabled = !tabTheatersBtn.Enabled;
            if (Globals.isRegionsActive != tabRegionsBtn.Enabled) tabRegionsBtn.Enabled = !tabRegionsBtn.Enabled;
            if (Globals.isResourcesActive != tabResourcesBtn.Enabled) tabResourcesBtn.Enabled = !tabResourcesBtn.Enabled;
            if (Globals.isWMActive != tabWMBtn.Enabled) tabWMBtn.Enabled = !tabWMBtn.Enabled;
            if (Globals.isOrbatActive != tabOrbatBtn.Enabled) tabOrbatBtn.Enabled = !tabOrbatBtn.Enabled;
        }

        // Display new tab
        private void addUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            mainUCPanel.Controls.Clear();
            mainUCPanel.Controls.Add(userControl);
            userControl.BringToFront();
        }
        #endregion

        #region toolbarButtons
        // Exit button clicked
        private void exitButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // Info button clicked
        private void infoButton_Click(object sender, EventArgs e)
        {
            // Get assembly version
            string versionNumber = ApplicationDeployment.IsNetworkDeployed
               ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
               : Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // Display assembly version in message box
            MessageBox.Show($"Current version: {versionNumber}\n" +
                $"Contributors: Michael '8', xperga", "About Editor", MessageBoxButtons.OK,
                MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        #endregion

        #region tabsButtons
        // Tab buttons click event
        // tabScenario is public, so export button can work
        public void tabScenarioBtn_Click(object sender, EventArgs e)
        {
            UC_Scenario uc = new UC_Scenario(this);
            addUserControl(uc);
        }

        private void tabSettingsBtn_Click(object sender, EventArgs e)
        {
            UC_Settings uc = new UC_Settings(this);
            addUserControl(uc);
        }

        private void tabTheatersBtn_Click(object sender, EventArgs e)
        {
            UC_Theaters uc = new UC_Theaters(this);
            addUserControl(uc);
        }

        private void tabRegionsBtn_Click(object sender, EventArgs e)
        {
            UC_Regions uc = new UC_Regions(this);
            addUserControl(uc);
        }

        private void tabResourcesBtn_Click(object sender, EventArgs e)
        {
            UC_Resources uc = new UC_Resources(this);
            addUserControl(uc);
        }

        private void tabWMBtn_Click(object sender, EventArgs e)
        {
            UC_WM uc = new UC_WM(this);
            addUserControl(uc);
        }

        private void tabOrbatBtn_Click(object sender, EventArgs e)
        {
            UC_Orbat uc = new UC_Orbat(this);
            addUserControl(uc);
        }
        #endregion
    }
}
