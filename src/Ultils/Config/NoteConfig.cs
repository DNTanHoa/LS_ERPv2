using System.Collections.Generic;

namespace Ultils.Config
{



    public class NoteConfig
    {
        public static string ConfigName = "Note";
        public List<NoteModel> Notes { get; set; } = new List<NoteModel>();
    }

    public class NoteModel
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}

