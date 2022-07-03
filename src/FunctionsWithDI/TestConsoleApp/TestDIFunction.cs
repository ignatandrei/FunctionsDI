
using RSCG_FunctionsWithDI_Base;

namespace TestFunctionsWithDI
{
    public class TestDI1
    {
        public int x;
    }
    public class TestDI2
    {
        public int x;
    }
    public class TestDI3
    {
        public int x;
    }
    public partial class TestDIFunctionAdvWithConstructor
    {
        [RSCG_FunctionsWithDI_Base.FromServices]
        private TestDI1 NewTestDI1;

        [RSCG_FunctionsWithDI_Base.FromServices]
        private TestDI2 NewTestDI2 { get; set; }

        private readonly TestDI3 myTestDI3;

        public TestDIFunctionAdvWithConstructor(TestDI3 test)
        {
            myTestDI3= test;
        }
        
    }
    public partial class TestDIFunctionAdvNoConstructor
    {
        [RSCG_FunctionsWithDI_Base.FromServices]
        private TestDI1 NewTestDI1;

        [RSCG_FunctionsWithDI_Base.FromServices]
        private TestDI2 NewTestDI2 { get; set; }




    }
    public partial class TestDIFunction
    {


        public bool TestMyFunc1([FromServices] TestDI1 t1, [FromServices] TestDI2 t2, int x, int y)
        {
            return true;
        }
        public bool TestMyFunc2([FromServices] TestDI1 t12,  int x, int y)
        {
            return true;
        }
        public bool TestMyFunc3([FromServices] TestDI1 t15, [FromServices] TestDI2 t2,  int y)
        {
            return true;
        }
        public bool TestMyFunc4([FromServices] TestDI1 t1, [FromServices] TestDI2 t2)
        {
            return true;
        }

        public bool TestMyFunc5(int x, int y)
        {
            return true;
        }
        public void TestMyFunc6([FromServices] TestDI1 t1, [FromServices] TestDI2 t2)
        {
            return ;
        }
    }
}
