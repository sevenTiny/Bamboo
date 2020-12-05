namespace Bamboo.Logging
{
    /// <summary>
    /// 常量
    /// </summary>
    internal static class LoggingConst
    {
        /// <summary>
        /// 默认应用名
        /// </summary>
        public const string DefaultAppName = "SevenTinyCloud";
        /// <summary>
        /// 配置文件内容
        /// </summary>
        public const string ConfigContent = @"
<log4net>
  <!--Console模式的输出配置-->
  <appender name=""Console"" type=""log4net.Appender.ConsoleAppender"">
    <layout type = ""log4net.Layout.PatternLayout"" >
      <!--Pattern to output the caller's file name and line number -->
      <conversionPattern value = ""%date [%thread] %5level %logger.%method [%line] - %property{scope} %property{test} MESSAGE: %message%newline %exception"" />
    </layout >
  </appender >

  <!--ConsoleAppender模式的输出配置-->
  <appender name=""ConsoleAppender"" type=""log4net.Appender.ManagedColoredConsoleAppender"">
    <mapping>
      <level value = ""ERROR"" />
      <foreColor value=""Red"" />
    </mapping>
    <mapping>
      <level value = ""WARN"" />
      <foreColor value=""Yellow"" />
    </mapping>
    <mapping>
      <level value = ""INFO"" />
      <foreColor value=""White"" />
    </mapping>
    <mapping>
      <level value = ""DEBUG"" />
      <foreColor value=""Green"" />
    </mapping>
    <layout type = ""log4net.Layout.PatternLayout"" >
      <conversionPattern value=""%date [%thread] %5level %logger.%method [%line] - %property{scope} %property{test} MESSAGE: %message%newline %exception"" />
    </layout>
  </appender>

  <!--Trace模式的输出配置-->
  <appender name = ""TraceAppender"" type=""log4net.Appender.TraceAppender"">
    <layout type = ""log4net.Layout.PatternLayout"" >
      <conversionPattern value=""%date [%thread] %5level %logger.%method [%line] - %property{scope} %property{test} MESSAGE: %message%newline %exception"" />
    </layout>
  </appender>

  <!--RollingFile模式的输出配置-->
  <appender name = ""RollingFile"" type=""log4net.Appender.RollingFileAppender"">
    <!--输出的文件夹-->
    <file value = ""SevenTinyLogs/log.log"" />

    <!--追加日志内容-->
    <appendToFile value=""true"" />

    <!--防止多线程时不能写Log,官方说线程非安全-->
    <lockingModel type = ""log4net.Appender.FileAppender+MinimalLock"" />

    <!--当备份文件时, 为文件名加的后缀-->
    <datePattern value = ""yyyyMMdd.log"" />

    <!--日志最大个数, 都是最新的-->
    <!--rollingStyle节点为Size时,只能有value个日志-->
    <!--rollingStyle节点为Composite时,每天有value个日志-->
    <maxSizeRollBackups value = ""20"" />

    <!--可用的单位:KB|MB|GB-->
    <maximumFileSize value = ""1MB"" />

    <!--置为true, 当前最新日志文件名永远为file节中的名字-->
    <staticLogFileName value = ""true"" />

    <!--日志模板-->
    <layout type=""log4net.Layout.PatternLayout"">
      <conversionPattern value = ""%date [%thread] %5level %logger.%method [%line] - %property{scope} %property{test} MESSAGE: %message%newline%exception"" />
    </layout >
  </appender >

  <root >
    <level value=""ALL"" />
    <appender-ref ref=""RollingFile"" />
    <appender-ref ref=""TraceAppender"" />
    <appender-ref ref=""ConsoleAppender"" />
  </root>
</log4net>
";
    }
}
