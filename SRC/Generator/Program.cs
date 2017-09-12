using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            var uuid = new uint[]{0xBADC1234, 0x12345678, 0x146790EF, 0xABCD4321};
            var cdk = new uint[4];
            Console.WriteLine("请输入用户码：");
            var usrCodeStr = Console.ReadLine();
            for (var index = 0; index != 4; ++index)
            {
                var lusrCode = uint.Parse(usrCodeStr.Substring(index * 8, 8), NumberStyles.HexNumber);
                cdk[index] = uuid[index] ^ lusrCode;
                var lstr = String.Format("{0:X8}", cdk[index]);
                Console.Write(lstr);
            }
            Console.WriteLine();
        }
    }
}
