﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace codecrafters_dns_server.src.Models
{
    public class DNSAnswer
    {
        public List<string> Labels { get; set; }
        public DNSType Type { get; set; }
        public DNSClass Class { get; set; }
        public byte TTL { get; set; }
        public ushort Length { get; set; }
        public List<byte> Data { get; set; }

        public DNSAnswer(List<string> labels, DNSType type, DNSClass dnsClass, byte ttl, ushort length, List<byte> data)
        {
            this.Labels = labels;
            this.Type = type;
            this.Class = dnsClass;
            this.TTL = ttl;
            this.Length = length;  
            this.Data = data;
        }

        public byte[] ToByteArray()
        {
            var bytes = new List<byte>();
            foreach (var label in Labels)
            {
                bytes.Add((byte)label.Length);
                bytes.AddRange(Encoding.ASCII.GetBytes(label));
            }

            bytes.Add(0);
            bytes.Add((byte)((short)Type >> 8));
            bytes.Add((byte)((short)Type & 0xFF));
            bytes.Add((byte)((short)Class >> 8));
            bytes.Add((byte)((short)Class & 0xFF));
            bytes.Add((byte)(TTL >> 24));
            bytes.Add((byte)(TTL >> 16));
            bytes.Add((byte)(TTL >> 8));
            bytes.Add((byte)(TTL & 0xFF));
            bytes.Add((byte)(Data.Count >> 8));
            bytes.Add((byte)(Data.Count & 0xFF));
            bytes.AddRange(Data);

            return bytes.ToArray();
        }
    }
}
