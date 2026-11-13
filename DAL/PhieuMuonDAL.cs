using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LibraryManagement.DTO;

namespace LibraryManagement.DAL
{
    public class PhieuMuonDAL : DBConnect
    {
        // 1. Lập phiếu mượn mới (Dùng Transaction)
        public bool LapPhieuMuon(PhieuMuonDTO pm, List<ChiTietMuonDTO> chiTietList)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    string sqlPM = @"INSERT INTO PhieuMuon (MaDocGia, NgayMuon, HanTra, TrangThai) 
                                     OUTPUT INSERTED.MaPhieu 
                                     VALUES (@MaDocGia, @NgayMuon, @HanTra, @TrangThai)";
                    SqlCommand cmdPM = new SqlCommand(sqlPM, conn, trans);
                    cmdPM.Parameters.AddWithValue("@MaDocGia", pm.MaDocGia);
                    cmdPM.Parameters.AddWithValue("@NgayMuon", pm.NgayMuon);
                    cmdPM.Parameters.AddWithValue("@HanTra", pm.HanTra);
                    cmdPM.Parameters.AddWithValue("@TrangThai", "Đang mượn");

                    int maPhieuMoi = (int)cmdPM.ExecuteScalar();

                    foreach (var ct in chiTietList)
                    {
                        string sqlCT = "INSERT INTO ChiTietMuon (MaPhieu, MaSach, SoLuong) VALUES (@MaPhieu, @MaSach, @SoLuong)";
                        SqlCommand cmdCT = new SqlCommand(sqlCT, conn, trans);
                        cmdCT.Parameters.AddWithValue("@MaPhieu", maPhieuMoi);
                        cmdCT.Parameters.AddWithValue("@MaSach", ct.MaSach);
                        cmdCT.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                        cmdCT.ExecuteNonQuery();

                        string sqlTruKho = "UPDATE DauSach SET SoLuongCon = SoLuongCon - @SoLuong WHERE MaSach = @MaSach";
                        SqlCommand cmdKho = new SqlCommand(sqlTruKho, conn, trans);
                        cmdKho.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                        cmdKho.Parameters.AddWithValue("@MaSach", ct.MaSach);
                        cmdKho.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        // 2. Lọc danh sách phiếu mượn theo Độc giả và Trạng thái
        public DataTable LocPhieuMuon(int? maDocGia, string trangThai)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = @"SELECT pm.MaPhieu, pm.MaDocGia, dg.HoTen, pm.NgayMuon, pm.HanTra, pm.TrangThai
                                 FROM PhieuMuon pm
                                 INNER JOIN DocGia dg ON pm.MaDocGia = dg.MaDocGia
                                 WHERE (@MaDocGia IS NULL OR pm.MaDocGia = @MaDocGia)
                                   AND (@TrangThai = N'Tất cả' OR pm.TrangThai = @TrangThai)
                                 ORDER BY pm.NgayMuon DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaDocGia", (object)maDocGia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", string.IsNullOrEmpty(trangThai) ? "Tất cả" : trangThai);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // 3. Trả sách: Parameterized SQL UPDATE đổi trạng thái & cộng lại SoLuongCon
        public bool TraSach(int maPhieu)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    string sqlUpdatePM = @"UPDATE PhieuMuon 
                                           SET TrangThai = N'Đã trả' 
                                           WHERE MaPhieu = @MaPhieu AND TrangThai != N'Đã trả'";
                    SqlCommand cmdPM = new SqlCommand(sqlUpdatePM, conn, trans);
                    cmdPM.Parameters.AddWithValue("@MaPhieu", maPhieu);

                    int rowsAffected = cmdPM.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        trans.Rollback();
                        return false;
                    }

                    string sqlCongKho = @"UPDATE ds
                                          SET ds.SoLuongCon = ds.SoLuongCon + ctm.SoLuong
                                          FROM DauSach ds
                                          INNER JOIN ChiTietMuon ctm ON ds.MaSach = ctm.MaSach
                                          WHERE ctm.MaPhieu = @MaPhieu";
                    SqlCommand cmdKho = new SqlCommand(sqlCongKho, conn, trans);
                    cmdKho.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    cmdKho.ExecuteNonQuery();

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        // 4. Cảnh báo sách quá hạn dùng SqlDataReader
        public List<PhieuMuonDTO> GetDanhSachQuaHan()
        {
            List<PhieuMuonDTO> listQuaHan = new List<PhieuMuonDTO>();

            using (SqlConnection conn = GetConnection())
            {
                string query = @"SELECT pm.MaPhieu, pm.MaDocGia, dg.HoTen, pm.NgayMuon, pm.HanTra, pm.TrangThai
                                 FROM PhieuMuon pm
                                 INNER JOIN DocGia dg ON pm.MaDocGia = dg.MaDocGia
                                 WHERE pm.HanTra < GETDATE() AND pm.TrangThai != N'Đã trả'";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listQuaHan.Add(new PhieuMuonDTO
                        {
                            MaPhieu = reader.GetInt32(0),
                            MaDocGia = reader.GetInt32(1),
                            HoTenDocGia = reader.GetString(2),
                            NgayMuon = reader.GetDateTime(3),
                            HanTra = reader.GetDateTime(4),
                            TrangThai = reader.GetString(5)
                        });
                    }
                }
            }
            return listQuaHan;
        }

        // 5a. Thống kê tổng lượt mượn theo từng thể loại (GROUP BY)
        public DataTable ThongKeTheoTheLoai_GroupBy()
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = @"SELECT ds.TheLoai AS [Thể Loại],
                                        COUNT(DISTINCT ctm.MaPhieu) AS [Số Lượt Mượn],
                                        ISNULL(SUM(ctm.SoLuong), 0) AS [Tổng Số Sách Mượn]
                                 FROM DauSach ds
                                 LEFT JOIN ChiTietMuon ctm ON ds.MaSach = ctm.MaSach
                                 GROUP BY ds.TheLoai";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // 5b. Thống kê tổng số lượt mượn của 1 thể loại cụ thể (ExecuteScalar)
        public int DemLuotMuonTheoTheLoai_Scalar(string theLoai)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = @"SELECT COUNT(DISTINCT ctm.MaPhieu)
                                 FROM ChiTietMuon ctm
                                 INNER JOIN DauSach ds ON ctm.MaSach = ds.MaSach
                                 WHERE ds.TheLoai = @TheLoai";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TheLoai", theLoai);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
    }
}