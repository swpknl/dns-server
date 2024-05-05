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

    // Create an empty response
    var dnsHeader = new DnsHeaderBuilder().SetID(1234)
        .SetQueryResponseIndicator(true)
        .SetOpCode(0)
        .SetAuthoritativeAnswer(false)
        .SetTruncation(false)
        .SetRecursionDesired(false)
        .SetRecursionAvailable(false)
        .SetReserved(0)
        .SetResponseCode(0)
        .SetQuestionCount(1)
        .SetAnswerRecordCount(1)
        .SetAuthorityRecordCount(0)
        .SetAdditionalRecordCount(0)
        .Build();
    var labels = new List<string> { "codecrafters", "io" };
    var question = new DNSQuestion(labels, DNSType.A, DNSClass.IN);
    var answer = new DNSAnswer(labels, DNSType.A, DNSClass.IN, 60, 4, [8, 8, 8, 8]);
    var message = new DNSMessage(dnsHeader,question, answer);
    byte[] response = message.ToByteArray();

    // Send response
    udpClient.Send(response, response.Length, sourceEndPoint);
}

