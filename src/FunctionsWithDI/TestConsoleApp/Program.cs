// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using TestFunctionsWithDI;

Console.WriteLine("Hello, World!");
var services = new ServiceCollection();
services.AddScoped<TestDIFunction>();
services.AddScoped<TestDI1>();
services.AddScoped<TestDI2>();
var b = services.BuildServiceProvider();
var test=b.GetService<TestDIFunction>();
test.TestMyFunc6();
Console.WriteLine(test.TestMyFunc3(10));