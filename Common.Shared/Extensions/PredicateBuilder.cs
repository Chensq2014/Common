﻿using System;
using System.Linq.Expressions;

namespace Common.Extensions
{
    ///// <summary>
    ///// 表达式目录树拼装生成器 同ExpressionExtension
    ///// </summary>
    //public static class PredicateBuilder
    //{
    //    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    //    {
    //        return first.AndAlso<T>(second, Expression.AndAlso);
    //    }

    //    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    //    {
    //        return first.AndAlso<T>(second, Expression.OrElse);
    //    }

    //    //public static Expression<Func<T, bool>> True<T>(this Expression<Func<T, bool>> expression)
    //    //{
    //    //    return s => true;
    //    //}

    //    private static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, Func<Expression, Expression, BinaryExpression> func)
    //    {
    //        var parameter = Expression.Parameter(typeof(T));

    //        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
    //        var left = leftVisitor.Visit(expr1.Body);

    //        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
    //        var right = rightVisitor.Visit(expr2.Body);

    //        return Expression.Lambda<Func<T, bool>>(func(left, right), parameter);
    //    }

    //    private class ReplaceExpressionVisitor : ExpressionVisitor
    //    {
    //        private readonly Expression _oldValue;
    //        private readonly Expression _newValue;

    //        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
    //        {
    //            _oldValue = oldValue;
    //            _newValue = newValue;
    //        }

    //        public override Expression Visit(Expression node)
    //        {
    //            if (node == _oldValue)
    //                return _newValue;
    //            return base.Visit(node);
    //        }
    //    }
    //}
}