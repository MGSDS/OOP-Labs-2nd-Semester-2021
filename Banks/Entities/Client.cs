using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banks.Entities
{
    public class Client
    {
        public Client(string surname, string name)
        {
            Address = null;
            Passport = null;
            Surname = surname;
            Name = name;
        }

        internal Client()
        {
            Address = null;
            Passport = null;
        }

        public Guid Id { get; internal init; }
        public string Name { get; internal set; }
        public string Surname { get; internal set; }
        public string Passport { get; internal set; }
        public string Address { get; internal set; }

        [NotMapped]
        public bool Verified => Address is not null && Passport is not null;

        internal void Notify()
        {
            throw new NotImplementedException($"Client with id {Id} notified, but notification logic is not implemented");
        }
    }
}