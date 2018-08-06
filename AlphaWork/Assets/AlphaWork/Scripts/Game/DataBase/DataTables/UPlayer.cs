using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite4Unity3d;

namespace AlphaWork
{
    public class UPlayer : ITable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public override string KeyName { get; set; }
        public int level { get; set; }   
        public string gamesetting { get; set; }
        public int sceneId { get; set; }
        public float xPos { get; set; }
        public float yPos { get; set; }
        public float zPos { get; set; }

        public override string ToString()
        {
            return string.Format("[user: name={0}, level={1}]", user, level);
        }
        public string user
        {
            get { return KeyName; }
            set { KeyName = value; }
        }
    }
}
