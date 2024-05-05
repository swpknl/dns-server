using System;
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

        public DNSQuestion(List<string> labels, DNSType dnsType, DNSClass dnsClass)
        {
            this.Labels = labels;
            this.Type = dnsType;
            this.Class = dnsClass;
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
