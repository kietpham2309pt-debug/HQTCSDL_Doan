using Doan.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Doan.ViewModel
{
    public class NhanVienViewModel : INotifyPropertyChanged
    {
        private string strCon = @"Data Source=.;
Initial Catalog=DoAnHQTCSDL;
Integrated Security=True;
TrustServerCertificate=True";

        // 1. Danh sách gốc để giữ toàn bộ dữ liệu từ SQL
        private List<NhanVien> _allNhanVien = new List<NhanVien>();

        // Danh sách hiển thị trên UI (DataGrid sẽ Binding vào đây)
        public ObservableCollection<NhanVien> DanhSachNV { get; set; }

        private NhanVien _selectedNV;
        public NhanVien SelectedNV
        {
            get => _selectedNV;
            set { _selectedNV = value; OnPropertyChanged("SelectedNV"); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
                ExecuteTimKiem(); // Gọi hàm lọc ngay khi chữ thay đổi
            }
        }

        public ICommand ThemCommand { get; set; }
        public ICommand SuaCommand { get; set; }
        public ICommand XoaCommand { get; set; }

        public NhanVienViewModel()
        {
            DanhSachNV = new ObservableCollection<NhanVien>();
            SelectedNV = new NhanVien { NgaySinh = DateTime.Now };

            ThemCommand = new RelayCommand(p => ExecuteThem());
            SuaCommand = new RelayCommand(p => ExecuteSua());
            XoaCommand = new RelayCommand(p => ExecuteXoa());

            LoadData();
        }

        // 2. LoadData bây giờ sẽ tải tất cả vào danh sách gốc
        public void LoadData()
        {
            try
            {
                _allNhanVien.Clear();
                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    string sql = "SELECT * FROM vw_DanhSachNhanVien";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        _allNhanVien.Add(new NhanVien
                        {
                            MaNV = (int)rdr["MaNV"],
                            HoTen = rdr["HoTen"].ToString(),
                            NgaySinh = DateTime.ParseExact(rdr["NgaySinh"].ToString(), "dd/MM/yyyy", null),
                            GioiTinh = rdr["GioiTinh"].ToString(),
                            ChucVu = rdr["ChucVu"].ToString(),
                            Luong = (decimal)rdr["Luong"]
                        });
                    }
                }
                // Sau khi tải xong, thực hiện hiển thị lên UI
                ExecuteTimKiem();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // 3. Hàm lọc dữ liệu trên RAM (In-memory Filter)
        public void ExecuteTimKiem()
        {
            IEnumerable<NhanVien> danhSachLoc = _allNhanVien;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string tuKhoa = SearchText.Trim().ToLower();

                danhSachLoc = danhSachLoc.Where(item =>
                    (item.HoTen ?? "").ToLower().Contains(tuKhoa) ||
                    (item.ChucVu ?? "").ToLower().Contains(tuKhoa) ||
                    (item.GioiTinh ?? "").ToLower().Contains(tuKhoa)
                );
            }

            // Cập nhật lại ObservableCollection để UI thay đổi
            DanhSachNV.Clear();
            foreach (var nv in danhSachLoc)
            {
                DanhSachNV.Add(nv);
            }
        }

        public void ExecuteThem()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    SqlCommand cmd = new SqlCommand("sp_ThemNhanVien", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@HoTen", SelectedNV.HoTen ?? "");
                    cmd.Parameters.AddWithValue("@NgaySinh", SelectedNV.NgaySinh);
                    cmd.Parameters.AddWithValue("@GioiTinh", SelectedNV.GioiTinh ?? "");
                    cmd.Parameters.AddWithValue("@ChucVu", SelectedNV.ChucVu ?? "");
                    cmd.Parameters.AddWithValue("@Luong", SelectedNV.Luong);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    LoadData(); // Load lại toàn bộ danh sách mới từ SQL
                    MessageBox.Show("Thêm thành công!");
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        public void ExecuteSua()
        {
            if (SelectedNV == null || SelectedNV.MaNV == 0) return;
            try
            {
                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    string sql = "UPDATE NhanVien SET HoTen=@Ten, NgaySinh=@NS, GioiTinh=@GT, ChucVu=@CV, Luong=@L WHERE MaNV=@Ma";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Ten", SelectedNV.HoTen);
                    cmd.Parameters.AddWithValue("@NS", SelectedNV.NgaySinh);
                    cmd.Parameters.AddWithValue("@GT", SelectedNV.GioiTinh);
                    cmd.Parameters.AddWithValue("@CV", SelectedNV.ChucVu);
                    cmd.Parameters.AddWithValue("@L", SelectedNV.Luong);
                    cmd.Parameters.AddWithValue("@Ma", SelectedNV.MaNV);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    LoadData(); // Load lại danh sách gốc
                    MessageBox.Show("Sửa thành công!");
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        public void ExecuteXoa()
        {
            if (SelectedNV == null || SelectedNV.MaNV == 0) return;
            if (MessageBox.Show("Xóa nhân viên này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(strCon))
                    {
                        SqlCommand cmd = new SqlCommand("DELETE FROM NhanVien WHERE MaNV=@Ma", conn);
                        cmd.Parameters.AddWithValue("@Ma", SelectedNV.MaNV);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        LoadData(); // Load lại danh sách gốc
                        SelectedNV = new NhanVien();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
