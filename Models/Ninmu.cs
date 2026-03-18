using System;
using tracker.Enums;

namespace tracker.Models {
    public class Ninmu {
        public int ID {get; set;}
        public string description {get; set;} = string.Empty;
        public Status ninmuJyoutai {get; set;}
        public DateTime createdAt {get; set;}
        public DateTime updatedAt {get; set;}
    }
}