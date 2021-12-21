using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Core.Lib.Physics
{
    public class CollisionManager: IUpdateable
    {
        private readonly Dictionary<ICollisionTarget, QuadtreeData> _targetDataDictionary =
            new Dictionary<ICollisionTarget, QuadtreeData>();

        private readonly Quadtree _collisionTree;

        public CollisionManager(RectangleF boundary) => _collisionTree = new Quadtree(boundary);

        public void Update(float deltaTime)
        {
            foreach (QuadtreeData data in _targetDataDictionary.Values)
            {
                
                ICollisionTarget target = data.Target;
                
                // if(target.StaticCollider) continue;
                data.RemoveFromAllParents();
                foreach (QuadtreeData quadtreeData in _collisionTree.Query(target.Bounds))
                {
                    CollisionEventArgs collisionInfo = new CollisionEventArgs
                    {
                        Other = quadtreeData.Target,
                        PenetrationVector = CalculatePenetrationVector(data.Bounds, quadtreeData.Bounds)
                    };
                    target.OnCollision(collisionInfo);
                    data.Bounds = data.Target.Bounds;
                }

                _collisionTree.Insert(data);
            }

            _collisionTree.Shake();
        }

        public void Insert(ICollisionTarget target)
        {
            if (_targetDataDictionary.ContainsKey(target))
                return;
            QuadtreeData data = new QuadtreeData(target);
            _targetDataDictionary.Add(target, data);
            _collisionTree.Insert(data);
        }

        public void Remove(ICollisionTarget target)
        {
            if (!_targetDataDictionary.ContainsKey(target))
                return;
            _targetDataDictionary[target].RemoveFromAllParents();
            _targetDataDictionary.Remove(target);
            _collisionTree.Shake();
        }

        public bool Contains(ICollisionTarget target) => _targetDataDictionary.ContainsKey(target);

        private static Vector2 CalculatePenetrationVector(IShapeF a, IShapeF b)
        {
            switch (a)
            {
                case RectangleF rect1_1:
                    if (b is RectangleF rect2)
                        return PenetrationVector(rect1_1, rect2);
                    RectangleF rect1 = rect1_1;
                    if (b is CircleF circ1)
                        return PenetrationVector(rect1, circ1);
                    break;
                case CircleF circ1_1:
                    if (b is CircleF circ2)
                        return PenetrationVector(circ1_1, circ2);
                    CircleF circ3 = circ1_1;
                    if (b is RectangleF rect3)
                        return PenetrationVector(circ3, rect3);
                    break;
            }

            throw new NotSupportedException("Shapes must be either a CircleF or RectangleF");
        }

        private static Vector2 PenetrationVector(RectangleF rect1, RectangleF rect2)
        {
            RectangleF rectangleF = RectangleF.Intersection(rect1, rect2);
            return rectangleF.Width >= (double)rectangleF.Height
                ? new Vector2(0.0f,
                    rect1.Center.Y < (double)rect2.Center.Y ? rectangleF.Height : -rectangleF.Height)
                : new Vector2(rect1.Center.X < (double)rect2.Center.X ? rectangleF.Width : -rectangleF.Width,
                    0.0f);
        }

        private static Vector2 PenetrationVector(CircleF circ1, CircleF circ2)
        {
            if (!circ1.Intersects(circ2))
                return Vector2.Zero;
            Vector2 vector2_1 = Point2.Displacement(circ1.Center, circ2.Center);
            Vector2 vector2_2 = !(vector2_1 != Vector2.Zero)
                ? -Vector2.UnitY * (circ1.Radius + circ2.Radius)
                : vector2_1.NormalizedCopy() * (circ1.Radius + circ2.Radius);
            return vector2_1 - vector2_2;
        }

        private static Vector2 PenetrationVector(CircleF circ, RectangleF rect)
        {
            Vector2 vector2_1 = rect.ClosestPointTo(circ.Center) - circ.Center;
            if (!rect.Contains(circ.Center) && !vector2_1.Equals(Vector2.Zero))
                return circ.Radius * vector2_1.NormalizedCopy() - vector2_1;
            Vector2 vector2_2 = Point2.Displacement(circ.Center, rect.Center);
            Vector2 vector2_3;
            if (vector2_2 != Vector2.Zero)
            {
                Vector2 vector2_4 = new Vector2(vector2_2.X, 0.0f);
                Vector2 vector2_5 = new Vector2(0.0f, vector2_2.Y);
                vector2_4.Normalize();
                vector2_5.Normalize();
                Vector2 vector2_6 = vector2_4 * (circ.Radius + rect.Width / 2f);
                vector2_5 *= circ.Radius + rect.Height / 2f;
                if (vector2_6.LengthSquared() < (double)vector2_5.LengthSquared())
                {
                    vector2_3 = vector2_6;
                    vector2_2.Y = 0.0f;
                }
                else
                {
                    vector2_3 = vector2_5;
                    vector2_2.X = 0.0f;
                }
            }
            else
                vector2_3 = -Vector2.UnitY * (circ.Radius + rect.Height / 2f);

            return vector2_2 - vector2_3;
        }

        private static Vector2 PenetrationVector(RectangleF rect, CircleF circ) =>
            -PenetrationVector(circ, rect);

        public void DebugDraw(SpriteBatch spriteBatch)
        {
            foreach (QuadtreeData data in _targetDataDictionary.Values)
            {
                var shape = data.Bounds;
                if (shape is RectangleF rect)
                {
                    spriteBatch.DrawRectangle(rect, Color.Lime, 2);
                }
            }
        }
    }
}