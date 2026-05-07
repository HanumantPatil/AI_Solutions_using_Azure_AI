using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeApp
{
    public interface ISorting
    {
        void Sort();
    }
    class MS : ISorting
    {
        public void Sort()
        {
            Console.WriteLine("Sorting using Merge Sort");
        }
    }
    class QS : ISorting
    {
        public void Sort()
        {
            Console.WriteLine("Sorting using Quick Sort");
        }
    }

    public class SortingService
    {
        private ISorting _sortingAlgorithm;
        public SortingService(ISorting sortingAlgorithm)
        {
            _sortingAlgorithm = sortingAlgorithm;
        }
        public void Sort()
        {
            _sortingAlgorithm.Sort();
        }
        public void ChangeSortingAlgorithm(ISorting newSortingAlgorithm)
        {
            _sortingAlgorithm = newSortingAlgorithm;
        }
    }

    internal class _6_StratergyDesignPattern
    {
        public static void Main()
        {
            SortingService sortingService = new SortingService(new MS());
            sortingService.Sort();
            
            sortingService.ChangeSortingAlgorithm(new QS());
            sortingService.Sort();
        }
    }
}
