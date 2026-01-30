using System;

namespace Masked
{
    public interface IData
    {
        bool Changed { get; }
        void ResetChanged();
    }

    [Serializable]
    public class Data : IData
    {
        protected bool _changed;

        public bool Changed => _changed;

        void IData.ResetChanged()
        {
            _changed = false;
        }
    }
}
