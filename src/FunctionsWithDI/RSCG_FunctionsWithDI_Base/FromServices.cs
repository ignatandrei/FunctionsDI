using System;

namespace RSCG_FunctionsWithDI_Base { 

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class FromServices: Attribute
    {

    }
}
