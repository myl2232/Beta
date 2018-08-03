using UnityEngine;

namespace AlphaWork
{
    public static class AssetUtility
    {
        public static void SolveNames(GameObject gb)
        {
            if (!gb)
                return;
            int count = gb.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                Transform trans = gb.transform.GetChild(i);
                string name = trans.gameObject.name;
                name = name.Replace(" ", "");
                trans.gameObject.name = name;//.Replace(" ", "");
                SolveNames(trans.gameObject);
            }
        }

        public static Transform GetParentForm<T>(Transform trans) where T : MonoBehaviour
        {
            if (trans == null || trans.parent == null)
            {
                return null;
            }
            if (trans.parent.GetComponent<T>() != null)
            {
                return trans.parent;
            }
            else
            {
                return GetParentForm<T>(trans.parent);
            }
        }

        public static Transform FindChild(Transform trans, string str)
        {
            if (!trans)
                return null;
            Transform result = trans.Find(str);
            if (!result)
            {
                int count = trans.childCount;
                for (int i = 0; i < count; ++i)
                {
                    if (result)
                        break;

                    Transform tr = trans.GetChild(i);
                    if (tr.gameObject.name == str)
                    {
                        result = tr;
                    }
                    else
                        result = FindChild(tr, str);
                }
            }
            else
                return result;

            return result;
        }

        public static string GetDataTableAsset(string assetName)
        {
            return string.Format("Assets/AlphaWork/DataTables/{0}.txt", assetName);
        }

        public static string GetDictionaryAsset(string assetName)
        {
            return string.Format("Assets/AlphaWork/Localization/{0}/Dictionaries/{1}.xml", GameEntry.Localization.Language.ToString(), assetName);
        }

        public static string GetFontAsset(string assetName)
        {
            return string.Format("Assets/AlphaWork/Localization/{0}/Fonts/{1}.ttf", GameEntry.Localization.Language.ToString(), assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return string.Format("Assets/AlphaWork/Scenes/{0}.unity", assetName);
        }
        public static string GetVillageScene(string assetName)
        {
            return string.Format("Assets/Top-Down city/Demo/{0}.unity", assetName);
        }

        public static string GetNavigationAsset(string assetName)
        {
            string[] splitNames = assetName.Split('/');
            string[] levelNames = splitNames[splitNames.Length - 1].Split('.');
            return string.Format("Assets/AlphaWork/Navigations/{0}/RecastNavmesh.asset", levelNames[0]);
        }

        public static string GetMusicAsset(string assetName)
        {
            return string.Format("Assets/AlphaWork/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return string.Format("Assets/AlphaWork/Sounds/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return string.Format("Assets/AlphaWork/Entities/{0}.prefab", assetName);
        }
        public static string GetEffectAsset2(string assetName)
        {
            return string.Format("Assets/AlphaWork/KY_effects/MagicEffectsPackFree/Prefab/{0}.prefab", assetName);
        }
        public static string GetEffectAsset(string assetName)
        {
            return string.Format("Assets/AlphaWork/Effect_FX/Prefab/{0}.prefab", assetName);
        }
        public static string GetUIFormAsset(string assetName)
        {
            return string.Format("Assets/AlphaWork/UI/UIForms/{0}.prefab", assetName);
        }

        public static string GetUISpriteAsset(string assetName)
        {
            return string.Format("Assets/AlphaWork/UI/UISprites/{0}.png", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return string.Format("Assets/AlphaWork/UI/UISounds/{0}.wav", assetName);
        }
    }
}
