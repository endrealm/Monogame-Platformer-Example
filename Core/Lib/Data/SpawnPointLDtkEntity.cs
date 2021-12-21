using LDtk;
using Microsoft.Xna.Framework;

namespace Core.Lib.Data
{
    public class SpawnPointLDtkEntity: LDtkEntity
    {
        public Vector2 Spawn { get => spawn; set => spawn = value; }
        public Vector2 spawn;
    }
}