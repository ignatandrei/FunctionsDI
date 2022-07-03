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

            CreateForMethods(context);
            CreateForClass(context);


        }
        private void CreateForClass(IncrementalGeneratorInitializationContext context)
        {
            var paramDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGenerationClass(s),
                transform: static (ctx, _) => GetSemanticTargetForGenerationClass(ctx))
            .Where(static m => m is not null)!; 

            var compilationAndEnums = context.CompilationProvider.Combine(paramDeclarations.Collect());

            context.RegisterSourceOutput(compilationAndEnums,
                static (spc, source) => ExecuteForClass(source.Item1, source.Item2, spc));

        }

        private static void ExecuteForClass(Compilation item1, ImmutableArray<ClassDeclarationSyntax> cdsArr, SourceProductionContext context)
        {
            Dictionary<ClassDeclarationSyntax, List<MemberDeclarationSyntax>> data = new();
            foreach (var cds in cdsArr)
            {
                //find the constructor , as any
                var existsConstructor = 0;
                ConstructorDeclarationSyntax? constructor= null;
                data.Add(cds, new List<MemberDeclarationSyntax>());
                foreach (var child in cds.ChildNodes())
                {
                    if(child is ConstructorDeclarationSyntax cdsChild)
                    {
                        existsConstructor++;
                        constructor = cdsChild;
                        continue;
                    }
                    if(child is FieldDeclarationSyntax fds)
                    {
                        if (!IsForDI(fds))
                            continue;
                        data[cds].Add(fds);
                    }
                    if (child is PropertyDeclarationSyntax mds)
                    {
                        if (!IsForDI(mds))
                            continue;
                        data[cds].Add(mds);
                    }
                }

                if (existsConstructor > 1)
                {
                    constructor = null;
                    existsConstructor = 0;
                }

                var (nameClass, namespaceClass) = NameAndNameSpace(cds);
                var nl = Environment.NewLine;

                var str = "";
                str += $"namespace {namespaceClass}{nl}";
                str += $"{{ {nl}";
                str += $"public partial class {nameClass}{nl}";
                str += $"{{ {nl}";
                if (existsConstructor == 1)
                {

                }
                else
                {

                }
                str += $"{nl} }}//class";
                str += $"{nl} }}//namespace";
                context.AddSource($"{nameClass}_gen_class", str);

            }
        }

        private void CreateForMethods(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<MethodDeclarationSyntax> paramDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGenerationParameter(s),
                transform: static (ctx, _) => GetSemanticTargetForGenerationMethods(ctx))
            .Where(static m => m is not null)!; 

            IncrementalValueProvider<(Compilation, ImmutableArray<MethodDeclarationSyntax>)> compilationAndEnums = context.CompilationProvider.Combine(paramDeclarations.Collect());

            context.RegisterSourceOutput(compilationAndEnums,
                static (spc, source) => Execute(source.Item1, source.Item2, spc));

        }
        private static Tuple<string,string> NameAndNameSpace(ClassDeclarationSyntax c)
        {
            var nameClass = c.Identifier.Text;
            var p = c.Parent;
            var namespaceClass = "";
            while (true)
            {
                if (p is BaseNamespaceDeclarationSyntax bnsds)
                {
                    namespaceClass = bnsds.Name.ToFullString();
                    break;
                }
                p = p?.Parent;
                if (p == null)
                    break;
            }
            return Tuple.Create(nameClass, namespaceClass);
        }
        private static void Execute(Compilation compilation, ImmutableArray<MethodDeclarationSyntax> methods, SourceProductionContext context)
        {
            var mets = methods.Distinct().ToArray();
            var x = mets.Length;
            
            x++;
            Dictionary<ClassDeclarationSyntax, List<MethodDeclarationSyntax>> data = new();
            List<ParameterSyntax> paramsDIAll = new();
            
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

                    paramsDIAll.Add(ps);

                }
            }
            var nl = Environment.NewLine;
            foreach(var cds in data){
                //spc.AddSource($"{cds.Key. })
                
                var c = cds.Key;
                var (nameClass, namespaceClass) = NameAndNameSpace(c);
                
                var str = "";
                str += $"namespace {namespaceClass}{nl}";
                str += $"{{ {nl}";
                str +=$"public partial class {nameClass}{nl}";
                str += $"{{ {nl}";

                var types = paramsDIAll.Select(it => it.Type).ToArray();
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

                var paramsConstructor = string.Join(",", arr.Select(it => it.type +" "+ it.name).ToArray());
                str += $"{nl}({paramsConstructor}){nl} {{ {nl}";

                foreach (var item in arr)
                {
                    str += $"this.{item.name}={item.name};{nl}";
                }
                str += $"{nl} }} //end constructor {nl}";
                foreach (var mds in cds.Value)
                {
                    string nameMethod = mds.Identifier.Text; 
                    str += $"{nl}//making call to {nameMethod}";
                    var ret = mds.ReturnType;
                    string? nameRet = "";
                    bool isVoid = false;
                    if (ret is PredefinedTypeSyntax pts)
                    {
                        isVoid = pts.Keyword.Text?.ToLower() == "void";
                        nameRet = pts.Keyword.Text;
                    };
                    
                    str += $"{nl}public {nameRet} {nameMethod}";
                    var parametersMethod = mds
                        .ParameterList
                        .Parameters;
                    var parametersDI = parametersMethod
                        .Where(it => paramsDIAll.Contains(it))
                        .Select(it => new { it.Identifier.Text, type = it.Type.ToFullString() })
                        .ToArray();


                    var parametersNoDI = parametersMethod
                        .Where(it => !paramsDIAll.Contains(it))
                        .Select(it => new { it.Identifier.Text, type = it.Type.ToFullString() })
                        .ToArray();
                    

                    var dataParams = string.Join(",", parametersNoDI.Select(it => $"{it.type} {it.Text}"));
                    str += $"({dataParams}){{ {nl}";
                    foreach(var item in parametersDI)
                    {
                        str += $"var {item.Text} = this._{item.type} ;{nl}";
                        str += $"if({item.Text} == null) throw new ArgumentException(\" service {item.type} is null in {nameClass} \");{nl}";
                    }
                    var returnString = isVoid ? " " : "return ";
                    var allArgs= parametersMethod
                        .Select(it => new { it.Identifier.Text, type = it.Type.ToFullString() })
                        .Select(it => $"{it.Text}")
                        .ToArray();
                    var allArgsString = string.Join(",", allArgs);
                    str += $"{returnString} {nameMethod}({allArgsString});{nl}";
                    str += $"}}{nl}";
                }
                str += $"{nl} }}//class";
                str += $"{nl} }}//namespace";
                context.AddSource($"{nameClass}_gen_methods", str);

            }

        }
        private static ClassDeclarationSyntax? GetSemanticTargetForGenerationClass(GeneratorSyntaxContext ctx)
        {
            var parameter = ctx.Node as MemberDeclarationSyntax;
            if (parameter == null)
                return null;
            //todo: verify constructor
            var parent = parameter.Parent;
            while (parent as ClassDeclarationSyntax is null)
            {
                parent = parent?.Parent;
                if (parent == null)//something wrong
                    break;
            }
            return parent as ClassDeclarationSyntax;


        }
        private static MethodDeclarationSyntax? GetSemanticTargetForGenerationMethods(GeneratorSyntaxContext ctx)
        {
            var parameter = ctx.Node as ParameterSyntax;
            if (parameter == null)
                return null;
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
        private static bool IsForDI(PropertyDeclarationSyntax parameter)
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
        private static bool IsForDI(FieldDeclarationSyntax parameter)
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
        private static bool IsSyntaxTargetForGenerationClass(SyntaxNode s)
        {
            var ret = false;
            if (s is PropertyDeclarationSyntax pds)
                ret = IsForDI(pds);

            if (ret)
                return true;

            if (s is FieldDeclarationSyntax f)
                ret = IsForDI(f);

            if (ret)
                return true;


            return false;
        }
        private static bool IsSyntaxTargetForGenerationParameter(SyntaxNode s)
        {
                return IsForDI(s as ParameterSyntax);


            
        }
    }
}
