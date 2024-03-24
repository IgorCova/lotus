using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LotusWebApp.Data;

/// <summary>
/// Спецификация
/// </summary>
/// <typeparam name="TEntity">Тип entity</typeparam>
public interface ISpecification<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Expression для фильтрации.
    /// </summary>
    Expression<Func<TEntity, bool>> ToExpression();

    /// <summary>
    /// Список инклюдов
    /// </summary>
    IReadOnlyList<Expression<Func<TEntity, dynamic>>> Includes { get; }

    /// <summary>
    /// Список сортировок
    /// </summary>
    IReadOnlyList<(Expression<Func<TEntity, dynamic>> keySelector, bool isAscending)> Orders { get; }

    /// <summary>
    /// Список инклудов через string
    /// </summary>
    List<string> IncludeStrings { get; }
}