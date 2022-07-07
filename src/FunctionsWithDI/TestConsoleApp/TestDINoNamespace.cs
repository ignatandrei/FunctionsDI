using Microsoft.Extensions.Logging;
using TestFunctionsWithDI;

public partial class TestDINoNamespace
{
    [RSCG_FunctionsWithDI_Base.FromServices]
    private readonly ILogger<TestDINoNamespace>? myLogger;
    
    private TestDI1 NewTestDI1;

    public TestDINoNamespace(TestDI1 ab)
    {
        this.NewTestDI1=ab;

    }
}
