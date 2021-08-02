using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cringe.Database;
using Cringe.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cringe.Services
{
    public class ReplayStorage
    {
        private readonly string _cachePath;
        private readonly ScoreDatabaseContext _scoreDatabaseContext;
        private readonly BeatmapDatabaseContext _beatmapDatabaseContext;

        public ReplayStorage(IConfiguration configuration, ScoreDatabaseContext scoreDatabaseContext, BeatmapDatabaseContext beatmapDatabaseContext)
        {
            _scoreDatabaseContext = scoreDatabaseContext;
            _beatmapDatabaseContext = beatmapDatabaseContext;

            _cachePath = configuration["ReplayStoragePath"];
            if (!Directory.Exists(_cachePath))
                Directory.CreateDirectory(_cachePath);
        }

        public async Task<byte[]> GetReplay(int scoreId)
        {
            var filePath = Path.Combine(_cachePath, scoreId.ToString());

            if (!File.Exists(filePath))
                return null;

            return await ReconstructReplay(scoreId, filePath);
        }

        public async Task<byte[]> GetRawReplay(int scoreId)
        {
            var filePath = Path.Combine(_cachePath, scoreId.ToString());

            if (!File.Exists(filePath))
                return null;

            return await File.ReadAllBytesAsync(filePath);
        }

        public async Task SaveReplay(int scoreId, Stream replay)
        {
            var filePath = Path.Combine(_cachePath, scoreId.ToString());
            await using var file = File.OpenWrite(filePath);
            await replay.CopyToAsync(file);
        }

        private async Task<byte[]> ReconstructReplay(int scoreId, string filePath)
        {
            var score = await _scoreDatabaseContext.Scores.FirstOrDefaultAsync(x => x.Id == scoreId);

            if (score is null)
                return null;

            var beatmap = await _beatmapDatabaseContext.Beatmaps.Select(x=> new {x.Id, x.Md5}).FirstOrDefaultAsync(x => x.Id == score.BeatmapId);

            if (beatmap is null)
                return null;

            var scoreData = string.Format("{0}p{1}o{2}o{3}t{4}a{5}r{6}e{7}y{8}o{9}u{10}{11}{12}",
                score.Count100 + score.Count300, score.Count50,
                score.CountGeki, score.CountKatu, score.CountMiss,
                beatmap.Md5, score.MaxCombo,
                score.FullCombo ? "True" : "False",
                score.PlayerUsername, score.Score, score.Rank,
                (int)score.Mods, "True");

            using var md5 = MD5.Create();
            var hash = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(scoreData)))
                .Replace("-", string.Empty)
                .ToLowerInvariant();

            await using var stream = new MemoryStream();

            await stream.WriteAsync(new[] { (byte) score.GameMode });
            await stream.WriteAsync(BitConverter.GetBytes(20210728));
            await stream.WriteAsync(PackString(beatmap.Md5));
            await stream.WriteAsync(PackString(score.PlayerUsername));
            await stream.WriteAsync(PackString(hash));
            await stream.WriteAsync(BitConverter.GetBytes((ushort) score.Count300));
            await stream.WriteAsync(BitConverter.GetBytes((ushort) score.Count100));
            await stream.WriteAsync(BitConverter.GetBytes((ushort) score.Count50));
            await stream.WriteAsync(BitConverter.GetBytes((ushort) score.CountGeki));
            await stream.WriteAsync(BitConverter.GetBytes((ushort) score.CountKatu));
            await stream.WriteAsync(BitConverter.GetBytes((ushort) score.CountMiss));
            await stream.WriteAsync(BitConverter.GetBytes((int) score.Score));
            await stream.WriteAsync(BitConverter.GetBytes((ushort) score.MaxCombo));
            await stream.WriteAsync(new[] { score.FullCombo ? (byte) 1 : (byte) 0});
            await stream.WriteAsync(BitConverter.GetBytes((int) score.Mods));
            await stream.WriteAsync(new byte[] { 0x00 });
            await stream.WriteAsync(BitConverter.GetBytes(score.PlayDateTime.Ticks));
            await stream.WriteAsync(await PackReplay(filePath));
            await stream.WriteAsync(BitConverter.GetBytes((long) scoreId));
            await stream.WriteAsync(BitConverter.GetBytes(0));

            return stream.ToArray();
        }

        private static byte[] PackString(string data)
        {
            using var stream = new MemoryStream();
            stream.WriteByte(0x0B);
            stream.WriteLEB128Unsigned((ulong) Encoding.GetEncoding(28591).GetByteCount(data)); // 28591 is latin1
            stream.Write(Encoding.GetEncoding(28591).GetBytes(data));

            return stream.ToArray();
        }

        private static async Task<byte[]> PackReplay(string filePath)
        {
            await using var file = File.OpenRead(filePath);
            await using var stream = new MemoryStream();
            await stream.WriteAsync(BitConverter.GetBytes((uint) file.Length));
            await file.CopyToAsync(stream);

            return stream.ToArray();
        }
    }
}
