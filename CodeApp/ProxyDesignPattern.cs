using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeApp
{

    interface IVideoService
    {
        string DownloadVideo(string id);
        string PlayVideo(string id);
    }

    class VLCVideoService : IVideoService
    {
        Dictionary<string, string> db;
        public string DownloadVideo(string id)
        {
            db.TryGetValue(id, out string val);
            return val;
        }

        public string PlayVideo(string id)
        {
            throw new NotImplementedException();
        }
    }



    internal class ProxyDesignPattern
    {
    }
}
