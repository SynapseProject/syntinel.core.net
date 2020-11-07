using System;

namespace Syntinel.Core
{
    public enum StatusType
    {
        New,
        Sent,
        Received,
        SentToResolver,
        InProgress,
        Completed,
        Error,
        Invalid
    }
}
