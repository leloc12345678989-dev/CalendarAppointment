using System;
using System.Windows.Forms;

namespace CalendarApp
{
    public partial class AddAppointmentDialog : Form
    {
        private TextBox txtTitle, txtStart, txtEnd;
        private TextBox txtDesc;
        private Button btnSave, btnCancel;

        public AddAppointmentDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Thêm cuộc hẹn";
            this.Size = new System.Drawing.Size(450, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 12;
            Label lblTitle = new Label { Text = "Tiêu đề:", Location = new System.Drawing.Point(12, y), Size = new System.Drawing.Size(80, 25) };
            txtTitle = new TextBox { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(300, 25) };

            y += 35;
            Label lblStart = new Label { Text = "Bắt đầu (yyyy-MM-dd HH:mm):", Location = new System.Drawing.Point(12, y), Size = new System.Drawing.Size(180, 25) };
            txtStart = new TextBox { Location = new System.Drawing.Point(200, y), Size = new System.Drawing.Size(200, 25), Text = DateTime.Now.ToString("yyyy-MM-dd HH:00") };

            y += 35;
            Label lblEnd = new Label { Text = "Kết thúc (yyyy-MM-dd HH:mm):", Location = new System.Drawing.Point(12, y), Size = new System.Drawing.Size(180, 25) };
            txtEnd = new TextBox { Location = new System.Drawing.Point(200, y), Size = new System.Drawing.Size(200, 25), Text = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:00") };

            y += 35;
            Label lblDesc = new Label { Text = "Mô tả:", Location = new System.Drawing.Point(12, y), Size = new System.Drawing.Size(80, 25) };
            txtDesc = new TextBox { Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(300, 80), Multiline = true, ScrollBars = ScrollBars.Vertical };

            y += 90;
            btnSave = new Button { Text = "Lưu", Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(100, 35), DialogResult = DialogResult.OK };
            btnSave.Click += BtnSave_Click;
            btnCancel = new Button { Text = "Hủy", Location = new System.Drawing.Point(220, y), Size = new System.Drawing.Size(100, 35), DialogResult = DialogResult.Cancel };

            Controls.Add(lblTitle);
            Controls.Add(txtTitle);
            Controls.Add(lblStart);
            Controls.Add(txtStart);
            Controls.Add(lblEnd);
            Controls.Add(txtEnd);
            Controls.Add(lblDesc);
            Controls.Add(txtDesc);
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
                MessageBox.Show("Sai định dạng thời gian bắt đầu", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!DateTime.TryParse(txtEnd.Text, out DateTime end))
            {
                MessageBox.Show("Sai định dạng thời gian kết thúc", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            this.Tag = new Appointment
            {
                Title = txtTitle.Text.Trim(),
                StartTime = start,
                EndTime = end,
                Description = txtDesc.Text
            };
        }

        public Appointment GetAppointment()
        {
            return (Appointment)this.Tag;
        }
    }
}