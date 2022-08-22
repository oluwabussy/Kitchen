﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modupekitchen.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modupekitchen.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Category { set; get; }
        public DbSet<SubCategory> SubCategory { set; get; }
        public DbSet<MenuItem> MenuItem { set; get; }
        public DbSet<Coupontable> Coupontable { set; get; }
        public DbSet<Coupon_t> Coupon_t { set; get; }
        public DbSet<ApplicationUser> ApplicationUser { set; get; }
        public DbSet<ShoppingCart> ShoppingCart { set; get; }
        public DbSet<OrderHeader> OrderHeader { set; get; }
        public DbSet<OrderDetails> OrderDetails { set; get; }
        public DbSet<PagingInfo> PagingInfo { set; get; }
    }
}
