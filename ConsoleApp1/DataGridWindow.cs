using CPF;
using CPF.Controls;

namespace CpfDemo
{ 
public class DataGridWindow:Window
{
    protected override void InitializeComponent()
    {
        Width = 700;
        Height = 500;
        Collection<DataGridColumn> columns = new Collection<DataGridColumn>();

        for (int i = 0; i < 200; i++)
        {
            columns.Add(new DataGridTextColumn()
            {
                Header = "测试"+(i+1),
                Binding = new DataGridBinding("p"+(i+1))
                {
                    BindingMode = BindingMode.TwoWay
                },
                Width = 50
            });
        }
 

        var datagrid = new DataGrid
        {
            Width = "90%",
            Height = "90%",
            Columns =columns,
            Bindings =
            {
                {
                    nameof(DataGrid.Items),
                    nameof(DataGridViewModel.Data)
                }
            }
            ,IsVirtualizing = true
        };
        
        
        Children.Add(new WindowFrame(this,datagrid));
    }
}}