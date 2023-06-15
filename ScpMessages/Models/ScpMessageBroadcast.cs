namespace ScpMessages.Models
{
    public class ScpMessageBroadcast
    {
        public string Message { get; set; }
        public ushort Time { get; set; }
        public ScpMessageBroadcast() { }
        public ScpMessageBroadcast(string Message, ushort Time)
        { 
            this.Message = Message;
            this.Time = Time;
        }
    }
}
