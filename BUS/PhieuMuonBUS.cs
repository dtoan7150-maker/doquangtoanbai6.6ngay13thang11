using System.Collections.Generic;
using System.Data;
using LibraryManagement.DAL;
using LibraryManagement.DTO;

namespace LibraryManagement.BUS
{
    public class PhieuMuonBUS
    {
        private readonly PhieuMuonDAL pmDal = new PhieuMuonDAL();
        private readonly DocGiaDAL dgDal = new DocGiaDAL();
        private readonly DauSachDAL dsDal = new DauSachDAL();

        public string TaoPhieuMuon(PhieuMuonDTO pm, List<ChiTietMuonDTO> chiTietList)
        {
            // Kiểm tra hạn thẻ độc giả (NgayHetHanThe >= GetDate())
            if (!dgDal.KiemTraTheConHan(pm.MaDocGia))
                return "Thẻ độc giả đã hết hạn! Không thể lập phiếu mượn mới.";

            if (pm.HanTra <= pm.NgayMuon)
                return "Hạn trả sách phải lớn hơn ngày mượn.";

            if (chiTietList == null || chiTietList.Count == 0)
                return "Vui lòng chọn ít nhất một đầu sách để mượn.";

            // Kiểm tra tồn kho của từng sách
            foreach (var item in chiTietList)
            {
                int tonKho = dsDal.GetSoLuongCon(item.MaSach);
                if (item.SoLuong <= 0 || item.SoLuong > tonKho)
                    return $"Sách mã {item.MaSach} không đủ số lượng trong kho (Còn lại: {tonKho}).";
            }

            return pmDal.LapPhieuMuon(pm, chiTietList)
                ? "Lập phiếu mượn thành công!"
                : "Có lỗi xảy ra khi lập phiếu mượn.";
        }

        public DataTable LocPhieuMuon(int? maDocGia, string trangThai)
            => pmDal.LocPhieuMuon(maDocGia, trangThai);

        public string TraSach(int maPhieu)
        {
            if (maPhieu <= 0) return "Mã phiếu mượn không hợp lệ.";
            return pmDal.TraSach(maPhieu)
                ? "Trả sách thành công! Đã cập nhật trạng thái và hoàn trả số lượng kho."
                : "Phiếu này đã được trả trước đó hoặc không tồn tại.";
        }

        public List<PhieuMuonDTO> GetDanhSachQuaHan()
            => pmDal.GetDanhSachQuaHan();

        public DataTable ThongKeTheoTheLoai()
            => pmDal.ThongKeTheoTheLoai_GroupBy();

        public int DemLuotMuonTheoTheLoai(string theLoai)
            => pmDal.DemLuotMuonTheoTheLoai_Scalar(theLoai);
    }
}