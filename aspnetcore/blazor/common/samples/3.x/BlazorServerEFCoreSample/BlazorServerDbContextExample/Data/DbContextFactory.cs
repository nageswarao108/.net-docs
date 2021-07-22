﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorServerDbContextExample.Data
{
    public class DbContextFactory<TContext> 
        : IDbContextFactory<TContext> where TContext : DbContext
    {
        private readonly IServiceProvider _provider;

        public DbContextFactory(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException($"{nameof(provider)}: You must configure an instance of IServiceProvider");
        }

        public TContext CreateDbContext() => ActivatorUtilities.CreateInstance<TContext>(_provider);
    }
}
