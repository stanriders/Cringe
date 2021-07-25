using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Cringe.Services
{
    public class ReplayStorage
    {
        private readonly string _cachePath;
        private readonly IConfiguration _configuration;

        public ReplayStorage(IConfiguration configuration)
        {
            _configuration = configuration;

            _cachePath = _configuration["ReplayStoragePath"];
            if (!Directory.Exists(_cachePath))
                Directory.CreateDirectory(_cachePath);
        }

        public Stream GetReplay(int scoreId)
        {
            var filePath = Path.Combine(_cachePath, scoreId.ToString());

            if (!File.Exists(filePath))
                return null;

            return ReconstructReplay(scoreId, filePath);
        }

        public async Task SaveReplay(int scoreId, Stream replay)
        {
            var filePath = Path.Combine(_cachePath, scoreId.ToString());
            await using var file = File.OpenWrite(filePath);
            await replay.CopyToAsync(file);
        }

        private Stream ReconstructReplay(int scoreId, string filePath)
        {
            // TODO: all this fuckery

            /*
             magicHash = generalUtils.stringMd5(
                "{}p{}o{}o{}t{}a{}r{}e{}y{}o{}u{}{}{}".format(
                    int(scoreData["100_count"]) + int(scoreData["300_count"]),
                    scoreData["50_count"],
                    scoreData["gekis_count"],
                    scoreData["katus_count"],
                    scoreData["misses_count"],
                    scoreData["beatmap_md5"],
                    scoreData["max_combo"],
                    "True" if int(scoreData["full_combo"]) == 1 else "False",
                    scoreData["username"],
                    scoreData["score"],
                    rank,
                    scoreData["mods"],
                    "True"
                )
            )
            # Add headers (convert to full replay)
            fullReplay = binaryHelper.binaryWrite([
                [scoreData["play_mode"], dataTypes.byte],
                [20150414, dataTypes.uInt32],
                [scoreData["beatmap_md5"], dataTypes.string],
                [scoreData["username"], dataTypes.string],
                [magicHash, dataTypes.string],
                [scoreData["300_count"], dataTypes.uInt16],
                [scoreData["100_count"], dataTypes.uInt16],
                [scoreData["50_count"], dataTypes.uInt16],
                [scoreData["gekis_count"], dataTypes.uInt16],
                [scoreData["katus_count"], dataTypes.uInt16],
                [scoreData["misses_count"], dataTypes.uInt16],
                [scoreData["score"], dataTypes.uInt32],
                [scoreData["max_combo"], dataTypes.uInt16],
                [scoreData["full_combo"], dataTypes.byte],
                [scoreData["mods"], dataTypes.uInt32],
                [0, dataTypes.byte],
                [toDotTicks(int(scoreData["time"])), dataTypes.uInt64],
                [rawReplay, dataTypes.rawReplay],
                [0, dataTypes.uInt32],
                [0, dataTypes.uInt32],
            ])
             */

            return null;
        }
    }
}
