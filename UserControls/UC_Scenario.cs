﻿using System;
using System.Windows.Forms;

namespace SRScenarioCreatorEnhanced.UserControls
{
    public partial class UC_Scenario : UserControl
    {
        private editorMainWindow mainWindow;
        public UC_Scenario(editorMainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;

            // Here load scenario data, saved during editing
            // -- When switching tabs, data on them resets, so we need to save it elsewhere

            activateOtherTabsIfPossible();
        }

        // Minimum required to allow to activate other tabs
        private void activateOtherTabsIfPossible()
        {
            // Basic requirements to allow unlocking tabs and other components
            if(areBasicRequirementsMet())
            {
                // Unlock Settings Tab and Export Button -- basic unlock
                Globals.isSettingsActive = true;
                exportScenarioButton.Enabled = true;

                // Unlock Theaters and Regions tabs
                if(checkModifyCVP.Checked)
                {
                    Globals.isTheatersActive = true;
                    Globals.isRegionsActive = true;
                }

                // Unlock Resources and WM tabs
                if(checkModifyWM.Checked)
                {
                    Globals.isResourcesActive = true;
                    Globals.isWMActive = true;
                }

                // Unlock Orbat tab
                if(checkModifyOOB.Checked)
                {
                    Globals.isOrbatActive = true;
                }
            }
            else // Disable them, if requirements are no longer met
            {
                exportScenarioButton.Enabled = false;

                Globals.isSettingsActive = false;
                Globals.isTheatersActive = false;
                Globals.isRegionsActive = false;
                Globals.isResourcesActive = false;
                Globals.isWMActive = false;
                Globals.isOrbatActive = false;
            }

            mainWindow.updateTabButtonsStatus();
        }

        // Checks if all required comboboxes / checkboxes are non-empty and it is safe to proceed
        private bool areBasicRequirementsMet()
        {
            if (string.IsNullOrEmpty(comboScenarioName.Text)) return false;
            if (string.IsNullOrEmpty(comboCacheName.Text) && checkCacheName.Checked == false) return false;
            if (string.IsNullOrEmpty(comboMapName.Text)) return false;
            if (string.IsNullOrEmpty(comboOOF.Text) && checkOOF.Checked == false) return false;
            if (string.IsNullOrEmpty(comboCVP.Text)) return false;
            if (string.IsNullOrEmpty(comboWM.Text)) return false;

            if(!checkNoneditDefault.Checked) // if default not selected
            {
                if (string.IsNullOrEmpty(comboUnit.Text)) return false;
                if (string.IsNullOrEmpty(comboPPLX.Text)) return false;
                if (string.IsNullOrEmpty(comboTTRX.Text)) return false;
                if (string.IsNullOrEmpty(comboTERX.Text)) return false;
                if (string.IsNullOrEmpty(comboNewsItems.Text)) return false;
                if (string.IsNullOrEmpty(comboProfile.Text)) return false;
            }

            return true;
        }

        // 'Maps' Checks
        private void checkNewMap_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkOOF_CheckedChanged(object sender, EventArgs e)
        {
            comboOOF.Enabled = !checkOOF.Checked;

            if (checkOOF.Checked)
            {
                comboOOF.Text = comboMapName.Text;
            }
        }

        // 'Modify' Checks
        private void checkModifyCVP_CheckedChanged(object sender, EventArgs e)
        {
            activateOtherTabsIfPossible();
        }

        private void checkModifyWM_CheckedChanged(object sender, EventArgs e)
        {
            activateOtherTabsIfPossible();
        }

        private void checkModifyOOB_CheckedChanged(object sender, EventArgs e)
        {
            activateOtherTabsIfPossible();
        }

        // 'General' Checks
        private void checkCacheName_CheckedChanged(object sender, EventArgs e)
        {
            comboCacheName.Enabled = !checkCacheName.Checked;
            
            if(checkCacheName.Checked)
            {
                comboCacheName.Text = comboScenarioName.Text;
            }

            activateOtherTabsIfPossible();
        }

        // 'Non-editable' Checks
        private void checkNoneditDefault_CheckedChanged(object sender, EventArgs e)
        {
            activateOtherTabsIfPossible();
        }

        private void comboScenarioName_TextUpdate(object sender, EventArgs e)
        {
            // Update Cache name if it should be the same
            if (checkCacheName.Checked)
            {
                comboCacheName.Text = comboScenarioName.Text;
            }
        }

        private void comboMapName_TextUpdate(object sender, EventArgs e)
        {
            // Update OOF name if it should be the same
            if (checkOOF.Checked)
            {
                comboOOF.Text = comboMapName.Text;
            }
        }
    }
}

// CODE TO USE TO UPDATE BTNs STATUS
/*
 * Globals.isSettingsActive = checkBox1.Checked;
            main.updateTabButtonsStatus();
*/

//checkBox1.Checked = Globals.isSettingsActive;


/*Code to unlock all tabs by checking checkbox
             * Globals.isSettingsActive = true;
            Globals.isTheatersActive = true;
            Globals.isRegionsActive = true;
            Globals.isResourcesActive = true;
            Globals.isWMActive = true;
            Globals.isOrbatActive = true;
            mainWindow.updateTabButtonsStatus();*/