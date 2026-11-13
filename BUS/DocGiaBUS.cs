using System.Data;
using LibraryManagement.DAL;
using LibraryManagement.DTO;

namespace LibraryManagement.BUS
{
    public class DocGiaBUS
    {
        private readonly DocGiaDAL dal = new DocGiaDAL();

        public DataTable GetAll() => dal.GetAllDocGia();

        public string ThemDocGia(DocGiaDTO dg)
        {
            if (string.IsNullOrWhiteSpace(dg.HoTen))
                return "Họ tên độc giả không được để trống.";
            if (string.IsNullOrWhiteSpace(dg.Email))
                return "Email không được để trống.";

            return dal.Insert(dg) ? "Thêm độc giả thành công!" : "Thêm độc giả thất bại.";
        }

        public string SuaDocGia(DocGiaDTO dg)
        {
            if (dg.MaDocGia <= 0) return "Vui lòng chọn độc giả cần sửa.";
            if (string.IsNullOrWhiteSpace(dg.HoTen)) return "Họ tên không được để trống.";

            return dal.Update(dg) ? "Cập nhật độc giả thành công!" : "Cập nhật thất bại.";
        }

        public string XoaDocGia(int maDocGia)
        {
            if (maDocGia <= 0) return "Vui lòng chọn độc giả cần xóa.";
            return dal.Delete(maDocGia) ? "Xóa độc giả thành công!" : "Xóa thất bại.";
        }
    }
}