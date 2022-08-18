﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

// Nice-To-Have
// If you want -- make 'required' labels disappear when data is correct

/// ***File controlling behaviour of 'Scenario' Tab***
/// This tab provides info about file used in a specific scenario - names and what files will be edited
/// Tab content is reseted on tab switch, so all data MUST be saved in a separate class
/// ON LOAD tab loads data from that class and fills modified components
/// BASIC REQUIREMENTS are needed to safely unlock other tabs without files errors
/// - e.g. if no file is selected and user tries to modify it

namespace SRScenarioCreatorEnhanced.UserControls
{
    public partial class UC_Scenario : UserControl
    {
        // Reference to editorMainWindow ; allows to edit currently active scenario's data
        private editorMainWindow mainWindow;
        public UC_Scenario(editorMainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;

            // Load file names to combo boxes
            loadFileNamesToEachComponent();

            // Load chosen options
            loadDataFromScenarioContent();

            activateOtherTabsIfPossible();
        }

        #region loadingFileNamesAndChoices

        /// <summary>
        /// Load names of available files from game directory to each combo box
        /// </summary>
        private void loadFileNamesToEachComponent()
        {
            // Load Scenarios
            loadDataToCombobox(comboScenarioName,"SCENARIO", @"\Scenario\Custom\");
            // Load Cache
            loadDataToCombobox(comboCacheName,"SAV", @"\Cache\");

            // Load Maps
            loadDataToCombobox(comboMapName,"MAPX", @"\Maps\");
            // Load OOF
            loadDataToCombobox(comboOOF,"OOF", @"\Maps\");

            // Load UNIT
            loadDataToCombobox(comboUnit, "UNIT", @"\Maps\Data\");
            // Load PPLX
            loadDataToCombobox(comboPPLX, "PPLX", @"\Maps\Data\");
            // Load TTRX
            loadDataToCombobox(comboTTRX, "TTRX", @"\Maps\Data\");
            // Load TERX
            loadDataToCombobox(comboTERX, "TERX", @"\Maps\Data\");
            // Load NEWSITEMS
            loadDataToCombobox(comboNewsItems, "NEWSITEMS", @"\Maps\Data\");
            // Load PROFILE
            loadDataToCombobox(comboProfile, "PRF", @"\Maps\Data\");

            // Load CVP
            loadDataToCombobox(comboCVP, "CVP", @"\Maps\");
            // Load WMDATA
            loadDataToCombobox(comboWM, "WMData", @"\Maps\Data\");
            // Load OOB
            loadDataToCombobox(comboOOB, "OOB", @"\Maps\Orbats");
        }

        /// <summary>
        /// Loads file names from given directory to given combobox
        /// </summary>
        /// <param name="comboName">Name of combobox to be filled with name</param>
        /// <param name="searchDirectory">Where to look for files (skip the base game directory) e.g. "Scenario\\Custom\\</param>
        /// <param name="extension">Target files extension, without dot</param>
        private void loadDataToCombobox(ComboBox comboName, string extension, string searchDirectory)
        {
            // Clear ComboBox content
            comboName.Items.Clear();

            // Add base game dir to make a full path
            searchDirectory = mainWindow.currentScenario.getBaseGameDirectory() + searchDirectory;
            // Fill combobox with found names
            comboName.Items.AddRange( getListOfFiles(searchDirectory, extension) );
        }

        /// <summary>
        /// Finds list of file names in the given directory, which have given extension
        /// </summary>
        /// <param name="searchDirectory">Where to look for target files; give full directory</param>
        /// <param name="extension">What extension are you looking for w/o dot e.g. "scenario"</param>
        /// <returns>List of file names of target extension found in directory</returns>
        private string[] getListOfFiles(string searchDirectory, string extension)
        {
            // Get files from searchDirectory, get only these with given extension.
            // Next, eliminate path and extension to leave just the file name.
            // By default it gives IEnumerable, so convert it to array
            return Directory.GetFiles(searchDirectory, $"*.{extension}",
                SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension).ToArray();
        }

        /// <summary>
        /// Loads data on user's choices saved in scenario class
        /// </summary>
        private void loadDataFromScenarioContent()
        {
            // Load text
            comboScenarioName.Text = mainWindow.currentScenario.scenarioName;
            comboCacheName.Text = mainWindow.currentScenario.cacheName;

            comboMapName.Text = mainWindow.currentScenario.mapName;
            comboOOF.Text = mainWindow.currentScenario.OOFName;

            comboUnit.Text = mainWindow.currentScenario.UnitName;
            comboPPLX.Text = mainWindow.currentScenario.PPLXName;
            comboTTRX.Text = mainWindow.currentScenario.TTRXName;
            comboTERX.Text = mainWindow.currentScenario.TERXName;
            comboNewsItems.Text = mainWindow.currentScenario.NewsItemsName;
            comboProfile.Text = mainWindow.currentScenario.ProfileName;

            comboCVP.Text = mainWindow.currentScenario.CVPName;
            comboWM.Text = mainWindow.currentScenario.WMName;
            comboOOB.Text = mainWindow.currentScenario.OOBName;
            comboPreCache.Text = mainWindow.currentScenario.PreCacheName;
            comboPostCache.Text = mainWindow.currentScenario.PostCacheName;

            // Load checks
            checkCacheName.Checked = mainWindow.currentScenario.cacheSameNameCheck;

            checkNewMap.Checked = mainWindow.currentScenario.newMapCheck;
            checkOOF.Checked = mainWindow.currentScenario.OOFSameNameCheck;

            checkNoneditDefault.Checked = mainWindow.currentScenario.allNonEditableDefaultCheck;

            checkModifyCVP.Checked = mainWindow.currentScenario.CVPModifyCheck;
            checkModifyWM.Checked = mainWindow.currentScenario.WMModifyCheck;
            checkModifyOOB.Checked = mainWindow.currentScenario.OOBModifyCheck;
        }

        #endregion

        #region managingOtherTabs

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
                Globals.isTheatersActive = checkModifyCVP.Checked;
                Globals.isRegionsActive = checkModifyCVP.Checked;

                // Unlock Resources and WM tabs
                Globals.isResourcesActive = checkModifyWM.Checked;
                Globals.isWMActive = checkModifyWM.Checked;

                // Unlock Orbat tab (if not empty!)
                Globals.isOrbatActive = checkModifyOOB.Checked && !string.IsNullOrEmpty(comboOOB.Text);
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
        #endregion

        #region CheckBoxesSection
        
        #region generalChecks
        // 'General' Checks
        private void checkCacheName_CheckedChanged(object sender, EventArgs e)
        {
            comboCacheName.Enabled = !checkCacheName.Checked;
            mainWindow.currentScenario.cacheSameNameCheck = checkCacheName.Checked;

            if (checkCacheName.Checked)
            {
                comboCacheName.Text = comboScenarioName.Text;
                mainWindow.currentScenario.cacheName = comboScenarioName.Text;
            }

            activateOtherTabsIfPossible();
        }
        #endregion

        #region mapsChecks
        private void checkNewMap_CheckedChanged(object sender, EventArgs e)
        {
            mainWindow.currentScenario.newMapCheck = checkNewMap.Checked;
            // Create new map: either copy-paste default map and empty it or copy-paste hard-coded content
        }

        private void checkOOF_CheckedChanged(object sender, EventArgs e)
        {
            comboOOF.Enabled = !checkOOF.Checked;
            mainWindow.currentScenario.OOFSameNameCheck = checkOOF.Checked;

            if (checkOOF.Checked)
            {
                comboOOF.Text = comboMapName.Text;
                mainWindow.currentScenario.OOFName = comboMapName.Text;
            }
        }
        #endregion

        #region nonEditableChecks
        // 'Non-editable' Checks
        private void checkNoneditDefault_CheckedChanged(object sender, EventArgs e)
        {
            activateOtherTabsIfPossible();
            comboUnit.Enabled = !checkNoneditDefault.Checked;
            comboPPLX.Enabled = !checkNoneditDefault.Checked;
            comboTTRX.Enabled = !checkNoneditDefault.Checked;
            comboTERX.Enabled = !checkNoneditDefault.Checked;
            comboNewsItems.Enabled = !checkNoneditDefault.Checked;
            comboProfile.Enabled = !checkNoneditDefault.Checked;

            mainWindow.currentScenario.allNonEditableDefaultCheck = checkNoneditDefault.Checked;
        }
        #endregion

        #region modifyChecks
        // 'Modify' Checks
        private void checkModifyCVP_CheckedChanged(object sender, EventArgs e)
        {
            mainWindow.currentScenario.CVPModifyCheck = checkModifyCVP.Checked;
            activateOtherTabsIfPossible();
        }

        private void checkModifyWM_CheckedChanged(object sender, EventArgs e)
        {
            mainWindow.currentScenario.WMModifyCheck = checkModifyWM.Checked;
            activateOtherTabsIfPossible();
        }

        private void checkModifyOOB_CheckedChanged(object sender, EventArgs e)
        {
            mainWindow.currentScenario.OOBModifyCheck = checkModifyOOB.Checked;
            activateOtherTabsIfPossible();
        }
        #endregion

        #endregion

        #region ComboBoxesTextUpdateSecion

        #region generalInfo
        private void comboScenarioName_TextUpdate(object sender, EventArgs e)
        {
            // Update Cache name if it should be the same
            if (checkCacheName.Checked)
            {
                comboCacheName.Text = comboScenarioName.Text;
                mainWindow.currentScenario.cacheName = comboScenarioName.Text;
            }

            // Update name in the scenario class
            mainWindow.currentScenario.scenarioName = comboScenarioName.Text;

            activateOtherTabsIfPossible();
        }

        private void comboCacheName_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.cacheName = comboCacheName.Text;

            activateOtherTabsIfPossible();
        }
        #endregion

        #region mapInfo
        private void comboMapName_TextUpdate(object sender, EventArgs e)
        {
            // Update Cache name if it should be the same
            if (checkOOF.Checked)
            {
                comboOOF.Text = comboMapName.Text;
                mainWindow.currentScenario.OOFName = comboMapName.Text;
            }

            // Update name in the scenario class
            mainWindow.currentScenario.mapName = comboMapName.Text;

            activateOtherTabsIfPossible();
        }
        private void comboOOF_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.OOFName = comboOOF.Text;

            activateOtherTabsIfPossible();
        }
        #endregion

        #region nonEditableDataRegion
        private void comboUnit_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.UnitName = comboUnit.Text;

            activateOtherTabsIfPossible();
        }

        private void comboPPLX_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.PPLXName = comboPPLX.Text;

            activateOtherTabsIfPossible();
        }

        private void comboTTRX_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.TTRXName = comboTTRX.Text;

            activateOtherTabsIfPossible();
        }

        private void comboTERX_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.TERXName = comboTERX.Text;

            activateOtherTabsIfPossible();
        }

        private void comboNewsItems_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.NewsItemsName = comboNewsItems.Text;

            activateOtherTabsIfPossible();
        }

        private void comboProfile_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.ProfileName = comboProfile.Text;

            activateOtherTabsIfPossible();
        }
        #endregion

        #region editableDataInfo
        private void comboCVP_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.CVPName = comboCVP.Text;

            activateOtherTabsIfPossible();
        }

        private void comboWM_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.WMName = comboWM.Text;

            activateOtherTabsIfPossible();
        }

        private void comboOOB_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.OOBName = comboOOB.Text;

            activateOtherTabsIfPossible();
        }

        private void comboPreCache_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.PreCacheName = comboPreCache.Text;

            activateOtherTabsIfPossible();
        }

        private void comboPostCache_TextUpdate(object sender, EventArgs e)
        {
            // Update name in the scenario class
            mainWindow.currentScenario.PostCacheName = comboPostCache.Text;

            activateOtherTabsIfPossible();
        }
        #endregion

        #endregion

        private void exportScenarioButton_Click(object sender, EventArgs e)
        {
            mainWindow.currentScenario.exportScenarioToFileAndFolder();
        }

        // Changed comboScenarioName index selection must be separate, to load data into other comboboxes correctly
        private void comboScenarioName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Execute standard commands
            comboScenarioName_TextUpdate(sender, e);

            // Load content from selected .scenario file
            mainWindow.currentScenario.loadDataFromScenarioFileToActiveScenario(comboScenarioName.Text);
            // Refresh tab
            // TODO
        }
    }
}