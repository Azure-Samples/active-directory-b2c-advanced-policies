using System.Collections.Generic;

namespace B2CAppMvc.Models.Home
{
    public class IndexViewModel
    {
        public IEnumerable<RentableGameViewModel> NewReleaseGames { get; set; }
    }

    public class RentableGameViewModel : GameViewModel
    {
        public decimal DiscountPrice { get; set; }

        public string[] GamerZones { get; set; }

        public decimal StandardPrice { get; set; }
    }

    public class GameViewModel
    {
        public string ImageSource { get; set; }

        public string Title { get; set; }
    }
}
