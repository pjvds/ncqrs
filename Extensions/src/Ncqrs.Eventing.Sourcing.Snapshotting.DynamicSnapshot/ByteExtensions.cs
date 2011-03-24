using System;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public static class ByteExtensions
    {
        public static int Find(this byte[] self, byte[] pattern)
        {
            int m = 0;
            int i = 0;
            var t = ComputeFailureTable(pattern);

            while (m + i < self.Length)
            {
                if (pattern[i] == self[m + i])
                {
                    if (i == pattern.Length - 1)
                        return m;
                    ++i;
                }
                else
                {
                    m += i - t[i];
                    i = (t[i] > -1) ? t[i] : 0;
                }
            }

            return -1;
        }

        public static bool Replace(this byte[] self, byte[] patternToReplace, byte[] newPattern)
        {
            if (patternToReplace.Length != newPattern.Length)
                return false;

            int pos = Find(self, patternToReplace);

            if (pos != -1)
            {
                newPattern.CopyTo(self, pos);
            }

            return (pos != -1);
        }

        private static int[] ComputeFailureTable(byte[] pattern)
        {
            int[] t = new int[pattern.Length];

            t[0] = -1;
            t[1] = 0;

            int i = 2;
            int j = 0;

            while (i < pattern.Length)
            {
                if (pattern[i - 1] == pattern[j])
                {
                    t[i++] = ++j;
                }
                else if (j > 0)
                {
                    j = t[j];
                }
                else
                {
                    t[i++] = 0;
                }
            }

            return t;
        }
    }
}
