using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codecrafters_dns_server.src.Models;

namespace codecrafters_dns_server.src.Builders
{
    public class DnsHeaderBuilder : IDnsHeaderBuilder
    {
        private readonly DNSHeader dnsHeader;
        public DnsHeaderBuilder()
        {
            this.dnsHeader = new DNSHeader();
        }

        public IDnsHeaderBuilder SetID(ushort id)
        {
            this.dnsHeader.ID = id;
            return this;
        }

        public IDnsHeaderBuilder SetQueryResponseIndicator(bool queryResponseIndicator)
        {
            this.dnsHeader.QueryResponseIndicator = queryResponseIndicator;
            return this;
        }

        public IDnsHeaderBuilder SetOpCode(byte opcode)
        {
            this.dnsHeader.OpCode = opcode;
            return this;
        }

        public IDnsHeaderBuilder SetAuthoritativeAnswer(bool authoritativeAnswer)
        {
            this.dnsHeader.AuthoritativeAnswer = authoritativeAnswer;
            return this;
        }

        public IDnsHeaderBuilder SetTruncation(bool truncation)
        {
            this.dnsHeader.Truncation = truncation;
            return this;
        }

        public IDnsHeaderBuilder SetRecursionDesired(bool recursionDesired)
        {
            this.dnsHeader.RecursionDesired = recursionDesired;
            return this;
        }

        public IDnsHeaderBuilder SetRecursionAvailable(bool recursionAvailable)
        {
            this.dnsHeader.RecursionAvailable = recursionAvailable;
            return this;
        }

        public IDnsHeaderBuilder SetReserved(byte reserved)
        {
            this.dnsHeader.Reserved = reserved;
            return this;
        }

        public IDnsHeaderBuilder SetResponseCode(byte responseCode)
        {
            this.dnsHeader.ResponseCode = responseCode;
            return this;
        }

        public IDnsHeaderBuilder SetQuestionCount(ushort questionCount)
        {
            this.dnsHeader.QuestionCount = questionCount;
            return this;
        }

        public IDnsHeaderBuilder SetAnswerRecordCount(ushort answerRecordCount)
        {
            this.dnsHeader.AnswerRecordCount = answerRecordCount;
            return this;
        }

        public IDnsHeaderBuilder SetAuthorityRecordCount(ushort authorityRecordCount)
        {
            this.dnsHeader.AuthorityRecordCount = authorityRecordCount;
            return this;
        }

        public IDnsHeaderBuilder SetAdditionalRecordCount(ushort additionalRecordCount)
        {
            this.dnsHeader.AdditionalRecordCount = additionalRecordCount;
            return this;
        }

        public DNSHeader Build()
        {
            return this.dnsHeader;
        }
    }
}
