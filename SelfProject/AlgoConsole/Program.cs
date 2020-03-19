using System;
using System.Collections.Generic;

namespace AlgoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            List<int> list = new List<int> { 4, 13, 2, 3 };
            list.Sort();
            foreach (var i in list)
            {
                Console.WriteLine(i);
            }

            //List<int> seqList = new List<int>();
            int maxSeq = 0;
            int m = 0;
            for (int i = 0; i < list.Count; i++) {
                if ((i + 1 != list.Count) && list[i + 1] - list[i] <= 1)
                {
                    m++;
                }
                else
                {
                    m++;
                    if (m > maxSeq) maxSeq = m;
                    m = 0;
                }
            }
            Console.WriteLine(maxSeq);

            Console.ReadLine();
        }
    }

    class Matrix
    {
        private int[,] mat = new int[5, 5];

        public int this[int i, int j]
        {
            set {
                mat[i, j] = value;
            }
        }
    }
}
