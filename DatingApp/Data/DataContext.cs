﻿using DatingApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions options): base(options) 
        {
            
        }

        public DbSet<AppUser> User { get; set; }
    }
}
