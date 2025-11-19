using System.Collections.Generic;

namespace FashionStore.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Banners> Banners { get; set; }
        public List<Products> FeaturedProducts { get; set; }
        public List<Products> NewProducts { get; set; }

        public HomeViewModel()
        {
            Banners = new List<Banners>();
            FeaturedProducts = new List<Products>();
            NewProducts = new List<Products>();
        }
    }
}