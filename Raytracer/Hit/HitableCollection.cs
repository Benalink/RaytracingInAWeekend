using System.Collections.Generic;

namespace Raytracer.Hit
{
    public class HitableCollection : IHitable
    {
        public List<IHitable> RayObjects { get; }

        public HitableCollection(List<IHitable> rayObjects)
        {
            this.RayObjects = rayObjects;
        }

        public bool TestHit(in Ray ray, float tMin, float tMax, ref HitRecord record)
        {
            var tempRec = new HitRecord();
            bool hit = false;
            float closestSoFar = tMax;
            
            foreach (IHitable rayObject in this.RayObjects)
            {
                if (rayObject.TestHit(in ray, tMin, closestSoFar, ref tempRec))
                {
                    hit = true;
                    closestSoFar = tempRec.T;
                    record = tempRec;
                }
            }

            return hit;
        }
    }
}