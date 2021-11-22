using System;
using BackupsExtra.ClearAlgorithms.Proxies;

namespace BackupsExtra.ClearAlgorithms
{
    public class ClearAlgorithmBuilder
    {
        private IClearAlgorithm _clearAlgorithm;

        public ClearAlgorithmBuilder()
        {
            _clearAlgorithm = new BaseClearAlgorithm();
        }

        public void AddMaxTime(TimeSpan maxAge)
        {
            _clearAlgorithm = new DateClearAlgorithmProxy(_clearAlgorithm, maxAge);
        }

        public void AddMaxCount(ushort count)
        {
            _clearAlgorithm = new CountClearAlgorithmProxy(_clearAlgorithm, count);
        }

        public void Clear()
        {
            _clearAlgorithm = new BaseClearAlgorithm();
        }

        public IClearAlgorithm Build()
        {
            IClearAlgorithm result = _clearAlgorithm;
            _clearAlgorithm = null;
            Clear();
            return result;
        }
    }
}