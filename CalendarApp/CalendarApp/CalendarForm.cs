using System;
using System.Windows.Forms;

namespace CalendarApp
{
    internal class CalendarForm : Form
    {
        private ListBox lstAppointments;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh, btnJoinGroup, btnGroupMeetings;
        private CalendarData currentData;

        public CalendarForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Calendar App - Quản lý cuộc hẹn";
            this.Size = new System.Drawing.Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Header label
            Label lblTitle = new Label
            {
                Text = "Danh sách cuộc hẹn",
                Location = new System.Drawing.Point(12, 12),
                Size = new System.Drawing.Size(400, 30),
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold)
            };
            Controls.Add(lblTitle);

            // ListBox for appointments
            lstAppointments = new ListBox
            {
                Location = new System.Drawing.Point(12, 50),
                Size = new System.Drawing.Size(960, 450),
                SelectionMode = SelectionMode.One,
                Font = new System.Drawing.Font("Courier New", 10)
            };
            Controls.Add(lstAppointments);

            // Buttons panel
            int buttonY = 510;
            int buttonX = 12;

            btnAdd = new Button
            {
                Text = "➕ Thêm cuộc hẹn",
                Location = new System.Drawing.Point(buttonX, buttonY),
                Size = new System.Drawing.Size(150, 35),
                Font = new System.Drawing.Font("Arial", 10)
            };
            btnAdd.Click += BtnAdd_Click;
            Controls.Add(btnAdd);

            btnEdit = new Button
            {
                Text = "✏️ Sửa",
                Location = new System.Drawing.Point(buttonX + 160, buttonY),
                Size = new System.Drawing.Size(120, 35),
                Font = new System.Drawing.Font("Arial", 10)
            };
            btnEdit.Click += BtnEdit_Click;
            Controls.Add(btnEdit);

            btnDelete = new Button
            {
                Text = "🗑️ Xóa",
                Location = new System.Drawing.Point(buttonX + 290, buttonY),
                Size = new System.Drawing.Size(120, 35),
                Font = new System.Drawing.Font("Arial", 10)
            };
            btnDelete.Click += BtnDelete_Click;
            Controls.Add(btnDelete);

            btnJoinGroup = new Button
            {
                Text = "👥 Tham gia nhóm",
                Location = new System.Drawing.Point(buttonX + 420, buttonY),
                Size = new System.Drawing.Size(140, 35),
                Font = new System.Drawing.Font("Arial", 10)
            };
            btnJoinGroup.Click += BtnJoinGroup_Click;
            Controls.Add(btnJoinGroup);

            btnGroupMeetings = new Button
            {
                Text = "👨‍💼 Cuộc họp nhóm",
                Location = new System.Drawing.Point(buttonX + 570, buttonY),
                Size = new System.Drawing.Size(150, 35),
                Font = new System.Drawing.Font("Arial", 10)
            };
            btnGroupMeetings.Click += BtnGroupMeetings_Click;
            Controls.Add(btnGroupMeetings);

            btnRefresh = new Button
            {
                Text = "🔄 Làm mới",
                Location = new System.Drawing.Point(buttonX + 730, buttonY),
                Size = new System.Drawing.Size(120, 35),
                Font = new System.Drawing.Font("Arial", 10)
            };
            btnRefresh.Click += BtnRefresh_Click;
            Controls.Add(btnRefresh);

            // Status label
            Label lblStatus = new Label
            {
                Name = "lblStatus",
                Text = "Sẵn sàng",
                Location = new System.Drawing.Point(12, 555),
                Size = new System.Drawing.Size(960, 25),
                Font = new System.Drawing.Font("Arial", 10)
            };
            Controls.Add(lblStatus);

