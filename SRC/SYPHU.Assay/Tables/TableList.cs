using System.Collections.Generic;
using System.Linq;

namespace SYPHU.Assay.Tables
{
    /// <summary>
    /// 输出表格列表
    /// </summary>
    public class TableList
    {
        public readonly List<TableDesc> List = new List<TableDesc>();

        private readonly List<TableCategory> _categories = new List<TableCategory>();

        /// <summary>
        /// 添加单个表格
        /// </summary>
        /// <param name="desc"></param>
        public void AddSingleTable(TableDesc desc)
        {
            if (desc.Category != TableCategory.OrigDataTable && desc.Category != TableCategory.LatinConvertTable)
            {
                if (_categories.Contains(desc.Category))
                {
                    RemoveTable(desc.Category);
                }
                _categories.Add(desc.Category);
                List.Add(desc);
            }
        }

        /// <summary>
        /// 添加多个表格--数据表格
        /// </summary>
        /// <param name="dataTables"></param>
        public void AddMultiTables(List<DataTable> dataTables)
        {
            if (dataTables != null && dataTables.Count > 0 && dataTables[0].Table.Category != TableCategory.OrigDataTable)
            {
                if (_categories.Contains(dataTables[0].Table.Category))
                {
                    RemoveTable(dataTables[0].Table.Category);
                }

                foreach (var dataTable in dataTables)
                {
                    _categories.Add(dataTable.Table.Category);
                    List.Add(dataTable.Table);
                }
            }
        }

        /// <summary>
        /// 添加多个表格--效价估计表
        /// </summary>
        /// <param name="potencyEstimateTables"></param>
        public void AddMultiTables(List<PotencyEstimateTable> potencyEstimateTables)
        {
            if (potencyEstimateTables != null && potencyEstimateTables.Count > 0)
            {
                if (_categories.Contains(potencyEstimateTables[0].Table.Category))
                {
                    RemoveTable(potencyEstimateTables[0].Table.Category);
                }

                foreach (var potencyEstimateTable in potencyEstimateTables)
                {
                    _categories.Add(potencyEstimateTable.Table.Category);
                    List.Add(potencyEstimateTable.Table);
                }
            }
        }

        public void RemoveResultTables()
        {
            RemoveTable(TableCategory.TranDataTable);
            RemoveTable(TableCategory.CorrDataTable);
            RemoveTable(TableCategory.LatinDataTable);
            RemoveTable(TableCategory.VarianceAnalysisTable);
            RemoveTable(TableCategory.ReliabilityConclusionTable);
            RemoveTable(TableCategory.PotencyEstimateTable);
        }

        public void RemoveAllTables()
        {
            List.Clear();
            _categories.Clear();
        }

        /// <summary>
        /// 移除表格
        /// </summary>
        /// <param name="category"></param>
        public void RemoveTable(TableCategory category)
        {
            List.RemoveAll(t => t.Category == category);
            _categories.RemoveAll(t => t == category);
        }

        /// <summary>
        /// 清空表格
        /// </summary>
        /// <param name="isNew">是否新建</param>
        public void Clear(bool isNew = false)
        {
            if (List == null || _categories == null)
            {
                return;
            }

            if (!isNew)
            {
                RemoveTable(TableCategory.CommonInfoTable);
                RemoveTable(TableCategory.CalcInfoTable);
                RemoveResultTables();
            }
            else
            {
                RemoveAllTables();
            }
        }

        /// <summary>
        /// 添加拉丁方转换表
        /// </summary>
        /// <param name="desc"></param>
        public void AddLatinConvertTable(TableDesc desc)
        {
            if (desc != null && desc.Category == TableCategory.LatinConvertTable)
            {
                if (_categories.Contains(TableCategory.LatinConvertTable))
                {
                    int pos = 0;
                    for (int i = 0; i < _categories.Count; i++)
                    {
                        if (_categories[i] == TableCategory.LatinConvertTable)
                        {
                            pos = i;
                            break;
                        }
                    }
                    for (int i = 1; i < desc.RowNum; i++)
                    {
                        if (i < List[pos].RowNum)
                        {
                            for (int j = 0; j < desc.ColNum; j++)
                            {
                                if (j < List[pos].ColNum)
                                {
                                    desc.Cells[i][j].Content = List[pos].Cells[i][j].Content;
                                }
                            }
                        }
                    }
                    RemoveTable(TableCategory.LatinConvertTable);
                }
                _categories.Add(desc.Category);
                List.Add(desc);
            }
        }

