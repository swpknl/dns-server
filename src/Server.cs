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
IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 2053;
IPEndPoint udpEndPoint = new IPEndPoint(ipAddress, port);

// Create UDP socket
UdpClient udpClient = new UdpClient(udpEndPoint);

while (true)
{
    // Receive data
    IPEndPoint sourceEndPoint = new IPEndPoint(IPAddress.Any, 0);
    byte[] receivedData = udpClient.Receive(ref sourceEndPoint);
    string receivedString = Encoding.ASCII.GetString(receivedData);
    Console.WriteLine($"Received {receivedData.Length} bytes from {sourceEndPoint}: {receivedString}");
    var dnsHeaderQuery = new DNSHeader().FromBytes(receivedData);
    // Create an empty response
    var dnsHeader = new DnsHeaderBuilder().SetID(dnsHeaderQuery.ID)
        .SetQueryResponseIndicator(dnsHeaderQuery.QueryResponseIndicator)
        .SetOpCode(dnsHeaderQuery.OpCode)
        .SetAuthoritativeAnswer(dnsHeaderQuery.AuthoritativeAnswer)
        .SetTruncation(dnsHeaderQuery.Truncation)
        .SetRecursionDesired(dnsHeaderQuery.RecursionDesired)
        .SetRecursionAvailable(dnsHeaderQuery.RecursionAvailable)
        .SetReserved(dnsHeaderQuery.Reserved)
        .SetResponseCode(dnsHeaderQuery.ResponseCode)
        .SetQuestionCount(dnsHeaderQuery.QuestionCount)
        .SetAnswerRecordCount(dnsHeaderQuery.AnswerRecordCount)
        .SetAuthorityRecordCount(dnsHeaderQuery.AuthorityRecordCount)
        .SetAdditionalRecordCount(dnsHeaderQuery.AdditionalRecordCount)
        .Build();
    var labels = new List<string> { "codecrafters", "io" };
    var question = new DNSQuestion(labels, DNSType.A, DNSClass.IN);
    var answer = new DNSAnswer(labels, DNSType.A, DNSClass.IN, 60, 4, [8, 8, 8, 8]);
    var message = new DNSMessage(dnsHeader,question, answer);
    byte[] response = message.ToByteArray();

    // Send response
    udpClient.Send(response, response.Length, sourceEndPoint);
}

