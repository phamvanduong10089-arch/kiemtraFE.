using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        [Display(Name = "Mã đơn hàng")]
        public int OrderId { get; set; }

        [Display(Name = "Mã sản phẩm")]
        public int ProductId { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Đơn giá")]
        public decimal Price { get; set; }

        [Display(Name = "Thành tiền")]
        public decimal Total { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}