        /// <summary>
        /// 添加原始数据表
        /// </summary>
        /// <param name="origDataTables"></param>
        public void AddOrigDataTables(List<DataTable> origDataTables)
        {
            if (origDataTables == null || origDataTables.Count == 0 ||
                origDataTables.Any(t => t.Table.Category != TableCategory.OrigDataTable))
            {
                return;
            }
            
            //找到列表中已经有的原始数据，储存位置
            var origDataTableId = new List<int>();
            if (_categories.Contains(TableCategory.OrigDataTable))
            {
                for (int i = 0; i < _categories.Count; i++)
                {
                    if (_categories[i] == TableCategory.OrigDataTable)
                    {
                        origDataTableId.Add(i);
                    }
                }

                for (int i = 0; i < origDataTableId.Count; i++)
                {
                    if (i < origDataTables.Count)
                    {
                        //添加表内数据
                        for (int j = List[origDataTableId[i]].DataStartRow; j < List[origDataTableId[i]].RowNum; j++)
                        {
                            if (j < origDataTables[i].Table.RowNum)
                            {
                                for (int k = List[origDataTableId[i]].DataStartCol; k < List[origDataTableId[i]].ColNum; k++)
                                {
                                    if (k < origDataTables[i].Table.ColNum)
                                    {
                                        origDataTables[i].Table.Cells[j][k].Content = List[origDataTableId[i]].Cells[j][k].Content;
                                    }
                                }
                            }
                        }
                        //添加A值，稀释倍数值
                        if (i < origDataTables.Count)
                        {
                            origDataTables[i].Table.Cells[2][1].Content = List[origDataTableId[i]].Cells[2][1].Content;
                            origDataTables[i].Table.Cells[3][1].Content = List[origDataTableId[i]].Cells[3][1].Content;
                        }
                        //添加d值
                        for (int j = 1; j < List[origDataTableId[i]].ColNum; j++)
                        {
                            if (j < origDataTables[i].Table.ColNum)
                            {
                                origDataTables[i].Table.Cells[4][j].Content = List[origDataTableId[i]].Cells[4][j].Content;
                            }
                        }
                    }
                }

                //移除现有原始数据表
                RemoveTable(TableCategory.OrigDataTable);
            }

            //添加新的原始数据表
            foreach (var dataTable in origDataTables)
            {
                _categories.Add(dataTable.Table.Category);
                List.Add(dataTable.Table);
            }
        }

        public void AddMergeCalcOrigDataTables(List<DataTable> origDataTables)
        {
            if (origDataTables == null || origDataTables.Count == 0 ||
                origDataTables.Any(t => t.Table.Category != TableCategory.OrigDataTable))
            {
                return;
            }

            //找到列表中已经有的原始数据，储存位置
            var origDataTableId = new List<int>();
            if (_categories.Contains(TableCategory.OrigDataTable))
            {
                for (int i = 0; i < _categories.Count; i++)
                {
                    if (_categories[i] == TableCategory.OrigDataTable)
                    {
                        origDataTableId.Add(i);
                    }
                }

                for (int i = 0; i < origDataTableId.Count; i++)
                {
                    if (i < origDataTables.Count)
                    {
                        //添加表内数据
                        for (int j = List[origDataTableId[i]].DataStartRow; j < List[origDataTableId[i]].RowNum; j++)
                        {
                            if (j < origDataTables[i].Table.RowNum)
                            {
                                for (int k = List[origDataTableId[i]].DataStartCol; k < List[origDataTableId[i]].ColNum; k++)
                                {
                                    if (k < origDataTables[i].Table.ColNum)
                                    {
                                        origDataTables[i].Table.Cells[j][k].Content = List[origDataTableId[i]].Cells[j][k].Content;
                                    }
                                }
                            }
                        }
                        //添加A值
                        if (i < origDataTables.Count)
                        {
                            origDataTables[i].Table.Cells[0][2].Content = List[origDataTableId[i]].Cells[0][2].Content;
                        }
                    }
                }

                //移除现有原始数据表
                RemoveTable(TableCategory.OrigDataTable);
            }

            //添加新的原始数据表
            foreach (var dataTable in origDataTables)
            {
                _categories.Add(dataTable.Table.Category);
                List.Add(dataTable.Table);
            }
        }
    }
}
