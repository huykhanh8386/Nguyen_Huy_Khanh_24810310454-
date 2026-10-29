using System;
using System.Collections.Generic;
using System.Data;
using CafePOS.DAL;
using CafePOS.DTO;

namespace CafePOS.BUS
{
    public class CafeBUS
    {
        private CafeDAL dal = new CafeDAL();

        public List<BanAnDTO> GetDanhSachBan() => dal.GetDanhSachBan();
        public DataTable GetChiTietHoaDonByBan(int maBan) => dal.GetChiTietHoaDonByBan(maBan);

        public void OrderMon(int maBan, int maMon, int soLuong, decimal donGia)
        {
            if (soLuong <= 0) throw new Exception("Số lượng phải lớn hơn 0!");
            if (donGia < 0) throw new Exception("Đơn giá không được nhỏ hơn 0!");
            dal.ThemMon(maBan, maMon, soLuong, donGia);
        }

        public void ThanhToanBan(int maBan) => dal.ThanhToan(maBan);
    }
}
