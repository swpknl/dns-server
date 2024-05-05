using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src.Models
{
    public class DNSAnswer
    {
        public List<string> Labels { get; set; }
        public DNSType Type { get; set; }
        public DNSClass Class { get; set; }
        public byte TTL { get; set; }
        public ushort Length { get; set; }
        public ushort Data { get; set; }

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
            bytes.Add((byte)(TimeToLive >> 24));
            bytes.Add((byte)(TimeToLive >> 16));
            bytes.Add((byte)(TimeToLive >> 8));
            bytes.Add((byte)(TimeToLive & 0xFF));
            bytes.Add((byte)(Data.Count >> 8));
            bytes.Add((byte)(Data.Count & 0xFF));
            bytes.AddRange(Data);

            return new byte[] { };
        }
    }
}
