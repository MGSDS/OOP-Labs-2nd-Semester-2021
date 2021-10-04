using System.Collections.Generic;

namespace IsuExtra.Entities
{
    public class Department
    {
        private List<Ognp> _ognps;
        public Department(char codeLetter, string name)
        {
            CodeLetter = codeLetter;
            Name = name;
            _ognps = new List<Ognp>();
        }

        public string Name { get; }
        public char CodeLetter { get; }
        public IReadOnlyList<Ognp> Ognps => _ognps;

        internal Ognp AddOgnp(string name)
        {
            var ognp = new Ognp(name, this);
            _ognps.Add(ognp);
            return ognp;
        }
    }
}