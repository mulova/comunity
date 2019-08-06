using UnityEngine;
using System;
using comunity;
using mulova.commons;
using System.Text.Ex;

namespace audio
{
    public class AudioData
    {
        [SpreadSheetColumn("A")] public string eventId;
        [SpreadSheetColumn("B")] public string bgm;
        [SpreadSheetColumn("C")] public string sfx;
        
        public override string ToString ()
        {
            return eventId;
        }
    }

    public class AudioTable : IndexTable<AudioData>
    {

        public AudioTable() : base("audio_table.csv")
        {
        }
        protected override void ProcessRow(int rowNo, AudioData r)
        {
        }

        protected override string GetKey(AudioData row)
        {
            return row.eventId;
        }

        public void Play(string id) {
            if (!initialized) { return; }
            AudioData data = GetRow(id);
            if (data == null) {
                return;
            }
            if (data.bgm.IsNotEmpty()) {
                if (data.bgm != "-") {
                    AudioBridge.Play(data.bgm);
                } else {
                    AudioBridge.Play(AudioBridge.STOP, "bgm");
                }
            }
            if (data.sfx.IsNotEmpty()) {
                AudioBridge.Play(data.sfx);
            }
        }
        
        public void PlayBGM(string id){
            AudioData data = GetRow(id);
            if(data == null)
                return;
            
            if(data.bgm.IsNotEmpty()){
                if(data.bgm != "-"){
                    AudioBridge.Play(data.bgm);
                } else {
                    AudioBridge.Play(AudioBridge.STOP, "bgm");
                }
            }
        }
        
        public void PlaySFX(string id){
            AudioData data = GetRow(id);
            if(data == null)
                return;
            
            if(data.sfx.IsNotEmpty()){
                AudioBridge.Play(data.sfx);
            }
        }
        
        public void StopSfx() {
            AudioBridge.Play(AudioBridge.STOP, "sfx");
        }
        
        public void StopBgm() {
            AudioBridge.Play(AudioBridge.STOP, "bgm");
        }
        
        protected override void LoadBytes(string path, Action<byte[]> callback)
        {
            Cdn.cache.GetBytes(path, callback);
        }
    }
}


