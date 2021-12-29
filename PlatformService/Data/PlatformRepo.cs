using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private AppDbContext _context;

        public PlatformRepo(AppDbContext context)
        {
            _context = context;
        }
        public void CreatePlatform(Platform plat)
        {
            // Check if the object exist or not
           if(plat == null){
               throw new ArgumentNullException(nameof(plat));
           }
           // Adding the object 
           _context.Platforms.Add(plat);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            // Nampilin semua data di Model Platform
           return _context.Platforms.ToList();
        }

        public Platform GetPlatformById(int id)
        {
            // Get by Id Using Lambda
           return _context.Platforms.FirstOrDefault(p=>p.Id == id);
        }

        public bool SaveChanges()
        {
           return (_context.SaveChanges() >= 0);
        }
    }
}