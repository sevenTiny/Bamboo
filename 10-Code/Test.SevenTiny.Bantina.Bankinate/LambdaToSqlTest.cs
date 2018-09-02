using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    public class LambdaToSqlTest
    {
        [Fact]
        public void ConvertWhereTest()
        {
            //var sql = LambdaToSql.ConvertWhere<CommonInfo>(t => t.Code.Equals("wangdong3"));
        }
    }

    public class CommonInfo
    {
        [Key]
        [AutoIncrease]
        public int Id { get; set; }
        [Column("`Name`")]
        public string Name { get; set; }
        [Column("`Code`")]
        public string Code { get; set; }
        [Column("`Description`")]
        public string Description { get; set; } = string.Empty;
        [Column("`Group`")]
        public string Group { get; set; } = string.Empty;
        [Column]
        public int SortNumber { get; set; } = 0;
        [Column]
        public int IsDeleted { get; set; } = 0;
        [Column]
        public int CreateBy { get; set; } = -1;
        [Column("`CreateTime`")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        [Column]
        public int ModifyBy { get; set; } = -1;
        [Column("`ModifyTime`")]
        public DateTime ModifyTime { get; set; } = DateTime.Now;
    }
}
