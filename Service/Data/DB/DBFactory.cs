using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using Service.Common.Entity.DataExtraction;
using System.Data;

namespace Service.Common.DB
{
    public class DBFactory
    {
        /// <summary>
        /// 摘要：数据库类型描述。与此对应在应用程序配置文件<appSettings>节当中应当存在key为"DBType"的<add>节。
        /// 该节值分为如下几种：SQL SERVER数据库用“MSSql”表示；ORACLE数据库用“Oracle”表示；XML数据库用“XML”表示。
        /// </summary>
        private static string DATABASE_TYPE = ConfigurationManager.AppSettings.Get("DBType");
        /// <summary>
        /// 摘要：创建数据库核心实例。
        /// </summary>
        /// <returns></returns>
        public static DBCoreBase CreateDB()
        {
            return CreateDB(string.Empty);
        }
        /// <summary>
        /// 摘要：创建数据库核心实例。
        /// </summary>
        /// <param name="connectionName">数据库连接串在配置文件中的名称</param>
        /// <returns></returns>
        public static DBCoreBase CreateDB(string connectionName)
        {
            DBCoreBase DB = (DBCoreBase)Assembly.Load("Service.Common").CreateInstance("Service.Common.DB." + DATABASE_TYPE + "Core");
            if (connectionName != string.Empty)
            {
                DB.ConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ToString();
            }
            else
            {
                //APPDBSTRING为默认主数据库连接串
                DB.ConnectionString = ConfigurationManager.ConnectionStrings["APPDBSTRING"].ToString();
            }
            return DB;
        }
        /// <summary>
        /// 摘要：创建数据库核心实例。
        /// </summary>
        /// <param name="data_Type">数据库类型</param>
        /// <returns></returns>
        public static DBCoreBase CreateDB(DBType data_Type)
        {
            return CreateDB(data_Type, string.Empty);
        }
        /// <summary>
        /// 摘要：创建数据库核心实例。
        /// </summary>
        /// <param name="data_Type">数据库类型</param>
        /// <param name="connectionName">数据库连接串在配置文件中的名称</param>
        /// <returns></returns>
        public static DBCoreBase CreateDB(DBType data_Type, string connectionName)
        {
            DBCoreBase DB = (DBCoreBase)Assembly.Load("Service.Common").CreateInstance("Service.Common.DB." + DATABASE_TYPE + "Core");
            if (connectionName != string.Empty)
            {
                DB.ConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ToString();
            }
            else
            {
                //APPDBSTRING为默认主数据库连接串
                DB.ConnectionString = ConfigurationManager.ConnectionStrings["APPDBSTRING"].ToString();
            }
            return DB;
        }
        /// <summary>
        /// 摘要：根据驱动创建数据库核心实例。
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="path">数据库驱动配置文件路径</param>
        /// <returns>返回DB实例对象，若返回值为null为不可识别的数据库类型。</returns>
        public static DBCoreBase CreateDBByProvider(string connectionString, string path)
        {
            DBCoreBase DB = null;
            if (connectionString == string.Empty) return DB;
            string provider = connectionString.Split(';')[0].Split('=')[1].ToString();
            string DBName = string.Empty;
            DBInstance DBi = new DBInstance();
            DBi.TagData = path;
            List<string[]> where = new List<string[]>();
            where.Add(new string[] { "Provider LIKE '%{0}%'", provider });
            DataTable dt = DBi.GetDataSet("SELECT * From DBType", where);

            if (dt.Rows.Count > 0)
            {
                DBName = dt.Rows[0]["Name"].ToString();
                DB = (DBCoreBase)Assembly.Load("Service.Common").CreateInstance("Service.Common.DB." + DBName + "Core");
                DB.DBprovider = provider;
                DB.ConnectionString = connectionString;
            }
            return DB;
        }

    }
}
