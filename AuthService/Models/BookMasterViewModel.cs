namespace UserService.ViewModels
{
    public class BookMasterViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public double Price { get; set; }
        public DateTime PublishedDate { get; set; }
        public string CategoryName { get; set; }
        public string Email { get; set; }
        public string BookContent { get; set; }
        public bool Active { get; set; }

    }
}
