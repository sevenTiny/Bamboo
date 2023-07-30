using System.ComponentModel;

namespace Bamboo.ScriptEngine
{
    /// <summary>
    /// 脚本对应的编程语言
    /// </summary>
    public enum DynamicScriptLanguage
    {
        [Description("C#")]
        CSharp = 1,
        [Description("Java")]
        Java = 2,
        [Description("JavaScript")]
        JavaScript = 3,
        [Description("Python")]
        Python = 4,
        [Description("Golang")]
        Golang = 5,
        [Description("Ruby")]
        Ruby = 6,
        [Description("Php")]
        Php = 7,
        [Description("C")]
        C = 8,
        [Description("C++")]
        CPP = 9,
        [Description("R")]
        R = 10,
        [Description("VisualBasic")]
        VB = 11,
        [Description("Scala")]
        Scala = 12,
        [Description("Shell")]
        Shell = 13,
        [Description("Delphi")]
        Delphi = 14,
        [Description("Fortran")]
        Fortran = 15,
        [Description("Pascal")]
        Pascal = 16,
        [Description("Json")]
        Json = 17,
        [Description("Xml")]
        Xml = 18,
    }
}
