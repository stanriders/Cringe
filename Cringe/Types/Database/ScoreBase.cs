using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cringe.Types.Enums;

namespace Cringe.Types.Database
{
    public class ScoreBase
    {
        [Key]
        public int Id { get; set; }

        public int Count300 { get; set; }
        public int Count100 { get; set; }
        public int Count50 { get; set; }
        public int CountGeki { get; set; }
        public int CountKatu { get; set; }
        public int CountMiss { get; set; }
        public long Score { get; set; }
        public int MaxCombo { get; set; }
        public bool FullCombo { get; set; }
        public string Rank { get; set; }
        public Mods Mods { get; set; }
        public bool Passed { get; set; }
        public GameModes GameMode { get; set; }

        [DisplayFormat(DataFormatString = "{0:O}")]
        public DateTime PlayDateTime { get; set; }
        public string OsuVersion { get; set; }
        public bool Quit { get; set; }
        public bool Failed { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Pp { get; set; }

        [NotMapped]
        public long ScoreV4 => (long)Math.Round(Pp * 1000);

        [NotMapped]
        public double ScoreRepresentation => ScoreV4;

        public int BeatmapId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerUsername { get; set; }

        [NotMapped]
        public int LeaderboardPosition { get; set; }

        [NotMapped]
        public Beatmap Beatmap { get; set; }

        [NotMapped]
        public Player Player { get; set; }

        [NotMapped]
        public SubmittedScore PreviousScore { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:N2}%")]
        public double Accuracy
        {
            get
            {
                double totalPoints = Count50 * 50 + Count100 * 100 + Count300 * 300;
                double totalHits = CountMiss + Count50 + Count100 + Count300;

                return totalPoints / (totalHits * 300) * 100;
            }
        }

        [NotMapped]
        public string BeatmapTitle =>
            string.IsNullOrEmpty(Beatmap?.FullTitle) ? BeatmapId.ToString() : Beatmap?.FullTitle;

        public override string ToString()
        {
            //18204696(scoreId)|sendlolipls(username)|167060(score)|663(combo)|0(50)|6(100)|483(300)|0(miss)|6(katu)|79(geki)|False(fc)|216(mods)|101029(???)|4(rank)|1596985749(date)|1(??)
            return
                $"{Id}|{PlayerUsername}|{ScoreRepresentation}|{MaxCombo}|{Count50}|{Count100}|{Count300}|{CountMiss}|{CountKatu}|{CountGeki}|{FullCombo}|{(int) Mods}|1|{LeaderboardPosition}|{((DateTimeOffset) PlayDateTime).ToUnixTimeSeconds()}|1\n";
        }
    }
}
