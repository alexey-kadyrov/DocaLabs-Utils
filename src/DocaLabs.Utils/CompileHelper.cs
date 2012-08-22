using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DocaLabs.Utils
{
    public class CompileHelper
    {
        public const string CSharp = "csharp";

        public string Language { get; set; }

        public bool GenerateInMemory { get; set; }

        /// <summary>
        /// Gets the list of assemblies which will be added as references to the compiled code.
        /// </summary>
        public HashSet<string> ReferencedAssemblies { get; private set; }

        public CompileHelper()
        {
            Language = CSharp;
            GenerateInMemory = true;
            ReferencedAssemblies = new HashSet<string>();
        }

        public Assembly Compile(params string[] sources)
        {
            var provider = CodeDomProvider.CreateProvider(Language);

            var results = provider.CompileAssemblyFromSource(InitializeCompilerParameters(), sources);

            ValidateCompilationResults(results);

            return results.CompiledAssembly;
        }

        public static IEnumerable<string> GetLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic)
                .Select(GetAssemblyLocation)
                .Where(x => x != null);
        }

        CompilerParameters InitializeCompilerParameters()
        {
            var options = new CompilerParameters
            {
                CompilerOptions = "/target:library /optimize",
                TreatWarningsAsErrors = true,
                GenerateExecutable = false,
                GenerateInMemory = GenerateInMemory,
                IncludeDebugInformation = false
            };

            foreach (var referencedAssembly in ReferencedAssemblies)
                options.ReferencedAssemblies.Add(referencedAssembly);

            return options;
        }

        static string GetAssemblyLocation(Assembly assembly)
        {
            if (assembly.GlobalAssemblyCache)
                return assembly.ManifestModule.FullyQualifiedName;
            
            return File.Exists(assembly.Location) 
                ? assembly.Location 
                : null;
        }

        static void ValidateCompilationResults(CompilerResults results)
        {
            if (results.Errors.HasErrors)
            {
                // If error throw ex with compiler errors in ex.Data
                var e = new InvalidOperationException(Resources.Text.compile_errors);
                e.Data["compilerErrors"] = results.Errors;
                throw e;
            }

            try
            {
                if (results.CompiledAssembly.GetTypes().Length == 0)
                    throw new InvalidOperationException(Resources.Text.no_compiled_type);
            }
            catch(Exception e)
            {
                throw new InvalidOperationException(Resources.Text.no_compiled_type, e);
            }
        }
    }
}
