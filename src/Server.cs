using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_dns_server.src.Builders;
using codecrafters_dns_server.src.Models;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
// // Resolve UDP address
UdpClient resolverUdpClient = null;
if (args.Length > 0 && args[0] == "--resolver")
{
    var resolverAddress = args[1];
    var resolverIpAddress = IPEndPoint.Parse(resolverAddress);
    resolverUdpClient = new UdpClient(resolverIpAddress.Address.ToString(), resolverIpAddress.Port);
}
IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 2053;
IPEndPoint udpEndPoint = new IPEndPoint(ipAddress, port);

// Create UDP socket
UdpClient udpClient = new UdpClient(udpEndPoint);

while (true)
{
    // Receive data
    IPEndPoint sourceEndPoint = new IPEndPoint(IPAddress.Any, 0);
    byte[] response = null;
    if (resolverUdpClient is not null && args.Length > 0)
    {
        byte[] receivedData = (await udpClient.ReceiveAsync()).Buffer;
        string receivedString = Encoding.ASCII.GetString(receivedData);
        Console.WriteLine($"Received {receivedData.Length} bytes from {sourceEndPoint}: {receivedString}");
        var dnsHeaderQuery = new DNSHeader().FromBytes(receivedData);
        Console.WriteLine(dnsHeaderQuery.ResponseCode);
        // Create an empty response
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
            .SetAnswerRecordCount(dnsHeaderQuery.QuestionCount)
            .SetAuthorityRecordCount(dnsHeaderQuery.AuthorityRecordCount)
            .SetAdditionalRecordCount(dnsHeaderQuery.AdditionalRecordCount)
            .Build();
        List<DNSQuestion> questions = new List<DNSQuestion>();
        List<DNSAnswer> answers = new List<DNSAnswer>();
        var resolverQuery = resolverUdpClient.Send(receivedData);
        var resolverResponse = await resolverUdpClient.ReceiveAsync();
        response = resolverResponse.Buffer;
        Console.WriteLine("Response : " + Encoding.UTF8.GetString(response));
    }
    else
    {
        byte[] receivedData = (await udpClient.ReceiveAsync()).Buffer;
        string receivedString = Encoding.ASCII.GetString(receivedData);
        Console.WriteLine($"Received {receivedData.Length} bytes from {sourceEndPoint}: {receivedString}");
        var dnsHeaderQuery = new DNSHeader().FromBytes(receivedData);
        Console.WriteLine(dnsHeaderQuery.ResponseCode);
        // Create an empty response
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
        List<DNSQuestion> questions = new List<DNSQuestion>();
        List<DNSAnswer> answers = new List<DNSAnswer>();
        var offset = 12;
        for (int i = 0; i < dnsHeader.QuestionCount; i++)
        {
            var questionQuery = new DNSQuestion().FromBytes(receivedData[offset..], out offset);
            offset += 12;
            Console.WriteLine(string.Concat(questionQuery.Labels));
            var question = new DNSQuestion(questionQuery.Labels, DNSType.A, DNSClass.IN);
            questions.Add(question);
        }

        foreach (var question in questions)
        {
            var answer = new DNSAnswer(question.Labels, DNSType.A, DNSClass.IN, 60, 4, [8, 8, 8, 8]);
            Console.WriteLine(string.Concat(answer.Labels));
            answers.Add(answer);
        }

        var message = new DNSMessage(dnsHeader, questions, answers);
        response = message.ToByteArray();

    }


    // Send response
    udpClient.Send(response, response.Length, sourceEndPoint);
}

