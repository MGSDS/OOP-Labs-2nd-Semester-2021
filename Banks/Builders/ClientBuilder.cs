using System;
using Banks.Entities;

namespace Banks.Builders
{
    public class ClientBuilder
    {
        private Client _client;

        public void SetName(string name, string surname)
        {
            if (name is null && surname is null)
                throw new ArgumentNullException("name or surname");

            _client = new Client(name, surname);
        }

        public void SetId(string passport)
        {
            if (passport is null)
                throw new ArgumentNullException("passport");
            _client.Passport = passport;
        }

        public void SetAddress(string address)
        {
            if (address is null)
                throw new ArgumentNullException("Address");
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