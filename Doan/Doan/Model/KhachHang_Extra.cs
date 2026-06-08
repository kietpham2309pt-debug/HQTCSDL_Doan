namespace Doan.Model
{
    // Thuộc tính bổ sung, KHÔNG ánh xạ vào CSDL.
    // XepHang được tính bằng FUNCTION fn_XepHangKhachHang và gán khi nạp danh sách.
    public partial class KhachHang
    {
        public string XepHang { get; set; }
    }
}
