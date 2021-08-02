using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cringe.Types.Enums;

namespace Cringe.Types.Database
{
    // this has to exist because of EF one-model-one-table mapping
    public class SubmittedScore : ScoreBase
    {
    }
}
