using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src.Models
{
    public static class Utility
    {
        public static void print2Hex(byte[] array)
        {
            string hexRepresentation = BitConverter.ToString(array).Replace("-", " ");
            string asciiRepresentaiton = ByteArrayToAsciiString(array);
            int length = hexRepresentation.Length;
            for (int i = 0; i < array.Length; i += 8)
            {
                Console.Write(hexRepresentation.Substring(3 * i, Math.Min(24, length - 3 * i)));
                if (3 * i + 24 > length)
                {
                    int j = 3 * i + 24 - length + 1;
                    for (; j > 0; j--)
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine(" " + asciiRepresentaiton.Substring(2 * i, Math.Min(16, asciiRepresentaiton.Length - 2 * i)));
            }
        }

        public static void addBigEndianToList(List<byte> list, int value, int length)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                list.Add((byte)((value >> (i * 8)) & 0xFF));
            }
        }

        public static void addBigEndianToList(List<byte> list, uint value, int length)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                list.Add((byte)((value >> (i * 8)) & 0xFF));
            }
        }

        static bool IsPrintableAscii(byte b)
        {
            return b >= 32 && b <= 126; // Printable ASCII range
        }


        public static string ByteArrayToAsciiString(byte[] byteArray)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in byteArray)
            {
                if (IsPrintableAscii(b))
                {
                    sb.Append((char)b);
                }
                else
                {
                    sb.Append(".");
                }
                sb.Append(" ");
            }

            return sb.ToString();
        }

        public static IPEndPoint? getIpFromString(string resolverAddress)
        {
            string[] parts = resolverAddress.Split(':');
            if (parts.Length == 2 && IPAddress.TryParse(parts[0], out IPAddress? ipAddress) && int.TryParse(parts[1], out int port))
            {
                return new IPEndPoint(ipAddress, port);
            }
            return null;
        }


        public static void WriteBinaryDataToFile(byte[] binaryData, string filePath)
        {
            // Open a file stream for writing
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                // Write the binary data to the file
                fileStream.Write(binaryData, 0, binaryData.Length);
            }
        }
    }
}
