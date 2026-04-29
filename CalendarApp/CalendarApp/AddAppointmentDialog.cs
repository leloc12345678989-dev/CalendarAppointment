using System;
using System.Windows.Forms;

namespace CalendarApp
{
    public partial class AddAppointmentDialog : Form
    {
        private TextBox txtTitle, txtStart, txtEnd, txtLocation;
        private TextBox txtDesc;
        private NumericUpDown nudReminder;
        private ComboBox cmbReminderUnit;
        private Button btnSave, btnCancel;

        public AddAppointmentDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Thêm cuộc hẹn";
            this.Size = new System.Drawing.Size(500, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 12;

            // Title
            Label lblTitle = new Label { Text = "Tiêu đề:", Location = new System.Drawing.Point(12, y), Size = new System.Drawing.Size(80, 25) };
            txtTitle = new TextBox { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(350, 25) };
            Controls.Add(lblTitle);
            Controls.Add(txtTitle);

            // Location
            y += 35;
            Label lblLocation = new Label { Text = "Địa điểm:", Location = new System.Drawing.Point(12, y), Size = new System.Drawing.Size(80, 25) };
            txtLocation = new TextBox { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(350, 25) };
            Controls.Add(lblLocation);
            Controls.Add(txtLocation);

            // Start Time
            y += 35;
            Label lblStart = new Label { Text = "Bắt đầu:", Location = new System.Drawing.Point(12, y), Size = new System.Drawing.Size(80, 25) };
            txtStart = new TextBox { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(350, 25), Text = DateTime.Now.ToString("yyyy-MM-dd HH:00") };
            Controls.Add(lblStart);
            Controls.Add(txtStart);

            // End Time
            y += 35;
            Label lblEnd = new Label { Text = "Kết thúc:", Location = new System.Drawing.Point(12, y), Size = new System.Drawing.Size(80, 25) };
            txtEnd = new TextBox { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(350, 25), Text = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:00") };
            Controls.Add(lblEnd);
            Controls.Add(txtEnd);

            // Description
            y += 35;
            Label lblDesc = new Label { Text = "Mô tả:", Location = new System.Drawing.Point(12, y), Size = new System.Drawing.Size(80, 25) };
            txtDesc = new TextBox { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(350, 80), Multiline = true, ScrollBars = ScrollBars.Vertical };
            Controls.Add(lblDesc);
            Controls.Add(txtDesc);

            // Reminder
            y += 90;
            Label lblReminder = new Label { Text = "Nhắc nhở:", Location = new System.Drawing.Point(12, y), Size = new System.Drawing.Size(80, 25) };
            nudReminder = new NumericUpDown { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(80, 25), Value = 15, Minimum = 0, Maximum = 1440 };
            cmbReminderUnit = new ComboBox 
            { 
                Location = new System.Drawing.Point(190, y), 
                Size = new System.Drawing.Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbReminderUnit.Items.AddRange(new[] { "phút", "giờ", "ngày" });
            cmbReminderUnit.SelectedIndex = 0;
            Controls.Add(lblReminder);
            Controls.Add(nudReminder);
            Controls.Add(cmbReminderUnit);

            // Save & Cancel buttons
            y += 40;
            btnSave = new Button { Text = "Lưu", Location = new System.Drawing.Point(150, y), Size = new System.Drawing.Size(100, 35), DialogResult = DialogResult.OK };
            btnSave.Click += BtnSave_Click;
            btnCancel = new Button { Text = "Hủy", Location = new System.Drawing.Point(260, y), Size = new System.Drawing.Size(100, 35), DialogResult = DialogResult.Cancel };
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Tiêu đề không được để trống", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!DateTime.TryParse(txtStart.Text, out DateTime start))
            {
                MessageBox.Show("Sai định dạng thời gian bắt đầu (yyyy-MM-dd HH:mm)", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!DateTime.TryParse(txtEnd.Text, out DateTime end))
            {
                MessageBox.Show("Sai định dạng thời gian kết thúc (yyyy-MM-dd HH:mm)", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (start >= end)
            {
                MessageBox.Show("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            var appointment = new Appointment
            {
                Title = txtTitle.Text.Trim(),
                Location = txtLocation.Text.Trim(),
                StartTime = start,
                EndTime = end,
                Description = txtDesc.Text
            };

            // Calculate reminder time
            if (nudReminder.Value > 0)
            {
                int minutes = (int)nudReminder.Value;
                if (cmbReminderUnit.SelectedIndex == 1) minutes *= 60; // hours
                if (cmbReminderUnit.SelectedIndex == 2) minutes *= 1440; // days
                appointment.ReminderMinutes = minutes;
            }

            this.Tag = appointment;
        }

        public Appointment GetAppointment()
        {
            return (Appointment)this.Tag;
        }

        public int GetReminderMinutes()
        {
            return ((Appointment)this.Tag)?.ReminderMinutes ?? 0;
        }
    }
}