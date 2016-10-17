using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ActionFilterMVC.Models
{
    public class SampleDBContext
    {
        private static List<ActionFilterMVC.Models.Client> Clients { get; set; }
        static SampleDBContext()
        {
            Clients = new List<Client>();
            Clients.Add(new Client() { ClientID = 1, Name = "Client 1", ClientNumber = "CLI-001" });
            Clients.Add(new Client() { ClientID = 2, Name = "Client 2", ClientNumber = "CLI-002" });
        }
        internal static IEnumerable<Client> GetClientList()
        {
            return Clients;
        }
        public static void Modify(Client client)
        {
            var localClient = Clients.FirstOrDefault(x => x.ClientID == client.ClientID);
            if (localClient == null) return;
            localClient.Country = client.Country;
            localClient.Email = client.Email;
            localClient.Name = client.Name;
            localClient.PostCode = client.PostCode;
            localClient.State = client.State;
            localClient.StreetAddress = client.StreetAddress;
        }
        internal static Client GetClient(string number)
        {
            return Clients.Find(x => x.ClientNumber == number);
        }
        internal static void AddClient(Client client)
        {
            client.ClientID = Clients.Max(x => x.ClientID) + 1;
            client.ClientNumber = "CLI-" + client.ClientID.ToString("000");
            Clients.Add(client);
        }
        internal static void RemoveClient(string number)
        {
            Clients.RemoveAll(x => x.ClientNumber == number);
        }
    }
}
