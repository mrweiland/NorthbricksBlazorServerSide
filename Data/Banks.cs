using System;
using System.Collections.Generic;

namespace BlazorTestServerSide.Data
{
    public class Bank
    {
        public string bic { get; set; }
        public string shortName { get; set; }
        public string fullName { get; set; }
        public string country { get; set; }
        public string logo { get; set; }
        public string website { get; set; }
    }
    public class Banks
    {
        public List<Bank> banks { get; set; }
    }
}