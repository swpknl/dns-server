using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace codecrafters_dns_server.src.Models
{
    public class DNSHeader
    {
        public ushort ID { get; set; }
        public bool QueryResponseIndicator { get; set; }
        public byte OpCode { get; set; }
        public bool AuthoritativeAnswer { get; set; }
        public bool Truncation { get; set; }
        public bool RecursionDesired { get; set; }
        public bool RecursionAvailable { get; set; }
        public byte Reserved { get; set; }
        public byte ResponseCode { get; set; }
        public ushort QuestionCount { get; set; }
        public ushort AnswerRecordCount { get; set; }
        public ushort AuthorityRecordCount { get; set; }
        public ushort AdditionalRecordCount { get; set; }

        public byte[] ToByteArray()
        {
            var bytes = new byte[12];
            var span = bytes.AsSpan();
            PutInt16ToSpan(span[..2], ID);
            byte flags1 = 0;
            byte flags2 = 0;
            if (QueryResponseIndicator)
            {
                flags1 |= 1 << 7;
            }
            // 0000 1111
            // 0 1111 0 0 0
            flags1 |= (byte)(OpCode << 3);
            if (AuthoritativeAnswer)
            {
                flags1 |= 1 << 2;
            }
            if (Truncation)
            {
                flags1 |= 1 << 1;
            }
            if (RecursionDesired)
            {
                flags1 |= 1;
            }
            if (RecursionAvailable)
            {
                flags2 |= 1 << 7;
            }
            flags2 |= ResponseCode;
            span[2] = flags1;
            span[3] = flags2;
            PutInt16ToSpan(span[4..6], QuestionCount);
            PutInt16ToSpan(span[6..8], AnswerRecordCount);
            PutInt16ToSpan(span[8..10], AuthorityRecordCount);
            PutInt16ToSpan(span[10..12], AdditionalRecordCount);
            return bytes;
        }

        private void PutInt16ToSpan(Span<byte> span, ushort value)
        {
            span[0] = (byte)(value >> 8);
            span[1] = (byte)(value & 0xFF);
        }

        public DNSHeader FromBytes(byte[] bytes)
        {
            var id = BinaryPrimitives.ReadUInt16BigEndian(bytes);
            var flags = bytes[2..];
            var flags1 = flags[0];
            var flags2 = flags[1];
            var isQuery = !GetBit(flags1, 1);
            var opCode = (byte)((flags1 >> 3) & 0xF);
            var isAuthoritativeAnswer = GetBit(flags1, 6);
            var truncation = GetBit(flags1, 7);
            var isRecursionDesired = GetBit(flags1, 8);
            var isRecursionAvailable = GetBit(flags2, 1);
            var responseCode = (byte)(flags2 & 0xF) == 0 ? (byte)0 : (byte)4;
            var questionCount = BinaryPrimitives.ReadUInt16BigEndian(bytes[4..]);
            var answerRecordCount = BinaryPrimitives.ReadUInt16BigEndian(bytes[6..]);
            var authorityRecordCount = BinaryPrimitives.ReadUInt16BigEndian(bytes[8..]);
            var additionalRecordCount = BinaryPrimitives.ReadUInt16BigEndian(bytes[10..]);
            return new DNSHeader()
            {
                ID = id,
                QuestionCount = questionCount,
                AnswerRecordCount = answerRecordCount,
                AuthorityRecordCount = authorityRecordCount,
                AdditionalRecordCount = additionalRecordCount,
                QueryResponseIndicator = isQuery,
                OpCode = opCode,
                AuthoritativeAnswer = isAuthoritativeAnswer,
                Truncation = truncation,
                RecursionDesired = isRecursionDesired,
                RecursionAvailable = isRecursionAvailable,
                ResponseCode = responseCode
            };
        }

        private bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << 8 - bitNumber)) != 0;
        }
    }
}
