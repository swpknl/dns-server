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
        private readonly DNSAnswer answer;
        
        public DNSMessage(DNSHeader header, DNSQuestion dnsQuestion, DNSAnswer dnsAnswer)
        {
            this.header = header;
            this.question = dnsQuestion;
            this.answer = dnsAnswer;
        }

        public byte[] ToByteArray()
        {
            var bytes = new List<byte>();
            bytes.AddRange(this.header.ToByteArray());
            bytes.AddRange(question.ToByteArray());
            bytes.AddRange(answer.ToByteArray());
            return bytes.ToArray();
        }
    }
}
