using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cringe.Types.Enums;

namespace Cringe.Types.Database
{
    public class Beatmap
    {
        [Key] public int Id { get; set; }

        public int? BeatmapSetId { get; set; }

        public GameModes Mode { get; set; }

        public string Md5 { get; set; }

        public RankedStatus Status { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public string DifficultyName { get; set; }

        public string Creator { get; set; }

        public double Bpm { get; set; }

        public double HpDrain { get; set; }

        public double CircleSize { get; set; }

        public double OverallDifficulty { get; set; }

        public double ApproachRate { get; set; }

        public int Length { get; set; }

        [NotMapped] public string FullTitle => $"{Artist} - {Title} [{DifficultyName}]";
    }
}