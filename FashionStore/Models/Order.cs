using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên khách hàng")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        [Display(Name = "Mã người dùng")]
        public string UserId { get; set; }

        // Navigation property - liên kết đến OrderDetails
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            OrderDate = DateTime.Now;
            Status = "Pending";
            PaymentMethod = "COD";
        }
    }
}