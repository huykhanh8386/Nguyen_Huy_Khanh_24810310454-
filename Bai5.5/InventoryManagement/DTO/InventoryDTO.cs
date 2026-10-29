using System;
using System.Collections.Generic;

namespace InventoryManagement.DTO
{
    public class VatTuDTO
    {
        public string MaVatTu { get; set; }
        public string TenVatTu { get; set; }
        public string DVT { get; set; }
        public decimal SoLuongTon { get; set; }
    }

    public class ChiTietPhieuKhoDTO
    {
        public string MaVatTu { get; set; }
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }

    public class PhieuKhoDTO
    {
        public int MaPhieu { get; set; }
        public string LoaiPhieu { get; set; } // "NHAP" hoặc "XUAT"
        public int? MaNCC { get; set; }
        public decimal TongTien { get; set; }
        public List<ChiTietPhieuKhoDTO> ChiTiets { get; set; } = new List<ChiTietPhieuKhoDTO>();
    }
}
