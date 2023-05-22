using SevenTiny.Bantina.Bankinate.Attributes;
using System;

namespace Test.Common.Model
{
    [Table("OperateTest")]
    [TableCaching]//二级缓存开关,需要上下文开启二级缓存
    public class OperationTest
    {
        [Key]
        [AutoIncrease]
        [Column]
        public int Id { get; set; }
        [Key]
        [Column]
        public int Key2 { get; set; }
        [Column]
        public string StringKey { get; set; }
        [Column]
        public int IntKey { get; set; }
        [Column]
        public int? IntNullKey { get; set; }
        [Column]
        public DateTime? DateNullKey { get; set; }
        [Column]
        public DateTime? DateTimeNullKey { get; set; }
        [Column]
        public double? DoubleNullKey { get; set; }
        /// <summary>
        /// SqlServer用float?会在FillAdapter转化中报错，因为SqlServer的类型对应的实体字段类型是double映射关系
        /// MySql用double?会在FillAdapter转化中报错，因为MySql库中对应float
        /// 这里需要针对具体的数据库进行针对性地设计实体地属性类型
        /// </summary>
        //[Column]
        public float? FloatNullKey { get; set; }
        public Guid GuidKey { get; set; }
    }
}

// sql of mysql
/* 

SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for OperateTest
-- ----------------------------
DROP TABLE IF EXISTS `OperateTest`;
CREATE TABLE `OperateTest` (
  `Id` int (11) NOT NULL AUTO_INCREMENT,
  `Key2` int (11) NOT NULL,
  `StringKey` varchar(500) DEFAULT NULL,
  `IntKey` int (11) NOT NULL,
  `IntNullKey` int (11) DEFAULT NULL,
  `DateNullKey` date DEFAULT NULL,
  `DateTimeNullKey` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `DoubleNullKey` double DEFAULT NULL,
  `FloatNullKey` float DEFAULT NULL,
  PRIMARY KEY(`Id`,`Key2`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

*/

// sql of sqlserver
/* 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OperateTest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key2] [int] NOT NULL,
	[StringKey] [nvarchar](50) NULL,
	[IntKey] [int] NOT NULL,
	[IntNullKey] [int] NULL,
	[DateNullKey] [date] NULL,
	[DateTimeNullKey] [datetime] NULL,
	[DoubleNullKey] [float] NULL,
	[FloatNullKey] [float] NULL,
 CONSTRAINT [PK_OperateTest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

*/
