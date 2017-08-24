/*
 * unbwt.cs for SAIS-CSharp
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
    unbwt(byte[] T, int n, int idx) {
      int i, p, len, half, m, c;
      /* Check arguments. */
      if((n < 0) || (idx < 0) || (n < idx) || ((0 < n) && (idx == 0))) {
        return -1;
      }
      if(n <= 1) { return 0; }
      /* Inverse BW transform. */
      int[] C = new int[256];
      for(c = 0; c < 256; ++c) { C[c] = 0; }
      for(i = 0; i < n; ++i) { ++C[T[i]]; }
      for(c = 0, i = 0; c < 256; ++c) { p = C[c]; C[c] = i; i += p; }
      int[] A = new int[n];
      for(i = 0; i < n; ++i) { A[i] = C[T[i]]++; }
      for(i = n - 1, p = 0; 0 <= i; --i) {
        p = A[p];
        for(m = 0, len = 256, half = len >> 1; 0 < len; len = half, half >>= 1) {
          if(C[m + half] <= p) { m += half + 1; half -= (len & 1) ^ 1; }
        }
        T[i] = (byte)m;
        if(p < idx) { ++p; }
      }
      A = null; C = null;
      return 0;
    }

    public static
    void
    Main(string[] args)
    {
      using (System.IO.FileStream fs = new System.IO.FileStream(
              args[0],
              System.IO.FileMode.Open,
              System.IO.FileAccess.Read))
      {
        System.Console.Write(args[0] + @" ... ");
        byte[] T = new byte[fs.Length - 4];
        byte[] U = new byte[4];
        fs.Read(U, 0, U.Length);
        fs.Read(T, 0, T.Length);
        int pidx = U[0] | (U[1] << 8) | (U[2] << 16) | (U[3] << 24);

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        unbwt(T, T.Length, pidx);
        sw.Stop();

        double sec = (double)sw.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
        System.Console.WriteLine(@"{0:f4} sec", sec);

        System.IO.File.WriteAllBytes(args[1], T);
        T = null;
      }
    }
  }
}
