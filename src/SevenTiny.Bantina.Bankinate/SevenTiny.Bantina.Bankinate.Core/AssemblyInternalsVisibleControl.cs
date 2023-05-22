using System.Runtime.CompilerServices;

//需要扩展的类型需要在此添加对应的程序集友元标识
[assembly: InternalsVisibleTo("SevenTiny.Bantina.Bankinate.Caching")]
[assembly: InternalsVisibleTo("SevenTiny.Bantina.Bankinate.Validation")]
[assembly: InternalsVisibleTo("SevenTiny.Bantina.Bankinate.MongoDb")]
[assembly: InternalsVisibleTo("SevenTiny.Bantina.Bankinate.MySql")]
[assembly: InternalsVisibleTo("SevenTiny.Bantina.Bankinate.SqlServer")]
[assembly: InternalsVisibleTo("Test.Core")]
namespace SevenTiny.Bantina.Bankinate.Core
{
    /// <summary>
    /// 内部程序集对外可见性控制专用，没有其他实际用途
    /// </summary>
    internal class AssemblyInternalsVisibleControl
    {
    }
}
