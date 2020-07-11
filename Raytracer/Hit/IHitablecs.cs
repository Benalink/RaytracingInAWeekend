namespace Raytracer.Hit
{
    public interface IHitable
    {
        bool TestHit(in Ray ray, float tMin, float tMax, ref HitRecord record);
    }
}