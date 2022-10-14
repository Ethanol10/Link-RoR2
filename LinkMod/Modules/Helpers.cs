using System;
using System.Collections.Generic;

namespace LinkMod.Modules
{
    internal static class Helpers
    {
        internal const string agilePrefix = "<style=cIsUtility>Agile.</style> ";

        internal static string ScepterDescription(string desc)
        {
            return "\n<color=#d299ff>SCEPTER: " + desc + "</color>";
        }

        internal static string EquipmentDescription(string desc)
        {
            return "\n<color=#ff5500>" + desc + "</color>";
        }

        internal static string DamageDescription(string desc)
        {
            return $"<style=cIsDamage>{desc}</style>";
        }

        internal static string LinkSpecificDescription(string desc)
        {
            return $"<color=#B0FF3E>{desc}</color>";
        }

        internal static string HealDescription(string desc)
        {
            return $"<color=#50ff04>{desc}</color>";
        }

        internal static string DownsideDescription(string desc) 
        {
            return $"<color=#DA0000>{desc}</color>";
        }

        public static T[] Append<T>(ref T[] array, List<T> list)
        {
            var orig = array.Length;
            var added = list.Count;
            Array.Resize<T>(ref array, orig + added);
            list.CopyTo(array, orig);
            return array;
        }

        public static Func<T[], T[]> AppendDel<T>(List<T> list) => (r) => Append(ref r, list);
    }
}