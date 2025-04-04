using System;
using System.Collections.Generic;

namespace Canves {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ManagedAttribute : Attribute {
        public string name;
        public ManagedAttribute(string name) {
            this.name = name;
        }
    }
}