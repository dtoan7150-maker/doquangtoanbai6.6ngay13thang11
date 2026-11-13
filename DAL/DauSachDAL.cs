using System.Data;
using System.Data.SqlClient;
using LibraryManagement.DTO;

namespace LibraryManagement.DAL
{
    public class DauSachDAL : DBConnect
    {
        public DataTable GetAllDauSach()
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "SELECT MaSach, TenSach, TheLoai, SoLuongCon FROM DauSach";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool Insert(DauSachDTO ds)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "INSERT INTO DauSach (TenSach, TheLoai, SoLuongCon) VALUES (@TenSach, @TheLoai, @SoLuongCon)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenSach", ds.TenSach);
                cmd.Parameters.AddWithValue("@TheLoai", ds.TheLoai);
                cmd.Parameters.AddWithValue("@SoLuongCon", ds.SoLuongCon);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(DauSachDTO ds)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "UPDATE DauSach SET TenSach = @TenSach, TheLoai = @TheLoai, SoLuongCon = @SoLuongCon WHERE MaSach = @MaSach";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaSach", ds.MaSach);
                cmd.Parameters.AddWithValue("@TenSach", ds.TenSach);
                cmd.Parameters.AddWithValue("@TheLoai", ds.TheLoai);
                cmd.Parameters.AddWithValue("@SoLuongCon", ds.SoLuongCon);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int maSach)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "DELETE FROM DauSach WHERE MaSach = @MaSach";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaSach", maSach);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public int GetSoLuongCon(int maSach)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "SELECT SoLuongCon FROM DauSach WHERE MaSach = @MaSach";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaSach", maSach);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? (int)result : 0;
            }
        }
    }
}