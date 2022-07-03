// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using TestFunctionsWithDI;

Console.WriteLine("Hello, World!");
var services = new ServiceCollection();
services.AddSingleton<TestDIFunction>();
services.AddSingleton<TestDI1>();
services.AddSingleton<TestDI2>();
services.AddSingleton<TestDI3>();
services.AddSingleton<TestDIFunctionAdvWithConstructor>();
services.AddSingleton<TestDIFunctionAdvNoConstructor>();
var serviceProvider = services.BuildServiceProvider();
var test=serviceProvider.GetService<TestDIFunction>();
Console.WriteLine(test.TestMyFunc1(10,3));
Console.WriteLine(test.TestMyFunc2(10,2));
Console.WriteLine(test.TestMyFunc3(10));
Console.WriteLine(test.TestMyFunc4());
Console.WriteLine(test.TestMyFunc5(1,3));
test.TestMyFunc6();

var t = serviceProvider.GetService<TestDIFunctionAdvNoConstructor>();
Console.WriteLine(t.NewTestDI1.x);

var t2 = serviceProvider.GetService<TestDIFunctionAdvWithConstructor>();
Console.WriteLine(t2.myTestDI3.x);

Console.WriteLine(t2.NewTestDI2.x);
