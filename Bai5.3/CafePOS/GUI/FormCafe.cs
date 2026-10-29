using System;
using System.Drawing;
using System.Windows.Forms;
using CafePOS.BUS;
using CafePOS.DTO;

namespace CafePOS.GUI
{
    public class FormCafe : Form
    {
        private FlowLayoutPanel flpTable;
        private DataGridView dgvOrder;
        private Button btnThanhToan;
        private CafeBUS bus = new CafeBUS();
        private int currentMaBan = -1;

        public FormCafe()
        {
            InitializeComponentManual();
            LoadBanAn();
        }

        private void InitializeComponentManual()
        {
            this.Size = new Size(800, 500);
            this.Text = "Hệ thống Quản lý Bàn Ăn - Cafe POS";

            flpTable = new FlowLayoutPanel { Width = 350, Dock = DockStyle.Left, AutoScroll = true };
            dgvOrder = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            btnThanhToan = new Button { Text = "Thanh Toán", Dock = DockStyle.Bottom, Height = 40, BackColor = Color.Orange };

            btnThanhToan.Click += BtnThanhToan_Click;

            this.Controls.Add(dgvOrder);
            this.Controls.Add(btnThanhToan);
            this.Controls.Add(flpTable);
        }

        private void LoadBanAn()
        {
            flpTable.Controls.Clear();
            var dsBan = bus.GetDanhSachBan();
            foreach (var ban in dsBan)
            {
                Button btn = new Button
                {
                    Width = 90,
                    Height = 90,
                    Text = $"{ban.TenBan}\n({ban.TrangThai})",
                    Tag = ban,
                    BackColor = ban.TrangThai == "Trống" ? Color.LightGreen : Color.LightPink
                };
                btn.Click += BtnBan_Click;
                flpTable.Controls.Add(btn);
            }
        }

        private void BtnBan_Click(object sender, EventArgs e)
        {
            var ban = (BanAnDTO)((Button)sender).Tag;
            currentMaBan = ban.MaBan;
            dgvOrder.DataSource = bus.GetChiTietHoaDonByBan(currentMaBan);
        }

        private void BtnThanhToan_Click(object sender, EventArgs e)
        {
            if (currentMaBan == -1)
            {
                MessageBox.Show("Vui lòng chọn bàn cần thanh toán!");
                return;
            }

            try
            {
                bus.ThanhToanBan(currentMaBan);
                MessageBox.Show("Thanh toán thành công!");
                LoadBanAn();
                dgvOrder.DataSource = null;
                currentMaBan = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thanh toán: " + ex.Message);
            }
        }
    }
}
