using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using codecrafters_dns_server.src.Builders;
using codecrafters_dns_server.src.Models;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
// // Resolve UDP address
UdpClient resolverUdpClient = null;
IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 2053;
IPEndPoint udpEndPoint = new IPEndPoint(ipAddress, port);
byte[] response = null;
// Create UDP socket
UdpClient udpClient = new UdpClient(udpEndPoint);
IPEndPoint resolverEndPoint = null;
// Receive data
if (args.Length == 2 && args[0] == "--resolver")
{
    Console.WriteLine("Resolver is passed");
    var resolverAddress = args[1];
    resolverEndPoint = IPEndPoint.Parse(resolverAddress);
    resolverUdpClient = new UdpClient(resolverEndPoint.Address.ToString(), resolverEndPoint.Port);
}

while (true)
{
    if (resolverEndPoint != null)
    {
        Console.WriteLine("In custom");
        IPEndPoint sourceEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] receivedData = (udpClient.Receive(ref sourceEndPoint));
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
            .SetAuthorityRecordCount(0)
            .SetAdditionalRecordCount(0)
            .Build();
        //var resolverQuery = resolverUdpClient.Send(receivedData);
        //var resolverResponse = await resolverUdpClient.ReceiveAsync();
        //response = resolverResponse.Buffer;
        //Console.WriteLine("Response : " + Encoding.UTF8.GetString(resolverResponse.Buffer));
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

        foreach(var question in questions)
        {
            Console.WriteLine("split message");
            var ip = new IPEndPoint(IPAddress.Any, 41232);
            resolverUdpClient.Send(new DNSMessage(dnsHeader.Copy(), new List<DNSQuestion>(){question}, new List<DNSAnswer>()).ToByteArray());
            var resolverResponse = await resolverUdpClient.ReceiveAsync();
            Console.WriteLine(resolverResponse.RemoteEndPoint);
            var rsp =
                DNSMessage.Read(resolverResponse.Buffer);
            Console.WriteLine("Received resonse: " + string.Concat(rsp.answers.SelectMany(x => x.Labels)));
            //questions.AddRange(rsp.questions);
            answers.AddRange(rsp.answers);
        }

        dnsHeader.AnswerRecordCount = (ushort)answers.Count;
        dnsHeader.QuestionCount = (ushort)questions.Count;
        var dnsMessage = new DNSMessage(dnsHeader, questions, answers);
        await udpClient.SendAsync(dnsMessage.ToByteArray(), sourceEndPoint);

        continue;
    }
    else
    {
        IPEndPoint sourceEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] receivedData = (udpClient.Receive(ref sourceEndPoint));
        string receivedString = Encoding.ASCII.GetString(receivedData);
        Console.WriteLine($"Received {receivedData.Length} : {receivedString}");
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
        await udpClient.SendAsync(response, sourceEndPoint);

    }


    // Send response
}

