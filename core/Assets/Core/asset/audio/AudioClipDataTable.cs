using System;
using System.Collections.Generic;
using System.Text;
using commons;

namespace core
{
    public class AudioDataTable : IndexTable<AudioClipData>
    {
        public AudioDataTable(string path) : base(path) { }

        protected override void ProcessRow(int rowNo, AudioClipData r) {}

        protected override string GetKey(AudioClipData row)
        {
            return row.key;
        }
    }

    [System.Serializable]
    public class AudioClipData {
        [SpreadSheetColumn("A")] public string key;
        [SpreadSheetColumn("B")] public string path;
        [SpreadSheetColumn("C")] public bool loop;
        [SpreadSheetColumn("D")] public bool interruptable = true; // self interruptable
        [SpreadSheetColumn("E")] public float length;

        public AudioClipData() {}

        public AudioClipData(string key, float length) {
            this.key = key;
            this.length = length;
        }

        public string ToCsv()
        {
            StringBuilder csv = new StringBuilder(128);
            csv.Append(key).Append(",");
            csv.Append(path).Append(",");
            csv.Append(loop).Append(",");
            csv.Append(interruptable).Append(",");
            csv.Append(length);
            return csv.ToString();
        }

        public override string ToString ()
        {
            return key;
        }
    }
}
