using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Instigate
{
    // handles all faction properties
    public class Faction
    {
        public Color color { get; set; }
        public List<Faction> feuds;
        public List<Faction> allies;

        public Faction(Color theColor)
        {
            color = theColor;
            feuds = new List<Faction>();
            allies = new List<Faction>();
        }

        public void addFeud(Faction offender)
        {
            if (offender != this && !feuds.Contains(offender))
            {
                feuds.Add(offender);
                if (allies.Contains(offender))
                    allies.Remove(offender);
            }
        }

        public void addAlly(Faction newFriend)
        {
            if (newFriend != this && !feuds.Contains(newFriend) && !allies.Contains(newFriend))
                allies.Add(newFriend);
        }
    }
}
