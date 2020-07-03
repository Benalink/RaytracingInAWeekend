namespace Raytracer.Hit
{
    public interface IHitable
    {
        bool TestHit(Ray ray, float tMin, float tMax, ref HitRecord record);
    }
}