using MudBlazor;
using System.Text.RegularExpressions;

namespace Authentication.web.Shared.Questions
{
    public partial class AssociationQ
    {
        private const string BankZone = "Banque";

        // Démo : chaque trou est noté [N] dans la phrase. Enregistrement et notation à venir.
        private const string Sentence = "Le langage [1] s'exécute sur la plateforme [2] et se compile en [3].";

        private List<Segment> _segments = ParseSentence(Sentence);

        private List<DropZoneItem> _items = new()
        {
            new() { Zone = BankZone, Name = "C#" },
            new() { Zone = BankZone, Name = ".NET" },
            new() { Zone = BankZone, Name = "IL" },
            new() { Zone = BankZone, Name = "Java" },
            new() { Zone = BankZone, Name = "JVM" },
        };

        private void ItemUpdated(MudItemDropInfo<DropZoneItem> dropItem) => dropItem.Item.Zone = dropItem.DropzoneIdentifier;

        // Un trou n'accepte qu'un mot ; la banque en accepte autant que nécessaire.
        private bool CanDrop(DropZoneItem item, string zone)
            => zone == BankZone || !_items.Any(i => i.Zone == zone && i != item);

        private static List<Segment> ParseSentence(string sentence)
        {
            var segments = new List<Segment>();
            int position = 0;

            foreach (Match match in Regex.Matches(sentence, @"\[(\d+)\]"))
            {
                if (match.Index > position)
                    segments.Add(new Segment(sentence.Substring(position, match.Index - position), false));

                segments.Add(new Segment("trou-" + match.Groups[1].Value, true));
                position = match.Index + match.Length;
            }

            if (position < sentence.Length)
                segments.Add(new Segment(sentence.Substring(position), false));

            return segments;
        }

        /// <summary>Morceau de la phrase : texte, ou trou (Value = identifiant de la dropzone).</summary>
        private record Segment(string Value, bool IsBlank);

        private class DropZoneItem
        {
            public string Zone { get; set; }
            public string Name { get; init; }
        }
    }
}
