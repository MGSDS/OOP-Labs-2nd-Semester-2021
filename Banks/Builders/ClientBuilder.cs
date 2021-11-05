using System;
using Banks.Entities;

namespace Banks.Builders
{
    public class ClientBuilder
    {
        private Client _client;

        public void SetName(string name, string surname)
        {
            _client = new Client(name, surname);
        }

        public void SetId(string passport)
        {
            _client.Passport = passport;
        }

        public void SetAddress(string address)
        {
            _client.Address = address;
        }

        public void Reset()
        {
            _client = null;
        }

        public Client Build()
        {
            if (_client is null)
                throw new InvalidOperationException("Client have no name");
            Client client = _client;
            Reset();
            return client;
        }
    }
}