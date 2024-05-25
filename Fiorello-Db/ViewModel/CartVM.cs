using Fiorello_Db.Models;

namespace Fiorello_Db.ViewModel
{
    public class CartVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public  string Image { get; set; }
        public int Count { get; set; }
        public float Price { get; set; }
        public float BasketTotalPrice { get; set; }
    }
}
