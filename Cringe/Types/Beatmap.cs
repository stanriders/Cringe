﻿using System.ComponentModel.DataAnnotations;

namespace Cringe.Types
{
    public class Beatmap
    {
        [Key] public int Id { get; set; }

        public string Md5 { get; set; }

        public bool Ranked { get; set; }
    }
}