using System;
using Banks.Entities;

namespace Banks.Builders
{
    public static class ClientEditor
    {
        public static void ChangeName(string name, Client client)
        {
            if (name is null)
                throw new ArgumentNullException("name");
            client.Name = name;
        }

        public static void ChangeSurname(string surname, Client client)
        {
            if (surname is null)
                throw new ArgumentNullException("surname");
            client.Surname = surname;
        }

        public static void ChangePassport(string passport, Client client)
        {
            if (passport is null)
                throw new ArgumentNullException("passport");
            client.Passport = passport;
        }

        public static void ChangeAddress(string address, Client client)
        {
            if (address is null)
                throw new ArgumentNullException("address");
            client.Address = address;
        }
    }
}