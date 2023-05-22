using System;
using System.Collections.Generic;
using SevenTiny.Bantina.Bankinate.Attributes;
using System.Text;

namespace Test.Common.Model
{
    [Table("OperateTest2")]
    public class OperationTest2
    {
        [Key]
        [Column]
        public Guid Uid { get; set; }
        [Column]
        public string StringKey { get; set; }
    }
}

// sql of mysql
/*

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for OperateTest2
-- ----------------------------
DROP TABLE IF EXISTS `OperateTest2`;
CREATE TABLE `OperateTest2` (
  `Uid` char(36) NOT NULL,
  `StringKey` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

     
*/
