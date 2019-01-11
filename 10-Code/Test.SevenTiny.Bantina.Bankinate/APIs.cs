/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 1/11/2019, 16:31:04 PM
* Modify: 
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: api列表
* Thx , Best Regards ~
*********************************************************/
using System.Collections.Generic;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    [Trait("desc", "这里是Bankinate ORM支持的Api说明列表")]
    [Trait("desc", "这里使用的是MySql数据库进行的测试，Api和数据库无关，适用于多种数据库")]
    public class APIs
    {
        public void Api()
        {
            //Init Db
            using (var db = new MySqlDb())
            {
                // Add
                // 添加单条数据
                db.Add(new Student() { Name = "Join" });
                // Add List
                // 添加集合
                db.Add<Student>(new List<Student>());

                // Update
                // 根据现有对象进行修改（内部实现采用主键作为条件，如果修改了主键的值，会出现意外结果）
                var stu = db.QueryOne<Student>(t => t.Id == 1);
                stu.Name = "susan";
                db.Update(stu);
                // Update
                // 根据条件进行修改（会修改全部Column标识的字段，请注意所有字段的值）
                db.Update<Student>(t => t.Id == 1, new Student { Name = "susan" });

                // Delete
                // 根据条件进行删除（内部实现采用主键作为条件，如果修改主键的值，会出现意外结果）
                var stu2 = db.QueryOne<Student>(t => t.Id == 1);
                db.Delete(stu2);
                // Delete
                //根据条件进行删除
                db.Delete<Student>(t => t.Id == 1);

                // Query ------
                // -- quick query
                // 快速查询接口，直接通过条件进行查询
                var list0 = db.QueryList<Student>(t => true);
                // -- use ToList();
                // 采用懒加载组合条件方式查询
                // all
                // 查询全部
                var list1 = db.Queryable<Student>().ToList();
                // name like %111%
                // 模糊查询
                var list2 = db.Queryable<Student>().Where(t => t.Name.Contains("111")).ToList();
                // name like 1%
                // 模糊查询
                var list3 = db.Queryable<Student>().Where(t => t.Name.StartsWith("1")).ToList();
                // name like %1
                // 模糊查询
                var list4 = db.Queryable<Student>().Where(t => t.Name.EndsWith("1")).ToList();
                // multi condition
                // 多个条件连接
                var list8 = db.Queryable<Student>().Where(t => t.Age == 1 && t.SchoolTime.Equals("fory")).ToList();
                // multi condition 2 same as up
                // 多个条件连接，和上面写一起效果一样，And拼接子条件
                var list9 = db.Queryable<Student>().Where(t => t.Age == 1).Where(t => t.SchoolTime.Equals("fory")).ToList();
                // select id,name
                // 只查某几个字段，非'*'
                var list5 = db.Queryable<Student>().Select(t => new { t.Name, t.Id }).ToList();
                // select name
                // 只查某个字段
                var list6 = db.Queryable<Student>().Select(t => t.Name).ToList();
                // top/limit 30
                // 查count条数据
                var list7 = db.Queryable<Student>().Limit(30).ToList();
                // orderby asc
                // 正序排序
                var list10 = db.Queryable<Student>().OrderBy(t => t.Id).ToList();
                // orderby desc
                // 倒序排序
                var list11 = db.Queryable<Student>().OrderByDescending(t => t.Id).ToList();
                //paging
                // 分页查询
                var list12 = db.Queryable<Student>().Paging(1, 10).ToList();

                // entity 
                // -- use top 1 internal !
                // 内部实现使用的 top 1 （SqlServer）语句，只查询一条，其他数据库用的其他查一条的方式
                // -- quick query
                // 快速查询一条
                var entity3 = db.QueryOne<Student>(t => t.Id == 1);
                // -- use ToEntity();
                // 采用懒加载拼接条件的方式返回一条数据
                var entity1 = db.Queryable<Student>().ToEntity();
                var entity2 = db.Queryable<Student>().Where(t => t.Name.Equals("1")).ToEntity();
                // like list query ...
                // 其他Api和查集合一致，不一一举例，结尾使用ToEntity();即可
                //...

                // count
                // -- quick query
                // 根据条件返回数据量
                var count1 = db.QueryCount<Student>(t => true);
                // -- use ToCount();
                // 用懒加载拼接条件的方式返回一条数据
                var count = db.Queryable<Student>().ToCount();
                // like list query ...
                // 其他Api和查集合一致，不一一举例，结尾使用ToCount();即可
                //...

                // exist
                // -- use count > 0 internal
                // 返回是否存在相关条件记录，内部使用判断数量是否>0实现
                var exist1 = db.QueryExist<Student>(t => true);
            }
        }

        [Fact]
        [Trait("desc", "某个具体的api可以放在这里执行尝试")]
        public void ApiTest()
        {
            using (var db = new MySqlDb())
            {
                //api...

            }
        }
    }
}
