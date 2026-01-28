using System;

namespace Masked
{
    public interface IData
    {
        bool Changed { get; }
    }

    [Serializable]
    public class Data : IData
    {
        protected bool _changed;

        public bool Changed => _changed;
    }
}
