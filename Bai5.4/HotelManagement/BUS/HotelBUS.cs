using System;
using HotelManagement.DAL;
using HotelManagement.DTO;

namespace HotelManagement.BUS
{
    public class HotelBUS
    {
        private HotelDAL dal = new HotelDAL();

        public KhachHangDTO TimKhachHang(string cccd) => dal.GetKhachHangByCCCD(cccd);

        public void CheckInPhong(string soPhong, string hoTen, string cccd, string sdt)
        {
            if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(cccd))
                throw new Exception("Họ tên và CCCD không được trống!");
            dal.CheckIn(soPhong, hoTen, cccd, sdt);
        }

        public decimal CheckOutPhong(string soPhong) => dal.CheckOut(soPhong);
    }
}
