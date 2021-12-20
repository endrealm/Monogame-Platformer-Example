
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Lib.Entities.Rendering
{
    public class RendererRegistry
    {
        private Dictionary<Type, IEntityRenderer> Renderers = new Dictionary<Type, IEntityRenderer>();

        public void RegisterRenderer<T>(IEntityRenderer<T> renderer) where T: IEntity {
            Renderers.Add(typeof(T), renderer);
        }
        
        public IEntityRenderer<T> GetRenderer<T>() where T: IEntity {
            var typeParameterType = typeof(T);
            
            return (IEntityRenderer<T>) Renderers[typeParameterType];
        }

        public IEntityRenderer[] AllRenderers => Renderers.Values.ToArray();
    }
}