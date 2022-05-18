using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class GameResource : MonoBehaviour
{
    public static GameResource active;
    public static int playerPPU = 20;
    public static int tilePPU = 10;


    bool DataDebug = true;
    //public Language language;
    public List<string> indexIDs = null;
    public ArrayList indexval = null;
    public List<GameObject> GUIData = new List<GameObject>();
    
    void Awake()
    {
        active = this;
        GameDirectory.CreateRequiredFiles();
        CreateIndex();
        AddIndex("Visuals", new List<GameObject>());

    }

    public Texture2D LoadTexture(string Folder, string FileName, string DefaultFileName)
    {
        if (LoadTexture(Folder, FileName) == null)

        {
            return LoadTexture(Folder, DefaultFileName);
        }
        return LoadTexture(Folder, FileName);
    }

        public Texture2D LoadTexture(string Folder, string FileName)
    {
        foreach (string Extension in GameDirectory.ImageFileExtension)
        {
            if (LoadTexture(Folder, FileName, Extension,false) != null)
            {
                return LoadTexture(Folder, FileName, Extension, false);
            }
        }
        return null;
    }
    public Texture2D LoadTexture(string Folder, string FileName, string Extension,bool readable)
    {
        Texture2D myTexture = null;

        if (GetIndex<Texture2D>("Texture2D " + FileName) != null)
        {

            myTexture = GetIndex<Texture2D>("Texture2D " + FileName);
        }
        else
        {
            if (Resources.Load<Texture2D>(GameDirectory.AssetFolder+Folder + FileName) != null)
            {
                myTexture = Resources.Load<Texture2D>(GameDirectory.AssetFolder +Folder + FileName);
            }
            else if (File.Exists(Application.dataPath + GameDirectory.SaveDataFolder + Folder + FileName + Extension))
            {
                string url = "file:///" + Application.dataPath + GameDirectory.SaveDataFolder + Folder + FileName + Extension;

                myTexture = new Texture2D(3, 3);

                WWW www = new WWW(url);

                while (www.isDone == false)
                {
                    new WaitForFixedUpdate();

                }

                if (www.isDone)
                {
                    www.LoadImageIntoTexture(myTexture);
                }
            }
            else
            {
                //Debug.Log("not found "+ Folder + FileName + Extension);
            }
            if (myTexture != null)
            {    
                AddIndex("Texture2D " + FileName, myTexture);
                myTexture.filterMode = FilterMode.Point;
                myTexture.anisoLevel = 10;
            }
        }
        return myTexture;
    }
    public ltype Load<ltype>(string Name, string folder, string ext, bool fresh)
    {
        string Dir = folder + Name + ext;
        if (DataDebug)
        {
            Debug.Log("Loading asset " + Name + " type " + typeof(ltype).Name);
        }
        if (!fresh && GetIndex<ltype>(typeof(ltype).ToString() + Name) != null)
        {

            if (DataDebug)
            {
                Debug.Log(Name + "Loaded from game memory.");
            }
            return GetIndex<ltype>(typeof(ltype) + Name);
        }
        else if (Resources.Load<TextAsset>(  folder + Name + GameDirectory.DataFileExtension) != null)
        {
            if (DataDebug)
            {
                Debug.Log("Loading base asset from Resource folder.");
            }
            TextAsset LoadData = Resources.Load<TextAsset>(  folder + Name + GameDirectory.DataFileExtension);

            ltype TempData = (ltype)(new XmlSerializer(typeof(ltype)).Deserialize(new StringReader(LoadData.text)));
            if (TempData != null)
            {
                AddIndex(Dir, TempData);
            }
            return TempData;
        }
        else if (File.Exists(Application.dataPath + GameDirectory.SaveDataFolder + Dir) == true)
        {
            if (DataDebug)
            {
                Debug.Log("Load from Custom Folder.");
            }
            FileStream SaveSpace = new FileStream(Application.dataPath + GameDirectory.SaveDataFolder + Dir, FileMode.Open);

            ltype TempData = (ltype)(new XmlSerializer(typeof(ltype)).Deserialize(SaveSpace));

            SaveSpace.Close();

            if (TempData != null)
            {
                AddIndex(typeof(ltype).ToString() + Name, TempData);
            }

            return TempData;
        }
        else
        {
            if (DataDebug)
            {
                Debug.Log("No file found!");
            }
            return default(ltype);
        }
    }

   /*
    public void LoadLanguage(GameResource game, string Name)
    {
        language = new Language(Load<LanguageData>(Name, GameDirectory.LanguageFile, GameDirectory.DataFileExtension, false));

        language.LoadWorld(game, activeWorld.FileName);
    }*/
    public void CreateIndex()
    {
        indexIDs = new List<string>();
        indexval = new ArrayList();
    }
    public void AddIndex(string NAME, object OBJ)
    {
        if (GetIndex<object>(name) != null)
        {
            indexval[indexIDs.IndexOf(NAME)] = OBJ;
        }
        else
        {
            indexval.Add(OBJ);
            indexIDs.Add(NAME);
        }
    }
    public type GetIndex<type>(string NAME)
    {
        int index = indexIDs.IndexOf(NAME);
        if (index >= 0)
        {
            return (type)indexval[index];
        }
        else
        {
            return default(type);
        }
    }
    public Texture2D ForceReadable(Texture2D Original)
    {
        byte[] TextureData = Original.EncodeToPNG();
        Texture2D Copy = new Texture2D(Original.width, Original.height);
        ImageConversion.LoadImage(Copy, TextureData, false);
        return Copy;
    }
}


