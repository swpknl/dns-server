using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace codecrafters_dns_server.src.Models
{
    public class DNSAnswer
    {
        public List<string> Labels { get; set; }
        public DNSType Type { get; set; }
        public DNSClass Class { get; set; }
        public byte TTL { get; set; }
        public ushort Length { get; set; }
        public List<byte> Data { get; set; }

        public DNSAnswer(List<string> labels, DNSType type, DNSClass dnsClass, byte ttl, ushort length, List<byte> data)
        {
            this.Labels = labels;
            this.Type = type;
            this.Class = dnsClass;
            this.TTL = ttl;
            this.Length = length;  
            this.Data = data;
        }

        public DNSAnswer()
        {
            
        }

        public static DNSAnswer Read(ReadOnlySpan<byte> buffer)
        {
            var count = 0;
            List<string> names = ReadLabels(buffer);
            var type = BinaryPrimitives.ReadUInt16BigEndian(buffer[count..]);
            count += 2;
            var @class = BinaryPrimitives.ReadUInt16BigEndian(buffer[count..]);
            count += 2;
            var ttl = BinaryPrimitives.ReadUInt32BigEndian(buffer[count..]);
            count += 4;
            var dataLength = BinaryPrimitives.ReadUInt16BigEndian(buffer[count..]);
            count += 2;
            var data = (buffer.Slice(count, dataLength).ToArray()).ToList();
            count += dataLength;
            var record = new DNSAnswer()
            {
                Labels = names,
                Type = (DNSType)type,
                Class = (DNSClass)@class,
                TTL = (byte)ttl,
                Data = data
            };
            return record;
        }

        private static List<string> ReadLabels(ReadOnlySpan<byte> buffer)
        {
            var labels = new List<string>();
            Console.WriteLine(Encoding.UTF8.GetString(buffer));
            var count = 0;
            // read until null termination
            while (buffer[0] != 0)
            {
                var strLen = buffer[0];
                Console.WriteLine(strLen);
                var str = Encoding.UTF8.GetString(buffer.Slice(1, strLen));
                labels.Add(str);
                buffer = buffer[(1 + strLen)..];
                count += 1 + strLen;
            }
            return labels;
        }

        public byte[] ToByteArray()
        {
            var bytes = new List<byte>();
            foreach (var label in Labels)
            {
                bytes.Add((byte)label.Length);
                bytes.AddRange(Encoding.ASCII.GetBytes(label));
            }

            bytes.Add(0);
            bytes.Add((byte)((short)Type >> 8));
            bytes.Add((byte)((short)Type & 0xFF));
            bytes.Add((byte)((short)Class >> 8));
            bytes.Add((byte)((short)Class & 0xFF));
            bytes.Add((byte)(TTL >> 24));
            bytes.Add((byte)(TTL >> 16));
            bytes.Add((byte)(TTL >> 8));
            bytes.Add((byte)(TTL & 0xFF));
            bytes.Add((byte)(Data.Count >> 8));
            bytes.Add((byte)(Data.Count & 0xFF));
            bytes.AddRange(Data);

            return bytes.ToArray();
        }
    }
}
