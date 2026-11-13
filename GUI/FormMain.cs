using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagement.BUS;
using LibraryManagement.DTO;

namespace LibraryManagement.GUI
{
    public class FormMain : Form
    {
        private readonly DocGiaBUS dgBus = new DocGiaBUS();
        private readonly DauSachBUS dsBus = new DauSachBUS();
        private readonly PhieuMuonBUS pmBus = new PhieuMuonBUS();

        // Controls Tab 1: Sách & Độc giả
        private DataGridView dgvSach, dgvDocGia;
        private TextBox txtMaSach, txtTenSach, txtMaDG, txtHoTen, txtEmail;
        private ComboBox cboTheLoai;
        private NumericUpDown numSoLuong;
        private DateTimePicker dtpNgayHetHan;

        // Controls Tab 2: Mượn / Trả
        private ComboBox cboDocGiaMuon, cboSachMuon, cboLocDocGia, cboLocTrangThai;
        private NumericUpDown numSoLuongMuon;
        private DateTimePicker dtpHanTra;
        private DataGridView dgvPhieuMuon;

        // Controls Tab 3: Quá hạn & Thống kê
        private DataGridView dgvQuaHan, dgvThongKe;
        private ComboBox cboTheLoaiTK;
        private Label lblTongLuotScalar;

