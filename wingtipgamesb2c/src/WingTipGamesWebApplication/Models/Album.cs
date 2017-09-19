namespace WingTipGamesWebApplication.Models
{
    public class Album
    {
        public Artist Artist { get; set; }

        public decimal DiscountPrice { get; set; }

        public string ImageSource { get; set; }

        public string[] ListenerGenres { get; set; }

        public decimal StandardPrice { get; set; }

        public string Title { get; set; }
    }
}
