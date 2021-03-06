﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewsPortalWebApi.Data_Access.Interfaces;
using NewsPortalWebApi.Data_Access.Models;

namespace NewsPortalWebApi.Data_Access.EFCore.Repositories
{
    /// <summary>
    /// Класс репозитория для новостей 
    /// </summary>
    public class NewsRepository : IRepository<News>        
    {
        private readonly NewsPortalWebApiContext _context;

        /// <summary>
        /// Констурктор репозитория
        /// </summary>
        /// <param name="context">
        /// Аргументом является контекст из базы данных
        /// </param>
        public NewsRepository(NewsPortalWebApiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Метод для получения всех новостей
        /// </summary>
        /// <returns>
        /// Возвращает все новости из базы данных
        /// </returns>
        public IEnumerable<News> GetAll()
        {
            return _context.News
                .Include(author => author.Author);
        }
        /// <summary>
        /// Метод для получения новости по Id
        /// </summary>
        /// <param name="id">
        /// Id нужной новости
        /// </param>
        /// <returns>
        /// Возвращает нужную новость</returns>
        public News Get(Guid id)
        {
            var news = _context.News
                .Include(author => author.Author)
                .Single(news => news.Id == id);
            return news;
        }
        /// <summary>
        /// Добавляет новость в базу данных
        /// </summary>
        /// <param name="entity">
        /// </param>
        public void Add(News entity)
        {
            _context.News.Add(entity);
        }
        /// <summary>
        /// Удаляет новость по id
        /// </summary>
        /// <param name="id">
        /// Id новости
        /// </param>
        public void Delete(Guid id)
        {
            News entity = _context.News.Find(id);
            if (entity != null)
                _context.News.Remove(entity);
        }

        /// <summary>
        /// Обнавляет статус новости на Modified
        /// </summary>
        /// <param name="entity"></param>
        public void Update(News entity)
        {
            _context.News.Update(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
