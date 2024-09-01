using System.ComponentModel;

namespace mini_ITS.Core.Models
{
    public enum PriorityValues
    {
        [Description("Normalny")]
        Normal,
        [Description("Wysoki")]
        High,
        [Description("Krytyczny")]
        Critical
    };
}