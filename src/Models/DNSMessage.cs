using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src.Models
{
    public class DNSMessage
    {
        private readonly DNSHeader header;
        private readonly DNSQuestion question;
        
        public DNSMessage(DNSHeader header, DNSQuestion dnsQuestion)
        {
            this.header = header;
            this.question = dnsQuestion;
        }

        public byte[] ToByteArray()
        {
            var bytes = new List<byte>();
            bytes.AddRange(this.header.ToByteArray());
            bytes.AddRange(question.ToByteArray());
            return bytes.ToArray();
        }
    }
}
