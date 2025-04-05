namespace Canves {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
    public class ManagedAttribute : Attribute {
        public string name;
        public ManagedAttribute(string name) {
            this.name = name;
        }
    }
}