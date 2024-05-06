using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src.Models
{
    public class DNSMessage
    {
        public  DNSHeader header;
        public List<DNSQuestion> questions;
        public List<DNSAnswer> answers;
        
        public DNSMessage(DNSHeader header, List<DNSQuestion> dnsQuestion, List<DNSAnswer> dnsAnswer)
        {
            this.header = header;
            this.questions = dnsQuestion;
            this.answers = dnsAnswer;
        }

        public byte[] ToByteArray()
        {
            var bytes = new List<byte>();
            bytes.AddRange(this.header.ToByteArray());
            foreach (var question in this.questions)
            {
                bytes.AddRange(question.ToByteArray());
            }

            if (this.answers != null)
            {
                foreach (var answer in this.answers)
                {
                    bytes.AddRange(answer.ToByteArray());
                }
            }
            
            
            return bytes.ToArray();
        }

        public IEnumerable<DNSMessage> SplitIntoSingularQuestions()
        {
            foreach (var question in questions)
            {
                var header = this.header.Copy();
                header.QuestionCount = 0;
                header.AnswerRecordCount = 0;
                header.AdditionalRecordCount = 0;
                header.AuthorityRecordCount = 0;
                questions.Add(question);
                var msg = new DNSMessage(this.header, questions, null);
                yield return msg;
            }
        }

        public static DNSMessage Read(ReadOnlySpan<byte> buffer)
        {
            var dnsHeader = new DNSHeader().FromBytes(buffer.ToArray());
            var count = 12;
            int offset = 12;
            var questions = new List<DNSQuestion>();
            for (int i = 0; i < dnsHeader.QuestionCount; i++)
            {
                var q = new DNSQuestion().FromBytes(buffer.ToArray()[offset..], out offset);
                count += offset;
                buffer = buffer[offset..];
                questions.Add(q);
                offset += 4;
            }
            var answers = new List<DNSAnswer>();
            for (int i = 0; i < dnsHeader.AnswerRecordCount; i++)
            {
                var q = DNSAnswer.Read(buffer.ToArray(), out offset);
                count += offset;
                buffer = buffer[offset..];
                answers.Add(q);
            }

            var msg = new DNSMessage(dnsHeader, questions, answers);
            return msg;
        }
    }
}
