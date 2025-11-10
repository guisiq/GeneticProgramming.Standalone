using System.Linq.Expressions;

namespace GeneticProgramming.Expressions.Abstractions
{
    /// <summary>
    /// Define um contrato para símbolos capazes de gerar expressões LINQ durante a compilação de árvores.
    /// </summary>
    /// <typeparam name="T">Tipo de valor manipulado pelo símbolo.</typeparam>
    public interface ICompilableSymbol<T> where T : notnull
    {
        /// <summary>
        /// Constrói a expressão correspondente a este símbolo, a partir das expressões dos filhos.
        /// </summary>
        /// <param name="childExpressions">Expressões já compiladas dos filhos.</param>
        /// <param name="variablesParameter">Parâmetro que representa o dicionário de variáveis no lambda final.</param>
        /// <returns>Expressão que calcula o valor do símbolo.</returns>
        Expression BuildExpression(Expression[] childExpressions, ParameterExpression variablesParameter);
    }
}
