namespace ScpMessages.Models
{
    public class IndividualUserToggleChoice
    {
        public bool EnableScpMessages { get; set; } = true;
        public bool EnableDamageMessages { get; set; } = true;
        public bool EnableItemMessages { get; set; } = true;
        public bool EnableMapMessages { get; set; } = true;
    }
}
