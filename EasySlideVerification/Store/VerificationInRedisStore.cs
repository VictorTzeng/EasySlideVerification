﻿using EasySlideVerification.Common;
using EasySlideVerification.Model;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasySlideVerification.Store
{
    /// <summary>
    /// Redis存储校验数据
    /// </summary>
    public class VerificationInRedisStore : ISlideVerificationStore
    {
        IDatabase store;

        public VerificationInRedisStore()
        {
            IConnectionMultiplexer connection = ConnectionMultiplexer.Connect(SlideVerificationRedisOptions.Default.Connection);
            this.store = connection.GetDatabase(SlideVerificationRedisOptions.Default.DatabaseIndex);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="data"></param>
        /// <param name="expire"></param>
        public void Add(SlideVerificationInfo data, TimeSpan expire)
        {
            HashEntry[] entries = new HashEntry[] {
                new HashEntry("BackgroudImage",data.BackgroudImage),
                new HashEntry("SlideImage",data.SlideImage),
                new HashEntry("OffsetX",data.OffsetX),
                new HashEntry("OffsetY",data.OffsetY),
            };
            this.store.HashSet($"{SlideVerificationRedisOptions.Default.KeyPrefix}{data.Key}", entries);
            this.store.KeyExpire($"{SlideVerificationRedisOptions.Default.KeyPrefix}{data.Key}", expire);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SlideVerificationInfo Get(string key)
        {
            SlideVerificationInfo result = null;
            HashEntry[] entries = this.store.HashGetAll($"{SlideVerificationRedisOptions.Default.KeyPrefix}{key}");
            if (entries != null && entries.Length > 0)
            {
                result = new SlideVerificationInfo();
                result.BackgroudImage = entries.First(a => a.Name == "BackgroudImage").Value;
                result.SlideImage = entries.First(a => a.Name == "SlideImage").Value;
                result.OffsetX = entries.First(a => a.Name == "OffsetX").Value.ToString().ToInt();
                result.OffsetY = entries.First(a => a.Name == "OffsetY").Value.ToString().ToInt();
            }

            return result;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Remove(string key)
        {
            this.store.KeyDelete($"{SlideVerificationRedisOptions.Default.KeyPrefix}{key}");
        }
    }
}
