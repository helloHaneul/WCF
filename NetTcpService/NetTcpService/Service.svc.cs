using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NetTcpService
{
    //PerSession
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class Service : IService
    {
        private static Dictionary<string, IBroadcastorCallBack> clients = new Dictionary<string, IBroadcastorCallBack>();  // session ID, callback

        public string GetSessionId()
        {
            return OperationContext.Current.SessionId;
        }

        public void GetInService(string id)
        {
            OperationContext.Current.Channel.Closed += new EventHandler(Channel_Closed);
            OperationContext.Current.Channel.Faulted += new EventHandler(Channel_Faulted);

            IBroadcastorCallBack callback = OperationContext.Current.GetCallbackChannel<IBroadcastorCallBack>();

            if (!clients.Keys.Contains(id))
            {
                clients.Add(id, callback);
            }          
        }

        void Channel_Closed(object sender, EventArgs e)
        {
            IClientChannel clientChannel = sender as IClientChannel;
            Dispose(clientChannel.SessionId);
        }

        void Channel_Faulted(object sender, EventArgs e)
        {
            IClientChannel clientChannel = sender as IClientChannel;
            Dispose(clientChannel.SessionId);
        }

        public void Dispose(string id)
        {
            if (clients.Keys.Contains(id))
            {
                clients.Remove(id);
            }
        }

        public int GetCurrentClients()
        {
            return clients.Count;
        }

        public void Broadcast(string message)
        {
            foreach (var client in clients)
            {
                client.Value.SendMessage(message);
            }
        }
    }
}
