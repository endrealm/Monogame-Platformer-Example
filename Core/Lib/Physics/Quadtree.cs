using System;
using System.Collections.Generic;
using Core.Lib.Math;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Core.Lib.Physics
{
    public class Quadtree
    {
        public const int DefaultMaxDepth = 7;
        public const int DefaultMaxObjectsPerNode = 25;
        protected List<Quadtree> Children = new List<Quadtree>();
        protected HashSet<QuadtreeData> Contents = new HashSet<QuadtreeData>();

        public Quadtree(RectangleF bounds)
        {
            this.CurrentDepth = 0;
            this.NodeBounds = bounds;
        }

        protected int CurrentDepth { get; set; }

        protected int MaxDepth { get; set; } = 7;

        protected int MaxObjectsPerNode { get; set; } = 25;

        public RectangleF NodeBounds { get; protected set; }

        public bool IsLeaf => this.Children.Count == 0;

        public int NumTargets()
        {
            List<QuadtreeData> quadtreeDataList = new List<QuadtreeData>();
            int num = 0;
            Queue<Quadtree> quadtreeQueue = new Queue<Quadtree>();
            quadtreeQueue.Enqueue(this);
            while (quadtreeQueue.Count > 0)
            {
                Quadtree quadtree = quadtreeQueue.Dequeue();
                if (!quadtree.IsLeaf)
                {
                    foreach (Quadtree child in quadtree.Children)
                        quadtreeQueue.Enqueue(child);
                }
                else
                {
                    foreach (QuadtreeData content in quadtree.Contents)
                    {
                        if (!content.Dirty)
                        {
                            ++num;
                            content.MarkDirty();
                            quadtreeDataList.Add(content);
                        }
                    }
                }
            }

            for (int index = 0; index < quadtreeDataList.Count; ++index)
                quadtreeDataList[index].MarkClean();
            return num;
        }

        public void Insert(QuadtreeData data)
        {
            if (!this.NodeBounds.Intersects(data.Target.Bounds))
                return;
            if (this.IsLeaf && this.Contents.Count >= this.MaxObjectsPerNode)
                this.Split();
            if (this.IsLeaf)
            {
                this.AddToLeaf(data);
            }
            else
            {
                foreach (Quadtree child in this.Children)
                    child.Insert(data);
            }
        }

        public void Remove(QuadtreeData data)
        {
            if (!this.IsLeaf)
                throw new InvalidOperationException("Cannot remove from a non leaf Quadtree");
            data.RemoveParent(this);
            this.Contents.Remove(data);
        }

        public void Shake()
        {
            if (this.IsLeaf)
                return;
            List<QuadtreeData> quadtreeDataList = new List<QuadtreeData>();
            int num = this.NumTargets();
            if (num == 0)
                this.Children.Clear();
            else if (num < this.MaxObjectsPerNode)
            {
                Queue<Quadtree> quadtreeQueue = new Queue<Quadtree>();
                quadtreeQueue.Enqueue(this);
                while (quadtreeQueue.Count > 0)
                {
                    Quadtree quadtree = quadtreeQueue.Dequeue();
                    if (!quadtree.IsLeaf)
                    {
                        foreach (Quadtree child in quadtree.Children)
                            quadtreeQueue.Enqueue(child);
                    }
                    else
                    {
                        foreach (QuadtreeData content in quadtree.Contents)
                        {
                            if (!content.Dirty)
                            {
                                this.AddToLeaf(content);
                                content.MarkDirty();
                                quadtreeDataList.Add(content);
                            }
                        }
                    }
                }

                this.Children.Clear();
            }

            for (int index = 0; index < quadtreeDataList.Count; ++index)
                quadtreeDataList[index].MarkClean();
        }

        private void AddToLeaf(QuadtreeData data)
        {
            data.AddParent(this);
            this.Contents.Add(data);
        }

        public void Split()
        {
            if (this.CurrentDepth + 1 >= this.MaxDepth)
                return;
            Point2 topLeft = this.NodeBounds.TopLeft;
            Point2 bottomRight = this.NodeBounds.BottomRight;
            Point2 center = this.NodeBounds.Center;
            RectangleF[] rectangleFArray = new RectangleF[4]
            {
                RectangleF.CreateFrom(topLeft, center),
                RectangleF.CreateFrom(new Point2(center.X, topLeft.Y), new Point2(bottomRight.X, center.Y)),
                RectangleF.CreateFrom(center, bottomRight),
                RectangleF.CreateFrom(new Point2(topLeft.X, center.Y), new Point2(center.X, bottomRight.Y))
            };
            for (int index = 0; index < rectangleFArray.Length; ++index)
            {
                this.Children.Add(new Quadtree(rectangleFArray[index]));
                this.Children[index].CurrentDepth = this.CurrentDepth + 1;
            }

            foreach (QuadtreeData content in this.Contents)
            {
                foreach (Quadtree child in this.Children)
                    child.Insert(content);
            }

            this.Clear();
        }

        private void Clear()
        {
            foreach (QuadtreeData content in this.Contents)
                content.RemoveParent(this);
            this.Contents.Clear();
        }

        public List<QuadtreeData> Query(IShapeF area)
        {
            List<QuadtreeData> recursiveResult = new List<QuadtreeData>();
            this.QueryWithoutReset(area, recursiveResult);
            for (int index = 0; index < recursiveResult.Count; ++index)
                recursiveResult[index].MarkClean();
            return recursiveResult;
        }

        private void QueryWithoutReset(IShapeF area, List<QuadtreeData> recursiveResult)
        {
            if (!this.NodeBounds.Intersects(area))
                return;
            if (this.IsLeaf)
            {
                foreach (QuadtreeData content in this.Contents)
                {
                    if (!content.Dirty && content.Bounds.Intersects(area))
                    {
                        recursiveResult.Add(content);
                        content.MarkDirty();
                    }
                }
            }
            else
            {
                int index = 0;
                for (int count = this.Children.Count; index < count; ++index)
                    this.Children[index].QueryWithoutReset(area, recursiveResult);
            }
        }

        public RaycastHit? Raycast(LineF line,
            Func<ICollisionTarget, bool> shouldHit)
        {
            if (!line.IntersectsWith(NodeBounds))
                return null;
            
            if (this.IsLeaf)
            {
                foreach (QuadtreeData content in this.Contents)
                {
                    if(content.Dirty) continue;
                    
                    var hit = line.IntersectsWith(content.Bounds, out var intersectionPoint);
                    if (!hit) continue;
                    if (!shouldHit.Invoke(content.Target)) continue;
                    
                    return new RaycastHit
                    {
                        IntersectionPoint = intersectionPoint,
                        Collider = content.Target
                    };
                }
            }
            else
            {
                int index = 0;
                for (int count = this.Children.Count; index < count; ++index)
                {
                    var hit = this.Children[index].Raycast(line, shouldHit);
                    if (hit != null) return hit;
                }
            }
            
            return null;
        }
    }
}