        public FormMain()
        {
            InitUI();
            try
            {
                LoadAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối SQL Server: " + ex.Message, "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitUI()
        {
            this.Text = "Hệ Thống Quản Lý Thư Viện (3-Tier Architecture)";
            this.Size = new Size(1050, 680);
            this.StartPosition = FormStartPosition.CenterScreen;

            TabControl tabControl = new TabControl { Dock = DockStyle.Fill };
            tabControl.TabPages.Add(CreateTabQuanLy());
            tabControl.TabPages.Add(CreateTabMuonTra());
            tabControl.TabPages.Add(CreateTabThongKeQuaHan());

            this.Controls.Add(tabControl);
        }

        // ==================== TAB 1: ĐẦU SÁCH & ĐỘC GIẢ ====================
        private TabPage CreateTabQuanLy()
        {
            TabPage tab = new TabPage("1. Quản lý Sách & Độc giả");

            // --- Nửa trên: Quản lý Đầu sách ---
            GroupBox grpSach = new GroupBox { Text = "Quản lý Đầu sách", Location = new Point(10, 10), Size = new Size(1000, 290) };
            grpSach.Controls.Add(new Label { Text = "Mã sách:", Location = new Point(15, 30), AutoSize = true });
            txtMaSach = new TextBox { Location = new Point(95, 27), Width = 80, ReadOnly = true };

            grpSach.Controls.Add(new Label { Text = "Tên sách:", Location = new Point(195, 30), AutoSize = true });
            txtTenSach = new TextBox { Location = new Point(265, 27), Width = 230 };

            grpSach.Controls.Add(new Label { Text = "Thể loại:", Location = new Point(515, 30), AutoSize = true });
            cboTheLoai = new ComboBox { Location = new Point(575, 27), Width = 170, DropDownStyle = ComboBoxStyle.DropDown };
            cboTheLoai.Items.AddRange(new object[] { "Công nghệ thông tin", "Kinh tế", "Văn học", "Ngoại ngữ" });

            grpSach.Controls.Add(new Label { Text = "Số lượng:", Location = new Point(765, 30), AutoSize = true });
            numSoLuong = new NumericUpDown { Location = new Point(835, 27), Width = 80, Maximum = 1000, Value = 1 };

            Button btnThemSach = new Button { Text = "Thêm Sách", Location = new Point(15, 65), Width = 100 };
            Button btnSuaSach = new Button { Text = "Sửa Sách", Location = new Point(125, 65), Width = 100 };
            Button btnXoaSach = new Button { Text = "Xóa Sách", Location = new Point(235, 65), Width = 100 };

            btnThemSach.Click += (s, e) => {
                MessageBox.Show(dsBus.ThemDauSach(new DauSachDTO {
                    TenSach = txtTenSach.Text, TheLoai = cboTheLoai.Text, SoLuongCon = (int)numSoLuong.Value
                }));
                LoadAllData();
            };
            btnSuaSach.Click += (s, e) => {
                if (!int.TryParse(txtMaSach.Text, out int id)) return;
                MessageBox.Show(dsBus.SuaDauSach(new DauSachDTO {
                    MaSach = id, TenSach = txtTenSach.Text, TheLoai = cboTheLoai.Text, SoLuongCon = (int)numSoLuong.Value
                }));
                LoadAllData();
            };
            btnXoaSach.Click += (s, e) => {
                if (!int.TryParse(txtMaSach.Text, out int id)) return;
                MessageBox.Show(dsBus.XoaDauSach(id));
                LoadAllData();
            };

            dgvSach = new DataGridView {
                Location = new Point(15, 100), Size = new Size(970, 175),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false
            };
            dgvSach.CellClick += (s, e) => {
                if (e.RowIndex < 0) return;
                var row = dgvSach.Rows[e.RowIndex];
                txtMaSach.Text = row.Cells["MaSach"].Value.ToString();
                txtTenSach.Text = row.Cells["TenSach"].Value.ToString();
                cboTheLoai.Text = row.Cells["TheLoai"].Value.ToString();
                numSoLuong.Value = Convert.ToDecimal(row.Cells["SoLuongCon"].Value);
            };

            grpSach.Controls.AddRange(new Control[] { txtMaSach, txtTenSach, cboTheLoai, numSoLuong, btnThemSach, btnSuaSach, btnXoaSach, dgvSach });

            // --- Nửa dưới: Quản lý Độc giả ---
            GroupBox grpDG = new GroupBox { Text = "Quản lý Độc giả", Location = new Point(10, 310), Size = new Size(1000, 290) };
            grpDG.Controls.Add(new Label { Text = "Mã ĐG:", Location = new Point(15, 30), AutoSize = true });
            txtMaDG = new TextBox { Location = new Point(75, 27), Width = 60, ReadOnly = true };

            grpDG.Controls.Add(new Label { Text = "Họ tên:", Location = new Point(150, 30), AutoSize = true });
            txtHoTen = new TextBox { Location = new Point(205, 27), Width = 200 };

            grpDG.Controls.Add(new Label { Text = "Email:", Location = new Point(420, 30), AutoSize = true });
            txtEmail = new TextBox { Location = new Point(470, 27), Width = 200 };

            grpDG.Controls.Add(new Label { Text = "Hạn thẻ:", Location = new Point(685, 30), AutoSize = true });
            dtpNgayHetHan = new DateTimePicker { Location = new Point(745, 27), Width = 140, Format = DateTimePickerFormat.Short };

            Button btnThemDG = new Button { Text = "Thêm ĐG", Location = new Point(15, 65), Width = 100 };
            Button btnSuaDG = new Button { Text = "Sửa ĐG", Location = new Point(125, 65), Width = 100 };
            Button btnXoaDG = new Button { Text = "Xóa ĐG", Location = new Point(235, 65), Width = 100 };

            btnThemDG.Click += (s, e) => {
                MessageBox.Show(dgBus.ThemDocGia(new DocGiaDTO {
                    HoTen = txtHoTen.Text, Email = txtEmail.Text, NgayHetHanThe = dtpNgayHetHan.Value
                }));
                LoadAllData();
            };
            btnSuaDG.Click += (s, e) => {
                if (!int.TryParse(txtMaDG.Text, out int id)) return;
                MessageBox.Show(dgBus.SuaDocGia(new DocGiaDTO {
                    MaDocGia = id, HoTen = txtHoTen.Text, Email = txtEmail.Text, NgayHetHanThe = dtpNgayHetHan.Value
                }));
                LoadAllData();
            };
            btnXoaDG.Click += (s, e) => {
                if (!int.TryParse(txtMaDG.Text, out int id)) return;
                MessageBox.Show(dgBus.XoaDocGia(id));
                LoadAllData();
            };

            dgvDocGia = new DataGridView {
                Location = new Point(15, 100), Size = new Size(970, 175),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false
            };
            dgvDocGia.CellClick += (s, e) => {
                if (e.RowIndex < 0) return;
                var row = dgvDocGia.Rows[e.RowIndex];
                txtMaDG.Text = row.Cells["MaDocGia"].Value.ToString();
                txtHoTen.Text = row.Cells["HoTen"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
                dtpNgayHetHan.Value = Convert.ToDateTime(row.Cells["NgayHetHanThe"].Value);
            };

            grpDG.Controls.AddRange(new Control[] { txtMaDG, txtHoTen, txtEmail, dtpNgayHetHan, btnThemDG, btnSuaDG, btnXoaDG, dgvDocGia });

            tab.Controls.AddRange(new Control[] { grpSach, grpDG });
            return tab;
        }

        // ==================== TAB 2: MƯỢN / TRẢ SÁCH ====================
        private TabPage CreateTabMuonTra()
        {
            TabPage tab = new TabPage("2. Mượn / Trả & Lọc Phiếu");

            GroupBox grpLapPhieu = new GroupBox { Text = "Lập Phiếu Mượn Mới (Kiểm tra hạn thẻ tự động)", Location = new Point(10, 10), Size = new Size(1000, 95) };
            grpLapPhieu.Controls.Add(new Label { Text = "Độc giả:", Location = new Point(15, 35), AutoSize = true });
            cboDocGiaMuon = new ComboBox { Location = new Point(75, 32), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

            grpLapPhieu.Controls.Add(new Label { Text = "Chọn sách:", Location = new Point(270, 35), AutoSize = true });
            cboSachMuon = new ComboBox { Location = new Point(345, 32), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };

            grpLapPhieu.Controls.Add(new Label { Text = "SL:", Location = new Point(580, 35), AutoSize = true });
            numSoLuongMuon = new NumericUpDown { Location = new Point(610, 32), Width = 55, Minimum = 1, Maximum = 50, Value = 1 };

            grpLapPhieu.Controls.Add(new Label { Text = "Hạn trả:", Location = new Point(680, 35), AutoSize = true });
            dtpHanTra = new DateTimePicker { Location = new Point(735, 32), Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Now.AddDays(14) };

            Button btnLapPhieu = new Button { Text = "Lập Phiếu Mượn", Location = new Point(870, 30), Width = 115, Height = 28 };
            btnLapPhieu.Click += (s, e) => {
                if (cboDocGiaMuon.SelectedValue == null || cboSachMuon.SelectedValue == null) return;
                var pm = new PhieuMuonDTO {
                    MaDocGia = Convert.ToInt32(cboDocGiaMuon.SelectedValue),
                    NgayMuon = DateTime.Now,
                    HanTra = dtpHanTra.Value
                };
                var dsMuon = new List<ChiTietMuonDTO> {
                    new ChiTietMuonDTO { MaSach = Convert.ToInt32(cboSachMuon.SelectedValue), SoLuong = (int)numSoLuongMuon.Value }
                };
                MessageBox.Show(pmBus.TaoPhieuMuon(pm, dsMuon), "Thông báo");
                LoadAllData();
            };

            grpLapPhieu.Controls.AddRange(new Control[] { cboDocGiaMuon, cboSachMuon, numSoLuongMuon, dtpHanTra, btnLapPhieu });

            GroupBox grpDanhSach = new GroupBox { Text = "Danh sách Phiếu mượn & Trả sách", Location = new Point(10, 115), Size = new Size(1000, 485) };
            grpDanhSach.Controls.Add(new Label { Text = "Lọc theo Độc giả:", Location = new Point(15, 30), AutoSize = true });
            cboLocDocGia = new ComboBox { Location = new Point(125, 27), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            grpDanhSach.Controls.Add(new Label { Text = "Trạng thái:", Location = new Point(345, 30), AutoSize = true });
            cboLocTrangThai = new ComboBox { Location = new Point(415, 27), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLocTrangThai.Items.AddRange(new object[] { "Tất cả", "Đang mượn", "Đã trả", "Quá hạn" });
            cboLocTrangThai.SelectedIndex = 0;

            Button btnLoc = new Button { Text = "Lọc Danh Sách", Location = new Point(575, 25), Width = 120, Height = 28 };
            btnLoc.Click += (s, e) => {
                if (cboLocDocGia.SelectedValue == null) return;
                int selectedDG = Convert.ToInt32(cboLocDocGia.SelectedValue);
                int? maDG = selectedDG == -1 ? (int?)null : selectedDG;
                dgvPhieuMuon.DataSource = pmBus.LocPhieuMuon(maDG, cboLocTrangThai.Text);
            };

            Button btnTraSach = new Button { Text = "Xác Nhận Trả Sách", Location = new Point(820, 25), Width = 160, Height = 28, BackColor = Color.LightGreen };
            btnTraSach.Click += (s, e) => {
                if (dgvPhieuMuon.CurrentRow == null) return;
                int maPhieu = Convert.ToInt32(dgvPhieuMuon.CurrentRow.Cells["MaPhieu"].Value);
                MessageBox.Show(pmBus.TraSach(maPhieu), "Trả sách");
                LoadAllData();
            };

            dgvPhieuMuon = new DataGridView {
                Location = new Point(15, 65), Size = new Size(970, 405),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false
            };

            grpDanhSach.Controls.AddRange(new Control[] { cboLocDocGia, cboLocTrangThai, btnLoc, btnTraSach, dgvPhieuMuon });
            tab.Controls.AddRange(new Control[] { grpLapPhieu, grpDanhSach });
            return tab;
        }

        // ==================== TAB 3: QUÁ HẠN & THỐNG KÊ ====================
        private TabPage CreateTabThongKeQuaHan()
        {
            TabPage tab = new TabPage("3. Cảnh báo Quá hạn & Thống kê");

            GroupBox grpQuaHan = new GroupBox { Text = "Danh sách Phiếu Quá Hạn (HanTra < GETDATE & Chưa trả)", Location = new Point(10, 10), Size = new Size(1000, 280) };
            Button btnKiemTraQuaHan = new Button { Text = "Quét Phiếu Quá Hạn", Location = new Point(15, 25), Width = 160, Height = 28, BackColor = Color.MistyRose };
            btnKiemTraQuaHan.Click += (s, e) => {
                var list = pmBus.GetDanhSachQuaHan();
                dgvQuaHan.DataSource = list;
                MessageBox.Show($"Phát hiện {list.Count} phiếu mượn quá hạn chưa trả!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            dgvQuaHan = new DataGridView {
                Location = new Point(15, 60), Size = new Size(970, 205),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false
            };
            grpQuaHan.Controls.AddRange(new Control[] { btnKiemTraQuaHan, dgvQuaHan });

            GroupBox grpThongKe = new GroupBox { Text = "Thống kê lượt mượn theo Thể loại", Location = new Point(10, 300), Size = new Size(1000, 300) };
            grpThongKe.Controls.Add(new Label { Text = "Chọn thể loại (ExecuteScalar):", Location = new Point(15, 30), AutoSize = true });
            cboTheLoaiTK = new ComboBox { Location = new Point(195, 27), Width = 180, DropDownStyle = ComboBoxStyle.DropDown };
            cboTheLoaiTK.Items.AddRange(new object[] { "Công nghệ thông tin", "Kinh tế", "Văn học" });
            cboTheLoaiTK.SelectedIndex = 0;

            Button btnThongKe = new Button { Text = "Thống Kê", Location = new Point(390, 25), Width = 110, Height = 28 };
            lblTongLuotScalar = new Label { Text = "Tổng lượt mượn: 0", Location = new Point(520, 30), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };

            btnThongKe.Click += (s, e) => {
                dgvThongKe.DataSource = pmBus.ThongKeTheoTheLoai();
                int soLuot = pmBus.DemLuotMuonTheoTheLoai(cboTheLoaiTK.Text);
                lblTongLuotScalar.Text = $"Tổng lượt mượn ({cboTheLoaiTK.Text}): {soLuot}";
            };

            dgvThongKe = new DataGridView {
                Location = new Point(15, 65), Size = new Size(970, 220),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false
            };
            grpThongKe.Controls.AddRange(new Control[] { cboTheLoaiTK, btnThongKe, lblTongLuotScalar, dgvThongKe });

            tab.Controls.AddRange(new Control[] { grpQuaHan, grpThongKe });
            return tab;
        }

        // ==================== NẠP DỮ LIỆU CHUNG ====================
        private void LoadAllData()
        {
            DataTable dtSach = dsBus.GetAll();
            DataTable dtDG = dgBus.GetAll();

            dgvSach.DataSource = dtSach;
            dgvDocGia.DataSource = dtDG;

            cboDocGiaMuon.DataSource = dtDG.Copy();
            cboDocGiaMuon.DisplayMember = "HoTen";
            cboDocGiaMuon.ValueMember = "MaDocGia";

            cboSachMuon.DataSource = dtSach.Copy();
            cboSachMuon.DisplayMember = "TenSach";
            cboSachMuon.ValueMember = "MaSach";

            DataTable dtDGLoc = dtDG.Copy();
            DataRow allRow = dtDGLoc.NewRow();
            allRow["MaDocGia"] = -1;
            allRow["HoTen"] = "-- Tất cả độc giả --";
            dtDGLoc.Rows.InsertAt(allRow, 0);

            cboLocDocGia.DataSource = dtDGLoc;
            cboLocDocGia.DisplayMember = "HoTen";
            cboLocDocGia.ValueMember = "MaDocGia";

            dgvPhieuMuon.DataSource = pmBus.LocPhieuMuon(null, "Tất cả");
            dgvQuaHan.DataSource = pmBus.GetDanhSachQuaHan();
            dgvThongKe.DataSource = pmBus.ThongKeTheoTheLoai();
        }
    }
}