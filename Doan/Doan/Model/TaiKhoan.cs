using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doan.Model
{
    internal class TaiKhoan
    {
        private string username;
        private string password;
        private string role;
        private string maNV_MaKH;

        public string Username { get { return username; } set { username = value; } }
        public string Password { get { return password; } set { password = value; } }
        public string Role { get { return role; } set { role = value; } }
        public string MaNV_MaKH { get { return maNV_MaKH; } set { maNV_MaKH = value; } }

        public TaiKhoan() { }
        public TaiKhoan(string username, string password, string role, string maNV_MaKH)
        {
            Username = username;
            Password = password;
            Role = role;
            MaNV_MaKH = maNV_MaKH;
        }
    }
}