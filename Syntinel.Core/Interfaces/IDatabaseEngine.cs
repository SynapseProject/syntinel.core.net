using System;
using System.Collections.Generic;
namespace Syntinel.Core
{
    public interface IDatabaseEngine
    {
        T Get<T>(string id);
        T Create<T>(T record, bool failIfExists = false);
        T Update<T>(T record, bool failIfMissing = false);
        void Delete<T>(string id, bool failIfMissing = false);

        // Get/Delete Record By A Complex Primary Key
        T Get<T>(string[] id);
        void Delete<T>(string[] id, bool failIfMissing = false);

        // Import/Export All Records From A Table
        List<T> Export<T>();
        void Import<T>(List<T> records);
    }
}
