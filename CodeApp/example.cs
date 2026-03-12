

public class NextGreaterElement
{
    public NextGreaterElement()
    {
        int[] nums1 = { 4, 1, 2 };
        int[] nums2 = { 1, 3, 4, 2 };
        int[] result = NextGreaterElements(nums1, nums2);
        Console.WriteLine(string.Join(", ", result)); // Output: [-1, 3, -1]
    }

    public int[] NextGreaterElements(int[] nums1, int[] nums2)
    {
        Dictionary<int, int> nextGreaterMap = new Dictionary<int, int>();
        Stack<int> stack = new Stack<int>();

        // Build the next greater element map for nums2
        foreach (int num in nums2)
        {
            while (stack.Count > 0 && stack.Peek() < num)
            {
                nextGreaterMap[stack.Pop()] = num;
            }
            stack.Push(num);
        }

        // For remaining elements in the stack, there is no greater element
        while (stack.Count > 0)
        {
            nextGreaterMap[stack.Pop()] = -1;
        }

        // Prepare the result for nums1 based on the next greater map
        int[] result = new int[nums1.Length];
        for (int i = 0; i < nums1.Length; i++)
        {
            result[i] = nextGreaterMap.ContainsKey(nums1[i]) ? nextGreaterMap[nums1[i]] : -1;
        }

        return result;
    }
}