using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CPF
{
    public class Obx<T>: BindingDescribe where T : CpfObject, new()
    {
        public Obx(Expression<Func<T, object>> sourceProperty)
        {
            var expression = GetMemberExpression(sourceProperty.Body);
            PropertyName = GetMemberPath(expression);
        }
        public Obx(Expression<Func<T, object>> sourceProperty, BindingMode binding)
        {
            var expression = GetMemberExpression(sourceProperty.Body);
            PropertyName = GetMemberPath(expression);
            BindingMode = binding;
        }
        public Obx(Expression<Func<T, object>> sourceProperty, BindingMode binding, Func<object, object> convert)
        {
            var expression = GetMemberExpression(sourceProperty.Body);
            PropertyName = GetMemberPath(expression);
            BindingMode = binding;
            Convert = convert;
        }
        private static MemberExpression GetMemberExpression(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                return memberExpression;
            }
            else if (expression is UnaryExpression unaryExpression)
            {
                return (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                throw new ArgumentException("Invalid expression. Expected a MemberExpression or UnaryExpression.");
            }
        }
        private static string GetMemberPath(MemberExpression expression)
        {
            if (expression.Expression is MemberExpression memberExpression)
            {
                return GetMemberPath(memberExpression) + "." + expression.Member.Name;
            }

            return expression.Member.Name;
        }
    }
}
