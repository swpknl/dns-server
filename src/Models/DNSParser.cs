using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src.Models
{
    public class DNSParser
    {
        private byte[] request;
        public int id, nq;
        byte[] header = new byte[12];

        public DNSParser(byte[] req)
        {
            request = req;
            id = request[0] << 8 + request[1];
            nq = (request[4] << 8) | request[5];
        }

        public byte[] getHeader()
        {

            header[0] = request[0];
            header[1] = request[1];

            int aa = 0, opcode = (request[2] >> 3) & 0x0F, rd = request[2] & 0x01;
            header[2] = (byte)((1 << 7) | (opcode << 3) | (aa << 2) | (0 << 1) | rd);

            int ra = 0, rz = 0, rc = 0;
            if (opcode != 0)
                rc = 4;

            header[3] = (byte)((ra << 7) | (rz << 4) | rc);

            int na = nq;
            header[4] = (byte)(nq >> 8);
            header[5] = (byte)(nq & 0xFF);
            header[6] = (byte)(na >> 8);
            header[7] = (byte)(na & 0xFF);

            int nra = 0, ara = 0;
            header[8] = (byte)(nra >> 8);
            header[9] = (byte)(nra & 0xFF);
            header[10] = (byte)(ara >> 8);
            header[11] = (byte)(ara & 0xFF);

            return header;
        }

        public int populateQuestion(int reqIdx, List<byte> questions)
        {
            int labelLength = request[reqIdx];
            while (labelLength != 0)
            {
                if ((labelLength & 0xC0) == 0xC0)
                {
                    int idx = (labelLength & 0x3F << 8) | request[reqIdx + 1];
                    reqIdx += 2;
                    labelLength = request[idx];
                    while (labelLength != 0)
                    {
                        for (int j = 0; j <= labelLength; j++)
                        {
                            questions.Add(request[idx++]);
                        }
                        labelLength = request[idx];
                    }
                }
                else
                {
                    for (int j = 0; j <= labelLength; j++)
                    {
                        questions.Add(request[reqIdx++]);
                    }
                }
                labelLength = request[reqIdx];
            }
            questions.Add(0);

            Utility.addBigEndianToList(questions, 1, 2);
            Utility.addBigEndianToList(questions, 1, 2);
            reqIdx += 5;
            return reqIdx;
        }

        public byte[] getQuestion()
        {
            List<byte> questions = new List<byte>();
            int reqIdx = 12;
            try
            {
                for (int i = 0; i < nq; i++)
                {
                    reqIdx = populateQuestion(reqIdx, questions);
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Index Out of Bound idx: " + reqIdx);
            }
            return questions.ToArray();
        }

        public byte[] getAnswers(uint[]? queriedIps)
        {
            List<byte> answers = new List<byte>();
            int reqIdx = 12;
            try
            {
                for (int i = 0; i < nq; i++)
                {
                    reqIdx = populateQuestion(reqIdx, answers);

                    int ttl = 60, length = 4;
                    uint ip = 123456;

                    if (queriedIps != null && queriedIps.Length > i)
                    {
                        ip = queriedIps[i];
                    }

                    Utility.addBigEndianToList(answers, ttl, 4);
                    Utility.addBigEndianToList(answers, length, 2);
                    Utility.addBigEndianToList(answers, ip, 4);

                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.Error.WriteLine("Errro: Malformed Request Recieved should be loneger than: " + reqIdx);
            }

            return answers.ToArray();
        }

        public byte[] getResponse(uint[]? queriedIps = null)
        {
            getHeader();
            byte[] question = getQuestion();
            byte[] answers = getAnswers(queriedIps);

            byte[] response = new byte[header.Length + question.Length + answers.Length];

            Array.Copy(header, 0, response, 0, header.Length);
            Array.Copy(question, 0, response, header.Length, question.Length);
            Array.Copy(answers, 0, response, header.Length + question.Length, answers.Length);

            Console.WriteLine($"Sending Response: {response.Length}");
            Utility.print2Hex(response);

            // Utility.WriteBinaryDataToFile(response, "bin.bin");

            return response;
        }
    }
}
