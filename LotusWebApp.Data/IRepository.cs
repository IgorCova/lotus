namespace LotusWebApp.Data;

/// <summary>
/// Репозиторий
/// </summary>
/// <typeparam name="TEntity">Класс-энтити</typeparam>
public interface IRepository<TEntity>
	where TEntity : class
{
	/// <summary>
	/// Возвращает количество элементов <see cref="TEntity"/>, удовлетворяющий условию <paramref name="specification"/>.
	/// </summary>
	/// <param name="specification">Спецификация объектов</param>
	Task<int> CountAsync(ISpecification<TEntity> specification);

	/// <summary>
	/// Возвращает количество элементов <see cref="TEntity"/>, удовлетворяющий условию <paramref name="specification"/>.
	/// </summary>
	/// <param name="specification">Спецификация объектов</param>
	int Count(ISpecification<TEntity> specification);

	/// <summary>
	/// Возвращает первый объект, подходящий под спецификацию <paramref name="specification"/> или <c>null</c>,
	/// если ни одного подходящего объекта не найдено
	/// </summary>
	/// <param name="specification">Спецификация объектов</param>
	TEntity? Find(ISpecification<TEntity> specification);

	/// <summary>
	/// Возвращает первый объект, подходящий под спецификацию <paramref name="specification"/> или <c>null</c>,
	/// если ни одного подходящего объекта не найдено
	/// </summary>
	/// <param name="specification">Спецификация объектов</param>
	/// <param name="cancellationToken">Токен отмены</param>
	Task<TEntity?> FindAsync(ISpecification<TEntity> specification,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Возвращает список объектов, размером не более <paramref name="take"/> элементов, удовлетворяющих
	/// спецификации <paramref name="specification"/>, пропуская первые <paramref name="skip"/> объектов.
	/// </summary>
	/// <param name="specification">Спецификация объектов</param>
	/// <param name="take">Максимальное количество возвращаемых объектов</param>
	/// <param name="skip">Количество объектов, которые нужно пропустить</param>
	IReadOnlyList<TEntity> Get(ISpecification<TEntity> specification, int take, int? skip = null);

	/// <summary>
	/// Возвращает список объектов, размером не более <paramref name="take"/> элементов, удовлетворяющих
	/// спецификации <paramref name="specification"/>, пропуская первые <paramref name="skip"/> объектов.
	/// </summary>
	/// <param name="specification">Спецификация объектов</param>
	/// <param name="take">Максимальное количество возвращаемых объектов</param>
	/// <param name="skip">Количество объектов, которые нужно пропустить</param>
	/// <param name="cancellationToken">Токен отмены</param>
	Task<IReadOnlyList<TEntity?>> GetAsync(ISpecification<TEntity> specification, int take,
		int? skip = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// Проверяет существование хотя бы одного элемента, удовлетворяющих
	/// спецификации <paramref name="specification"/>.
	/// </summary>
	/// <param name="specification">Спецификация объектов</param>
	/// <param name="cancellationToken">Токен отмены</param>
	Task<bool> ExistAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

	/// <summary>
	/// Возвращает список всех объектов, удовлетворяющих спецификации <paramref name="specification"/>.
	/// </summary>
	/// <param name="specification">Спецификация объектов</param>
	IReadOnlyList<TEntity> GetAll(ISpecification<TEntity> specification);

	/// <summary>
	/// Возвращает список объектов удовлетворяющих
	/// спецификации <paramref name="specification"/>
	/// </summary>
	/// <param name="specification">Спецификация объектов</param>
	/// <param name="cancellationToken">Токен отмены</param>
	Task<IReadOnlyList<TEntity>> GetReadOnlyAllAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

	/// <summary>
	/// Помечает <paramref name="entity"/> в качестве объекта для добавления в базу данных.
	/// </summary>
	/// <param name="entity">Объект для добавления в базу данных</param>
	void Add(TEntity entity);

	/// <summary>
	/// Сохраняет изменения в базу
	/// </summary>
	int Save();

	/// <summary>
	/// Сохраняет изменения в базу
	/// </summary>
	/// <param name="cancellationToken">Токен отмены</param>
	Task<int> SaveAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Удаление объекта в бд
	/// </summary>
	/// <param name="specification"></param>
	/// <returns></returns>
	bool Remove(ISpecification<TEntity> specification);
}