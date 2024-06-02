using Microsoft.EntityFrameworkCore;

namespace LotusWebApp.Data;

/// <summary>
/// Базовый репозиторий
/// </summary>
/// <typeparam name="TEntity">Тип entity</typeparam>
public class RepositoryBase<TEntity> : IRepository<TEntity>, IDisposable
	where TEntity : class
{
	public readonly ApplicationDbContext Context;

	/// <summary>
	/// .ctor
	/// </summary>
	/// <param name="context">Контекст базы данных</param>
	protected RepositoryBase(ApplicationDbContext context)
	{
		Context = context;
	}

	private IQueryable<TEntity> ApplySpec(ISpecification<TEntity> spec)
	{
		IQueryable<TEntity> set = Context.Set<TEntity>();

		// Поддерживаются только include первого уровня.
		foreach (var include in spec.Includes)
		{
			set = set.Include(include);
		}

		foreach (var includeString in spec.IncludeStrings)
		{

			set = set.Include(includeString);
		}

		set = set.Where(spec.ToExpression());

		if (spec.Orders.Any())
		{
			var orderedSet = spec.Orders[0].isAscending ?
				set.OrderBy(spec.Orders[0].keySelector) :
				set.OrderByDescending(spec.Orders[0].keySelector);

			foreach (var (keySelector, isAscending) in spec.Orders.Skip(1))
			{
				orderedSet = isAscending ?
					orderedSet.ThenBy(keySelector) :
					orderedSet.ThenByDescending(keySelector);
			}

			return orderedSet;
		}

		return set;
	}

	/// <inheritdoc />
	public Task<int> CountAsync(ISpecification<TEntity> specification)
	{
		if (specification == null)
		{
			throw new ArgumentNullException(nameof(specification));
		}

		var set = ApplySpec(specification);

		return set.CountAsync();
	}

	/// <inheritdoc />
	public int Count(ISpecification<TEntity> specification)
	{
		if (specification == null)
		{
			throw new ArgumentNullException(nameof(specification));
		}

		var set = ApplySpec(specification);

		return set.Count();
	}

	/// <inheritdoc />
	public TEntity? Find(ISpecification<TEntity> specification)
	{
		if (specification == null)
		{
			throw new ArgumentNullException(nameof(specification));
		}

		var set = ApplySpec(specification);

		return set.FirstOrDefault();
	}

	/// <inheritdoc />
	public async Task<TEntity?> FindAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
	{
		if (specification == null)
		{
			throw new ArgumentNullException(nameof(specification));
		}

		var set = ApplySpec(specification);

		return await set.FirstOrDefaultAsync(cancellationToken);
	}

	/// <inheritdoc />
	public IReadOnlyList<TEntity> Get(ISpecification<TEntity> specification, int take, int? skip = null)
	{
		if (specification == null)
		{
			throw new ArgumentNullException(nameof(specification));
		}

		var set = ApplySpec(specification);

		set = set.Take(take);

		if (skip.HasValue)
		{
			set = set.Skip(skip.Value);
		}

		return set.ToList();
	}

	public async Task<bool> ExistAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
	{
		var set = ApplySpec(specification);
		return await set.AnyAsync(cancellationToken);
	}

	/// <inheritdoc />
	public IReadOnlyList<TEntity> GetAll(ISpecification<TEntity> specification)
	{
		if (specification == null)
		{
			throw new ArgumentNullException(nameof(specification));
		}

		var set = ApplySpec(specification);

		return set.ToList();
	}

	/// <inheritdoc />
	public async Task<IReadOnlyList<TEntity?>> GetAsync(ISpecification<TEntity> specification, int take, int? skip = null, CancellationToken cancellationToken = default)
	{
		if (specification == null)
		{
			throw new ArgumentNullException(nameof(specification));
		}

		var set = ApplySpec(specification);

		set = set.Take(take);

		if (skip.HasValue)
		{
			set = set.Skip(skip.Value);
		}

		return await set.ToListAsync(cancellationToken);
	}

	/// <inheritdoc />
	public async Task<IReadOnlyList<TEntity>> GetReadOnlyAllAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
	{
		if (specification == null)
		{
			throw new ArgumentNullException(nameof(specification));
		}

		var set = ApplySpec(specification);

		return await set.AsNoTracking().ToListAsync(cancellationToken);
	}

	/// <inheritdoc />
	public void Add(TEntity entity)
	{
		if (entity == null)
		{
			throw new ArgumentNullException(nameof(entity));
		}

		Context.Set<TEntity>().Add(entity);
	}

	public void AddRange(IEnumerable<TEntity> entities)
	{
		if (entities == null)
		{
			throw new ArgumentNullException(nameof(entities));
		}

		Context.Set<TEntity>().AddRange(entities);
	}

	/// <inheritdoc />
	public int Save()
	{
		return Context.SaveChanges();
	}

	/// <inheritdoc />
	public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
	{
		return await Context.SaveChangesAsync(cancellationToken);
	}

	/// <inheritdoc />
	public bool Remove(ISpecification<TEntity> specification)
	{
		var result = Context.Remove(specification.ToExpression());
		return result.State == EntityState.Deleted;
	}

	#region Disposing
	private bool _disposed;

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
			return;

		if (disposing)
		{
			// called via myClass.Dispose().
			// OK to use any private object references
		}
		// Release unmanaged resources.
		Context.Dispose();
		// Set large fields to null.
		_disposed = true;
	}

	public void Dispose() // Implement IDisposable
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	~RepositoryBase() // the finalizer
	{
		Dispose(false);
	}
	#endregion
}