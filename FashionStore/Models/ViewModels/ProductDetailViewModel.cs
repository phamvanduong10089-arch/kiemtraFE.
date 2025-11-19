using System.Collections.Generic;

namespace FashionStore.Models.ViewModels
{
    public class ProductDetailViewModel
    {
        public Products Product { get; set; }
        public List<Products> SimilarProducts { get; set; }
        public List<Products> TopDeals { get; set; }

        public ProductDetailViewModel()
        {
            SimilarProducts = new List<Products>();
            TopDeals = new List<Products>();
        }
    }
}