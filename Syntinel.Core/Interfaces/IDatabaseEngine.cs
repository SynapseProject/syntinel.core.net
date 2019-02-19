using System;
namespace Syntinel.Core
{
    public interface IDatabaseEngine
    {
        T Get<T>(string id);
        T Create<T>(T record, bool failIfExists = false);
        T Update<T>(T record, bool failIfMissing = false);
    }
}
