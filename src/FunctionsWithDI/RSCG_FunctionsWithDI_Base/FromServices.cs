using System;

namespace RSCG_FunctionsWithDI_Base { 

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class FromServices: Attribute
    {

    }
}
