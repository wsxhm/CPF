using System.Collections;
using System.Data;
using CPF;

namespace CpfDemo
{

    public class DataGridViewModel : CpfObject
    {
        public static int MAX = byte.MaxValue;
        public DataGridViewModel()
        {

            var data = new DataTable();

            for (int i = 0; i < MAX; i++)
            {
                data.Columns.Add("p" + (i + 1));
            }


            for (int i = 0; i < 1000; i++)
            {
                var row = data.NewRow();
                for (int j = 0; j < MAX; j++)
                {
                    if (j != 1)
                    {
                        row[j] = i;
                    }
                }
                data.Rows.Add(row);
            }

            Data = data.ToItems();
        }
        public IList Data
        {
            get { return (IList)GetValue(); }
            set { SetValue(value); }
        }


    }
}