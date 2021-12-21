using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Core.Lib.Math
{
    public class DebugDrawer
    {
        public static readonly bool DEBUG = true;
        private static List<Tuple<LineF, Color>> lines = new List<Tuple<LineF, Color>>();
        public static void DrawLine(LineF line, Color green)
        {
            if(!DEBUG) return;
            lines.Add(new Tuple<LineF, Color>(line, green));
        }

        public static void DrawAll(SpriteBatch spriteBatch)
        {
            if(!DEBUG) return;
            
            foreach (var tuple in lines)
            {
                spriteBatch.DrawLine(tuple.Item1.Origin, tuple.Item1.End, tuple.Item2);
            }
            
            lines.Clear();
        }
    }
}