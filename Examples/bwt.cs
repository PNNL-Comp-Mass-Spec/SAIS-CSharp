/*
 * bwt.cs for SAIS-CSharp
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
    public static
    void
    Main(string[] args)
    {
      if(args.Length != 2) { System.Console.WriteLine(@"usage: bwt INFILE OUTFILE"); return; }

      System.Console.Write(args[0] + @" ... ");

      byte[] T = System.IO.File.ReadAllBytes(args[0]);
      int[] A = new int[T.Length];

      System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
      sw.Start();
      int pidx = SAIS.bwt(T, T, A, T.Length);
      sw.Stop();

      double sec = (double)sw.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
      System.Console.WriteLine(@"{0:f4} sec", sec);

      using (System.IO.FileStream outfs = new System.IO.FileStream(
          args[1],
          System.IO.FileMode.Create,
          System.IO.FileAccess.Write))
      {
        byte[] U = new byte[4];
        U[0] = (byte)(pidx & 0xff); U[1] = (byte)((pidx >> 8) & 0xff);
        U[2] = (byte)((pidx >> 16) & 0xff); U[3] = (byte)((pidx >> 24) & 0xff);
        outfs.Write(U, 0, U.Length);
        outfs.Write(T, 0, T.Length);
        U = null;
      }

      T = null;
      A = null;
    }
  }
}
