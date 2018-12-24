using System;
using System.Collections.Generic;
using System.Data;

namespace PickaxeCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var table = new DataTable();
            table.Columns.Add("col1", typeof(float));
            var row = table.NewRow();
            row[0] = 1.0f;
        }
    }
}
