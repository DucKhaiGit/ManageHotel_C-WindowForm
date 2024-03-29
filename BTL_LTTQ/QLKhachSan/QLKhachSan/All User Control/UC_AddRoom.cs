using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLKhachSan.All_User_Control
{
    public partial class UC_AddRoom : UserControl
    {
   
        public UC_AddRoom()
        {
            InitializeComponent();
        }
        function dtbase = new function();
        private void btnAddRoom_Click(object sender, EventArgs e)
        {
            string roomNo = txtRoomNum.Text;
            string roomType = cbbRoomType.SelectedItem.ToString();
            string bed = cbbBedType.SelectedItem.ToString();
            decimal pricePerHour;

            if (decimal.TryParse(txtPrice.Text, out pricePerHour))
            {
                // Thực hiện lưu thông tin phòng vào cơ sở dữ liệu
                string query = "INSERT INTO rooms (roomNo, roomType, bed, price, booked) " +
                               "VALUES (@roomNo, @roomType, @bed, @price, 'NO')";

                using (SqlConnection connection = new SqlConnection(function.GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@roomNo", roomNo);
                        cmd.Parameters.AddWithValue("@roomType", roomType);
                        cmd.Parameters.AddWithValue("@bed", bed);
                        cmd.Parameters.AddWithValue("@price", pricePerHour);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Hiển thị thông tin phòng trên DataGridView
                UpdateDataGridView();
            }
            else
            {
                MessageBox.Show("Giá tiền không hợp lệ. Vui lòng kiểm tra lại.");
            }
        }

        private void UpdateDataGridView()
        {
            // Lấy thông tin phòng từ cơ sở dữ liệu và hiển thị lên DataGridView
            string query = "SELECT * FROM rooms";

            using (SqlConnection connection = new SqlConnection(function.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvRoom.DataSource = dataTable;
                }
            }
        }

        private void btnShowRoom_Click(object sender, EventArgs e)
        {
            UpdateDataGridView();
        }
    }
}
