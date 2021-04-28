
using System.ComponentModel.DataAnnotations;

namespace osuLocalBancho.Types
{
    public class SubmittedScore
    {
        [Key]
        public int Id { get; set; }
        public string FileMd5 { get; set; }
        public string PlayerUsername { get; set; }
        // %s%s%s = scoreData[2]
        public int Count300 { get; set; }
        public int Count100 { get; set; }
        public int Count50 { get; set; }
        public int CountGeki { get; set; }
        public int CountKatu { get; set; }
        public int CountMiss { get; set; }
        public int Score { get; set; }
        public int MaxCombo { get; set; }
        public bool FullCombo { get; set; }
        public string Rank { get; set; }
        public int Mods { get; set; }
        public bool Passed { get; set; }
        public int GameMode { get; set; }
        public int PlayDateTime { get; set; }
        //self.calculateAccuracy()
        public string OsuVersion { get; set; }
        public bool Quit { get; set; }
        public bool Failed { get; set; }
        public double Pp { get; set; }
    }
}
