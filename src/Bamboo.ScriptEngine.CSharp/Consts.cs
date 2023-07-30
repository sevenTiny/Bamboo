namespace Bamboo.ScriptEngine.CSharp
{
    internal static class Consts
    {
        /// <summary>
        /// dll输出目录
        /// </summary>
        public const string BambooScriptEngineCompileOutPut = "BambooScriptEngineCompileOutPut";
        /// <summary>
        /// 包下载目录
        /// </summary>
        public const string BambooScriptEnginePackageDownload = "BambooScriptEnginePackageDownload";
        public const string AssemblyScriptKey = "GeneratedAssembly_{0}_{1}_{2}";
        /// <summary>
        /// 从Nuget包中找对应版本的dll的匹配顺序
        /// 例如：如果netstandard2版本的dll已经匹配成功，就不会继续匹配netstandard3和net5版本
        /// </summary>
        public static string[] MatchDllVersionFromNugetPackageSequnce = new string[] { "lib/netstandard2", "lib/netstandard3", "lib/net5" };
    }
}
