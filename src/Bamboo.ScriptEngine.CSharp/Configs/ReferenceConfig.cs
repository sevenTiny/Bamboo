using System.Reflection;

namespace Bamboo.ScriptEngine.CSharp.Configs
{
    public class ReferenceConfig
    {
        /// <summary>
        /// 运行时动态注入的程序集
        /// </summary>
        public Assembly[] RuntimeDependentAssemblies { get; set; }
    }
}
