﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacSlopes.Entities;

namespace MacSlopes.Services.Abstract
{
    public interface ICategoryReporitory
    {
        IQueryable<Category> GetCategories();

        Category GetCategory(string Id);

        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);

        IQueryable<Category> Search(string Query);
        Task<bool> SaveChangesAsync();

        bool VerifyName(string name);
    }
}
