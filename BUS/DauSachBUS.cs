using System.Data;
using LibraryManagement.DAL;
using LibraryManagement.DTO;

namespace LibraryManagement.BUS
{
    public class DauSachBUS
    {
        private readonly DauSachDAL dal = new DauSachDAL();

        public DataTable GetAll() => dal.GetAllDauSach();

        public string ThemDauSach(DauSachDTO ds)
        {
            if (string.IsNullOrWhiteSpace(ds.TenSach))
                return "Tên sách không được để trống.";
            if (ds.SoLuongCon < 0)
                return "Số lượng sách phải lớn hơn hoặc bằng 0.";

            return dal.Insert(ds) ? "Thêm đầu sách thành công!" : "Thêm thất bại.";
        }

        public string SuaDauSach(DauSachDTO ds)
        {
            if (ds.MaSach <= 0) return "Vui lòng chọn đầu sách cần sửa.";
            if (ds.SoLuongCon < 0) return "Số lượng sách không hợp lệ.";

            return dal.Update(ds) ? "Cập nhật đầu sách thành công!" : "Cập nhật thất bại.";
        }

        public string XoaDauSach(int maSach)
        {
            if (maSach <= 0) return "Vui lòng chọn đầu sách cần xóa.";
            return dal.Delete(maSach) ? "Xóa đầu sách thành công!" : "Xóa thất bại.";
        }
    }
}