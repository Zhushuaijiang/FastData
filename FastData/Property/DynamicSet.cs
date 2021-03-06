﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FastData.Property
{
    /// <summary>
    /// 动态属性setvalue
    /// </summary>
    internal class DynamicSet<T>
    {
        private static bool IsSetCache;
        private static Action<object, string, object> SetValueDelegate;
                
        // 构建函数        
        static DynamicSet()
        {
            SetValueDelegate = GenerateSetValue();
        }
        
        #region 动态setvalue
        /// <summary>
        /// 动态setvalue
        /// </summary>
        /// <param name="instance">类型</param>
        /// <param name="memberName">成员</param>
        /// <param name="newValue">值</param>
        public void SetValue(T instance, string memberName, object newValue, bool IsCache)
        {
            IsSetCache = IsCache;
            SetValueDelegate(instance, memberName, newValue);
        }
        #endregion
        
        #region 动态生成setvalue
        /// <summary>
        /// 动态生成setvalue
        /// </summary>
        /// <returns></returns>
        private static Action<object, string, object> GenerateSetValue()
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var newValue = Expression.Parameter(typeof(object), "newValue");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            //var task = new List<Task>();

            foreach (var propertyInfo in PropertyCache.GetPropertyInfo<T>(IsSetCache))
            {
               // task.Add(Task.Factory.StartNew(() =>
                //{
                    var property = Expression.Property(Expression.Convert(instance, typeof(T)), propertyInfo.Name);
                    var setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType));
                    var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));
                    cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
               // }));
            }

            //Task.WaitAll(task.ToArray());
            var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
            var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

            return Expression.Lambda<Action<object, string, object>>(methodBody, instance, memberName, newValue).Compile();
        }
        #endregion
    }
}
