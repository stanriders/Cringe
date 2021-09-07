using System.Threading.Tasks;

namespace Cringe.Bancho.Services.Bots
{
    public interface IBotService
    {
        public Task Invoke<T>(string path, T data);
    }
}
