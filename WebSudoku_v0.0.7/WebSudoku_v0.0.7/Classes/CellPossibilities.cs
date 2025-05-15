using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSudoku_v0._0._7.Classes
{
    public class CellPossibilities : ICellPossibilities
    {
        public List<int> List { get; set; } = new List<int>();  
    }
}
