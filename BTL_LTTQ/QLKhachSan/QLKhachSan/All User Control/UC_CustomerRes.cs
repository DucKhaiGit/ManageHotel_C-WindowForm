using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLKhachSan.All_User_Control
{
    public partial class UC_CustomerRes : UserControl
    {
        
        public UC_CustomerRes()
        {
            InitializeComponent();
        }
        private DataTable dataTable = new DataTable();

        private void UC_CustomerRes_Load(object sender, EventArgs e)
        {

        }

        //hàm thực hiện tìm số phòng từ csdl sau khi loại giường và loại phòng được chọn
        private void SearchRoomNum() 
        {
            if (cbbCusBedType.SelectedIndex != -1 && cbbCusRoomType.SelectedIndex != -1)
            {
                // Lấy lựa chọn từ ComboBox cbbCusBedType và cbbCusRoomType
                string selectedBedType = cbbCusBedType.SelectedItem.ToString();
                string selectedRoomType = cbbCusRoomType.SelectedItem.ToString();

                // Tạo một đối tượng của lớp function
                function dataAccess = new function();

                // Thực hiện truy vấn để lấy danh sách số phòng từ cơ sở dữ liệu dựa trên lựa chọn của người dùng
                string query = "SELECT roomNo FROM rooms WHERE bed = @BedType AND roomType = @RoomType";
                using (SqlConnection connection = new SqlConnection(function.GetConnectionString()))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Gán giá trị cho các tham số trong truy vấn SQL
                        command.Parameters.AddWithValue("@BedType", selectedBedType);
                        command.Parameters.AddWithValue("@RoomType", selectedRoomType);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            cbbCusRoomNum.Items.Clear();

                            while (reader.Read())
                            {
                                cbbCusRoomNum.Items.Add(reader["roomNo"].ToString());
                            }
                        }
                    }
                }
            }    
           
        }

        private void cbbCusBedType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchRoomNum();
        }

        private void cbbCusRoomType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchRoomNum();
        }

        

        private void cbbCusRoomNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRoomPrice();
        }
        private void UpdateRoomPrice()
        {
            if (cbbCusRoomNum.SelectedIndex != -1)
            {
                // Lấy số phòng đã chọn
                string selectedRoomNumber = cbbCusRoomNum.SelectedItem.ToString();

                // Thực hiện truy vấn để lấy giá tiền từ cơ sở dữ liệu dựa trên số phòng
                string query = "SELECT price FROM rooms WHERE roomNo = @roomNumber";

                using (SqlConnection connection = new SqlConnection(function.GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@roomNumber", selectedRoomNumber);

                        connection.Open();

                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            // Hiển thị giá tiền lên txtCusRoomPrice
                            txtCusRoomPrice.Text = result.ToString();
                        }
                    }
                }
            }
        }
        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ giao diện
            string customerName = txtCusName.Text;
            string customerPhone = txtCusNum.Text;
            string customerNationality = txtCusNational.Text;
            string customerGender = cbbCusGender.SelectedItem.ToString();
            string customerDOB = dtpCusDoB.Value.ToString("yyyy-MM-dd");
            string customerIdProof = txtCusIDNum.Text;
            string customerAddress = txtCusAddr.Text;
            string selectedRoomNumber = cbbCusRoomNum.SelectedItem.ToString();
            string customerBedType = cbbCusBedType.SelectedItem.ToString();
            string customerRoomType = cbbCusRoomType.SelectedItem.ToString();
            decimal roomPrice = decimal.Parse(txtCusRoomPrice.Text);
            // Lấy giá trị ngày đăng kí thuê phòng từ DateTimePicker
            string checkinDate = dtpCusRentDay.Value.ToString("yyyy-MM-dd");

            // Lấy room ID dựa trên số phòng đã chọn
            int roomId = GetRoomIdByRoomNumber(selectedRoomNumber);

            if (roomId > 0)
            {
                // Thêm thông tin khách hàng vào cơ sở dữ liệu
                int customerId = AddCustomerToDatabase(customerName, customerPhone, customerNationality, customerGender, customerDOB, customerIdProof, customerAddress, roomId, checkinDate);

                if (customerId > 0)
                {
                    // Cập nhật trạng thái phòng đã chọn từ "No" thành "Yes" và lưu vào cơ sở dữ liệu
                    UpdateRoomStatus(selectedRoomNumber);
                }
            }
        }
        private int AddCustomerToDatabase(string name, string phone, string nationality, string gender, string dob, string idProof, string address,int roomId, string checkinDate)
        {
            // Lấy chuỗi kết nối
            string connectionString = function.GetConnectionString();

            // Thực hiện truy vấn để thêm khách hàng vào cơ sở dữ liệu và lấy mã khách hàng sau khi thêm
            string query = "INSERT INTO customer (cname, mobile, nationality, gender, dob, idproof, address, roomid,checkin) " +
                           "VALUES (@name, @phone, @nationality, @gender, @dob, @idProof, @address, @roomId,@checkinDate);" +
                           "SELECT SCOPE_IDENTITY();"; // Lấy mã khách hàng sau khi thêm

            int customerId = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@phone", phone);
                    command.Parameters.AddWithValue("@nationality", nationality);
                    command.Parameters.AddWithValue("@gender", gender);
                    command.Parameters.AddWithValue("@dob", dob);
                    command.Parameters.AddWithValue("@idProof", idProof);
                    command.Parameters.AddWithValue("@address", address);
                    command.Parameters.AddWithValue("@roomId", roomId);                   
                    command.Parameters.AddWithValue("@checkinDate", checkinDate);
                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        customerId = Convert.ToInt32(result);
                    }
                }
            }
            return customerId;
        }
        private int GetRoomIdByRoomNumber(string roomNumber)
        {
            // Lấy chuỗi kết nối
            string connectionString = function.GetConnectionString();

            // Thực hiện truy vấn để lấy room ID dựa trên số phòng
            string query = "SELECT roomid FROM rooms WHERE roomNo = @roomNumber";

            int roomId = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roomNumber", roomNumber);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        roomId = Convert.ToInt32(result);
                    }
                }
            }
            return roomId;
        }

        private void UpdateRoomStatus(string roomNumber)
        {
            // Lấy chuỗi kết nối
            string connectionString = function.GetConnectionString();

            // Thực hiện truy vấn để cập nhật trạng thái phòng
            string updateQuery = "UPDATE rooms SET booked = 'YES' WHERE roomNo = @roomNumber";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@roomNumber", roomNumber);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
