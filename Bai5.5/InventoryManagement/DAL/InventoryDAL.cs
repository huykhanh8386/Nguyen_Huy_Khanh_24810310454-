using System;
using System.Data;
using System.Data.SqlClient;
using InventoryManagement.DTO;

namespace InventoryManagement.DAL
{
    public class InventoryDAL
    {
        public DataTable TimKiemVatTu(string keyword)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM VatTu WHERE MaVatTu LIKE @Key OR TenVatTu LIKE @Key";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Key", "%" + keyword + "%");
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }

        public void LuuPhieuKhoTransaction(PhieuKhoDTO phieu)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    string insertPhieu = @"INSERT INTO PhieuKho(LoaiPhieu, NgayLap, MaNCC, TongTien) 
                                           VALUES(@LoaiPhieu, GETDATE(), @MaNCC, @TongTien); 
                                           SELECT SCOPE_IDENTITY();";
                    SqlCommand cmdPhieu = new SqlCommand(insertPhieu, conn, trans);
                    cmdPhieu.Parameters.AddWithValue("@LoaiPhieu", phieu.LoaiPhieu);
                    cmdPhieu.Parameters.AddWithValue("@MaNCC", (object)phieu.MaNCC ?? DBNull.Value);
                    cmdPhieu.Parameters.AddWithValue("@TongTien", phieu.TongTien);
                    int maPhieu = Convert.ToInt32(cmdPhieu.ExecuteScalar());

                    foreach (var ct in phieu.ChiTiets)
                    {
                        string insertCT = "INSERT INTO ChiTietPhieuKho(MaPhieu, MaVatTu, SoLuong, DonGia) VALUES(@MaPhieu, @MaVatTu, @SoLuong, @DonGia)";
                        SqlCommand cmdCT = new SqlCommand(insertCT, conn, trans);
                        cmdCT.Parameters.AddWithValue("@MaPhieu", maPhieu);
                        cmdCT.Parameters.AddWithValue("@MaVatTu", ct.MaVatTu);
                        cmdCT.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                        cmdCT.Parameters.AddWithValue("@DonGia", ct.DonGia);
                        cmdCT.ExecuteNonQuery();

                        if (phieu.LoaiPhieu == "NHAP")
                        {
                            string updateKho = "UPDATE VatTu SET SoLuongTon = SoLuongTon + @SoLuong WHERE MaVatTu = @MaVatTu";
                            SqlCommand cmdKho = new SqlCommand(updateKho, conn, trans);
                            cmdKho.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                            cmdKho.Parameters.AddWithValue("@MaVatTu", ct.MaVatTu);
                            cmdKho.ExecuteNonQuery();
                        }
                        else if (phieu.LoaiPhieu == "XUAT")
                        {
                            string checkTon = "SELECT SoLuongTon FROM VatTu WHERE MaVatTu = @MaVatTu";
                            SqlCommand cmdTon = new SqlCommand(checkTon, conn, trans);
                            cmdTon.Parameters.AddWithValue("@MaVatTu", ct.MaVatTu);
                            decimal tonHienTai = Convert.ToDecimal(cmdTon.ExecuteScalar());

                            if (tonHienTai < ct.SoLuong)
                            {
                                throw new Exception($"Không đủ hàng trong kho cho vật tư {ct.MaVatTu}! Tồn hiện tại: {tonHienTai}");
                            }

                            string updateKho = "UPDATE VatTu SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaVatTu = @MaVatTu";
                            SqlCommand cmdKho = new SqlCommand(updateKho, conn, trans);
                            cmdKho.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                            cmdKho.Parameters.AddWithValue("@MaVatTu", ct.MaVatTu);
                            cmdKho.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public decimal GetTongGiaTriTonKho()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "SELECT ISNULL(SUM(SoLuongTon), 0) FROM VatTu";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }
    }
}
