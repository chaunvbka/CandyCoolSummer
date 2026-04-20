#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable IDE0051 // Remove unused private members


namespace Texell.CandyCoolSummer
{
    using UnityEngine;

    /// <summary>
    /// SV = striped vertical, SH = striped horizontal.
    /// </summary>
    public enum CandyType
    {
        SV_BLUE = 0,
        SV_YELLOW = 1,
        SV_RED = 2,
        SV_GREEN = 3,
        SV_PURPLE = 4,
        SV_PINK = 5,
        CHOCOLATE_MILK = 6,
        SH_BLUE = 7,
        SH_YELLOW = 8,
        SH_RED = 9,
        SH_GREEN = 10,
        SH_PURPLE = 11,
        SH_PINK = 12,

        BLUE = 14,
        YELLOW = 15,
        RED = 16,
        GREEN = 17,
        PURPLE = 18,
        PINK = 19,

        STING_BLUE = 21,
        STING_YELLOW = 22,
        STING_RED = 23,
        STING_GREEN = 24,
        STING_PURPLE = 25,
        STING_PINK = 26,
        SWIRL_BLUE = 27,
        SWIRL_YELLOW = 28,
        SWIRL_RED = 29,
        SWIRL_GREEN = 30,
        SWIRL_PURPLE = 31,
        SWIRL_PINK = 32,
    }

    public class Candy : MonoBehaviour
    {
        public CandyType Type;
    }
}


