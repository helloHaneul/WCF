using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NetTcpService
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IBroadcastorCallBack))]
    public interface IService
    {
        [OperationContract]
        string GetSessionId();

        [OperationContract(IsOneWay = true)]    // 클라이언트로부터의 회신 기다리지 않음
        void GetInService(string id);

        [OperationContract(IsOneWay = true)]
        void Dispose(string id);

        [OperationContract(IsOneWay = true)]
        void Broadcast(string message);

        [OperationContract]
        int GetCurrentClients();
    }
    
    public interface IBroadcastorCallBack
    {
        [OperationContract(IsOneWay = true)]
        void SendMessage(string message);
    }
}
