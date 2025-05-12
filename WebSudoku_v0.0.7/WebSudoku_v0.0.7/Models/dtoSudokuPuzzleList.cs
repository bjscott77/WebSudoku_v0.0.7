
using System.Collections;

namespace WebSudoku_v0._0._7.Models
{
    public class DtoSudokuPuzzleList : List<EntityBase>
    {
        public List<EntityBase> Puzzles { get; set; } = new List<EntityBase>();

        public DtoSudokuPuzzleList()
        {
        }

        public DtoSudokuPuzzleList(IEnumerable<EntityBase> collection) : base(collection)
        {
            Puzzles.AddRange(collection);
        }

        public DtoSudokuPuzzleList(int capacity) : base(capacity)
        {
        }

        ///on base HERE is GetIdAsString().  Access via this.GetIdAsString()
    }
}
