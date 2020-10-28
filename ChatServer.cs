using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net.Config;
using System.IO;
namespace ChatServer
{
    class ChatServer : ApplicationBase
    {
        public static ILogger Log { get; }
        public static ChatServer Server { get; private set; }

        private Dictionary<Guid, ChatPeer> peerDictionary;

        static ChatServer()
        {
            Log = LogManager.GetCurrentClassLogger();
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            var peer = new ChatPeer(initRequest);
            while (peerDictionary.ContainsKey(peer.Guid))
            {
                peer.RefreshGuid();
            }
            peerDictionary.Add(peer.Guid, peer);
            return peer;
        }

        protected override void Setup()
        {
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationPath, "log");
            FileInfo file = new FileInfo(Path.Combine(this.BinaryPath, "log4net.config"));
            if (file.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(file);

                peerDictionary = new Dictionary<Guid, ChatPeer>();
                Server = this;

                Log.Info("Server Setup Successiful.......");
            }
        }

        protected override void TearDown()
        {
        }
        public void ErasePeer(ChatPeer peer)
        {
            if (peerDictionary.ContainsKey(peer.Guid))
            {
                peerDictionary.Remove(peer.Guid);
            }
        }
    }
}
