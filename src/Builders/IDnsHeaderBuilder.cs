using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codecrafters_dns_server.src.Models;

namespace codecrafters_dns_server.src.Builders
{
    public interface IDnsHeaderBuilder
    {
        IDnsHeaderBuilder SetID(ushort id);
        IDnsHeaderBuilder SetQueryResponseIndicator(bool queryResponseIndicator);
        IDnsHeaderBuilder SetOpCode(byte opcode);
        IDnsHeaderBuilder SetAuthoritativeAnswer(bool authoritativeAnswer);
        IDnsHeaderBuilder SetTruncation(bool truncation);
        IDnsHeaderBuilder SetRecursionDesired(bool recursionDesired);
        IDnsHeaderBuilder SetRecursionAvailable(bool recursionAvailable);
        IDnsHeaderBuilder SetReserved(byte reserved);
        IDnsHeaderBuilder SetResponseCode(byte responseCode);
        IDnsHeaderBuilder SetQuestionCount(ushort questionCount);
        IDnsHeaderBuilder SetAnswerRecordCount(ushort answerRecordCount);
        IDnsHeaderBuilder SetAuthorityRecordCount(ushort authorityRecordCount);
        IDnsHeaderBuilder SetAdditionalRecordCount(ushort additionalRecordCount);
        DNSHeader Build();
    }
}
