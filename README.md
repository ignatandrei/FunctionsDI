# FunctionsDI

[![Nuget](https://img.shields.io/nuget/v/RSCG_FunctionsWithDI)](https://www.nuget.org/packages/RSCG_FunctionsWithDI)

Generate (constructor) and functions calls similar with ASP.NET Core WebAPI ( [FromServices] will be provided by DI )
Also, verifies for null  .

# Usage

Reference into the csproj

```xml
<ItemGroup>
    <PackageReference Include="RSCG_FunctionsWithDI" Version="2022.6.19.1605" ReferenceOutputAssembly="false" OutputItemType="Analyzer" />
    <PackageReference Include="RSCG_FunctionsWithDI_Base" Version="2022.6.19.1605" />
</ItemGroup>	
```

Then for every class you can write [FromServices]

```csharp
using RSCG_FunctionsWithDI_Base;
//namespace if necessary
    public partial class TestDIFunction
    {
        public bool TestMyFunc1([FromServices] TestDI1 t1, [FromServices] TestDI2 t2, int x, int y)
        {
            return true;
        }
	}
```

generates the constructor with needed details 

```csharp

public partial class TestDIFunction
{ 
private TestDI1 _TestDI1;
private TestDI2 _TestDI2;
public TestDIFunction  (TestDI1 _TestDI1,TestDI2 _TestDI2) //constructor generated with needed DI
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

so you can call 
```csharp

var test=serviceProvider.GetService<TestDIFunction>();
Console.WriteLine(test.TestMyFunc1(10,3)); // calling without the [FromServices] arguments

```