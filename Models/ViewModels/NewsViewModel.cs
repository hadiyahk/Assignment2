namespace Lab5.Models.ViewModels
{
    public class NewsViewModel
    {
        public IEnumerable<Fan> Fans { get; set; }
        public IEnumerable<SportClub> SportsClubs { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }



    }
}
