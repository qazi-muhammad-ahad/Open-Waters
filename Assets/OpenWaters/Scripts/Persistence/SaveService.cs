using System;
using System.IO;
using UnityEngine;

namespace OpenWaters
{
    public interface IRunStorage { void Write(string json); string Read(bool backup); bool Exists(bool backup); }
    public interface IBackupPreservingRunStorage : IRunStorage { void Write(string json,bool preserveBackup); }
    public sealed class FileRunStorage : IBackupPreservingRunStorage
    {
        readonly string path;
        public FileRunStorage(string path){this.path=path;}
        public bool Exists(bool backup)=>File.Exists(path+(backup?".bak":""));
        public string Read(bool backup)=>File.ReadAllText(path+(backup?".bak":""));
        public void Write(string json)=>Write(json,false);
        public void Write(string json,bool preserveBackup)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string temp=path+".tmp";File.WriteAllText(temp,json);
            if(File.Exists(path))File.Replace(temp,path,preserveBackup?null:path+".bak");else File.Move(temp,path);
        }
    }
    // WebGL uses PlayerPrefs / IndexedDB sync via Unity's supported PlayerPrefs.Save path.
    // Hosted-browser persistence still requires acceptance testing on the target browser.
    public sealed class BrowserRunStorage : IBackupPreservingRunStorage
    {
        const string Key="OpenWaters.Run.v1";
        public bool Exists(bool backup)=>PlayerPrefs.HasKey(Key+(backup?".bak":""));
        public string Read(bool backup)=>PlayerPrefs.GetString(Key+(backup?".bak":""));
        public void Write(string json)=>Write(json,false);
        public void Write(string json,bool preserveBackup)
        {
            if(!preserveBackup && Exists(false))PlayerPrefs.SetString(Key+".bak",Read(false));
            PlayerPrefs.SetString(Key,json);PlayerPrefs.Save();
        }
    }
    public sealed class SaveService
    {
        readonly IRunStorage storage;
        readonly DemoDefinitions data;
        public string Message {get;private set;}="";
        public bool Temporary {get;private set;}
        public SaveService(IRunStorage storage,DemoDefinitions data){this.storage=storage;this.data=data;}
        public bool Save(RunState state)
        {
            try
            {
                GameSession.ValidateState(state,data);
                bool preserveBackup=false;
                if(storage.Exists(false))
                {
                    try{GameSession.ValidateState(JsonUtility.FromJson<RunState>(storage.Read(false)),data);}
                    catch{preserveBackup=true;}
                }
                string json=JsonUtility.ToJson(state,true);
                if(storage is IBackupPreservingRunStorage safeStorage)safeStorage.Write(json,preserveBackup);
                else
                {
                    if(preserveBackup && storage.Exists(true))throw new IOException("Storage cannot safely preserve the recovery backup.");
                    storage.Write(json);
                }
                Temporary=false;Message="";return true;
            }
            catch(Exception e){Temporary=true;Message="Temporary session: save failed. "+e.Message;return false;}
        }
        public RunState Load()
        {
            string failure="";
            for(int i=0;i<2;i++)
            {
                try
                {
                    if(!storage.Exists(i==1))continue;
                    var state=JsonUtility.FromJson<RunState>(storage.Read(i==1));GameSession.ValidateState(state,data);
                    Message=i==1?"Recovered the last valid backup. "+failure:"";return state;
                }
                catch(Exception e){failure="Save could not be loaded: "+e.Message;}
            }
            Message=failure==""?"":"No valid run found. "+failure+" Start a new run.";return null;
        }
    }
    [Serializable] public sealed class DemoSettings
    {
        public int uiPreferenceVersion;
        public bool reducedMotion, mute, tutorial;
        public DisplayMode displayMode=DisplayMode.Fullscreen;
        public float master=.65f, ambience=.3f, sfx=.7f;
        public static DemoSettings Load()
        {
            try
            {
                var settings=JsonUtility.FromJson<DemoSettings>(PlayerPrefs.GetString("OpenWaters.Settings","{}"))??new DemoSettings();
                bool migrated=settings.uiPreferenceVersion<1;
                if(migrated){settings.tutorial=false;settings.uiPreferenceVersion=1;}
                settings.Validate();
                // Legacy true came from the old default; only a new explicit opt-in should enable tips.
                if(migrated && PlayerPrefs.HasKey("OpenWaters.Settings"))
                {
                    try{settings.Save();}catch{ /* Keep usable migrated preferences if storage is unavailable. */ }
                }
                return settings;
            }
            catch{return new DemoSettings();}
        }
        static float Volume(float value,float fallback)=>float.IsNaN(value)||float.IsInfinity(value)?fallback:Mathf.Clamp01(value);
        void Validate()
        {
            master=Volume(master,.65f);ambience=Volume(ambience,.3f);sfx=Volume(sfx,.7f);
            if(!Enum.IsDefined(typeof(DisplayMode),displayMode))displayMode=DisplayMode.Fullscreen;
        }
        public void Save(){uiPreferenceVersion=1;Validate();PlayerPrefs.SetString("OpenWaters.Settings",JsonUtility.ToJson(this));PlayerPrefs.Save();}
    }
}
