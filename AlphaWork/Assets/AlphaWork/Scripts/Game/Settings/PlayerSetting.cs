using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameFramework.Event;

namespace AlphaWork
{
    public class PlayerSetting : ScriptableObject
    {
        //public PlayerSetting()
        //{
        //    Initialize();
        //}
        //private void Initialize()
        //{
        //    if(GameEntry.Event)
        //        GameEntry.Event.Subscribe(RefreshPosArgs.EventId, OnRefreshMainPos);
        //}

        //public void OnRefreshMainPos(object sender, GameEventArgs e)
        //{
        //    RefreshPosArgs arg = e as RefreshPosArgs;
        //    if (arg != null && GameBase.MainEthan.gameObject == arg.Gb)
        //    {
        //        arg.Gb.transform.position = arg.TransCache.position + new Vector3(0, 2, 0);
        //        List<UPlayer> players;
        //        GameEntry.DataBase.DataDevice.GetDataByKey<UPlayer>(GameEntry.Config.GameSetting.CurrentUser, out players);
        //        if (players.Count >= 0)
        //        {
        //            Vector3 pos = arg.Gb.transform.position;
        //            players[0].xPos = pos.x;
        //            players[0].yPos = pos.y;
        //            players[0].zPos = pos.z;
        //            GameEntry.DataBase.DataDevice.UpdateData<UPlayer>(players[0]);
        //        }
        //    }
        //}
    }
}
