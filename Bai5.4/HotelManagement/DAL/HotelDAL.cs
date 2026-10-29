using System;
using System.Data;
using System.Data.SqlClient;
using HotelManagement.DTO;

namespace HotelManagement.DAL
{
    public class HotelDAL
    {
        public KhachHangDTO GetKhachHangByCCCD(string cccd)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM KhachHang WHERE CCCD = @CCCD";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CCCD", cccd);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new KhachHangDTO
                    {
                        MaKH = (int)reader["MaKH"],
                        HoTen = reader["HoTen"].ToString(),
                        CCCD = reader["CCCD"].ToString(),
                        SoDienThoai = reader["SoDienThoai"].ToString()
                    };
                }
            }
            return null;
        }

        public int CheckIn(string soPhong, string hoTen, string cccd, string sdt)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    string khQuery = @"IF NOT EXISTS (SELECT * FROM KhachHang WHERE CCCD = @CCCD)
                                        INSERT INTO KhachHang(HoTen, CCCD, SoDienThoai) VALUES(@HoTen, @CCCD, @SDT);
                                       SELECT MaKH FROM KhachHang WHERE CCCD = @CCCD;";
                    SqlCommand cmdKH = new SqlCommand(khQuery, conn, transaction);
                    cmdKH.Parameters.AddWithValue("@HoTen", hoTen);
                    cmdKH.Parameters.AddWithValue("@CCCD", cccd);
                    cmdKH.Parameters.AddWithValue("@SDT", sdt ?? (object)DBNull.Value);
                    int maKH = Convert.ToInt32(cmdKH.ExecuteScalar());

                    string pdpQuery = "INSERT INTO PhieuDatPhong(SoPhong, MaKH, NgayNhanPhong) VALUES(@SoPhong, @MaKH, GETDATE()); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmdPDP = new SqlCommand(pdpQuery, conn, transaction);
                    cmdPDP.Parameters.AddWithValue("@SoPhong", soPhong);
                    cmdPDP.Parameters.AddWithValue("@MaKH", maKH);
                    int maPhieu = Convert.ToInt32(cmdPDP.ExecuteScalar());

                    string phongQuery = "UPDATE Phong SET TrangThai = N'Đang ở' WHERE SoPhong = @SoPhong";
                    SqlCommand cmdPhong = new SqlCommand(phongQuery, conn, transaction);
                    cmdPhong.Parameters.AddWithValue("@SoPhong", soPhong);
                    cmdPhong.ExecuteNonQuery();

                    transaction.Commit();
                    return maPhieu;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public decimal CheckOut(string soPhong)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    string pdpQuery = "SELECT MaPhieu, NgayNhanPhong FROM PhieuDatPhong WHERE SoPhong = @SoPhong AND NgayTraPhong IS NULL";
                    SqlCommand cmdPDP = new SqlCommand(pdpQuery, conn, transaction);
                    cmdPDP.Parameters.AddWithValue("@SoPhong", soPhong);
                    SqlDataReader reader = cmdPDP.ExecuteReader();
                    
                    if (!reader.Read()) throw new Exception("Phòng chưa được nhận!");
                    int maPhieu = (int)reader["MaPhieu"];
                    DateTime ngayNhan = (DateTime)reader["NgayNhanPhong"];
                    reader.Close();

                    string pQuery = "SELECT GiaTheoGio FROM Phong WHERE SoPhong = @SoPhong";
                    SqlCommand cmdP = new SqlCommand(pQuery, conn, transaction);
                    cmdP.Parameters.AddWithValue("@SoPhong", soPhong);
                    decimal giaGio = Convert.ToDecimal(cmdP.ExecuteScalar());

                    string dvQuery = "SELECT ISNULL(SUM(s.SoLuong * d.DonGia), 0) FROM SuDungDichVu s JOIN DichVu d ON s.MaDV = d.MaDV WHERE s.MaPhieu = @MaPhieu";
                    SqlCommand cmdDV = new SqlCommand(dvQuery, conn, transaction);
                    cmdDV.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    decimal tienDichVu = Convert.ToDecimal(cmdDV.ExecuteScalar());

                    double soGio = Math.Ceiling((DateTime.Now - ngayNhan).TotalHours);
                    if (soGio < 1) soGio = 1;
                    decimal tongTien = ((decimal)soGio * giaGio) + tienDichVu;

                    string updatePDP = "UPDATE PhieuDatPhong SET NgayTraPhong = GETDATE(), TongTienThanhToan = @TongTien WHERE MaPhieu = @MaPhieu";
                    SqlCommand cmdUpPDP = new SqlCommand(updatePDP, conn, transaction);
                    cmdUpPDP.Parameters.AddWithValue("@TongTien", tongTien);
                    cmdUpPDP.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    cmdUpPDP.ExecuteNonQuery();

                    string updatePhong = "UPDATE Phong SET TrangThai = N'Đang dọn dẹp' WHERE SoPhong = @SoPhong";
                    SqlCommand cmdUpPhong = new SqlCommand(updatePhong, conn, transaction);
                    cmdUpPhong.Parameters.AddWithValue("@SoPhong", soPhong);
                    cmdUpPhong.ExecuteNonQuery();

                    transaction.Commit();
                    return tongTien;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
