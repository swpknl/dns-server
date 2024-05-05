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
        
        public DNSMessage(DNSHeader header)
        {
            this.header = header;
        }

        public byte[] ToByteArray()
        {
            return this.header.ToByteArray();
        }
    }
}
