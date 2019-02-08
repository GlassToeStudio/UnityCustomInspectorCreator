using System;
using System.Linq;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection.Emit;

namespace GTS.InspectorGeneration.Utilities
{
    public class Compiler
    {
        /// <summary>
        /// Attempt to get the Type from the passed in string typeName.
        /// </summary>
        public Type GetTypeFromString(string typeName)
        {
            MessageLogger.LogType(typeName);
            Type t = Type.GetType(typeName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            MessageLogger.LogType(t.ToString());
            return t;
        }

        /// <summary>
        /// Attempt to compile a csharp script (.cs) at the given path.
        /// <para>Succesfull compilation will result in the return of the class's Type, else return null.</para>
        /// </summary>
        public Type GetTypeForCompiledClassAtPath(string path)
        {
            CompilerParameters parameters = CreateCompileParameters();

            CompilerResults results = LoadAndCompile(path, parameters);

            if (results.Errors.HasErrors)
            {
                foreach (var error in results.Errors)
                {
                    MessageLogger.ErrorCompileErrors(error);
                }

                MessageLogger.ErrorFailedToCompileClassAt(path);
                return null;
            }

            var type = GetType(results);

            return type;
        }

        private CompilerParameters CreateCompileParameters()
        {
            CompilerParameters parameters = new CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = true
            };

            // Add all referenced assemblies
            parameters.ReferencedAssemblies.AddRange(
                AppDomain.CurrentDomain.GetAssemblies()
                .Where(item => !(item.ManifestModule is ModuleBuilder))
                .Select(item => item.Location).ToArray());

            return parameters;
        }

        private CompilerResults LoadAndCompile(string path, CompilerParameters parameters)
        {
            var provider = new CSharpCodeProvider();
            CompilerResults results = provider.CompileAssemblyFromFile(parameters, path);

            return results;
        }

        private Type GetType(CompilerResults results)
        {
            var types = results.CompiledAssembly.GetTypes();
            // Only want the first type.
            // results.CompiledAssembly.GetType() returns a different Type.
            return types[0];
        }
    }
}
