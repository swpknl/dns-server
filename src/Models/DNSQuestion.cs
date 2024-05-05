using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src.Models
{
    public class DNSQuestion
    {
        public List<string> Labels { get; set; }
        public DNSType Type { get; set; }
        public DNSClass Class { get; set; }

        public DNSQuestion()
        {
            
        }


        public DNSQuestion(List<string> labels, DNSType dnsType, DNSClass dnsClass)
        {
            this.Labels = labels;
            this.Type = dnsType;
            this.Class = dnsClass;
        }

        public DNSQuestion FromBytes(byte[] array)
        {
            var readonlySpan = new ReadOnlySpan<byte>(array);
            var labelArray = Encoding.UTF8.GetString(readonlySpan[..^4]);
            Console.WriteLine(labelArray);
            var type = BinaryPrimitives.ReadInt16BigEndian(readonlySpan[^4..]);
            var @class = BinaryPrimitives.ReadInt16BigEndian(readonlySpan[^2..]);
            return new DNSQuestion()
            {
                Labels = labelArray.Split(".").ToList(),
                Type = (DNSType)(type),
                Class = (DNSClass)(@class)
            };
        }

        public byte[] ToByteArray()
        {
            var bytes =
                new List<byte>(Labels.Count + 1 + Labels.Sum(x => x.Length) + 4);
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
            return bytes.ToArray();
        }
    }
}
