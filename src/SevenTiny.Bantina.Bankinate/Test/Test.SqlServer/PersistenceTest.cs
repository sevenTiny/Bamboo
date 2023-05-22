//using SevenTiny.Bantina.Bankinate;
//using SevenTiny.Bantina.Bankinate.Attributes;
//using System;
//using System.Collections.Generic;
//using Test.Common;
//using Test.Common.Model;
//using Xunit;

//namespace Test.SqlServer
//{
//    [DataBase("SevenTinyTest")]
//    public class DataPreseterDb : SqlServerDbContext<DataPreseterDb>
//    {
//        public DataPreseterDb() : base("")
//        {

//        }
//    }

//    /// <summary>
//    /// 数据预置类
//    /// </summary>
//    public class PersistenceTest
//    {
//        //[Fact]
//        [Trait("desc", "初始化测试数据")]
//        public void InitData()
//        {
//            return;
//            using (var db = new DataPreseterDb())
//            {
//                //清空所有数据,并重置索引
//                db.ExecuteSql("truncate table " + db.GetTableName<OperationTest>());

//                //预置测试数据
//                List<OperationTest> models = new List<OperationTest>();
//                for (int i = 1; i < 1001; i++)
//                {
//                    db.Add<OperationTest>(new OperationTest
//                    {
//                        Key2 = i,
//                        StringKey = $"test_{i}",
//                        IntKey = i,
//                        IntNullKey = null,
//                        DateNullKey = DateTime.Now.Date,
//                        DateTimeNullKey = DateTime.Now,
//                        DoubleNullKey = i,
//                        FloatNullKey = i
//                    });
//                }
//            }
//            Assert.True(true);
//        }
//    }
//}
