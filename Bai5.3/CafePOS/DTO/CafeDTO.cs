using System;

namespace CafePOS.DTO
{
    public class BanAnDTO
    {
        public int MaBan { get; set; }
        public string TenBan { get; set; }
        public string TrangThai { get; set; }
    }

    public class DanhMucDTO
    {
        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
    }

    public class MonAnDTO
    {
        public int MaMon { get; set; }
        public string TenMon { get; set; }
        public int MaDanhMuc { get; set; }
        public decimal DonGia { get; set; }
    }

    public class HoaDonDTO
    {
        public int MaHoaDon { get; set; }
        public int MaBan { get; set; }
        public DateTime GioVao { get; set; }
        public DateTime? GioRa { get; set; }
        public decimal TongTien { get; set; }
        public bool TrangThai { get; set; }
    }
}
