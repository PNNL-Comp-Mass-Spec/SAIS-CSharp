/*
 * suftest.cs for SAIS-CSharp
 * Copyright (c) 2010 Yuta Mori. All Rights Reserved.
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using SuffixArray;

namespace suftest
{
  class MainClass
  {
    private static
    int
    check(byte[] T, int[] SA, int n, bool verbose)
    {
      int[] C = new int[256];
      int i, p, q, t;
      int c;

      if (verbose) { System.Console.Write(@"sufcheck: "); }
      if (n == 0)
      {
        if (verbose) { System.Console.WriteLine("Done."); }
        return 0;
      }

      /* Check arguments. */
      if ((T == null) || (SA == null) || (n < 0))
      {
        if (verbose) { System.Console.WriteLine("Invalid arguments."); }
        return -1;
      }

      /* check range: [0..n-1] */
      for (i = 0; i < n; ++i)
      {
        if ((SA[i] < 0) || (n <= SA[i]))
        {
          if (verbose)
          {
            System.Console.WriteLine("Out of the range [0," + (n - 1) + "].");
            System.Console.WriteLine("  SA[" + i + "]=" + SA[i]);
          }
          return -2;
        }
      }

      /* check first characters. */
      for (i = 1; i < n; ++i)
      {
        if (T[SA[i - 1]] > T[SA[i]])
        {
          if (verbose)
          {
            System.Console.WriteLine("Suffixes in wrong order.");
            System.Console.Write("  T[SA[" + (i - 1) + "]=" + SA[i - 1] + "]=" + T[SA[i - 1]]);
            System.Console.WriteLine(" > T[SA[" + i + "]=" + SA[i] + "]=" + T[SA[i]]);
          }
          return -3;
        }
      }

      /* check suffixes. */
      for (i = 0; i < 256; ++i) { C[i] = 0; }
      for (i = 0; i < n; ++i) { ++C[T[i]]; }
      for (i = 0, p = 0; i < 256; ++i)
      {
        t = C[i];
        C[i] = p;
        p += t;
      }

      q = C[T[n - 1]];
      C[T[n - 1]] += 1;
      for (i = 0; i < n; ++i)
      {
        p = SA[i];
        if (0 < p)
        {
          c = T[--p];
          t = C[c];
        }
        else
        {
          c = T[p = n - 1];
          t = q;
        }
        if ((t < 0) || (p != SA[t]))
        {
          if (verbose)
          {
            System.Console.WriteLine("Suffixes in wrong position.");
            System.Console.WriteLine("  SA[" + t + "]=" + ((0 <= t) ? SA[t] : -1) + " or");
            System.Console.WriteLine("  SA[" + i + "]=" + SA[i]);
          }
          return -4;
        }
        if (t != q)
        {
          ++C[c];
          if ((n <= C[c]) || (T[SA[C[c]]] != c)) { C[c] = -1; }
        }
      }
      C = null;

      if (verbose) { System.Console.WriteLine("Done."); }
      return 0;
    }

    public static
    void
    Main(string[] args)
    {
      int i;
      for (i = 0; i < args.Length; ++i)
      {
        using (System.IO.FileStream fs = new System.IO.FileStream(
                args[i],
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read))
        {
          System.Console.Write(args[i] + @": {0:d} bytes ... ", fs.Length);
          byte[] T = new byte[fs.Length];
          int[] SA = new int[fs.Length];
          fs.Read(T, 0, T.Length);

          System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
          sw.Start();
          SAIS.sufsort(T, SA, T.Length);
          sw.Stop();

          double sec = (double)sw.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
          System.Console.WriteLine(@"{0:f4} sec", sec);

          check(T, SA, T.Length, true);
          T = null;
          SA = null;
        }
      }
    }
  }
}
