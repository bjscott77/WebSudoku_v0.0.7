
using System;
using System.Runtime.InteropServices;

namespace WebSudoku_v0._0._7.Classes
{
    public class SudokuBoard : ISudokuBoard
    {
        public ISudokuManager SudokuManager { get; set; } = new SudokuManager();
        public Cells Cells { get; set; } = new Cells();
        
        //obsolete - Handled by DB now
        //public List<Puzzle> Puzzles { get; set; } = new List<Puzzle>();
        
        public ISudokuDimensions Dimensions { get; set; } = new SudokuDimensions(81, 9);
        public string CurrentStartingPuzzle { get; set; } = string.Empty;

        public SudokuBoard()
        {

        }
        public SudokuBoard(string puzzle)
        {
            InitializeBoard(puzzle);
            InitializeOdds();
        }
        public SudokuBoard(ISudokuDimensions dimensions)
        {
            Dimensions = dimensions;
            InitializeBoard();
            InitializeOdds();
        }

        //  obsolete - Handled on FE now
        public void DisplayBoard()
        {
            string board = $"{Cells.List[0].DisplayValue} {Cells.List[1].DisplayValue} {Cells.List[2].DisplayValue} {Cells.List[3].DisplayValue} {Cells.List[4].DisplayValue} {Cells.List[5].DisplayValue} {Cells.List[6].DisplayValue} {Cells.List[7].DisplayValue} {Cells.List[8].DisplayValue}\n";
            board += $"{Cells.List[9].DisplayValue} {Cells.List[10].DisplayValue} {Cells.List[11].DisplayValue} {Cells.List[12].DisplayValue} {Cells.List[13].DisplayValue} {Cells.List[14].DisplayValue} {Cells.List[15].DisplayValue} {Cells.List[16].DisplayValue} {Cells.List[17].DisplayValue}\n";
            board += $"{Cells.List[18].DisplayValue} {Cells.List[19].DisplayValue} {Cells.List[20].DisplayValue} {Cells.List[21].DisplayValue} {Cells.List[22].DisplayValue} {Cells.List[23].DisplayValue} {Cells.List[24].DisplayValue} {Cells.List[25].DisplayValue} {Cells.List[26].DisplayValue}\n";
            board += $"{Cells.List[27].DisplayValue} {Cells.List[28].DisplayValue} {Cells.List[29].DisplayValue} {Cells.List[30].DisplayValue} {Cells.List[31].DisplayValue} {Cells.List[32].DisplayValue} {Cells.List[33].DisplayValue} {Cells.List[34].DisplayValue} {Cells.List[35].DisplayValue}\n";
            board += $"{Cells.List[36].DisplayValue} {Cells.List[37].DisplayValue} {Cells.List[38].DisplayValue} {Cells.List[39].DisplayValue} {Cells.List[40].DisplayValue} {Cells.List[41].DisplayValue} {Cells.List[42].DisplayValue} {Cells.List[43].DisplayValue} {Cells.List[44].DisplayValue}\n";
            board += $"{Cells.List[45].DisplayValue} {Cells.List[46].DisplayValue} {Cells.List[47].DisplayValue} {Cells.List[48].DisplayValue} {Cells.List[49].DisplayValue} {Cells.List[50].DisplayValue} {Cells.List[51].DisplayValue} {Cells.List[52].DisplayValue} {Cells.List[53].DisplayValue}\n";
            board += $"{Cells.List[54].DisplayValue} {Cells.List[55].DisplayValue} {Cells.List[56].DisplayValue} {Cells.List[57].DisplayValue} {Cells.List[58].DisplayValue} {Cells.List[59].DisplayValue} {Cells.List[60].DisplayValue} {Cells.List[61].DisplayValue} {Cells.List[62].DisplayValue}\n";
            board += $"{Cells.List[63].DisplayValue} {Cells.List[64].DisplayValue} {Cells.List[65].DisplayValue} {Cells.List[66].DisplayValue} {Cells.List[67].DisplayValue} {Cells.List[68].DisplayValue} {Cells.List[69].DisplayValue} {Cells.List[70].DisplayValue} {Cells.List[71].DisplayValue}\n";
            board += $"{Cells.List[72].DisplayValue} {Cells.List[73].DisplayValue} {Cells.List[74].DisplayValue} {Cells.List[75].DisplayValue} {Cells.List[76].DisplayValue} {Cells.List[77].DisplayValue} {Cells.List[78].DisplayValue} {Cells.List[79].DisplayValue} {Cells.List[80].DisplayValue}\n";

        }

        public void DisplayOdds()
        {
            foreach(var cell in Cells.List)
            {
                var odds = string.Join(", ", cell.CellPossibilities.List);
                Console.WriteLine(odds);
            }
        }
        
        public bool InitializeOdds()
        {
            for (int i = 0; i < Cells.List.Count; i++)
            {
                Cells = SudokuManager.InitialOddsSetup(Cells, i);
                Cells = SudokuManager.SetCellOdds(Cells, i);
            }
            return true;
        }

        public bool InitializeBoard(string puzzle = "350602004007040013069831007503000096000300745946000800692400008800703000004020001")
        {
            CurrentStartingPuzzle = puzzle;
            Cells = new Cells();
            int index = 0;
            foreach (var cellValue in puzzle.ToCharArray())
            {
                Cells.List.Add(SudokuManager.SetNextCell(cellValue.ToString(), index));
                index++;
            }
            return true;
        }
    }
}
