using System;
using System.Collections.Generic;
using Core.Lib.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Core.Lib
{
    public class DebugDrawer
    {
        public const bool Debug = false;
        private static readonly List<Tuple<LineF, Color>> Lines = new();
        public static void DrawLine(LineF line, Color green)
        {
            if(!Debug) return;
            Lines.Add(new Tuple<LineF, Color>(line, green));
        }

        public static void DrawAll(SpriteBatch spriteBatch)
        {
            if(!Debug) return;
            
            foreach (var tuple in Lines)
            {
                spriteBatch.DrawLine(tuple.Item1.Origin, tuple.Item1.End, tuple.Item2);
            }
            
            Lines.Clear();
        }
    }
}