using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ATMService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IATMService" in both code and config file together.
    [ServiceContract]
    public interface IATMService
    {
        [OperationContract]
        RequestResponse Balance(int AccountNumber);

        [OperationContract]
        RequestResponse Deposit(int AccountNumber, decimal Amount, string Currency);

        [OperationContract]
        RequestResponse Withdraw(int AccountNumber, decimal Amount, string Currency);

        [OperationContract]
        List<Account> LoadAccounts();

        // TODO: Add your service operations here
    }

    [DataContract]
    public class Account
    {
        [DataMember]
        public int AccountNumber { get; set; }

        [DataMember]
        public decimal Balance { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public bool IsProcessing { get; set; }
    }

    [DataContract]
    public class RequestResponse
    {
        [DataMember]
        public int AccountNumber { get; set; }

        [DataMember]
        public bool Successful { get; set; }

        [DataMember]
        public decimal Balance { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
