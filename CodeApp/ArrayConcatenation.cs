using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeApp
{
    internal class ArrayConcatenation
    {
        public int[] GetConcatenation(int[] nums)
        {
            int n = nums.Length;
            int[] ans = new int[2 * n];
            for (int i = 0; i < 2 * n; i++)
            {
                ans[i] = nums[i % n];
            }
            return ans;
        }
    }
}
