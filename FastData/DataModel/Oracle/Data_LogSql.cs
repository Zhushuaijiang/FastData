﻿using System;
using FastData.Property;

namespace FastData.DataModel.Oracle
{
    /// <summary>
    /// sql日志
    /// </summary>
    /// </summary>
    [Table(Comments = "sql日志")]
    internal class Data_LogSql
    {
        /// <summary>
        /// sql id
        /// </summary>
        [Column(Comments = "Sql id", DataType = "Char", Length = 64, IsNull = false, IsKey = true)]
        public string SqlId { get; set; }

        /// <summary>
        /// sql语句
        /// </summary>
        [Column(Comments = "sql语句", DataType = "Clob", IsNull = true)]
        public string Content { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        [Column(Comments = "执行时间", DataType = "Char", Length = 16, IsNull = true)]
        public string ExecTime { get; set; }
        
        /// <summary>
        /// 语句类型
        /// </summary>
        [Column(Comments = "语句类型", DataType = "Char", Length = 16, IsNull = false)]
        public string SqlType { get; set; }

        /// <summary>
        /// 增加时间
        /// </summary>
        [Column(Comments = "增加时间", DataType = "Date", IsNull = false)]
        public DateTime AddTime { get; set; }
    }
}
