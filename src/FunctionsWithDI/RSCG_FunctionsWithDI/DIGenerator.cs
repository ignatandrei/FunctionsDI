using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RSCG_FunctionsWithDI_Base;
using System;
using System.Collections.Immutable;

namespace RSCG_FunctionsWithDI
{
    [Generator]
    public class DIGenerator : IIncrementalGenerator
    {
        static string nameAttr = nameof(FromServices);
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<MethodDeclarationSyntax> paramDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s), 
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) 
            .Where(static m => m is not null)!; // filter out attributed enums that we don't care about

            IncrementalValueProvider<(Compilation, ImmutableArray<MethodDeclarationSyntax>)> compilationAndEnums = context.CompilationProvider.Combine(paramDeclarations.Collect());

            context.RegisterSourceOutput(compilationAndEnums,
            static (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        private static void Execute(Compilation compilation, ImmutableArray<MethodDeclarationSyntax> methods, SourceProductionContext context)
        {
            var mets = methods.Distinct().ToArray();
            var x = mets.Length;
            
            x++;
            Dictionary<ClassDeclarationSyntax, List<MethodDeclarationSyntax>> data = new();
            List<ParameterSyntax> paramsDI = new();
            
            foreach(var mds in mets)
            {
                var p = mds.Parent;
                while(p as ClassDeclarationSyntax is null)
                {
                    p = p?.Parent;
                    if (p== null)//something wrong
                        break;
                }
                var cds = p as ClassDeclarationSyntax;
                if (!data.ContainsKey(cds))
                {
                    data.Add(cds, new());
                }
                data[cds].Add(mds);

                var parameters = mds.ParameterList.Parameters;
                foreach(var ps in parameters)
                {

                    if (!IsForDI(ps))
                        continue;

                    paramsDI.Add(ps);

                }
            }
            var nl = Environment.NewLine;
            foreach(var cds in data){
                //spc.AddSource($"{cds.Key. })
                
                var c = cds.Key;
                var nameClass = c.Identifier.Text;
                var p = c.Parent;
                var namespaceClass = "";
                while (true)
                {
                    if(p is BaseNamespaceDeclarationSyntax bnsds)
                    {
                        namespaceClass = bnsds.Name.ToFullString();
                        break;
                    }
                    p = p.Parent;
                }

                var str = "";
                str += $"namespace {namespaceClass}{nl}";
                str += $"{{ {nl}";
                str +=$"public partial class {nameClass}{nl}";
                str += $"{{ {nl}";

                var types = paramsDI.Select(it => it.Type).ToArray();
                var arr = types
                    .Select(it => it as IdentifierNameSyntax)
                    .Where(it => it != null)
                    .Select(it => it.Identifier)
                    .Where(it => it != null)
                    .Select(it => it.Text)
                    .Distinct()
                    .Select(it => new { type = it, name = "_" + it })
                    .ToArray();
                foreach(var item in arr)
                {
                    str += $"private {item.type} {item.name};{nl}";
                }
                str += $"public {nameClass}  {nl}";
                //ctor vs services passing to ctor
                //dictionary of dependencies with enum

                var paramsConstructor = string.Join(",", arr.Select(it => it.type +" _"+ it.name).ToArray());
                str += $"{nl}({paramsConstructor}){nl} {{ {nl}";

                foreach (var item in arr)
                {
                    str += $"this.{item.name}={item.name};{nl}";
                }
                str += $"{nl} }} //end constructor {nl}";
                foreach (var mds in cds.Value)
                {
                    string nameMethod = c.Identifier.Text; 
                    str += $"{nl}//making call to {nameMethod}";
                }
                str += $"{nl} }}//class";
                str += $"{nl} }}//namespace";
                context.AddSource($"{nameClass}_gen", str);

            }

        }

        private static MethodDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext ctx)
        {
            var parameter = ctx.Node as ParameterSyntax;
            //todo: verify constructor
            var parent = parameter.Parent;
            while(parent as MethodDeclarationSyntax is null)
            {
                parent = parent?.Parent;
                if (parent == null)//something wrong
                    break;
            }
            return parent as MethodDeclarationSyntax;
            

        }
        private static bool IsForDI(ParameterSyntax parameter)
        {
            if (parameter == null)
                return false;

            var hasAttr = parameter.AttributeLists.Count > 0;

            if (!hasAttr)
                return false;

            hasAttr = false;
            foreach (var attr in parameter!.AttributeLists)
            {
                if (attr.ToFullString().Contains(nameAttr))
                {
                    hasAttr = true;

                }
            }
            if (!hasAttr)
                return false;

            return true;

        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode s)
        {
            
            return IsForDI(s as ParameterSyntax);

        }
    }
}
