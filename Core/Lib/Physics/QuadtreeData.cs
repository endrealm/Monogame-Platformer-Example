using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended;

namespace Core.Lib.Physics
{
    public class QuadtreeData
    {
        public HashSet<Quadtree> Parents = new HashSet<Quadtree>();

        public QuadtreeData(ICollisionTarget target)
        {
            this.Target = target;
            this.Bounds = target.Bounds;
        }

        public void RemoveParent(Quadtree parent) => this.Parents.Remove(parent);

        public void AddParent(Quadtree parent) => this.Parents.Add(parent);

        public void RemoveFromAllParents()
        {
            foreach (Quadtree quadtree in this.Parents.ToList<Quadtree>())
                quadtree.Remove(this);
            this.Parents.Clear();
        }

        public ICollisionTarget Target { get; set; }

        public bool Dirty { get; private set; }

        public void MarkDirty() => this.Dirty = true;

        public void MarkClean() => this.Dirty = false;

        public IShapeF Bounds { get; set; }
    }
}