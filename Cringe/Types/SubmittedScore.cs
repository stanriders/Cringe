using System;
using System.ComponentModel.DataAnnotations;

namespace Cringe.Types
{
    public class SubmittedScore
    {
        [Key] public int Id { get; set; }

        public string FileMd5 { get; set; }

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

        public DateTime PlayDateTime { get; set; }

        //self.calculateAccuracy()
        public string OsuVersion { get; set; }
        public bool Quit { get; set; }
        public bool Failed { get; set; }
        public double Pp { get; set; }

        public Player Player { get; set; }

        public override string ToString()
        {
            //18204696(scoreId)|sendlolipls(username)|167060(score/pp)|663(combo)|0(50)|6(100)|483(300)|0(miss)|6(katu)|79(geki)|False(fc)|216(mods)|101029(???)|4(rank)|1596985749(date)|1(??)
            return $"{Id}|{Player.Username}|{Score}|{MaxCombo}|{Count50}|{Count100}|{Count300}|{CountMiss}|{CountKatu}|{CountGeki}|{FullCombo}|{Mods}|1|1|{PlayDateTime:yyMMddhhmmss}|1\n";
            
        }
    }
}