using SevenTiny.Bantina.Bankinate.Attributes;
using System;

namespace Test.Common.Model
{
    [Table("Student")]
    [TableCaching]
    public class Student
    {
        [AutoIncrease]
        [Key]
        public int Id { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public int Age { get; set; }
        [Column]
        public DateTime? SchoolTime { get; set; }
    }
}

/*

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for Student
-- ----------------------------
DROP TABLE IF EXISTS `Student`;
CREATE TABLE `Student` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) DEFAULT NULL,
  `Age` int(11) NOT NULL,
  `SchoolTime` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1005 DEFAULT CHARSET=utf8;

*/