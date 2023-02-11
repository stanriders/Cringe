using System;

namespace Cringe.Web.Services
{
    [Flags]
    public enum ScoreFlags : int
    {
        Clean                  = 0,
        Speed                  = 1 << 0,
        IncorrectMod           = 1 << 1,
        MultipleOsuClients     = 1 << 2,
        ChecksumFail           = 1 << 3,
        FlashlightChecksumFail = 1 << 4,
        OsuExecutableChecksum  = 1 << 5,
        MissingProcesslist     = 1 << 6,
        FlashlightImage        = 1 << 7,
        Spinner                = 1 << 8,
        TransparentWindow      = 1 << 9,
        FastPress              = 1 << 10,
        RawMouseDiscrepancy    = 1 << 11,
        RawKeyboardDiscrepancy = 1 << 12
    }

    public class FlagsService
    {
        public ScoreFlags DecodeFlags(string osuver)
        {
            // other servers count whitespaces as flags
            // we're _special_ since we store flags as some random chars in db probably due to sqlite or parsing bugging out
            // that's why we have to count amount of chars after the version but thankfully the version is always 8 chars long
            return (ScoreFlags) (osuver.Length - 8);
        }
    }
}
