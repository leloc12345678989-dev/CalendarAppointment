using System;
using System.Windows.Forms;

namespace CalendarApp
{
    public class GroupMeetingDialog : Form
    {
        private TextBox txtParticipant;
        private ListBox lstParticipants;
        private Label lblCount;
        private Button btnAdd, btnRemove, btnJoin, btnCancel;
        private GroupMeeting groupMeeting;

        public GroupMeetingDialog(GroupMeeting meeting = null)
        {
            groupMeeting = meeting ?? new GroupMeeting();
            InitializeComponent();
            RefreshParticipantList();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý cuộc họp nhóm";
            this.Size = new System.Drawing.Size(500, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Title
            Label lblTitle = new Label
            {
                Text = "Thành viên tham gia",
                Location = new System.Drawing.Point(12, 12),
                Size = new System.Drawing.Size(400, 25),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };
            Controls.Add(lblTitle);

            // Participant input
            int y = 45;
            Label lblParticipant = new Label { Text = "Tên thành viên:", Location = new System.Drawing.Point(12, y), Size = new System.Drawing.Size(100, 25) };
            txtParticipant = new TextBox { Location = new System.Drawing.Point(120, y), Size = new System.Drawing.Size(250, 25) };
            Controls.Add(lblParticipant);
            Controls.Add(txtParticipant);

            // Add button
            btnAdd = new Button
            {
                Text = "➕ Thêm",
                Location = new System.Drawing.Point(380, y),
                Size = new System.Drawing.Size(100, 25)
            };
            btnAdd.Click += BtnAdd_Click;
            Controls.Add(btnAdd);

            // Participants list
            y += 35;
            Label lblList = new Label { Text = "Danh sách thành viên:", Location = new System.Drawing.Point(12, y), Size = new System.Drawing.Size(150, 25) };
            Controls.Add(lblList);

            y += 30;
            lstParticipants = new ListBox
            {
                Location = new System.Drawing.Point(12, y),
                Size = new System.Drawing.Size(468, 200),
                SelectionMode = SelectionMode.One
            };
            Controls.Add(lstParticipants);

            // Count label
            y += 210;
            lblCount = new Label
            {
                Text = $"Số thành viên: {groupMeeting.GetParticipantCount()}/{groupMeeting.MaxParticipants}",
                Location = new System.Drawing.Point(12, y),
                Size = new System.Drawing.Size(300, 25),
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };
            Controls.Add(lblCount);

            // Remove button
            btnRemove = new Button
            {
                Text = "🗑️ Xóa",
                Location = new System.Drawing.Point(320, y),
                Size = new System.Drawing.Size(100, 25)
            };
            btnRemove.Click += BtnRemove_Click;
            Controls.Add(btnRemove);

            // Buttons
            y += 35;
            btnJoin = new Button
            {
                Text = "✓ Tham gia",
                Location = new System.Drawing.Point(200, y),
                Size = new System.Drawing.Size(100, 35),
                DialogResult = DialogResult.OK
            };
            btnCancel = new Button
            {
                Text = "Đóng",
                Location = new System.Drawing.Point(310, y),
                Size = new System.Drawing.Size(100, 35),
                DialogResult = DialogResult.Cancel
            };
            Controls.Add(btnJoin);
            Controls.Add(btnCancel);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtParticipant.Text))
            {
                MessageBox.Show("Vui lòng nhập tên thành viên", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!groupMeeting.AddParticipant(txtParticipant.Text.Trim()))
            {
                MessageBox.Show("Không thể thêm thành viên (đã đạt giới hạn hoặc trùng)", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RefreshParticipantList();
            txtParticipant.Clear();
            txtParticipant.Focus();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (lstParticipants.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn thành viên", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedParticipant = lstParticipants.Items[lstParticipants.SelectedIndex].ToString();
            groupMeeting.RemoveParticipant(selectedParticipant);
            RefreshParticipantList();
        }

        private void RefreshParticipantList()
        {
            lstParticipants.Items.Clear();
            foreach (var participant in groupMeeting.Participants)
            {
                lstParticipants.Items.Add(participant);
            }
            lblCount.Text = $"Số thành viên: {groupMeeting.GetParticipantCount()}/{groupMeeting.MaxParticipants}";
        }

        public GroupMeeting GetGroupMeeting()
        {
            return groupMeeting;
        }
    }
}
