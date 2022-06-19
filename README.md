# FunctionsDI
DI for functions


#Usage
```csharp
    public partial class TestDIFunction
    {
        public bool TestMyFunc1([FromServices] TestDI1 t1, [FromServices] TestDI2 t2, int x, int y)
        {
            return true;
        }
	}
```

generates 

```csharp

public partial class TestDIFunction
{ 
private TestDI1 _TestDI1;
private TestDI2 _TestDI2;
public TestDIFunction  

(TestDI1 _TestDI1,TestDI2 _TestDI2)
 { 
this._TestDI1=_TestDI1;
this._TestDI2=_TestDI2;

 } //end constructor 

//making call to TestMyFunc1
public bool TestMyFunc1(int  x,int  y){ 
var t1 = this._TestDI1  ;
if(t1 == null) throw new ArgumentException(" service TestDI1  is null in TestDIFunction ");
var t2 = this._TestDI2  ;
if(t2 == null) throw new ArgumentException(" service TestDI2  is null in TestDIFunction ");
return  TestMyFunc1(t1,t2,x,y);
}

```
