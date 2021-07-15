namespace Cringe.Types.Bancho
{
    public class BeatmapChart
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public uint RankBefore { get; set; }
        public uint RankAfter { get; set; }

        public ulong ScoreBefore { get; set; }
        public ulong ScoreAfter { get; set; }

        public uint ComboBefore { get; set; }
        public uint ComboAfter { get; set; }

        public float AccuracyBefore { get; set; }
        public float AccuracyAfter { get; set; }

        public ushort PpBefore { get; set; }
        public ushort PpAfter { get; set; }

        public override string ToString()
        {
            return "chartId:beatmap|" +
                   $"chartUrl:{Url}|" +
                   $"chartName:{Name}|" +
                   $"rankBefore:{RankBefore}|" +
                   $"rankAfter:{RankAfter}|" +
                   $"ranked_scoreBefore:{ScoreBefore}|" +
                   $"ranked_scoreAfter:{ScoreAfter}|" +
                   $"total_scoreBefore:{ScoreBefore}|" +
                   $"total_scoreAfter:{ScoreAfter}|" +
                   $"max_comboBefore:{ComboBefore}|" +
                   $"max_comboAfter:{ComboAfter}|" +
                   $"accuracyBefore:{AccuracyBefore}|" +
                   $"accuracyAfter:{AccuracyAfter}|" +
                   $"ppBefore:{PpBefore}|" +
                   $"ppAfter:{PpAfter}|" +
                   "score_id:1\n";
        }
    }
}