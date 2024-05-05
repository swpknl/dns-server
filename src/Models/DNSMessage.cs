using System;
using System.Collections.Generic;
using System.Linq;
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
            var result = new ReadOnlySpan<byte>(new byte[1024]);
            var header = result[..12];
            header = this.header.ToByteArray();
            var question = result[12..];
            question = this.question.ToByteArray();
            return result.ToArray();
        }
    }
}
