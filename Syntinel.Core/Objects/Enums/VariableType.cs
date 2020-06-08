using System;

namespace Syntinel.Core
{
    public enum VariableType
    {
        number,         // Inputs : Numeric Value
        text,           // Inputs : Text Value
        boolean,        // Inputs : Boolean Value
        choice,         // Inputs and Actions : List of Options, Select One
        multichoice,    // Inputs and Actions : List of Options, Select One or More
        button,         // Actions : Submit
        link            // Actions : Open Hyperlink
    }
}
