using Data.Contexts;
using Data.Models;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories;

public interface IBaseRepository<TEntity, TModel> where TEntity : class
{
    Task<RepositoryResult<bool>> AddAsync(TEntity entity);
    Task<RepositoryResult<bool>> DeleteAsync(TEntity entity);
    Task<RepositoryResult<bool>> ExistsAsync(Expression<Func<TEntity, bool>> findBy);
    Task<RepositoryResult<IEnumerable<TSelect>>> GetAllAsync<TSelect>(Expression<Func<TEntity, TSelect>>? selector = null, bool orderByDescending = false, Expression<Func<TEntity, object>>? sortByColumn = null, Expression<Func<TEntity, bool>>? filterBy = null, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? where = null, int take = 0, params Expression<Func<TEntity, object>>[] includes);
    Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes);
    Task<RepositoryResult<bool>> UpdateAsync(TEntity entity);
}

public abstract class BaseRepository<TEntity, TModel> : IBaseRepository<TEntity, TModel> where TEntity : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _table;

    protected BaseRepository(AppDbContext context)
    {
        _context = context;
        _table = _context.Set<TEntity>();
    }

    public virtual async Task<RepositoryResult<bool>> AddAsync(TEntity entity)
    {
        if (entity == null)
        {
            return new RepositoryResult<bool>
            {
                Succeded = false,
                Error = "Entity is null",
                StatusCode = 400
            };
        }

        try
        {
            _table.Add(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult<bool>
            {
                Succeded = true,
                StatusCode = 201
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult<bool>
            {
                Succeded = false,
                Error = ex.Message,
                StatusCode = 500
            };
        }
    }

    public virtual async Task<RepositoryResult<bool>> DeleteAsync(TEntity entity)
    {
        if (entity == null)
        {
            return new RepositoryResult<bool>
            {
                Succeded = false,
                Error = "Entity is null",
                StatusCode = 400
            };
        }

        try
        {
            _table.Remove(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult<bool>
            {
                Succeded = true,
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult<bool>
            {
                Succeded = false,
                StatusCode = 500,
                Error = ex.Message
            };
        }
    }

    public virtual async Task<RepositoryResult<bool>> ExistsAsync(Expression<Func<TEntity, bool>> findBy)
    {
        if (findBy == null)
        {
            return new RepositoryResult<bool>
            {
                Succeded = false,
                StatusCode = 400,
                Error = "Expression cannot be null."
            };
        }

        var exists = await _table.AsNoTracking().AnyAsync(findBy);
        return new RepositoryResult<bool>
        {
            Succeded = exists,
            StatusCode = exists ? 200 : 404,
            Error = exists ? null : "Entity not found"
        };
    }

    public virtual async Task<RepositoryResult<IEnumerable<TSelect>>> GetAllAsync<TSelect>(
    Expression<Func<TEntity, TSelect>>? selector = null,
    bool orderByDescending = false,
    Expression<Func<TEntity, object>>? sortByColumn = null,
    Expression<Func<TEntity, bool>>? filterBy = null,
    Expression<Func<TEntity, object>>? sortBy = null,
    Expression<Func<TEntity, bool>>? where = null,
    int take = 0,
    params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _table;

        if (where != null) query = query.Where(where);
        if (filterBy != null) query = query.Where(filterBy);
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        if (sortByColumn != null)
            query = orderByDescending ? query.OrderByDescending(sortByColumn) : query.OrderBy(sortByColumn);
        if (sortBy != null)
            query = orderByDescending ? query.OrderByDescending(sortBy) : query.OrderBy(sortBy);

        if (take > 0) query = query.Take(take);

        var entities = await query.Select(selector ?? (x => (TSelect)(object)x)).ToListAsync();

        return new RepositoryResult<IEnumerable<TSelect>>
        {
            Succeded = true,
            StatusCode = 200,
            Result = entities
        };
    }


    public virtual async Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _table;

        if (includes != null && includes.Length != 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        var entity = await query.AsNoTracking().FirstOrDefaultAsync(where);
        if (entity == null)
        {
            return new RepositoryResult<TModel>
            {
                Succeded = false,
                StatusCode = 404,
                Error = "Entity not found"
            };
        }

        var result = entity.MapTo<TModel>();
        return new RepositoryResult<TModel>
        {
            Succeded = true,
            StatusCode = 200,
            Result = result
        };
    }

    public virtual async Task<RepositoryResult<bool>> UpdateAsync(TEntity entity)
    {
        if (entity == null)
        {
            return new RepositoryResult<bool>
            {
                Succeded = false,
                Error = "Entity is null",
                StatusCode = 400
            };
        }

        try
        {
            _table.Update(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult<bool>
            {
                Succeded = true,
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult<bool>
            {
                Succeded = false,
                Error = ex.Message,
                StatusCode = 500
            };
        }
    }
}
