
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
    public partial class TestDIFunctionAdv
    {
        [RSCG_FunctionsWithDI_Base.FromServices]
        private TestDI3 NewTestDI3;

        [RSCG_FunctionsWithDI_Base.FromServices]
        private TestDI2 NewTestDI2 { get; set; }

        private readonly TestDI3 myTestDI3;
        protected TestDIFunctionAdv(TestDI3 myTestDI3)
        {
            this.myTestDI3 = myTestDI3;
        }

        
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
