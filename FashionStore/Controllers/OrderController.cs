using FashionStore.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FashionStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly FashionStoreDBEntities db = new FashionStoreDBEntities();

        // GET: Order/Checkout
        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as CartModel;
            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            if (!User.Identity.IsAuthenticated)
            {
                TempData["WarningMessage"] = "Vui lòng đăng nhập để đặt hàng!";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Order") });
            }

            var model = new OrderModel
            {
                CartItems = cart.Items,
                GrandTotal = cart.GrandTotal,
                CustomerName = User.Identity.Name
            };

            return View(model);
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(OrderModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var cart = Session["Cart"] as CartModel;
                    if (cart != null)
                    {
                        model.CartItems = cart.Items;
                        model.GrandTotal = cart.GrandTotal;
                    }
                    return View(model);
                }

                // SỬA LỖI: Thêm dòng khai báo cartCheck
                var cartCheck = Session["Cart"] as CartModel;

                if (cartCheck == null || !cartCheck.Items.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống!";
                    return RedirectToAction("Index", "Cart");
                }

                // Lưu đơn hàng
                var orderId = SaveOrder(model, cartCheck);

                if (model.PaymentMethod == "PayPal")
                {
                    TempData["InfoMessage"] = "Tính năng PayPal đang được phát triển";
                }

                // Xóa giỏ hàng
                Session["Cart"] = null;
                Session["CartCount"] = 0;

                TempData["SuccessMessage"] = $"Đặt hàng thành công! Mã đơn hàng: #{orderId}";
                return RedirectToAction("OrderSuccess", new { id = orderId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi đặt hàng: " + ex.Message;

                var cart = Session["Cart"] as CartModel;
                if (cart != null)
                {
                    model.CartItems = cart.Items;
                    model.GrandTotal = cart.GrandTotal;
                }

                return View(model);
            }
        }

        private int SaveOrder(OrderModel model, CartModel cart)
        {
            using (var db = new FashionStoreDBEntities())
            {
                // Lấy UserId từ bảng Users
                var currentUser = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
                string userId = currentUser?.Id.ToString();

                // Tạo đơn hàng mới
                var order = new Order
                {
                    CustomerName = model.CustomerName,
                    Phone = model.Phone,
                    Address = model.Address,
                    Email = model.Email,
                    PaymentMethod = model.PaymentMethod,
                    Note = model.Note,
                    TotalAmount = cart.GrandTotal,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    UserId = userId ?? "0"
                };

                db.Orders.Add(order);
                db.SaveChanges();

                // Lưu chi tiết đơn hàng
                foreach (var item in cart.Items)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Total = item.Quantity * item.Price
                    };
                    db.OrderDetails.Add(orderDetail);
                }

                db.SaveChanges();
                return order.Id;
            }
        }

        // GET: Order/OrderSuccess/5
        public ActionResult OrderSuccess(int id)
        {
            using (var db = new FashionStoreDBEntities())
            {
                var order = db.Orders.Find(id);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                    return RedirectToAction("Index", "Home");
                }

                return View(order);
            }
        }

        // GET: Order/MyOrder
        public ActionResult MyOrder()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["WarningMessage"] = "Vui lòng đăng nhập để xem đơn hàng!";
                return RedirectToAction("Login", "Account");
            }

            using (var db = new FashionStoreDBEntities())
            {
                var currentUser = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
                if (currentUser == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng!";
                    return RedirectToAction("Index", "Home");
                }

                var orders = db.Orders
                              .Where(o => o.UserId == currentUser.Id.ToString())
                              .OrderByDescending(o => o.OrderDate)
                              .ToList();

                return View(orders);
            }
        }

        // GET: Order/OrderDetail/5
        public ActionResult OrderDetail(int id)
        {
            using (var db = new FashionStoreDBEntities())
            {
                var order = db.Orders.Find(id);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                    return RedirectToAction("MyOrder");
                }

                // Load OrderDetails
                var orderDetails = db.OrderDetails.Where(od => od.OrderId == id).ToList();
                order.OrderDetails = orderDetails;

                // Kiểm tra quyền xem đơn hàng
                var currentUser = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
                if (order.UserId != currentUser?.Id.ToString())
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xem đơn hàng này!";
                    return RedirectToAction("MyOrder");
                }

                return View(order);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}