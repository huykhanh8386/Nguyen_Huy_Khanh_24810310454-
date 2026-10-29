using System;

namespace HotelManagement.DTO
{
    public class PhongDTO
    {
        public string SoPhong { get; set; }
        public string LoaiPhong { get; set; }
        public decimal GiaTheoGio { get; set; }
        public string TrangThai { get; set; }
    }

    public class KhachHangDTO
    {
        public int MaKH { get; set; }
        public string HoTen { get; set; }
        public string CCCD { get; set; }
        public string SoDienThoai { get; set; }
    }

    public class PhieuDatPhongDTO
    {
        public int MaPhieu { get; set; }
        public string SoPhong { get; set; }
        public int MaKH { get; set; }
        public DateTime NgayNhanPhong { get; set; }
        public DateTime? NgayTraPhong { get; set; }
        public decimal TongTienThanhToan { get; set; }
    }
}
