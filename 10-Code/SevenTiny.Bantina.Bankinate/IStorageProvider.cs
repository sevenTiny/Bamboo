using System.Collections.Generic;

namespace SevenTiny.Bantina.Bankinate
{
    public interface IStorageProvider
    {
        void Add();
        bool Update();
        bool Delete();
        object Select(int id);
        IList<object> Select();
    }
}
