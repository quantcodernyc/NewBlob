using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 
Assumptions:
1. There is single blob shape in 10x10 array. 
2. Data Quality: Presumed supplied 2D array has valid blob and correct binary values.
 * 
Credits & Comments
1. Google - blob related information and techniques
2. GitHub seach and found promising java solution by author "dnance" & other solutions available
3. What did I do? 
    A. First I converted code to C# (~ 20 minutes)
    B. Spent next 2 hours trying various solutions 
4. Time Spent - Approx 3 hours dedicated plus some not dedicated time thinking about problem
5. How to improve this solution: I would like to submit 2 solutions and ideas where I would like to invest my time to improve this solution.
   Solution 1: Blob.cs --> This solution works in majority of tests I performed to find blob boundary, however it has higher cell reads compared to NewBlob.cs
   There are instances where this algorithm read single cell twice which can be optimized. I tried same in NewBlob.cs 
   Sotion 2: NewBlob.cs --> This solution is very efficient in terms on cell reads, however in fails to identify boundary correctly where blob has singular linear strings flowing out at edges. 
   This solution should work very efficiently for 99% natural blobs.
   Summary: I would invest more time on Solution 2 with code which limit left and right boundaries limitations and fall back to regular limit finding when left > 5 or right < 5 
   Testing: In order for best usage of time, I relied on breaking solution approach by supplying extreme cases where solution would fail. Below is one example of blob I chose to fail my Solution Blob2.cs
   I also checked Mathnet Numerics library to see if there is ready library available to fill 2D array with binary values which qualify as blob. 
   I would like to replace integer 2D array with BitArray which should help in terms of scalability and performance exponentially by taking very less memory and faster computing.
   For Solution 1: Cell reads are minimized by order of selecting if we find right boundary first. Predictive model would help to improve this solution. 

 Sample Blob to break Solution - NewBlob
 
 int[,] blob = {
                                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                    {0, 0, 1, 1, 1, 0, 0, 0, 1, 0},
                                    {0, 0, 1, 1, 1, 1, 1, 0, 1, 0},
                                    {0, 1, 1, 0, 0, 0, 1, 0, 1, 0},
                                    {0, 1, 1, 1, 1, 1, 1, 0, 1, 0},
                                    {0, 1, 0, 0, 1, 0, 1, 0, 1, 0},
                                    {0, 1, 0, 0, 1, 0, 1, 1, 1, 0},
                                    {0, 1, 0, 0, 1, 1, 1, 0, 1, 0},
                                    {0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
                                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                           };
 
 
*/


namespace sample
{
    class BlobBoundaryFinder
    {
        private int totalReads = 0;
        private int top = 0;
        private int bottom = 9;
        private int left = 0;
        private int right = 9;
        private int[,] blob;

        public BlobBoundaryFinder(int[,] blob)
        {
            this.blob = blob;
            init();
        }

        private int readBlobValue(int i, int j)
        {
            int value = 0;

            try
            {
                value = blob[i, j];
                totalReads++;
            }
            catch (IndexOutOfRangeException e)
            {
            }

            return value;
        }

        private Boolean checkPreviousColumn(int i, int j)
        {
            Boolean previous = false;

            for (; i <= bottom; i = i + 2)
            {
                if (readBlobValue(i, j) == 1)
                {
                    previous = true;
                    break;
                }
            }

            return previous;
        }

        private Boolean checkPreviousColumnfull(int i, int j)
        {
            Boolean previous = false;

            for (; i <= bottom; i = i + 1)
            {
                if (readBlobValue(i, j) == 1)
                {
                    previous = true;
                    break;
                }
            }

            return previous;
        }

        private Boolean checkPreviousRow(int i, int j)
        {
            Boolean previous = false;

            for (; j <= right; j = j + 2)
            {
                if (readBlobValue(i, j) == 1)
                {
                    previous = true;
                    break;
                }
            }

            return previous;
        }

        private Boolean checkPreviousRowfull(int i, int j)
        {
            Boolean previous = false;

            for (; j <= right; j = j + 1)
            {
                if (readBlobValue(i, j) == 1)
                {
                    previous = true;
                    break;
                }
            }

            return previous;
        }

        private void findRight()
        {
            int offset = 0;

            if (right != 9)
            {

                for (int i = bottom; i >= top; i--)
                {
                    int value = readBlobValue(i, right + 1);

                    if (value == 1 && right < 9)
                    {
                        right++;
                        i++;
                    }
                    else if (right == 9)
                    {
                        return;
                    }
                }
                return;
            }

            for (int j = 9; j >= left; j--)
            {
                offset = top + (j % 2);

                for (int i = offset; i <= bottom; i = i + 2)
                {
                    int value = readBlobValue(i, j);

                    if (value == 1)
                    {
                        right = ((checkPreviousColumn(i, j + 1)) ? j + 1 : j);
                        return;
                    }
                }
            }
            return;
        }

        private void findLeft()
        {
            int offset = 0;

            if (left != 0)
            {
                left--;
                for (int i = top; i <= bottom; i++)
                {
                    int value = readBlobValue(i, left - 1);

                    if (value == 1 && left > 0)
                    {
                        left--;
                        i--;
                    }
                    else if (left == 0)
                    {
                        return;
                    }
                }
                return;
            }
            else
            {
                for (int j = 0; j <= right; j++)
                {
                    offset = top + (j % 2);

                    for (int i = offset; i <= bottom; i = i + 2)
                    {
                        int value = readBlobValue(i, j);

                        if (value == 1)
                        {
                            left = ((checkPreviousColumn(i, j - 1)) ? j - 1 : j);
                            return;
                        }
                    }
                }
            }
            return;
        }

        private void findTop()
        {
            int offset = 0;

            for (int i = 0; i <= bottom; i++)
            {
                offset = left + (i % 2);

                for (int j = offset; j <= right; j = j + 2)
                {
                    int value = readBlobValue(i, j);

                    if (value == 1)
                    {
                        top = ((checkPreviousRow(i - 1, j)) ? i - 1 : i);
                        left = j;
                        return;
                    }
                }
            }
            return;
        }

        private void findBottom()
        {
            int offset = 0;

            for (int i = 9; i >= top; i--)
            {
                offset = right - (i % 2);

                for (int j = offset; j >= left; j = j - 2)
                {
                    int value = readBlobValue(i, j);
                    if (value == 1)
                    {
                        bottom = ((checkPreviousRow(i + 1, j)) ? i + 1 : i);
                        right = j;
                        return;
                    }
                }

                /*
                offset = left + (i % 2);

                for (int j = offset; j <= right; j = j + 2)
                {
                    int value = readBlobValue(i, j);
                    if (value == 1)
                    {
                        bottom = ((checkPreviousRow(i + 1, j)) ? i + 1 : i);
                        return;
                    }
                }
                */
            }

            return;
        }

        public void init()
        {

            findTop();
            findLeft();
            findBottom();
            findRight();
            //LBRT - 52, TLBR - 54, TBLR - 54, BTLR - 54, RBLT - 51, RLBT - 51, TBLR - 
        }

        public int getTotalReads()
        {
            return totalReads;
        }

        public int getTop()
        {
            return top;
        }

        public int getBottom()
        {
            return bottom;
        }

        public int getLeft()
        {
            return left;
        }

        public int getRight()
        {
            return right;
        }

        static void Main(String[] args)
        {
            #region commentary
            //Ideally I would like to fill 2D array with random boolean values
            /*
            string text = System.IO.File.ReadAllText(@"C:\temp\sample.txt");
            char[] delimiterChars = { ' ', ',', '.', ':', '{', '}', '\t', '\n', '\r' };
            string[] str = text.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
            // Display the file contents to the console. Variable text is a string.
            System.Console.WriteLine("Contents of sample.txt = {0}", text);
            */

            /*
             //This part of code fill jagged array with 0 & 1 but not guaranteed blob, with more time I would like to refine this part
                    int[,] test = new int[10, 10];
                    int min = 0;
                    int max = 1;
                    Random rand = new Random();
                    int[][] testblob = new int[10][];
                    for (int x = 0; x < 10; x++)
                    {
                        testblob[x] = Enumerable
                                        .Repeat(0, 10)
                                        .Select(i => rand.Next(0, 1))
                                        .ToArray();
                    }
            */
            #endregion commentary


            int[,] blob = {
                                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                    {0, 0, 1, 1, 1, 0, 0, 0, 0, 0},
                                    {0, 0, 1, 1, 1, 1, 1, 0, 0, 0},
                                    {0, 0, 1, 0, 0, 0, 1, 0, 0, 0},
                                    {0, 0, 1, 1, 1, 1, 1, 0, 0, 0},
                                    {0, 0, 0, 0, 1, 0, 1, 0, 0, 0},
                                    {0, 0, 0, 0, 1, 0, 1, 0, 0, 0},
                                    {0, 0, 0, 0, 1, 1, 1, 0, 0, 0},
                                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                               };

            BlobBoundaryFinder finder = new BlobBoundaryFinder(blob);
            Console.WriteLine("Cell Reads: " + finder.getTotalReads());
            Console.WriteLine("Top: " + finder.getTop());
            Console.WriteLine("Left: " + finder.getLeft());
            Console.WriteLine("Bottom: " + finder.getBottom());
            Console.WriteLine("Right: " + finder.getRight());
            Console.Read();

        }
    }
}