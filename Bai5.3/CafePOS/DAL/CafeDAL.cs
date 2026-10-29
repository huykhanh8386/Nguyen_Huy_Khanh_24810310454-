using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CafePOS.DTO;

namespace CafePOS.DAL
{
    public class CafeDAL
    {
        public List<BanAnDTO> GetDanhSachBan()
        {
            List<BanAnDTO> list = new List<BanAnDTO>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM BanAn";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new BanAnDTO
                    {
                        MaBan = (int)reader["MaBan"],
                        TenBan = reader["TenBan"].ToString(),
                        TrangThai = reader["TrangThai"].ToString()
                    });
                }
            }
            return list;
        }

        public DataTable GetChiTietHoaDonByBan(int maBan)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT m.TenMon, c.SoLuong, c.DonGia, (c.SoLuong * c.DonGia) AS ThanhTien 
                                 FROM HoaDon h 
                                 JOIN ChiTietHoaDon c ON h.MaHoaDon = c.MaHoaDon 
                                 JOIN MonAn m ON c.MaMon = m.MaMon 
                                 WHERE h.MaBan = @MaBan AND h.TrangThai = 0";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@MaBan", maBan);
                adapter.Fill(dt);
            }
            return dt;
        }

        public void ThemMon(int maBan, int maMon, int soLuong, decimal donGia)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    string checkHD = "SELECT MaHoaDon FROM HoaDon WHERE MaBan = @MaBan AND TrangThai = 0";
                    SqlCommand cmdCheck = new SqlCommand(checkHD, conn, transaction);
                    cmdCheck.Parameters.AddWithValue("@MaBan", maBan);
                    object result = cmdCheck.ExecuteScalar();

                    int maHD;
                    if (result == null)
                    {
                        string insertHD = "INSERT INTO HoaDon(MaBan, GioVao, TrangThai) VALUES(@MaBan, GETDATE(), 0); SELECT SCOPE_IDENTITY();";
                        SqlCommand cmdHD = new SqlCommand(insertHD, conn, transaction);
                        cmdHD.Parameters.AddWithValue("@MaBan", maBan);
                        maHD = Convert.ToInt32(cmdHD.ExecuteScalar());

                        string updateBan = "UPDATE BanAn SET TrangThai = N'Có khách' WHERE MaBan = @MaBan";
                        SqlCommand cmdBan = new SqlCommand(updateBan, conn, transaction);
                        cmdBan.Parameters.AddWithValue("@MaBan", maBan);
                        cmdBan.ExecuteNonQuery();
                    }
                    else
                    {
                        maHD = Convert.ToInt32(result);
                    }

                    string insertCT = @"IF EXISTS (SELECT * FROM ChiTietHoaDon WHERE MaHoaDon = @MaHD AND MaMon = @MaMon)
                                            UPDATE ChiTietHoaDon SET SoLuong = SoLuong + @SoLuong WHERE MaHoaDon = @MaHD AND MaMon = @MaMon
                                        ELSE
                                            INSERT INTO ChiTietHoaDon(MaHoaDon, MaMon, SoLuong, DonGia) VALUES(@MaHD, @MaMon, @SoLuong, @DonGia)";
                    SqlCommand cmdCT = new SqlCommand(insertCT, conn, transaction);
                    cmdCT.Parameters.AddWithValue("@MaHD", maHD);
                    cmdCT.Parameters.AddWithValue("@MaMon", maMon);
                    cmdCT.Parameters.AddWithValue("@SoLuong", soLuong);
                    cmdCT.Parameters.AddWithValue("@DonGia", donGia);
                    cmdCT.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void ThanhToan(int maBan)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    string getHD = "SELECT MaHoaDon FROM HoaDon WHERE MaBan = @MaBan AND TrangThai = 0";
                    SqlCommand cmdGet = new SqlCommand(getHD, conn, transaction);
                    cmdGet.Parameters.AddWithValue("@MaBan", maBan);
                    object result = cmdGet.ExecuteScalar();

                    if (result != null)
                    {
                        int maHD = Convert.ToInt32(result);

                        string sumQuery = "SELECT SUM(SoLuong * DonGia) FROM ChiTietHoaDon WHERE MaHoaDon = @MaHD";
                        SqlCommand cmdSum = new SqlCommand(sumQuery, conn, transaction);
                        cmdSum.Parameters.AddWithValue("@MaHD", maHD);
                        decimal tongTien = Convert.ToDecimal(cmdSum.ExecuteScalar());

                        string updateHD = "UPDATE HoaDon SET GioRa = GETDATE(), TongTien = @TongTien, TrangThai = 1 WHERE MaHoaDon = @MaHD";
                        SqlCommand cmdHD = new SqlCommand(updateHD, conn, transaction);
                        cmdHD.Parameters.AddWithValue("@TongTien", tongTien);
                        cmdHD.Parameters.AddWithValue("@MaHD", maHD);
                        cmdHD.ExecuteNonQuery();

                        string updateBan = "UPDATE BanAn SET TrangThai = N'Trống' WHERE MaBan = @MaBan";
                        SqlCommand cmdBan = new SqlCommand(updateBan, conn, transaction);
                        cmdBan.Parameters.AddWithValue("@MaBan", maBan);
                        cmdBan.ExecuteNonQuery();

                        transaction.Commit();
                    }
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
