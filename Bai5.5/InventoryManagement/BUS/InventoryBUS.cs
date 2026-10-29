using System;
using System.Data;
using InventoryManagement.DAL;
using InventoryManagement.DTO;

namespace InventoryManagement.BUS
{
    public class InventoryBUS
    {
        private InventoryDAL dal = new InventoryDAL();

        public DataTable TimKiemVatTu(string keyword) => dal.TimKiemVatTu(keyword ?? "");

        public void LapPhieuKho(PhieuKhoDTO phieu)
        {
            if (phieu.ChiTiets == null || phieu.ChiTiets.Count == 0)
                throw new Exception("Phiếu kho phải có ít nhất 1 vật tư!");

            foreach (var item in phieu.ChiTiets)
            {
                if (item.SoLuong <= 0) throw new Exception("Số lượng xuất/nhập phải lớn hơn 0!");
                if (item.DonGia < 0) throw new Exception("Đơn giá không được âm!");
            }

            dal.LuuPhieuKhoTransaction(phieu);
        }

        public decimal LayTongTonKho() => dal.GetTongGiaTriTonKho();
    }
}
