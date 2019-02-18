using System;

namespace Syntinel.Core
{
    public enum VariableType
    {
        Number,         // Inputs : Numeric Value
        Text,           // Inputs : Text Value
        Boolean,        // Inputs : Boolean Value
        Choice,         // Inputs and Actions : List of Options
        Button,         // Actions : Submit
        Link            // Actions : Open Hyperlink
    }
}
