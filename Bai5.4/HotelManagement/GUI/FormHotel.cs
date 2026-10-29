using System;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.BUS;

namespace HotelManagement.GUI
{
    public class FormHotel : Form
    {
        private HotelBUS bus = new HotelBUS();
        private TextBox txtSoPhong, txtHoTen, txtCCCD, txtSDT;
        private Button btnCheckIn, btnCheckOut;

        public FormHotel()
        {
            InitializeComponentManual();
        }

        private void InitializeComponentManual()
        {
            this.Size = new Size(400, 300);
            this.Text = "Quản Lý Khách Sạn - Check-in / Check-out";

            Label lblPhong = new Label { Text = "Số phòng:", Location = new Point(20, 20), AutoSize = true };
            txtSoPhong = new TextBox { Location = new Point(120, 18), Width = 200 };

            Label lblCCCD = new Label { Text = "CCCD:", Location = new Point(20, 60), AutoSize = true };
            txtCCCD = new TextBox { Location = new Point(120, 58), Width = 200 };

            Label lblTen = new Label { Text = "Họ tên:", Location = new Point(20, 100), AutoSize = true };
            txtHoTen = new TextBox { Location = new Point(120, 98), Width = 200 };

            Label lblSDT = new Label { Text = "SĐT:", Location = new Point(20, 140), AutoSize = true };
            txtSDT = new TextBox { Location = new Point(120, 138), Width = 200 };

            btnCheckIn = new Button { Text = "Check-In", Location = new Point(50, 190), Width = 100, BackColor = Color.LightGreen };
            btnCheckOut = new Button { Text = "Check-Out", Location = new Point(200, 190), Width = 100, BackColor = Color.LightCoral };

            btnCheckIn.Click += (s, e) => {
                try {
                    bus.CheckInPhong(txtSoPhong.Text, txtHoTen.Text, txtCCCD.Text, txtSDT.Text);
                    MessageBox.Show("Check-in thành công!");
                } catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            };

            btnCheckOut.Click += (s, e) => {
                try {
                    decimal tongTien = bus.CheckOutPhong(txtSoPhong.Text);
                    MessageBox.Show($"Check-out thành công! Tổng tiền: {tongTien:N0} VNĐ");
                } catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            };

            this.Controls.AddRange(new Control[] { lblPhong, txtSoPhong, lblCCCD, txtCCCD, lblTen, txtHoTen, lblSDT, txtSDT, btnCheckIn, btnCheckOut });
        }
    }
}
