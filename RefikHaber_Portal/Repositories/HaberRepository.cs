using RefikHaber.Models;
using RefikHaber.Utility;
using Microsoft.EntityFrameworkCore;
using RefikHaber.Models;

namespace RefikHaber.Repostories
{
    public class HaberRepository : GenericRepository<Haber>
    {
        private readonly UygulamaDbContext _uygulamaDbContext;

        public HaberRepository(UygulamaDbContext uygulamaDbContext) : base(uygulamaDbContext)
        {
            _uygulamaDbContext = uygulamaDbContext;
        }

        public void Guncelle(Haber haber)
        {
            _uygulamaDbContext.Update(haber);
        }

        public void Kaydet()
        {
            _uygulamaDbContext.SaveChanges();
        }

        public void Sil(object haber)
        {
            _uygulamaDbContext.Remove(haber);
        }

        public IEnumerable<Haber> GetAllHaberler()
        {
            return _uygulamaDbContext.Haberler.ToList();
        }
    }
}
