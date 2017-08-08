using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace Service.Common.DB
{
    /// <summary>
    /// 各类型数据库访问接口
    /// </summary>
    public interface IDBCore
    {
        string Insert(object T);
        void Update(object T);
        void Delete(string PK,string tableName ,List<string> value);
        DataTable GetDataSet(string dataSetSql, List<string[]> where, List<string[]> orderByColumn, int rowBegin, int rowEnd);
        DataTable GetDataSet(string dataSetSql, List<string[]> where, List<string[]> orderByColumn);
        DataTable GetDataSet(string dataSetSql, List<string[]> where);
        int     GetDataSetCount(string dataSetSql, List<string[]> where);
    }
}
