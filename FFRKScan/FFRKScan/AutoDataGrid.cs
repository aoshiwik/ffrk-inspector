using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace FFRKScan
{
    class CustomDataGridView : DataGridView
    {
        public CustomDataGridView()
        {
            DoubleBuffered = true;
        }
    }

    public partial class AutoDataGrid : UserControl
    {
        public AutoDataGrid()
        {
            InitializeComponent();

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        class FillState
        {
            public DataColumn[] columns;
            public string primaryKey;
            public Action<DataTable> updateRows;
        }

        public void Clear()
        {
            dataGridView1.DataSource = null;
        }

        public void SaveToFile()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "csv files (*.csv)|*.csv";

            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream myStream;

                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    var table = dataGridView1.DataSource as DataTable;

                    if (table != null)
                    {
                        SaveCSV(table, myStream);
                    }

                    myStream.Close();
                }
            }
        }

        private void SaveCSV(DataTable dt, Stream myStream)
        {
            using (var writer = new StreamWriter(myStream))
            {
                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Where(each=>each.ColumnMapping!= MappingType.Hidden).Select(column => column.ColumnName);
                writer.WriteLine(string.Join(",", columnNames));

                foreach (DataRow row in dt.Rows)
                {
                 //   IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                    IEnumerable<string> fields = columnNames.Select(each => string.Concat("\"", row[each].ToString().Replace("\"", "\"\""), "\""));
                    writer.WriteLine(string.Join(",", fields));
                }

            }
        }

        public void Populate(DataColumn[] columns, string primaryKey, Action<DataTable> updateRows)
        {
            if (columns == null || updateRows == null)
            {
                return;
            }

            var state = new FillState { columns = columns, primaryKey = primaryKey, updateRows = updateRows };
            ThreadPool.QueueUserWorkItem(FillData, state);
        }

        void FillData(object state)
        {
            FillState fs = state as FillState;

            var dataTable = new DataTable();

            dataTable.Columns.AddRange(fs.columns);

            if (fs.primaryKey != null)
            {
                dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns[fs.primaryKey] };
            }

            fs.updateRows(dataTable);

            this.Invoke((MethodInvoker)delegate
            {
                dataGridView1.DataSource = dataTable;

                if (fs.primaryKey != null)
                {
                 //   dataGridView1.Columns[fs.primaryKey].Visible = false;
                }
            });
        }
    }
}
