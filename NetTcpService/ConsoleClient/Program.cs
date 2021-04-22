using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    public class BroadcastCallback : BroadcastService.IServiceCallback
    {
        private System.Threading.SynchronizationContext syncContext = AsyncOperationManager.SynchronizationContext;

        public EventHandler _callbackHandler;

        private void OnReceived(object message)
        {
            this._callbackHandler.Invoke(message, null);
        }

        public void SendMessage(string message)
        {
            syncContext.Post(new System.Threading.SendOrPostCallback(OnReceived), message);
        }
    }

    class Program
    {
        static void event_reached(object sender, EventArgs e)
        {
            string message = sender.ToString();
            
            Console.WriteLine();
            Console.WriteLine("!!!!!!! Callback from server !!!!!!!");
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }

        static void Main(string[] args)
        {
            BroadcastCallback callback = new BroadcastCallback();
            callback._callbackHandler += event_reached;

            InstanceContext context = new InstanceContext(callback);

            BroadcastService.ServiceClient service = new BroadcastService.ServiceClient(context);

            string id = service.GetSessionId();

            Console.WriteLine("Current session id : " + id);

            service.GetInService(id);

            int num = service.GetCurrentClients();

            Console.WriteLine("Current clients : " + num);

            Console.Write("If you want to send message, please insert 'go' >> ");

            string s = Console.ReadLine();

            if (s.Equals("go"))
            {
                Console.Write("Send message >> ");
                string msg = Console.ReadLine();
                service.Broadcast(msg);
            }

            service.Dispose(id);
        }
    }
}
