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
        private readonly DNSHeader header;
        private readonly List<DNSQuestion> questions;
        private readonly List<DNSAnswer> answers;
        
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

        public void AddQuestion(DNSQuestion question)
        {
            this.questions.Add(question);
            header.QuestionCount++;
        }
        public void AddAnswer(DNSAnswer record)
        {
            answers.Add(record);
            header.AnswerRecordCount++;
        }
    }
}
