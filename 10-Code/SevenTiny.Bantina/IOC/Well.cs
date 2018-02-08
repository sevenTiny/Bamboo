/*********************************************************
 * CopyRight: QIXIAO CODE BUILDER. 
 * Version: 5.0.0
 * Author: sevenTiny
 * Address: Earth
 * Create: 2017-12-03 21:01:25
 * Update: 2017-12-4 23:56:47
 * E-mail: dong@qixiao.me | wd8622088@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: Well is a Inversion of Control container create by 7Tiny
 * Thx , Best Regards ~
 *********************************************************/
using SevenTiny.Bantina.Extensions;
using System;
using System.Collections.Generic;

namespace SevenTiny.Bantina.IOC
{
    /// <summary>
    /// Well register use this
    /// </summary>
    public class Well : IWell
    {
        private readonly Dictionary<int, object> _container;

        private Well()
        {
            if (_container == null)
            {
                _container = new Dictionary<int, object>();
            }
        }

        #region Singleton
        private static readonly Well _instance = new Well();
        public static Well Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        #region Register
        /// <summary>
        /// Register Class by type
        /// </summary>
        /// <typeparam name="TEntity">type</typeparam>
        /// <param name="entity">entity</param>
        public void Register<TEntity>(TEntity entity) where TEntity : class
        {
            _container.AddOrUpdate(typeof(TEntity).Name.GetHashCode(), entity);
        }
        #endregion

        #region Resolve
        /// <summary>
        /// Resolve Entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TEntity Resolve<TEntity>() where TEntity : class
        {
            TEntity entity = _container[typeof(TEntity).Name.GetHashCode()] as TEntity;
            if (entity == null)
            {
                entity = Activator.CreateInstance<TEntity>();
                _container.AddOrUpdate(typeof(TEntity).Name.GetHashCode(), entity);
            }
            return entity;
        }
        #endregion
    }

    /// <summary>
    /// Well:get instance use this
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Well<T> : IWell<T> where T : class
    {
        public static T Instance => Well.Instance.Resolve<T>();
    }
}
