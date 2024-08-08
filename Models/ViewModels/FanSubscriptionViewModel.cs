namespace Lab5.Models.ViewModels
{
    public class FanSubscriptionViewModel
    {
        public Fan Fan { get; set; }
        public int FanId { get; set; }
        public IEnumerable<SportClubSubscriptionViewModel> Subscriptions { get; set; }

    }
}
