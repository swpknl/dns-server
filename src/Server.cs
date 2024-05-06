using System;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using codecrafters_dns_server.src.Builders;
using codecrafters_dns_server.src.Models;

UdpClient resolverUdpClient = null;
IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 2053;
IPEndPoint udpEndPoint = new IPEndPoint(ipAddress, port);
byte[] response = null;
UdpClient udpClient = new UdpClient(udpEndPoint);
IPEndPoint resolverEndPoint = null;
IPEndPoint sourceEndPoint = new IPEndPoint(IPAddress.Any, 0);
if (args.Length == 2 && args[0] == "--resolver")
{
    var resolverAddress = args[1];
    resolverEndPoint = IPEndPoint.Parse(resolverAddress);
    resolverUdpClient = new UdpClient(resolverEndPoint.Address.ToString(), resolverEndPoint.Port);
}

while (true)
{
    byte[] receivedData = (udpClient.Receive(ref sourceEndPoint));
    var dnsHeaderQuery = new DNSHeader().FromBytes(receivedData);
    List<DNSQuestion> questions = new List<DNSQuestion>();
    List<DNSAnswer> answers = new List<DNSAnswer>();
    var dnsHeader = GetDNSHeader(dnsHeaderQuery);
    if (resolverEndPoint != null)
    {
        await GetAnswerFromDNSForwarder(dnsHeaderQuery, receivedData, questions, dnsHeader, answers);
        dnsHeader.AnswerRecordCount = (ushort)questions.Count;
        dnsHeader.QuestionCount = (ushort)answers.Count;
    }
    else
    {
        GetAnswerFromDnsQuestion(dnsHeader, receivedData, questions, answers);
    }

    var dnsMessage = new DNSMessage(dnsHeader, questions, answers);
    response = dnsMessage.ToByteArray();
    await udpClient.SendAsync(response, sourceEndPoint);
}

DNSHeader GetDNSHeader(DNSHeader dnsHeaderQuery)
{
    var dnsHeader = new DnsHeaderBuilder().SetID(dnsHeaderQuery.ID)
        .SetQueryResponseIndicator(dnsHeaderQuery.QueryResponseIndicator)
        .SetOpCode(dnsHeaderQuery.OpCode)
        .SetAuthoritativeAnswer(dnsHeaderQuery.AuthoritativeAnswer)
        .SetTruncation(dnsHeaderQuery.Truncation)
        .SetRecursionDesired(dnsHeaderQuery.RecursionDesired)
        .SetRecursionAvailable(dnsHeaderQuery.RecursionAvailable)
        .SetReserved(dnsHeaderQuery.Reserved)
        .SetResponseCode(dnsHeaderQuery.OpCode == 0 ? (byte)0 : (byte)4)
        .SetQuestionCount(dnsHeaderQuery.QuestionCount)
        .SetAnswerRecordCount(dnsHeaderQuery.AnswerRecordCount)
        .SetAuthorityRecordCount(dnsHeaderQuery.AuthorityRecordCount)
        .SetAdditionalRecordCount(dnsHeaderQuery.AdditionalRecordCount)
        .Build();
    return dnsHeader;
}

async Task GetAnswerFromDNSForwarder(DNSHeader dnsHeaderQuery1, byte[] bytes, List<DNSQuestion> dnsQuestions, DNSHeader dnsHeader, List<DNSAnswer> dnsAnswers)
{
    var offset = 0;
    for (int i = 0; i < dnsHeaderQuery1.QuestionCount; i++)
    {
        offset += 12;
        var array = bytes.ToArray();
        var questionQuery = new DNSQuestion().FromBytes(array[offset..], out offset);
        offset -= offset;
        var question = new DNSQuestion(questionQuery.Labels, DNSType.A, DNSClass.IN);
        dnsQuestions.Add(question);
        resolverUdpClient.Send(new DNSMessage(dnsHeader.Copy(1), new List<DNSQuestion>() { question }, new List<DNSAnswer>()).ToByteArray());
        var resolverResponse = await resolverUdpClient.ReceiveAsync();
        var rsp =
            new DNSMessage().Read(resolverResponse.Buffer);
        dnsAnswers.AddRange(rsp.answers);
    }
}

void GetAnswerFromDnsQuestion(DNSHeader dnsHeader1, byte[] receivedData, List<DNSQuestion> questions, List<DNSAnswer> answers)
{
    var offset = 12;
    for (int i = 0; i < dnsHeader1.QuestionCount; i++)
    {
        var questionQuery = new DNSQuestion().FromBytes(receivedData[offset..], out offset);
        offset += 12;
        var question = new DNSQuestion(questionQuery.Labels, DNSType.A, DNSClass.IN);
        questions.Add(question);
    }

    foreach (var question in questions)
    {
        var answer = new DNSAnswer(question.Labels, DNSType.A, DNSClass.IN, 60, 4, [8, 8, 8, 8]);
        answers.Add(answer);
    }
}
