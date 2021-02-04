﻿using GraniteHouse.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraniteHouse.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ProductTypes>  productTypes { get; set; }
        public DbSet<SpecialTags> specialTags { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<ProductsSelectedForAppointment> ProductsSelectedForAppointment { get; set; }

        public DbSet<Appointments> Appointments { get; set; }
        
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

    }
}
