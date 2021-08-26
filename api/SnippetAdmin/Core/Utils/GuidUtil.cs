using System;
using System.Security.Cryptography;

namespace SnippetAdmin.Core.Utils
{
    public class GuidUtil
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public static Guid NewSequentialGuid()
        {
            byte[] array = new byte[7];
            _rng.GetBytes(array);
            ulong ticks = (ulong)DateTimeOffset.UtcNow.Ticks;
            ushort num = 4;
            ushort num2 = 8;
            ushort num3 = (ushort)((ticks << 48 >> 52) | (ushort)(num << 12));
            byte b = (byte)((ticks << 60 >> 60) | (byte)(num2 << 4));
            if (!BitConverter.IsLittleEndian)
            {
                byte[] array2 = new byte[16];
                byte[] bytes = BitConverter.GetBytes(ticks);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes);
                }
                Buffer.BlockCopy(bytes, 0, array2, 0, 6);
                array2[6] = (byte)(num3 << 8 >> 8);
                array2[7] = (byte)(num3 >> 8);
                array2[8] = b;
                Buffer.BlockCopy(array, 0, array2, 9, 7);
                return new Guid(array2);
            }
            return new Guid((uint)(ticks >> 32), (ushort)(ticks << 32 >> 48), num3, b, array[0], array[1], array[2], array[3], array[4], array[5], array[6]);
        }
    }
}