            this.Load += CalendarForm_Load;
        }

        private void CalendarForm_Load(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void LoadData()
        {
            currentData = AppointmentService.LoadFromFile();
        }

        private void RefreshList()
        {
            LoadData();
            lstAppointments.Items.Clear();
            
            // Show all appointments and group meetings
            foreach (var appt in currentData.Appointments)
            {
                lstAppointments.Items.Add($"[Cuộc hẹn] {appt.ToString()}");
            }

            foreach (var meeting in currentData.GroupMeetings)
            {
                lstAppointments.Items.Add($"[Nhóm] {meeting.ToString()}");
            }

            UpdateStatus($"Tổng: {currentData.Appointments.Count + currentData.GroupMeetings.Count} cuộc hẹn");
        }

        private void UpdateStatus(string message)
        {
            if (Controls.ContainsKey("lblStatus"))
            {
                Controls["lblStatus"].Text = message;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (AddAppointmentDialog dialog = new AddAppointmentDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Appointment newAppt = dialog.GetAppointment();
                    int reminderMinutes = dialog.GetReminderMinutes();

                    // Validate
                    var validation = AppointmentService.ValidateAppointment(newAppt);
                    if (validation != ValidationResult.VALID)
                    {
                        MessageBox.Show(GetValidationError(validation), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Check overlap
                    var overlaps = AppointmentService.CheckOverlap(newAppt, currentData.Appointments);
                    if (overlaps.Count > 0)
                    {
                        DialogResult result = MessageBox.Show(
                            $"Cuộc hẹn trùng với {overlaps.Count} cuộc hẹn khác. Bạn muốn thay thế chúng?",
                            "Cảnh báo", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            // Replace overlapping appointments
                            foreach (var overlap in overlaps)
                            {
                                currentData.Appointments.Remove(overlap);
                            }
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            return;
                        }
                    }

                    currentData.Appointments.Add(newAppt);

                    // Add reminder if specified
                    if (reminderMinutes > 0)
                    {
                        AppointmentService.AddReminder(newAppt.Id, newAppt.Title, DateTime.Now.AddMinutes(reminderMinutes));
                    }

                    AppointmentService.SaveToFile(currentData);
                    RefreshList();
                    MessageBox.Show("Thêm cuộc hẹn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (lstAppointments.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn một cuộc hẹn", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int index = lstAppointments.SelectedIndex;
            
            // Determine if it's a regular appointment or group meeting
            Appointment selectedAppt = null;
            bool isGroupMeeting = false;

            if (index < currentData.Appointments.Count)
            {
                selectedAppt = currentData.Appointments[index];
            }
            else
            {
                isGroupMeeting = true;
                selectedAppt = currentData.GroupMeetings[index - currentData.Appointments.Count];
            }

            using (AddAppointmentDialog dialog = new AddAppointmentDialog())
            {
                dialog.Text = "Sửa cuộc hẹn";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Appointment editedAppt = dialog.GetAppointment();

                    var validation = AppointmentService.ValidateAppointment(editedAppt);
                    if (validation != ValidationResult.VALID)
                    {
                        MessageBox.Show(GetValidationError(validation), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (isGroupMeeting)
                    {
                        var groupMeeting = currentData.GroupMeetings[index - currentData.Appointments.Count];
                        groupMeeting.Title = editedAppt.Title;
                        groupMeeting.Location = editedAppt.Location;
                        groupMeeting.StartTime = editedAppt.StartTime;
                        groupMeeting.EndTime = editedAppt.EndTime;
                        groupMeeting.Description = editedAppt.Description;
                    }
                    else
                    {
                        currentData.Appointments[index] = editedAppt;
                    }

                    AppointmentService.SaveToFile(currentData);
                    RefreshList();
                    MessageBox.Show("Cập nhật cuộc hẹn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstAppointments.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn một cuộc hẹn", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa cuộc hẹn này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int index = lstAppointments.SelectedIndex;

                if (index < currentData.Appointments.Count)
                {
                    currentData.Appointments.RemoveAt(index);
                }
                else
                {
                    currentData.GroupMeetings.RemoveAt(index - currentData.Appointments.Count);
                }

                AppointmentService.SaveToFile(currentData);
                RefreshList();
                MessageBox.Show("Xóa cuộc hẹn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnJoinGroup_Click(object sender, EventArgs e)
        {
            if (lstAppointments.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn một cuộc hẹn", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int index = lstAppointments.SelectedIndex;

            // Check if it's a group meeting
            if (index >= currentData.Appointments.Count)
            {
                var groupMeeting = currentData.GroupMeetings[index - currentData.Appointments.Count];

                using (GroupMeetingDialog dialog = new GroupMeetingDialog(groupMeeting))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        var updatedMeeting = dialog.GetGroupMeeting();
                        currentData.GroupMeetings[index - currentData.Appointments.Count] = updatedMeeting;
                        AppointmentService.SaveToFile(currentData);
                        RefreshList();
                        MessageBox.Show("Cập nhật cuộc họp nhóm thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một cuộc họp nhóm", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnGroupMeetings_Click(object sender, EventArgs e)
        {
            using (AddAppointmentDialog dialog = new AddAppointmentDialog())
            {
                dialog.Text = "Tạo cuộc họp nhóm";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Appointment newAppt = dialog.GetAppointment();

                    var validation = AppointmentService.ValidateAppointment(newAppt);
                    if (validation != ValidationResult.VALID)
                    {
                        MessageBox.Show(GetValidationError(validation), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var groupMeeting = new GroupMeeting
                    {
                        Title = newAppt.Title,
                        Location = newAppt.Location,
                        StartTime = newAppt.StartTime,
                        EndTime = newAppt.EndTime,
                        Description = newAppt.Description
                    };

                    currentData.GroupMeetings.Add(groupMeeting);
                    AppointmentService.SaveToFile(currentData);

                    // Open group meeting dialog to add participants
                    using (GroupMeetingDialog groupDialog = new GroupMeetingDialog(groupMeeting))
                    {
                        groupDialog.ShowDialog();
                        AppointmentService.SaveToFile(currentData);
                    }

                    RefreshList();
                    MessageBox.Show("Tạo cuộc họp nhóm thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshList();
            MessageBox.Show("Làm mới danh sách thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GetValidationError(ValidationResult result)
        {
            return result switch
            {
                ValidationResult.EMPTY_TITLE => "Tiêu đề không được để trống",
                ValidationResult.NEGATIVE_DURATION => "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc",
                _ => "Dữ liệu không hợp lệ"
            };
        }
    }
}
