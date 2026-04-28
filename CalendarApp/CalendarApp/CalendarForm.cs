using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalendarApp
{
    internal class CalendarForm : Form
    {
        private ListBox lstAppointments;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;
        private List<Appointment> appointments;

        public CalendarForm()
        {
            InitializeComponent();
            LoadAppointments();
        }

        private void InitializeComponent()
        {
            this.Text = "Calendar App - Quản lý cuộc hẹn";
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Header label
            Label lblTitle = new Label
            {
                Text = "Danh sách cuộc hẹn",
                Location = new System.Drawing.Point(12, 12),
                Size = new System.Drawing.Size(300, 30),
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold)
            };
            Controls.Add(lblTitle);

            // ListBox for appointments
            lstAppointments = new ListBox
            {
                Location = new System.Drawing.Point(12, 50),
                Size = new System.Drawing.Size(860, 450),
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

            btnRefresh = new Button
            {
                Text = "🔄 Làm mới",
                Location = new System.Drawing.Point(buttonX + 420, buttonY),
                Size = new System.Drawing.Size(120, 35),
                Font = new System.Drawing.Font("Arial", 10)
            };
            btnRefresh.Click += BtnRefresh_Click;
            Controls.Add(btnRefresh);

            this.Load += CalendarForm_Load;
        }

        private void CalendarForm_Load(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void LoadAppointments()
        {
            appointments = AppointmentService.LoadFromFile();
        }

        private void RefreshList()
        {
            LoadAppointments();
            lstAppointments.Items.Clear();
            foreach (var appt in appointments)
            {
                lstAppointments.Items.Add(appt.ToString());
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (AddAppointmentDialog dialog = new AddAppointmentDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Appointment newAppt = dialog.GetAppointment();

                    // Validate
                    if (!AppointmentService.ValidateTime(newAppt))
                    {
                        MessageBox.Show("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (AppointmentService.IsOverlap(newAppt, appointments))
                    {
                        MessageBox.Show("Cuộc hẹn trùng với cuộc hẹn khác", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    appointments.Add(newAppt);
                    AppointmentService.SaveToFile(appointments);
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
            Appointment selectedAppt = appointments[index];

            using (AddAppointmentDialog dialog = new AddAppointmentDialog())
            {
                dialog.Text = "Sửa cuộc hẹn";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Appointment editedAppt = dialog.GetAppointment();

                    if (!AppointmentService.ValidateTime(editedAppt))
                    {
                        MessageBox.Show("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    appointments[index] = editedAppt;
                    AppointmentService.SaveToFile(appointments);
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
                appointments.RemoveAt(index);
                AppointmentService.SaveToFile(appointments);
                RefreshList();
                MessageBox.Show("Xóa cuộc hẹn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshList();
            MessageBox.Show("Làm mới danh sách thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
