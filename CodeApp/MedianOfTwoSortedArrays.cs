using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeApp
{
    internal class MedianOfTwoSortedArrays
    {
        public MedianOfTwoSortedArrays()
        {
            int[] nums1 = { 1, 3 };
            int[] nums2 = { 2 };
            double result = FindMedianSortedArrays(nums1, nums2);
            Console.WriteLine(result); // Output: 2.0
        }

        public double FindMedianSortedArrays(int[] nums1, int[] nums2)
        {
            int m = nums1.Length;
            int n = nums2.Length;
            int totalLength = m + n;
            // Create a merged array to hold the combined elements
            int[] merged = new int[totalLength];
            int i = 0, j = 0, k = 0;
            // Merge the two arrays
            while (i < m && j < n)
            {
                if (nums1[i] < nums2[j])
                {
                    merged[k++] = nums1[i++];
                }
                else
                {
                    merged[k++] = nums2[j++];
                }
            }
            // If there are remaining elements in nums1
            while (i < m)
            {
                merged[k++] = nums1[i++];
            }
            // If there are remaining elements in nums2
            while (j < n)
            {
                merged[k++] = nums2[j++];
            }
            // Calculate the median
            if (totalLength % 2 == 0)
            {
                return (merged[totalLength / 2 - 1] + merged[totalLength / 2]) / 2.0;
            }
            else
            {
                return merged[totalLength / 2];
            }
        }
    }
}
