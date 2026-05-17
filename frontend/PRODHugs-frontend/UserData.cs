
using System.ComponentModel;

namespace PRODHugs_frontend
{
    public class UserData
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public int Coins { get; set; }
        public BindingList<HugElement>? HugsHistory { get; set; } = [];
        public BindingList<HugElement>? HugsInbox { get; set; } = [];
        public int TotalHugs { get; set; }
        public int InitiatedHugs { get; set; }
        public int AcceptedHugs { get; set; }
        public string? Gender { get; set; }
        public string? Rank { get; set; }
        public int? TelegramId { get; set; }
        public int DailyStreak { get; set; }
        public string? Tag { get; set; }
    }
}
