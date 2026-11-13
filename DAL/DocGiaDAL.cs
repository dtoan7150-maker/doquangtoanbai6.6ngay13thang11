using System.Data;
using System.Data.SqlClient;
using LibraryManagement.DTO;

namespace LibraryManagement.DAL
{
    public class DocGiaDAL : DBConnect
    {
        public DataTable GetAllDocGia()
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "SELECT MaDocGia, HoTen, Email, NgayHetHanThe FROM DocGia";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool Insert(DocGiaDTO dg)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "INSERT INTO DocGia (HoTen, Email, NgayHetHanThe) VALUES (@HoTen, @Email, @NgayHetHanThe)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@HoTen", dg.HoTen);
                cmd.Parameters.AddWithValue("@Email", dg.Email);
                cmd.Parameters.AddWithValue("@NgayHetHanThe", dg.NgayHetHanThe);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(DocGiaDTO dg)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "UPDATE DocGia SET HoTen = @HoTen, Email = @Email, NgayHetHanThe = @NgayHetHanThe WHERE MaDocGia = @MaDocGia";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaDocGia", dg.MaDocGia);
                cmd.Parameters.AddWithValue("@HoTen", dg.HoTen);
                cmd.Parameters.AddWithValue("@Email", dg.Email);
                cmd.Parameters.AddWithValue("@NgayHetHanThe", dg.NgayHetHanThe);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int maDocGia)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "DELETE FROM DocGia WHERE MaDocGia = @MaDocGia";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaDocGia", maDocGia);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Kiểm tra hạn thẻ độc giả: NgayHetHanThe >= GETDATE()
        public bool KiemTraTheConHan(int maDocGia)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "SELECT COUNT(*) FROM DocGia WHERE MaDocGia = @MaDocGia AND NgayHetHanThe >= GETDATE()";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaDocGia", maDocGia);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
    }
}