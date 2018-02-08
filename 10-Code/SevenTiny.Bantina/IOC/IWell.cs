/*********************************************************
 * CopyRight: QIXIAO CODE BUILDER. 
 * Version: 5.0.0
 * Author: sevenTiny
 * Address: Earth
 * Create: 2017-12-03 22:42:07
 * Update: 2017-12-03 22:42:07
 * E-mail: dong@qixiao.me | wd8622088@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
namespace SevenTiny.Bantina.IOC
{
    public interface IWell
    {
        void Register<TEntity>(TEntity entity) where TEntity : class;
        TEntity Resolve<TEntity>() where TEntity : class;
    }
    public interface IWell<T> where T : class
    {

    }
}
