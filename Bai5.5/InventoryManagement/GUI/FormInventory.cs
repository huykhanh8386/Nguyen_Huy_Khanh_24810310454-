using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using InventoryManagement.BUS;
using InventoryManagement.DTO;

namespace InventoryManagement.GUI
{
    public class FormInventory : Form
    {
        private InventoryBUS bus = new InventoryBUS();
        private DataGridView dgvVatTu, dgvChiTiet;
        private TextBox txtTimKiem, txtMaVT, txtSoLuong, txtDonGia;
        private ComboBox cbLoaiPhieu;
        private Button btnTim, btnThemCT, btnLuuPhieu, btnThongKe;
        private Label lblTongTon;

        private List<ChiTietPhieuKhoDTO> listChiTiet = new List<ChiTietPhieuKhoDTO>();

        public FormInventory()
        {
            InitializeComponentManual();
            LoadDataVatTu("");
        }

        private void InitializeComponentManual()
        {
            this.Size = new Size(900, 600);
            this.Text = "Quản Lý Quy Trình Xuất - Nhập Kho";

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 50 };
            txtTimKiem = new TextBox { Location = new Point(10, 15), Width = 200 };
            btnTim = new Button { Text = "Tìm Vật Tư", Location = new Point(220, 13) };
            btnThongKe = new Button { Text = "Tổng Tồn Kho", Location = new Point(320, 13), Width = 100 };
            lblTongTon = new Label { Location = new Point(430, 18), AutoSize = true, Font = new Font("Arial", 10, FontStyle.Bold) };

            btnTim.Click += (s, e) => LoadDataVatTu(txtTimKiem.Text);
            btnThongKe.Click += (s, e) => lblTongTon.Text = "Tổng tồn: " + bus.LayTongTonKho().ToString("N0");

            pnlTop.Controls.AddRange(new Control[] { txtTimKiem, btnTim, btnThongKe, lblTongTon });

            dgvVatTu = new DataGridView { Dock = DockStyle.Left, Width = 400, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvVatTu.CellClick += (s, e) => {
                if (e.RowIndex >= 0)
                    txtMaVT.Text = dgvVatTu.Rows[e.RowIndex].Cells["MaVatTu"].Value.ToString();
            };

            Panel pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            cbLoaiPhieu = new ComboBox { Location = new Point(10, 20), Width = 120 };
            cbLoaiPhieu.Items.AddRange(new string[] { "NHAP", "XUAT" });
            cbLoaiPhieu.SelectedIndex = 0;

            txtMaVT = new TextBox { Location = new Point(140, 20), Width = 100 };
            txtSoLuong = new TextBox { Location = new Point(250, 20), Width = 80 };
            txtDonGia = new TextBox { Location = new Point(340, 20), Width = 80 };

            btnThemCT = new Button { Text = "Thêm vào phiếu", Location = new Point(10, 50), Width = 120 };
            btnLuuPhieu = new Button { Text = "LƯU PHIẾU KHO", Location = new Point(140, 50), Width = 150, BackColor = Color.LightSkyBlue };

            dgvChiTiet = new DataGridView { Location = new Point(10, 90), Size = new Size(440, 400), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            btnThemCT.Click += BtnThemCT_Click;
            btnLuuPhieu.Click += BtnLuuPhieu_Click;

            pnlRight.Controls.AddRange(new Control[] { cbLoaiPhieu, txtMaVT, txtSoLuong, txtDonGia, btnThemCT, btnLuuPhieu, dgvChiTiet });

            this.Controls.Add(pnlRight);
            this.Controls.Add(dgvVatTu);
            this.Controls.Add(pnlTop);
        }

        private void LoadDataVatTu(string kw)
        {
            dgvVatTu.DataSource = bus.TimKiemVatTu(kw);
        }

        private void BtnThemCT_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaVT.Text) || !decimal.TryParse(txtSoLuong.Text, out decimal sl) || !decimal.TryParse(txtDonGia.Text, out decimal dg))
            {
                MessageBox.Show("Vui lòng nhập đúng thông tin!");
                return;
            }

            listChiTiet.Add(new ChiTietPhieuKhoDTO { MaVatTu = txtMaVT.Text, SoLuong = sl, DonGia = dg });
            dgvChiTiet.DataSource = null;
            dgvChiTiet.DataSource = listChiTiet;
        }

        private void BtnLuuPhieu_Click(object sender, EventArgs e)
        {
            try
            {
                decimal tongTien = 0;
                foreach (var item in listChiTiet) tongTien += item.SoLuong * item.DonGia;

                PhieuKhoDTO phieu = new PhieuKhoDTO
                {
                    LoaiPhieu = cbLoaiPhieu.SelectedItem.ToString(),
                    TongTien = tongTien,
                    ChiTiets = listChiTiet
                };

                bus.LapPhieuKho(phieu);
                MessageBox.Show("Lưu phiếu kho thành công!");
                
                listChiTiet.Clear();
                dgvChiTiet.DataSource = null;
                LoadDataVatTu("");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi giao dịch (Rollback): " + ex.Message);
            }
        }
    }
}
