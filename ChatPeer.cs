using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.IO;
namespace ChatServer
{
    class ChatPeer : ClientPeer
    {
        public Guid Guid { get; private set; }

        private int maxGuessNumber;
        private int correctNumber;
        private int remainedChance;
        private bool alreadySendHelloWorld;
        public ChatPeer(InitRequest initRequest) : base(initRequest)
        {
            Guid = Guid.NewGuid();
        }
        private static event Action<ChatPeer, EventData, SendParameters> BroadcastMessage;
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            ChatServer.Server.ErasePeer(this);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (operationRequest.OperationCode == 1)
            {
                var pa = (string)operationRequest.Parameters[1];
                var parameter = new Dictionary<byte, object>()
                {
                    {(byte)1,pa }
                };
                var response = new OperationResponse(operationRequest.OperationCode,parameter);
                SendOperationResponse(response,new SendParameters());
                
            }

        }
        private void OnBroadcastMessage(ChatPeer peer, EventData eventData, SendParameters sendParameters)
        {
            if (peer != this) // do not send chat custom event to peer who called the chat custom operation 
            {
                SendEvent(eventData, sendParameters);
            }
        }

        public void RefreshGuid()
        {
            Guid = Guid.NewGuid();
        }
    }
}
