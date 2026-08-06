using Communication.Hardware.Timy;
using Communication.Shared.Dto;
using Communication.Shared.ExtensionsAndUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TestHost
{
    #region Timy

    // Everything related to the Timy Access feature lives inside this region:
    //   - Fields (group collections, current-selection / suppression state)
    //   - Initialization (InitializeTimyAccessTab)
    //   - Event Handlers (list/grid/button events for Day & Week groups, Add Command, Generate JSON, Save)
    //   - Helper Methods (list refresh, detail load, grid flush, dropdown rebuild, small utilities)
    //   - Serialization (BuildTimyConfiguration / Generate JSON)
    //   - Save / Load (persist to and restore from TimyAccessConfiguration.json)
    //   - Models / helper types (TimyAccessConfiguration, TimyDayIndexItem)

    /// <summary>
    /// Behaviour for the Timy Access tab (tpTimyAccess). The controls themselves are declared in
    /// FormHost.Designer.cs so they are visible in the Windows Form Designer; this file only wires
    /// up the data binding, add/remove logic and JSON generation.
    /// </summary>
    public partial class FormHost
    {
        private const int TimyMaxGroups = 8;

        private readonly BindingList<DtoTimyDayTimezoneGroup> _timyDayGroups =
            new BindingList<DtoTimyDayTimezoneGroup>();
        private readonly BindingList<DtoTimyWeekTimezoneGroup> _timyWeekGroups =
            new BindingList<DtoTimyWeekTimezoneGroup>();

        // Groups whose grids are currently bound; used to flush edits back to the model.
        private DtoTimyDayTimezoneGroup _currentTimyDayGroup;
        private DtoTimyWeekTimezoneGroup _currentTimyWeekGroup;
        private bool _suppressTimyEvents;

        /// <summary>
        /// Runtime wiring that can't be expressed in the designer (enum data sources, grid binding
        /// mode, initial enabled state). Call once, right after InitializeComponent().
        /// </summary>
        private void InitializeTimyAccessTab()
        {
            dgvTimyDayTimezones.AutoGenerateColumns = false;
            dgvTimyWeekTimezones.AutoGenerateColumns = false;

            // WeekDay column shows the DayOfWeek values.
            colWeekDay.DataSource = Enum.GetValues(typeof(DayOfWeek));

            RefreshTimyWeekIndexItems();
            UpdateTimyDayDetailEnabled();
            UpdateTimyWeekDetailEnabled();
            InitializeTimySetForUserTab();
            InitializeTimyHolidayTab();

            // Restore the last saved configuration (if any) when the form opens.
            LoadTimyConfigurationFromDisk();
        }

        // ---------- Day group handlers ----------

        private void BtnTimyDayAdd_Click(object sender, EventArgs e)
        {
            if (_timyDayGroups.Count >= TimyMaxGroups)
            {
                MessageBox.Show($"A maximum of {TimyMaxGroups} Day Timezone Groups can be created.");
                return;
            }

            FlushTimyDayGrid();
            var group = new DtoTimyDayTimezoneGroup
            {
                Id = NextTimyId(_timyDayGroups.Select(g => g.Id)),
                DeviceIndex = NextAvailableTimyIndex(_timyDayGroups.Select(g => g.DeviceIndex)),
                Title = "Day Group " + (_timyDayGroups.Count + 1),
                DayTimezoneIntervals = new List<DtoTimyDayTimezoneInterval>()
            };
            _timyDayGroups.Add(group);
            RefreshTimyDayGroupList();
            lstTimyDayGroups.SelectedIndex = _timyDayGroups.Count - 1;
            RefreshTimyWeekIndexItems();
        }

        private void BtnTimyDayRemove_Click(object sender, EventArgs e)
        {
            var index = lstTimyDayGroups.SelectedIndex;
            if (index < 0) return;

            // Detach the grid first so nothing is flushed back into the removed group.
            _currentTimyDayGroup = null;
            dgvTimyDayTimezones.DataSource = null;
            _timyDayGroups.RemoveAt(index);

            RefreshTimyDayGroupList();
            if (_timyDayGroups.Count > 0)
                lstTimyDayGroups.SelectedIndex = Math.Min(index, _timyDayGroups.Count - 1);
            else
                LoadTimyDayGroupDetail();

            RefreshTimyWeekIndexItems();
        }

        private void LstTimyDayGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressTimyEvents) return;
            FlushTimyDayGrid();
            LoadTimyDayGroupDetail();
        }

        private void NumTimyDayDeviceIndex_ValueChanged(object sender, EventArgs e)
        {
            if (_suppressTimyEvents) return;
            var group = SelectedTimyDayGroup;
            if (group != null) group.DeviceIndex = (int)numTimyDayDeviceIndex.Value;
        }

        private void TxtTimyDayTitle_TextChanged(object sender, EventArgs e)
        {
            if (_suppressTimyEvents) return;
            var group = SelectedTimyDayGroup;
            if (group == null) return;
            group.Title = txtTimyDayTitle.Text;
            RefreshTimyDayGroupList();
            RefreshTimyWeekIndexItems();
        }

        private void LoadTimyDayGroupDetail()
        {
            var group = SelectedTimyDayGroup;
            _suppressTimyEvents = true;
            if (group == null)
            {
                numTimyDayDeviceIndex.Value = 0;
                txtTimyDayTitle.Text = string.Empty;
                dgvTimyDayTimezones.DataSource = null;
                _currentTimyDayGroup = null;
            }
            else
            {
                numTimyDayDeviceIndex.Value = ClampToNumeric(group.DeviceIndex, numTimyDayDeviceIndex);
                txtTimyDayTitle.Text = group.Title ?? string.Empty;
                if (group.DayTimezoneIntervals == null) group.DayTimezoneIntervals = new List<DtoTimyDayTimezoneInterval>();
                dgvTimyDayTimezones.DataSource = new BindingList<DtoTimyDayTimezoneInterval>(group.DayTimezoneIntervals);
                _currentTimyDayGroup = group;
            }
            _suppressTimyEvents = false;
            UpdateTimyDayDetailEnabled();
        }

        private void FlushTimyDayGrid()
        {
            dgvTimyDayTimezones.EndEdit();
            if (_currentTimyDayGroup != null &&
                dgvTimyDayTimezones.DataSource is BindingList<DtoTimyDayTimezoneInterval> list)
            {
                _currentTimyDayGroup.DayTimezoneIntervals = list.ToList();
            }
        }

        private void RefreshTimyDayGroupList()
        {
            _suppressTimyEvents = true;
            var selected = lstTimyDayGroups.SelectedIndex;
            lstTimyDayGroups.BeginUpdate();
            lstTimyDayGroups.Items.Clear();
            for (var i = 0; i < _timyDayGroups.Count; i++)
                lstTimyDayGroups.Items.Add($"{i + 1} - {DisplayTitle(_timyDayGroups[i].Title)}");
            if (selected >= 0 && selected < lstTimyDayGroups.Items.Count)
                lstTimyDayGroups.SelectedIndex = selected;
            lstTimyDayGroups.EndUpdate();
            _suppressTimyEvents = false;
        }

        private void UpdateTimyDayDetailEnabled()
        {
            var enabled = SelectedTimyDayGroup != null;
            numTimyDayDeviceIndex.Enabled = enabled;
            txtTimyDayTitle.Enabled = enabled;
            dgvTimyDayTimezones.Enabled = enabled;
        }

        private DtoTimyDayTimezoneGroup SelectedTimyDayGroup =>
            lstTimyDayGroups.SelectedIndex >= 0 && lstTimyDayGroups.SelectedIndex < _timyDayGroups.Count
                ? _timyDayGroups[lstTimyDayGroups.SelectedIndex]
                : null;

        // ---------- Week group handlers ----------

        private void BtnTimyWeekAdd_Click(object sender, EventArgs e)
        {
            if (_timyWeekGroups.Count >= TimyMaxGroups)
            {
                MessageBox.Show($"A maximum of {TimyMaxGroups} Week Timezone Groups can be created.");
                return;
            }

            FlushTimyWeekGrid();
            var group = new DtoTimyWeekTimezoneGroup
            {
                Id = NextTimyId(_timyWeekGroups.Select(g => g.Id)),
                DeviceIndex = NextAvailableTimyIndex(_timyWeekGroups.Select(g => g.DeviceIndex)),
                Title = "Week Group " + (_timyWeekGroups.Count + 1),
                Timezones = new List<DtoTimyWeekTimezone>()
            };

            // Pre-populate one row per weekday, pointing at the first available day zone.
            var defaultIndex = _timyDayGroups.Count > 0 ? _timyDayGroups.Min(g => g.DeviceIndex) : 0;
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                group.Timezones.Add(new DtoTimyWeekTimezone
                {
                    WeekTimezoneGroupId = group.Id,
                    WeekDay = day,
                    DayTimezoneIndex = defaultIndex
                });
            }

            _timyWeekGroups.Add(group);
            RefreshTimyWeekGroupList();
            lstTimyWeekGroups.SelectedIndex = _timyWeekGroups.Count - 1;
        }

        private void BtnTimyWeekRemove_Click(object sender, EventArgs e)
        {
            var index = lstTimyWeekGroups.SelectedIndex;
            if (index < 0) return;

            _currentTimyWeekGroup = null;
            dgvTimyWeekTimezones.DataSource = null;
            _timyWeekGroups.RemoveAt(index);

            RefreshTimyWeekGroupList();
            if (_timyWeekGroups.Count > 0)
                lstTimyWeekGroups.SelectedIndex = Math.Min(index, _timyWeekGroups.Count - 1);
            else
                LoadTimyWeekGroupDetail();
        }

        private void LstTimyWeekGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressTimyEvents) return;
            FlushTimyWeekGrid();
            LoadTimyWeekGroupDetail();
        }

        private void NumTimyWeekDeviceIndex_ValueChanged(object sender, EventArgs e)
        {
            if (_suppressTimyEvents) return;
            var group = SelectedTimyWeekGroup;
            if (group != null) group.DeviceIndex = (int)numTimyWeekDeviceIndex.Value;
        }

        private void TxtTimyWeekTitle_TextChanged(object sender, EventArgs e)
        {
            if (_suppressTimyEvents) return;
            var group = SelectedTimyWeekGroup;
            if (group == null) return;
            group.Title = txtTimyWeekTitle.Text;
            RefreshTimyWeekGroupList();
        }

        private void LoadTimyWeekGroupDetail()
        {
            var group = SelectedTimyWeekGroup;
            _suppressTimyEvents = true;
            if (group == null)
            {
                numTimyWeekDeviceIndex.Value = 0;
                txtTimyWeekTitle.Text = string.Empty;
                dgvTimyWeekTimezones.DataSource = null;
                _currentTimyWeekGroup = null;
            }
            else
            {
                numTimyWeekDeviceIndex.Value = ClampToNumeric(group.DeviceIndex, numTimyWeekDeviceIndex);
                txtTimyWeekTitle.Text = group.Title ?? string.Empty;
                if (group.Timezones == null) group.Timezones = new List<DtoTimyWeekTimezone>();
                dgvTimyWeekTimezones.DataSource = new BindingList<DtoTimyWeekTimezone>(group.Timezones);
                _currentTimyWeekGroup = group;
            }
            _suppressTimyEvents = false;
            UpdateTimyWeekDetailEnabled();
        }

        private void FlushTimyWeekGrid()
        {
            dgvTimyWeekTimezones.EndEdit();
            if (_currentTimyWeekGroup != null &&
                dgvTimyWeekTimezones.DataSource is BindingList<DtoTimyWeekTimezone> list)
            {
                _currentTimyWeekGroup.Timezones = list.ToList();
            }
        }

        private void RefreshTimyWeekGroupList()
        {
            _suppressTimyEvents = true;
            var selected = lstTimyWeekGroups.SelectedIndex;
            lstTimyWeekGroups.BeginUpdate();
            lstTimyWeekGroups.Items.Clear();
            for (var i = 0; i < _timyWeekGroups.Count; i++)
                lstTimyWeekGroups.Items.Add($"{i + 1} - {DisplayTitle(_timyWeekGroups[i].Title)}");
            if (selected >= 0 && selected < lstTimyWeekGroups.Items.Count)
                lstTimyWeekGroups.SelectedIndex = selected;
            lstTimyWeekGroups.EndUpdate();
            _suppressTimyEvents = false;

            // The user Weekzone combo mirrors the week groups, so refresh it on every change.
            RefreshUserWeekzoneItems();
        }

        private void UpdateTimyWeekDetailEnabled()
        {
            var enabled = SelectedTimyWeekGroup != null;
            numTimyWeekDeviceIndex.Enabled = enabled;
            txtTimyWeekTitle.Enabled = enabled;
            dgvTimyWeekTimezones.Enabled = enabled;
        }

        private DtoTimyWeekTimezoneGroup SelectedTimyWeekGroup =>
            lstTimyWeekGroups.SelectedIndex >= 0 && lstTimyWeekGroups.SelectedIndex < _timyWeekGroups.Count
                ? _timyWeekGroups[lstTimyWeekGroups.SelectedIndex]
                : null;

        /// <summary>
        /// Rebuilds the "1 - Office Hours" options offered by the DayTimezone dropdown on the week
        /// grid. Called whenever the day groups are added, removed or renamed.
        /// </summary>
        private void RefreshTimyWeekIndexItems()
        {
            if (colWeekDayTimezoneIndex == null) return;

            var items = new List<TimyDayIndexItem>
            {
                // Empty option: stored as DayTimezoneIndex 0 (= "no DayTimezone").
                new TimyDayIndexItem(0, string.Empty)
            };
            foreach (var group in _timyDayGroups)
                items.Add(new TimyDayIndexItem(group.DeviceIndex, $"{group.DeviceIndex} - {DisplayTitle(group.Title)}"));

            colWeekDayTimezoneIndex.DataSource = items;
            colWeekDayTimezoneIndex.DisplayMember = nameof(TimyDayIndexItem.Display);
            colWeekDayTimezoneIndex.ValueMember = nameof(TimyDayIndexItem.Value);

            if (dgvTimyWeekTimezones.IsHandleCreated)
                dgvTimyWeekTimezones.Invalidate();

            // The Holiday tab's DayTimezone combo mirrors the same day groups.
            RefreshTimyHolidayDayTimezoneItems();
        }

        // ---------- JSON ----------

        private void BtnTimyGenerateJson_Click(object sender, EventArgs e)
        {
            txtTimyJson.Text = SerializeTimyConfiguration(BuildTimyConfiguration());
            tcTimyInner.SelectedTab = tpTimyJsonOutput;
        }

        /// <summary>Path of the persisted configuration file, kept beside the executable.</summary>
        private static string TimyConfigurationFilePath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TimyAccessConfiguration.json");

        /// <summary>Collects the current UI state (device number + all groups) into the root model.</summary>
        private TimyAccessConfiguration BuildTimyConfiguration()
        {
            FlushTimyDayGrid();
            FlushTimyWeekGrid();

            return new TimyAccessConfiguration
            {
                DeviceNumber = txtTimyDeviceNumber.Text,
                DayTimezoneGroups = _timyDayGroups.ToList(),
                WeekTimezoneGroups = _timyWeekGroups.ToList(),
                UserAccess = BuildTimyUserAccess(),
                Holidays = _timyHolidays.ToList()
            };
        }

        private static string SerializeTimyConfiguration(TimyAccessConfiguration configuration) =>
            JsonConvert.SerializeObject(configuration, Formatting.Indented, new StringEnumConverter());

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Save current configuration
            try
            {
                var json = SerializeTimyConfiguration(BuildTimyConfiguration());
                File.WriteAllText(TimyConfigurationFilePath, json);
                MessageBox.Show("Configuration saved.");
            }
            catch (Exception exp)
            {
                MessageBox.Show("Failed to save configuration: " + exp.Message);
            }
        }

        /// <summary>
        /// Reads the persisted configuration on start-up and repopulates the tab. Missing/empty files
        /// are ignored; a corrupt file shows a message but never prevents the form from opening.
        /// </summary>
        private void LoadTimyConfigurationFromDisk()
        {
            try
            {
                var path = TimyConfigurationFilePath;
                if (!File.Exists(path)) return;

                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json)) return;

                var configuration = JsonConvert.DeserializeObject<TimyAccessConfiguration>(json);
                if (configuration == null) return;

                ApplyTimyConfiguration(configuration);
            }
            catch (Exception exp)
            {
                MessageBox.Show("Failed to load the saved configuration: " + exp.Message);
            }
        }

        /// <summary>Replaces the whole tab state with the supplied configuration and refreshes every dependent control.</summary>
        private void ApplyTimyConfiguration(TimyAccessConfiguration configuration)
        {
            _suppressTimyEvents = true;

            // Detach the grids so nothing is flushed back into the outgoing groups.
            _currentTimyDayGroup = null;
            _currentTimyWeekGroup = null;
            dgvTimyDayTimezones.DataSource = null;
            dgvTimyWeekTimezones.DataSource = null;

            txtTimyDeviceNumber.Text = configuration.DeviceNumber ?? string.Empty;

            _timyDayGroups.Clear();
            if (configuration.DayTimezoneGroups != null)
            {
                foreach (var group in configuration.DayTimezoneGroups.Take(TimyMaxGroups))
                {
                    if (group.DayTimezoneIntervals == null) group.DayTimezoneIntervals = new List<DtoTimyDayTimezoneInterval>();
                    // Indexes are 1-based; fix up any legacy 0 values saved before this rule existed.
                    if (group.DeviceIndex < 1)
                        group.DeviceIndex = NextAvailableTimyIndex(_timyDayGroups.Select(g => g.DeviceIndex));
                    _timyDayGroups.Add(group);
                }
            }

            _timyWeekGroups.Clear();
            if (configuration.WeekTimezoneGroups != null)
            {
                foreach (var group in configuration.WeekTimezoneGroups.Take(TimyMaxGroups))
                {
                    if (group.Timezones == null) group.Timezones = new List<DtoTimyWeekTimezone>();
                    if (group.DeviceIndex < 1)
                        group.DeviceIndex = NextAvailableTimyIndex(_timyWeekGroups.Select(g => g.DeviceIndex));
                    _timyWeekGroups.Add(group);
                }
            }

            _suppressTimyEvents = false;

            // Rebuild the list boxes and the DayTimezone dropdown options, then load the first group.
            RefreshTimyDayGroupList();
            RefreshTimyWeekGroupList();
            RefreshTimyWeekIndexItems();

            if (_timyDayGroups.Count > 0)
                lstTimyDayGroups.SelectedIndex = 0;
            else
                LoadTimyDayGroupDetail();

            if (_timyWeekGroups.Count > 0)
                lstTimyWeekGroups.SelectedIndex = 0;
            else
                LoadTimyWeekGroupDetail();

            ApplyTimyUserAccess(configuration.UserAccess);
            ApplyTimyHolidays(configuration.Holidays);
        }

        private void btnAddDayTimezone_Click(object sender, EventArgs e)
        {
            if (!txtTimyDeviceNumber.Text.CanConvertToInt32())
            {
                MessageBox.Show("شماره دستگاه Timy را وارد کنید");
                return;
            }

            var deviceInCache = _deviceComponent.SearchDeviceCache
                (d => d.DeviceNumber == txtTimyDeviceNumber.Text.ToInt32()).FirstOrDefault();
            if (deviceInCache == null)
            {
                MessageBox.Show("شماره دستگاه معتبر نمی باشد");
                return;
            }

            FlushTimyDayGrid();
            FlushTimyWeekGrid();
            var deviceConverted = _deviceComponent.ConvertDeviceToDeviceInfo
                (new List<DtoDevice> { deviceInCache }).FirstOrDefault();

            var deviceCommands = TimyPushCommands.GetDayTimezoneControlCommand
                (deviceConverted, _timyDayGroups.ToList(), 10, null, null, null, null);
            _deviceCommandComponent.Insert(deviceCommands);

        }

        private void btnAddWeekTimezone_Click(object sender, EventArgs e)
        {
            if (!txtTimyDeviceNumber.Text.CanConvertToInt32())
            {
                MessageBox.Show("شماره دستگاه Timy را وارد کنید");
                return;
            }

            var deviceInCache = _deviceComponent.SearchDeviceCache
                (d => d.DeviceNumber == txtTimyDeviceNumber.Text.ToInt32()).FirstOrDefault();
            if (deviceInCache == null)
            {
                MessageBox.Show("شماره دستگاه معتبر نمی باشد");
                return;
            }

            FlushTimyDayGrid();
            FlushTimyWeekGrid();
            var deviceConverted = _deviceComponent.ConvertDeviceToDeviceInfo
                (new List<DtoDevice> { deviceInCache }).FirstOrDefault();

            var deviceCommands = TimyPushCommands.GetWeekTimezoneControlCommand
                (deviceConverted, _timyWeekGroups.ToList(), 10, null, null, null, null);
            _deviceCommandComponent.Insert(deviceCommands);

        }

        // ---------- helpers ----------

        private void Timy_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Invalid free-text ints, or a DayTimezoneIndex not present in the current option list:
            // swallow so the grid quietly reverts the cell instead of throwing.
            e.ThrowException = false;
        }

        private static int NextTimyId(IEnumerable<int> existingIds)
        {
            var ids = existingIds.ToList();
            return ids.Count == 0 ? 1 : ids.Max() + 1;
        }

        /// <summary>Smallest 1-based index not already used (device timezone slots start at 1, not 0).</summary>
        private static int NextAvailableTimyIndex(IEnumerable<int> usedIndexes)
        {
            var used = new HashSet<int>(usedIndexes);
            var index = 1;
            while (used.Contains(index)) index++;
            return index;
        }

        private static string DisplayTitle(string title) =>
            string.IsNullOrWhiteSpace(title) ? "(untitled)" : title;

        private static decimal ClampToNumeric(int value, NumericUpDown control)
        {
            if (value < control.Minimum) return control.Minimum;
            if (value > control.Maximum) return control.Maximum;
            return value;
        }
    }

    /// <summary>
    /// Root model persisted to / loaded from disk and shown by the Generate JSON feature.
    /// Holds the device number together with every Day and Week timezone group.
    /// </summary>
    public class TimyAccessConfiguration
    {
        public string DeviceNumber { get; set; }

        public List<DtoTimyDayTimezoneGroup> DayTimezoneGroups { get; set; }

        public List<DtoTimyWeekTimezoneGroup> WeekTimezoneGroups { get; set; }

        public FormHost.TimyUserAccess UserAccess { get; set; }

        public List<DtoTimyHoliday> Holidays { get; set; }
    }

    /// <summary>Display/value pair backing the DayTimezone dropdown on the week grid.</summary>
    internal class TimyDayIndexItem
    {
        public TimyDayIndexItem(int value, string display)
        {
            Value = value;
            Display = display;
        }

        public int Value { get; }
        public string Display { get; }
    }

    #endregion
}
