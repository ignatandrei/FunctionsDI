﻿using Microsoft.AspNetCore.Mvc;

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
    public partial class TestDIFunction
    {
        public bool TestMyFunc([FromServices] TestDI1 t1, [FromServices] TestDI2 t2, int x, int y)
        {
            return true;
        }
        public bool TestMyFunc2([FromServices] TestDI1 t1,  int x, int y)
        {
            return false;
        }
        public bool TestMyFunc3([FromServices] TestDI1 t1, [FromServices] TestDI2 t2,  int y)
        {
            return false;
        }
        public bool TestMyFunc4([FromServices] TestDI1 t1, [FromServices] TestDI2 t2)
        {
            return false;
        }
        public bool TestMyFunc5(int x, int y)
        {
            return false;
        }
    }
}
