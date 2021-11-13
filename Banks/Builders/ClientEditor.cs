using System;
using System.Collections.Generic;
using Banks.Entities;

namespace Banks.Builders
{
    public class ClientEditor
    {
        private Client _client;
        private List<Action> _actions;
        public ClientEditor(Client client)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            _actions = new List<Action>();
            _client = client;
        }

        public void ChangeName(string name)
        {
            if (name is null)
                throw new ArgumentNullException("name");
            _actions.Add(() => _client.Name = name);
        }

        public void ChangeSurname(string surname)
        {
            if (surname is null)
                throw new ArgumentNullException("surname");
            _actions.Add(() => _client.Surname = surname);
        }

        public void ChangePassport(string passport)
        {
            if (passport is null)
                throw new ArgumentNullException("passport");
            _actions.Add(() => _client.Passport = passport);
        }

        public void ChangeAddress(string address)
        {
            if (address is null)
                throw new ArgumentNullException("address");
            _actions.Add(() => _client.Address = address);
        }

        internal void ApplyChanges()
        {
            foreach (Action action in _actions)
            {
                action.Invoke();
            }
        }
    }
}