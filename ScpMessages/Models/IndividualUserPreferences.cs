namespace ScpMessages.Models
{
    public class IndividualUserPreferences
    {
        public bool EnableScpMessages { get; set; } = true;
        public bool EnableDamageMessages { get; set; } = true;
        public bool EnableItemMessages { get; set; } = true;
        public bool EnableMapMessages { get; set; } = true;
        public bool EnableTeamRespawnMessages { get; set; } = true;
        public float MessageDisplayTime { get; set; } = 5;
    }
}
